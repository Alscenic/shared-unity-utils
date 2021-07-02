using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

namespace CGenStudios.UnityUtils
{
    public class Console : SingletonMonoBehaviour<Console>
    {
        public enum ParameterType
        {
            String,
            Integer,
            Decimal,
            Switch,
            Special,
        }

        public enum ArgumentResult
        {
            Success,
            TooFewArgs,
            TooManyArgs,
            WrongType,
            BadPattern,
            CommandDoesNotExist,
            BadQuotes,
        }

        public enum LogLevel
        {
            Success,
            Info,
            Command,
            Log,
            Warning,
            Error,
        }

        /// <summary>
        /// The command structure.
        /// </summary>
        public struct Prototype
        {
            public Prototype(string identifier, string description, Parameter[] parameters, System.Action<Command> action)
            {
                //
                // Assign parameters

                Identifier = identifier.ToLower();
                Description = description;
                Parameters = parameters;
                Action = action;

                //
                // UsageShort

                StringBuilder usageShortStr = new StringBuilder(Identifier);
                foreach (Parameter parameter in parameters)
                {
                    usageShortStr.Append(' ');
                    usageShortStr.Append(parameter.UsageShort);
                }
                UsageShort = usageShortStr.ToString();

                //
                // UsageFull

                StringBuilder usageFullStr = new StringBuilder(Identifier);
                foreach (Parameter parameter in parameters)
                {
                    usageFullStr.Append(' ');
                    usageFullStr.Append(parameter.UsageFull);
                }
                UsageFull = usageFullStr.ToString();
            }

            public string Identifier { get; }

            public string Description { get; }

            public Parameter[] Parameters { get; }

            public System.Action<Command> Action { get; }

            public string UsageShort { get; }

            public string UsageFull { get; }

            public void LogHelp()
            {
                Log(Identifier + " - " + Description);
                Log("> Usage: " + UsageFull);
            }

            public void Fire(Command cmd)
            {
                Action?.Invoke(cmd);
            }
        }

        /// <summary>
        /// A command variable.
        /// </summary>
        public struct Parameter
        {
            // Const
            public const string BEGIN_REQUIRED = "[";

            public const string END_REQUIRED = "]";

            public const string BEGIN_OPTIONAL = "{";

            public const string END_OPTIONAL = "}";

            public const string MATCH_FLOAT_PATTERN = @"^\-?([0-9]*\.)?[0-9]+$";

            public const string MATCH_INT_PATTERN = @"^\-?[0-9]*$";

            public const string MATCH_ANY_PATTERN = @"[\s\S]";

            public const string MATCH_TRUE_PATTERN = @"(?i)\b((yes)|(true)|(y)|(t)|(enabled?)|(1)|(on)|(set))\b";

            public const string MATCH_FALSE_PATTERN = @"(?i)\b((no)|(false)|(n)|(f)|(disabled?)|(0)|(off)|(reset))\b";

            //
            // Constructor

            public Parameter(string name, ParameterType type, string usage, string @default, string pattern)
            {
                //
                //Assign parameters

                Name = name;
                Type = type;
                Default = @default;
                Pattern = pattern;
                UsageShort = null;
                UsageFull = null;
                UsageDetail = usage;

                //
                // Usage short

                StringBuilder usageShortStr = new StringBuilder();

                usageShortStr.Append(Default == null ? Parameter.BEGIN_REQUIRED : Parameter.BEGIN_OPTIONAL);
                usageShortStr.Append(NameLower);
                usageShortStr.Append(Default == null ? Parameter.END_REQUIRED : Parameter.END_OPTIONAL);

                UsageShort = usageShortStr.ToString();

                //
                // Usage full

                StringBuilder usageFullStr = new StringBuilder();

                usageFullStr.Append(Default == null ? Parameter.BEGIN_REQUIRED : Parameter.BEGIN_OPTIONAL);
                usageFullStr.Append(NameLower);
                usageFullStr.Append(": ");
                usageFullStr.Append(Type.ToString().ToLower());
                usageFullStr.Append(Default == null ? Parameter.END_REQUIRED : Parameter.END_OPTIONAL);

                UsageFull = usageFullStr.ToString();
            }

            //
            // Public

            public string Name { get; }

            public string NameLower => Name.ToLower();

            public ParameterType Type { get; }

