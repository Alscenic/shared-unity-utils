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
        }

        public enum ArgumentResult
        {
            Success,
            TooFewArgs,
            TooManyArgs,
            WrongType,
            CommandDoesNotExist,
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
                Instance.Log(Identifier + " - " + Description);
                Instance.Log("Usage: " + UsageFull);
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
            public const string BEGIN_REQUIRED = "<";

            public const string END_REQUIRED = ">";

            public const string BEGIN_OPTIONAL = "[";

            public const string END_OPTIONAL = "]";

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
            // Static methods

            public static ArgumentResult FromInput(Prototype prototype, string input, out Argument[] args)
            {
                string[] rawArgs = input.Split(' ');
                args = new Argument[prototype.Parameters.Length];

                for (int i = 0; i < rawArgs.Length; i++)
                {
                    if (i >= prototype.Parameters.Length)
                    {
                        return ArgumentResult.TooManyArgs;
                    }
                }

                for (int i = 0; i < prototype.Parameters.Length; i++)
                {
                    if (prototype.Parameters[i].Default != null)
                    {
                        break;
                    }

                    if (i >= args.Length)
                    {
                        return ArgumentResult.TooFewArgs;
                    }
                }

                for (int i = 0; i < prototype.Parameters.Length; i++)
                {
                    args[i] = new Argument(i < rawArgs.Length ? rawArgs[i] : prototype.Parameters[i].Default, prototype.Parameters[i]);

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
            public LogEntry(object message, LogLevel logLevel)
            {
                Message = message;
                LogLevel = logLevel;
                LogTime = Time.time;
            }

            public object Message { get; }

            public LogLevel LogLevel { get; }

            public float LogTime { get; }

            public string Prefix
            {
                get
                {
                    StringBuilder stringBuilder = new StringBuilder();

                    switch (LogLevel)
                    {
                        case LogLevel.Success:
                            stringBuilder.Append("<color=64FF64>");
                            break;
                        case LogLevel.Info:
                            stringBuilder.Append("<color=96C8FF>");
                            break;
                        case LogLevel.Command:
                            stringBuilder.Append("<color=DDDDDD>");
                            break;
                        case LogLevel.Log:
                            stringBuilder.Append("<color=FFFFFF>");
                            break;
                        case LogLevel.Warning:
                            stringBuilder.Append("<color=FFFF64>");
                            break;
                        case LogLevel.Error:
                            stringBuilder.Append("<color=FF6464>");
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
                return Prefix + " " + Message.ToString();
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
            Prototypes.Add(new Prototype("help", "Logs all valid commands and their uses.", new Parameter[]
            {
                new Parameter("cmd",ParameterType.String,"The command to get help with","",Parameter.MATCH_ANY_PATTERN)
            }, (command) =>
            {
                if (Prototypes.Contains(command.Arguments[0].Input))
                {
                    Prototypes[command.Arguments[0].Input].LogHelp();
                }
                else
                {
                    foreach (Prototype prototype in Prototypes)
                    {
                        prototype.LogHelp();
                    }
                }
            }));
        }

        public class Window
        {
            public const float DEFAULT_WIDTH = 0.4f;

            public const float DEFAULT_HEIGHT = 0.4f;

            public const float MIN_WIDTH = 0.1f;

            public const float MIN_HEIGHT = 0.1f;

            private Rect LocalScreenRect { get; set; } = new Rect(0.0f, 0.0f, DEFAULT_WIDTH, DEFAULT_HEIGHT);

            public Rect ScreenRect
            {
                get => LocalScreenRect;
                set
                {
                    LocalScreenRect = value;

                    LocalScreenRect = new Rect(new Vector2(
                        Mathf.Clamp(LocalScreenRect.x, 0.0f, Mathf.Max(0.0f, 1.0f - LocalScreenRect.width)),
                        Mathf.Clamp(LocalScreenRect.y, 0.0f, Mathf.Max(0.0f, 1.0f - LocalScreenRect.height))),
                        LocalScreenRect.size);

                    LocalScreenRect = new Rect(LocalScreenRect.position, new Vector2(
                        Mathf.Clamp(LocalScreenRect.width, MIN_WIDTH, 1.0f - LocalScreenRect.x),
                        Mathf.Clamp(LocalScreenRect.height, MIN_HEIGHT, 1.0f - LocalScreenRect.y)));
                }
            }

            public Rect ActualRect
            {
                get =>
                    new Rect(new Vector2(ScreenRect.x * Screen.width, ScreenRect.y * Screen.height),
                    new Vector2(ScreenRect.width * Screen.width, ScreenRect.height * Screen.height));
                set => ScreenRect =
                    new Rect(new Vector2(value.x / Screen.width, value.y / Screen.height),
                    new Vector2(value.width / Screen.width, value.height / Screen.height));
            }

            public float GripSize { get; set; } = 0.015f;

            public float InputFieldSize { get; set; } = 0.023f;

            public float ActualGripSize => GripSize * Screen.height;

            public float ActualInputFieldSize => InputFieldSize * Screen.height;

            public bool MouseDown { get; private set; } = false;

            public bool Grabbing { get; private set; } = false;

            public bool SizingX { get; private set; } = false;

            public bool SizingY { get; private set; } = false;

            public Vector2 MouseDownOrigin { get; private set; } = new Vector2();

            public Vector2 MouseDownOffset { get; private set; } = new Vector2();

            public string CurrentInput { get; set; } = "";

            public Rect GrabBarRect => new Rect(ActualRect.position, new Vector2(ActualRect.width, ActualGripSize));

            public Rect SizeBarXRect => new Rect(ActualRect.position + new Vector2(ActualRect.size.x - ActualGripSize, ActualGripSize),
            new Vector2(ActualGripSize, ActualRect.height - ActualGripSize));

            public Rect SizeBarYRect => new Rect(ActualRect.position + new Vector2(0.0f, ActualRect.size.y - ActualGripSize),
            new Vector2(ActualRect.width, ActualGripSize));

            public Rect InputFieldRect => new Rect(ActualRect.position + new Vector2(0.0f, ActualRect.size.y - ActualGripSize - ActualInputFieldSize),
            new Vector2(ActualRect.size.x - ActualGripSize, ActualInputFieldSize));

            public Rect LogAreaScrollRect => new Rect(ActualRect.position + new Vector2(0.0f, ActualGripSize),
            ActualRect.size - new Vector2(ActualGripSize, ActualGripSize * 2.0f + ActualInputFieldSize));

            public Rect LogAreaRect => new Rect(Vector2.zero, new Vector2(0.0f, LogAreaScrollRect.size.y - 1.0f) + new Vector2(MaxLogAreaWidth, -GUI.skin.horizontalScrollbar.CalcHeight(new GUIContent(), 100.0f)));

            public float MaxLogAreaWidth { get; private set; } = 0.0f;

            private Vector2 MousePos => Event.current.mousePosition;

            public Vector2 ScrollPos { get; set; } = new Vector2();

            public void Draw()
            {
                GUI.Box(ActualRect, new GUIContent());
                GUI.Box(GrabBarRect, new GUIContent());
                GUI.Box(SizeBarXRect, new GUIContent());
                GUI.Box(SizeBarYRect, new GUIContent());

                GUI.SetNextControlName("ConsoleInput");
                CurrentInput = GUI.TextField(InputFieldRect, CurrentInput);
                while (CurrentInput.IndexOf('`') >= 0)
                {
                    CurrentInput = CurrentInput.Replace("`", "");
                }

                ScrollPos = GUI.BeginScrollView(LogAreaScrollRect, ScrollPos, LogAreaRect);

                MaxLogAreaWidth = 0.0f;

                float lineHeight = GUI.skin.label.CalcHeight(new GUIContent(" "), 10.0f);
                int i = 0;
                while ((i + 1) * lineHeight <= LogAreaRect.height && i < Instance.LocalLogger.Entries.Count)
                {
                    string line = i.ToString("n0") + ". " + Instance.LocalLogger.Entries[i].ToString();
                    Vector2 lineSize = GUI.skin.label.CalcSize(new GUIContent(line));
                    MaxLogAreaWidth = Mathf.Max(lineSize.x, MaxLogAreaWidth);
                    GUI.Label(new Rect(new Vector2(0.0f, i * lineHeight), new Vector2(lineSize.x, lineSize.y)), line);
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
                        MouseDownOffset = MousePos - ActualRect.position;

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

                if (Grabbing)
                {
                    ActualRect = new Rect(MousePos - MouseDownOffset, ActualRect.size);
                }
                else
                {
                    if (SizingX)
                    {
                        ActualRect = new Rect(ActualRect.position, new Vector2((MousePos.x - MouseDownOrigin.x) + MouseDownOffset.x, ActualRect.size.y));
                    }

                    if (SizingY)
                    {
                        ActualRect = new Rect(ActualRect.position, new Vector2(ActualRect.size.x, (MousePos.y - MouseDownOrigin.y) + MouseDownOffset.y));
                    }
                }
            }

            public void Open()
            {
                GUI.FocusControl("ConsoleInput");
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
                        Instance.Log("Could not execute command: " + result.ToString(), LogLevel.Warning);
                    }
                }

                CurrentInput = "";
            }
        }

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
            if (UnityEngine.InputSystem.Keyboard.current.backquoteKey.wasPressedThisFrame)
            {
                Toggle();
            }

            if (IsOpen && UnityEngine.InputSystem.Keyboard.current.enterKey.wasPressedThisFrame)
            {
                MainWindow.Send();
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

        public void Log(object message)
        {
            Log(message, LogLevel.Log);
        }

        public void Log(object message, LogLevel logLevel)
        {
            LocalLogger.Entries.Add(new LogEntry(message, logLevel));
        }

        public static ArgumentResult ParseCommand(string input, out Command command)
        {
            Instance.Log(input, LogLevel.Command);

            int identifierEnd = input.IndexOf(' ');
            string identifier = input.Substring(0, identifierEnd < 0 ? input.Length : identifierEnd);

            if (Instance.Prototypes.Contains(identifier))
            {
                Prototype prototype = Instance.Prototypes[identifier];

                string fullRaw = input.Trim();
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