            public string UsageShort { get; }

            public string UsageFull { get; }

            public string UsageDetail { get; }

            public string Default { get; }

            public string Pattern { get; }

            //
            // Methods

            public bool Validate(string input)
            {
                return Validate(input, Pattern);
            }

            public bool Validate(string input, string pattern)
            {
                return Regex.IsMatch(input, pattern);
            }
        }

        /// <summary>
        /// Information passed into a parameter by the user.
        /// </summary>
        public struct Argument
        {
            // Constructor
            public Argument(string input, Parameter parameter)
            {
                Input = input;
                Parameter = parameter;
            }

            //
            // Public

            public string Input { get; }

            public Parameter Parameter { get; }

            public bool IsFloat => Parameter.Validate(Input, Parameter.MATCH_FLOAT_PATTERN);

            public bool IsInt => Parameter.Validate(Input, Parameter.MATCH_INT_PATTERN);

            public bool IsTrue => Parameter.Validate(Input, Parameter.MATCH_TRUE_PATTERN);

            public bool IsFalse => Parameter.Validate(Input, Parameter.MATCH_FALSE_PATTERN);

            public bool IsBool => IsTrue || IsFalse;

            public bool IsValid
            {
                get
                {
                    switch (Parameter.Type)
                    {
                        case ParameterType.Integer:
                            return IsInt;
                        case ParameterType.Decimal:
                            return IsFloat;
                        case ParameterType.Switch:
                            return IsBool;
                        case ParameterType.Special:
                            return Parameter.Validate(Input, Parameter.Pattern);
                    }

                    return true;
                }
            }

            public float GetFloat
            {
                get
                {
                    if (IsFloat)
                    {
                        return float.Parse(Input);
                    }

                    return 0.0f;
                }
            }

            public int GetInt
            {
                get
                {
                    if (IsInt)
                    {
                        return int.Parse(Input);
                    }

                    return 0;
                }
            }

            //
            // * Static methods

            public static ArgumentResult FromInput(Prototype prototype, string input, out Argument[] args)
            {
                input = input.Trim();

                // while (input.IndexOf("  ") >= 0)
                // {
                //     input = input.Replace("  ", " ");
                // }

                bool inQuotes = false;
                List<string> rawArgs = new List<string>();

                if (!string.IsNullOrEmpty(input))
                {
                    List<StringBuilder> strArgs = new List<StringBuilder>();
                    strArgs.Add(new StringBuilder());

                    for (int i = 0; i < input.Length; i++)
                    {
                        bool isQuote = input[i] == '"' && (i == 0 || input[i - 1] != '\\');

                        if (isQuote)
                        {
                            inQuotes = !inQuotes;
                        }

                        if ((input[i] == ' ' && !inQuotes) || isQuote)
                        {
                            if (strArgs[strArgs.Count - 1].Length > 0)
                            {
                                strArgs.Add(new StringBuilder());
                            }
                        }
                        else
                        {
                            strArgs[strArgs.Count - 1].Append(input[i]);
                        }
                    }

                    for (int i = 0; i < strArgs.Count; i++)
                    {
                        if (strArgs[i].Length > 0)
                        {
                            rawArgs.Add(strArgs[i].ToString().Trim());
                        }
                    }
                }


                args = new Argument[prototype.Parameters.Length];

                if (inQuotes)
                {
                    return ArgumentResult.BadQuotes;
                }

                if (rawArgs.Count > prototype.Parameters.Length)
                {
                    return ArgumentResult.TooManyArgs;
                }

                for (int i = 0; i < prototype.Parameters.Length; i++)
                {
                    if (prototype.Parameters[i].Default != null)
                    {
                        break;
                    }

                    if (i >= rawArgs.Count)
                    {
                        return ArgumentResult.TooFewArgs;
                    }
                }

                for (int i = 0; i < prototype.Parameters.Length; i++)
                {
                    args[i] = new Argument(i < rawArgs.Count ? rawArgs[i] : prototype.Parameters[i].Default, prototype.Parameters[i]);

                    if (!args[i].IsValid)
                    {
                        return ArgumentResult.WrongType;
                    }
                }

                return ArgumentResult.Success;
            }
        }

        /// <summary>
        /// A command.
        /// </summary>
        public struct Command
        {
            //
            // Constructor

            public Command(Prototype prototype, string fullRaw, string raw, Argument[] arguments, ArgumentResult argumentResult)
            {
                Prototype = prototype;
                FullRaw = fullRaw;
                Raw = raw;
                Arguments = arguments;
                ArgumentResult = argumentResult;
            }

            //
            // Public

            public Prototype Prototype { get; }

            public string FullRaw { get; }

            public string Raw { get; }

            public Argument[] Arguments { get; }

            public ArgumentResult ArgumentResult { get; }
        }

        public class PrototypeCollection : IEnumerable<Prototype>
        {
            public PrototypeCollection() { }

            public Prototype this[string identifier] => Prototypes[identifier.ToLower()];

            private Dictionary<string, Prototype> Prototypes { get; } = new Dictionary<string, Prototype>();

            public int Count => Prototypes.Count;

            public bool Contains(string identifier)
            {
                return Prototypes.ContainsKey(identifier.ToLower());
            }

            public void Add(Prototype prototype)
            {
                Prototypes.Add(prototype.Identifier.ToLower(), prototype);
            }

            public void Remove(string identifier)
            {
                Prototypes.Remove(identifier.ToLower());
            }

            public void Remove(Prototype prototype)
            {
                Prototypes.Remove(prototype.Identifier.ToLower());
            }

            public void Clear()
            {
                Prototypes.Clear();
            }

            public IEnumerator<Prototype> GetEnumerator()
            {
                return Prototypes.Values.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return Prototypes.Values.GetEnumerator();
            }
        }

        public class Logger
        {
            public List<LogEntry> Entries { get; } = new List<LogEntry>();
        }

        public struct LogEntry
        {
            public LogEntry(object message, LogLevel logLevel, string color)
            {
                Message = message;
                LogLevel = logLevel;
                Color = color;
                LogTime = Time.time;
            }

            public object Message { get; }

            public LogLevel LogLevel { get; }

            public string Color { get; }

            public float LogTime { get; }

            public string Prefix
            {
                get
                {
                    StringBuilder stringBuilder = new StringBuilder();

                    switch (LogLevel)
                    {
                        case LogLevel.Success:
                            stringBuilder.Append("<color=#64FF64>");
                            break;
                        case LogLevel.Info:
                            stringBuilder.Append("<color=#96C8FF>");
                            break;
                        case LogLevel.Command:
                            stringBuilder.Append("<color=#999999>");
                            break;
                        case LogLevel.Log:
                            stringBuilder.Append("<color=#FFFFFF>");
                            break;
                        case LogLevel.Warning:
                            stringBuilder.Append("<color=#FFFF64>");
                            break;
                        case LogLevel.Error:
                            stringBuilder.Append("<color=#FF6464>");
                            break;
                    }

                    stringBuilder.Append("[");
                    stringBuilder.Append(LogLevel.ToString());
                    stringBuilder.Append("]</color>");

                    return stringBuilder.ToString();
                }
            }

            public override bool Equals(object obj)
            {
                return base.Equals(obj);
            }

            public override int GetHashCode()
            {
                return base.GetHashCode();
            }

            public override string ToString()
            {
                return Prefix + (Color == null ? " " : " <color=" + Color + ">") + Message.ToString() + (Color == null ? "" : "</color>");
            }
        }

        public bool IsOpen { get; private set; } = false;

        public Logger LocalLogger { get; } = new Logger();

        protected PrototypeCollection Prototypes { get; } = new PrototypeCollection();

        public GUISkin Skin { get; set; } = null;

        [RuntimeInitializeOnLoadMethod]
        private static void RuntimeInitializeOnLoadMethod()
        {
            Initialize();
        }

        protected override void Initialized()
        {
            base.Initialized();

            Skin = Resources.Load<GUISkin>("alscenicUtils_guiSkin_console");

            AddBaseCommands();

            Log("Console initialized", LogLevel.Info);
        }

        private void AddBaseCommands()
        {
            Prototypes.Add(new Prototype("help", "Logs all valid commands and their uses", new Parameter[]
            {
                new Parameter("cmd",ParameterType.String,"The command to get help with","",Parameter.MATCH_ANY_PATTERN)
            }, (command) =>
            {
                if (command.Arguments[0].Input != "")
                {
                    if (Prototypes.Contains(command.Arguments[0].Input))
                    {
                        Prototypes[command.Arguments[0].Input].LogHelp();
                    }
                    else
                    {
                        Log("Command does not exist", LogLevel.Info);
                    }
                }
                else
                {
                    string color = "#F0DC3C";
                    Log("<b>Shared Utils Console</b> by <b>Alscenic</b>", LogLevel.Info, color);
                    Log("github.com/Alscenic/shared-unity-utils", LogLevel.Info, color);
                    Log("Type <b>\"list\"</b> for a list of all registered commands", LogLevel.Info, color);
                }
            }));

            Prototypes.Add(new Prototype("list", "Lists all registered commands and their parameters", new Parameter[] { }, (command) =>
            {
                foreach (Prototype prototype in Prototypes)
                {
                    Log(prototype.UsageShort, LogLevel.Info);
                }
            }));

            Prototypes.Add(new Prototype("log", "Adds a log entry", new Parameter[]
            {
                new Parameter("msg",ParameterType.String,"The log message",null,Parameter.MATCH_ANY_PATTERN),
                new Parameter("lvl",ParameterType.Integer,"The log level","3",Parameter.MATCH_INT_PATTERN)
            }, (command) =>
            {
                Log(command.Arguments[0].Input, (LogLevel)Mathf.Clamp(command.Arguments[1].GetInt, 0, typeof(LogLevel).GetEnumValues().Length - 1));
            }));

            Prototypes.Add(new Prototype("logtest", "Adds a log entry of each level", new Parameter[] { }, (command) =>
            {
                for (int i = 0; i < typeof(LogLevel).GetEnumValues().Length; i++)
                {
                    Log("Log level " + i, (LogLevel)i);
                }
            }));

            Prototypes.Add(new Prototype("phys.gravity", "Adds a log entry", new Parameter[]
            {
                new Parameter("x",ParameterType.Decimal,"Gravity X",null,Parameter.MATCH_FLOAT_PATTERN),
                new Parameter("y",ParameterType.Decimal,"Gravity Y",null,Parameter.MATCH_FLOAT_PATTERN),
                new Parameter("z",ParameterType.Decimal,"Gravity Z",null,Parameter.MATCH_FLOAT_PATTERN)
            }, (command) =>
            {
                Physics.gravity = new Vector3(command.Arguments[0].GetFloat, command.Arguments[1].GetFloat, command.Arguments[2].GetFloat);
            }));

            Prototypes.Add(new Prototype("phys2d.gravity", "Adds a log entry", new Parameter[]
            {
                new Parameter("x",ParameterType.Decimal,"Gravity X",null,Parameter.MATCH_FLOAT_PATTERN),
                new Parameter("y",ParameterType.Decimal,"Gravity Y",null,Parameter.MATCH_FLOAT_PATTERN)
            }, (command) =>
            {
                Physics2D.gravity = new Vector2(command.Arguments[0].GetFloat, command.Arguments[1].GetFloat);
            }));
        }

        public class Window
        {
            public const float DEFAULT_WIDTH = 800.0f;

            public const float DEFAULT_HEIGHT = 600.0f;

            public const float MIN_WIDTH = 400.0f;

            public const float MIN_HEIGHT = 300.0f;

            private Rect LocalRect { get; set; } = new Rect(20.0f, 20.0f, DEFAULT_WIDTH, DEFAULT_HEIGHT);

            public Rect WindowRect
            {
                get => LocalRect;
                set
                {
                    LocalRect = value;

                    LocalRect = new Rect(new Vector2(
                        Mathf.Clamp(LocalRect.x, 0.0f, Mathf.Max(0.0f, Screen.width - LocalRect.width)),
                        Mathf.Clamp(LocalRect.y, 0.0f, Mathf.Max(0.0f, Screen.height - LocalRect.height))),
                        LocalRect.size);

                    LocalRect = new Rect(LocalRect.position, new Vector2(
                        Mathf.Clamp(LocalRect.width, MIN_WIDTH, Screen.width - LocalRect.x),
                        Mathf.Clamp(LocalRect.height, MIN_HEIGHT, Screen.height - LocalRect.y)));
                }
            }

            public Vector4 Padding => new Vector4(GripSize, GripSize, GripSize, GripSize);

            public Rect InteriorRect => new Rect(WindowRect.position + new Vector2(Padding.x, Padding.y), WindowRect.size - new Vector2(Padding.x, Padding.y) - new Vector2(Padding.z, Padding.w));

            public float GripSize { get; set; } = 20.0f;

            public float InputFieldSize { get; set; } = 32.0f;

            public bool MouseDown { get; private set; } = false;

            public bool Grabbing { get; private set; } = false;

            public bool SizingX { get; private set; } = false;

            public bool SizingY { get; private set; } = false;

            public Vector2 MouseDownOrigin { get; private set; } = new Vector2();

            public Vector2 MouseDownOffset { get; private set; } = new Vector2();

            public string CurrentInput { get; set; } = "";

            public Rect GrabBarRect => new Rect(WindowRect.position, new Vector2(WindowRect.width, GripSize));

            public Rect SizeBarXRect => new Rect(WindowRect.position + new Vector2(WindowRect.size.x - GripSize, GripSize),
            new Vector2(GripSize, WindowRect.height - GripSize));

            public Rect SizeBarYRect => new Rect(WindowRect.position + new Vector2(0.0f, WindowRect.size.y - GripSize),
            new Vector2(WindowRect.width, GripSize));

            public Rect InputFieldRect => new Rect(InteriorRect.position + new Vector2(0.0f, InteriorRect.size.y - InputFieldSize),
            new Vector2(InteriorRect.size.x, InputFieldSize));

            public Rect LogAreaScrollRect => new Rect(InteriorRect.position,
            InteriorRect.size - new Vector2(0.0f, InputFieldSize));

            public float ScrollbarHeight => GUI.skin.horizontalScrollbar.CalcHeight(new GUIContent(), 100.0f);

            public Rect LogAreaRect => new Rect(Vector2.zero, new Vector2(0.0f, LogAreaScrollRect.size.y - 1.0f) + new Vector2(MaxLogAreaWidth, -ScrollbarHeight));

            public float MaxLogAreaWidth { get; private set; } = 0.0f;

            private Vector2 MousePos => Event.current.mousePosition;

            public Vector2 ScrollPos { get; set; } = new Vector2();

            private int LocalScrollLine { get; set; } = 0;

            private bool FocusInputField { get; set; } = false;

            public int ScrollLine
            {
                get => LocalScrollLine;
                set => LocalScrollLine = Mathf.Clamp(value, 0, Instance.LocalLogger.Entries.Count - 1);
            }

            public void Draw()
            {
                GUI.Box(WindowRect, new GUIContent());

                // Vector2 actualGripSquare = new Vector2(GripSize, GripSize);
                // GUI.Box(new Rect(WindowRect.position + WindowRect.size - actualGripSquare, actualGripSquare), new GUIContent(), GUI.skin.GetStyle("grabme"));

                GUI.SetNextControlName("ConsoleInput");
                CurrentInput = GUI.TextField(InputFieldRect, CurrentInput);
                if (FocusInputField)
                {
                    GUI.FocusControl("ConsoleInput");
                    FocusInputField = false;
                }

                while (CurrentInput.IndexOf('`') >= 0)
                {
                    CurrentInput = CurrentInput.Replace("`", "");
                }

                ScrollPos = GUI.BeginScrollView(LogAreaScrollRect, ScrollPos, LogAreaRect);

                MaxLogAreaWidth = 0.0f;

                float lineHeight = GUI.skin.label.CalcHeight(new GUIContent(" "), 10.0f);
                int i = 0;
                while ((i + 1) * lineHeight <= LogAreaRect.height && Instance.LocalLogger.Entries.Count - (i + ScrollLine) - 1 >= 0)
                {
                    int lineNum = Instance.LocalLogger.Entries.Count - (i + ScrollLine) - 1;
                    string line = lineNum.ToString("n0") + ". " + Instance.LocalLogger.Entries[lineNum].ToString();
                    Vector2 lineSize = GUI.skin.label.CalcSize(new GUIContent(line));
                    MaxLogAreaWidth = Mathf.Max(lineSize.x, MaxLogAreaWidth);
                    GUI.Label(new Rect(new Vector2(0.0f, LogAreaRect.height - i * lineHeight - ScrollbarHeight), new Vector2(lineSize.x, lineSize.y)), line);
                    i++;
                }
                GUI.EndScrollView(false);
            }

            public void CalculateMouse()
            {
                if (Event.current.isMouse && Event.current.button == 0)
                {
                    if (MouseDown && Event.current.type == EventType.MouseUp)
                    {
                        MouseDown = false;
                        Grabbing = false;
                        SizingX = false;
                        SizingY = false;
                    }
                    else if (!MouseDown && Event.current.type == EventType.MouseDown)
                    {
                        MouseDown = true;
                        MouseDownOrigin = MousePos;
                        MouseDownOffset = MousePos - WindowRect.position;

                        if (GrabBarRect.Contains(MousePos))
                        {
                            Grabbing = true;
                        }
                        else
                        {
                            if (SizeBarXRect.Contains(MousePos))
                            {
                                SizingX = true;
                            }
                            if (SizeBarYRect.Contains(MousePos))
                            {
                                SizingY = true;
                            }
                        }
                    }
                }

                if (WindowRect.Contains(MousePos) && Event.current.isScrollWheel)
                {
                    ScrollLine -= (int)Mathf.Sign(Event.current.delta.y);
                }

                if (Grabbing)
                {
                    WindowRect = new Rect(MousePos - MouseDownOffset, WindowRect.size);
                }
                else
                {
                    if (SizingX)
                    {
                        WindowRect = new Rect(WindowRect.position, new Vector2((MousePos.x - MouseDownOrigin.x) + MouseDownOffset.x, WindowRect.size.y));
                    }

                    if (SizingY)
                    {
                        WindowRect = new Rect(WindowRect.position, new Vector2(WindowRect.size.x, (MousePos.y - MouseDownOrigin.y) + MouseDownOffset.y));
                    }
                }
            }

            public void Open()
            {
                FocusInputField = true;
            }

            public void Send()
            {
                if (!string.IsNullOrWhiteSpace(CurrentInput))
                {
                    ArgumentResult result = ParseCommand(CurrentInput, out Command cmd);
                    if (result == ArgumentResult.Success)
                    {
                        cmd.Prototype.Fire(cmd);
                    }
                    else
                    {
                        Log("Could not execute command: " + result.ToString(), LogLevel.Info);
                    }
                }

                CurrentInput = "";
            }
        }

        public bool ConsoleEnabled { get; set; } = true;

        public Window MainWindow { get; } = new Window();

        private void OnGUI()
        {
            if (IsOpen)
            {
                GUI.skin = Skin;

                MainWindow.Draw();
                MainWindow.CalculateMouse();
            }
        }

        private void Update()
        {
            if (ConsoleEnabled)
            {
                if (UnityEngine.InputSystem.Keyboard.current.backquoteKey.wasPressedThisFrame)
                {
                    Toggle();
                }

                if (IsOpen && (UnityEngine.InputSystem.Keyboard.current.enterKey.wasPressedThisFrame || UnityEngine.InputSystem.Keyboard.current.numpadEnterKey.wasPressedThisFrame))
                {
                    MainWindow.Send();
                }
            }
        }

        public void Open()
        {
            IsOpen = true;
            MainWindow.Open();
        }

        public void Close()
        {
            IsOpen = false;
        }

        public void Toggle()
        {
            if (IsOpen)
            {
                Close();
            }
            else
            {
                Open();
            }
        }

        public static void Log(object message)
        {
            Log(message, LogLevel.Log);
        }

        public static void Log(object message, LogLevel logLevel)
        {
            Log(message, logLevel, null);
        }

        public static void Log(object message, LogLevel logLevel, string color)
        {
            Instance.LocalLogger.Entries.Add(new LogEntry(message, logLevel, color));
        }

        public static ArgumentResult ParseCommand(string input, out Command command)
        {
            input = input.Trim();

            Log(input, LogLevel.Command, "#999999");

            int identifierEnd = input.IndexOf(' ');
            string identifier = input.Substring(0, identifierEnd < 0 ? input.Length : identifierEnd);

            if (Instance.Prototypes.Contains(identifier))
            {
                Prototype prototype = Instance.Prototypes[identifier];

                string fullRaw = input;
                string raw = fullRaw.Substring(prototype.Identifier.Length).Trim();
                ArgumentResult argResult = Argument.FromInput(prototype, raw, out Argument[] arguments);

                command = new Command(prototype, fullRaw, raw, arguments, argResult);

                return argResult;
            }

            command = new Command();
            return ArgumentResult.CommandDoesNotExist;
        }
    }
}
