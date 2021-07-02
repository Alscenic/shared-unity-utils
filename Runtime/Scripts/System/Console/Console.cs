using System.Linq;
using UnityEngine;

namespace CGenStudios.UnityUtils
{
    public partial class Console : SingletonMonoBehaviour<Console>
    {
        /// <summary>
        /// Whether the console is currently open.
        /// </summary>
        public bool IsOpen { get; private set; } = false;

        /// <summary>
        /// The Logger. (List of log entries)
        /// </summary>
        public Logger LocalLogger { get; } = new Logger();

        /// <summary>
        /// List of all registered prototypes.
        /// </summary>
        protected PrototypeCollection Prototypes { get; } = new PrototypeCollection();

        /// <summary>
        /// The GUISkin to use.
        /// </summary>
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

        #region BaseCommands

        private void AddBaseCommands()
        {
            AddPrototype(new Prototype("help", "Logs all valid commands and their uses", new Parameter[]
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

            AddPrototype(new Prototype("list", "Lists all registered commands and their parameters", new Parameter[] { }, (command) =>
            {
                foreach (Prototype prototype in Prototypes)
                {
                    Log(prototype.UsageShort, LogLevel.Info);
                }
            }));

            AddPrototype(new Prototype("log", "Adds a log entry", new Parameter[]
            {
                new Parameter("msg",ParameterType.String,"The log message",null,Parameter.MATCH_ANY_PATTERN),
                new Parameter("lvl",ParameterType.Integer,"The log level","3",Parameter.MATCH_INT_PATTERN),
                new Parameter("color",ParameterType.Integer,"The message color","",Parameter.MATCH_HEXCOLOR_PATTERN)
            }, (command) =>
            {
                Log(command.Arguments[0].Input, (LogLevel)Mathf.Clamp(command.Arguments[1].GetInt, 0, typeof(LogLevel).GetEnumValues().Length - 1), command.Arguments[2].Input);
            }));

            AddPrototype(new Prototype("logtest", "Adds a log entry of each level", new Parameter[] { }, (command) =>
            {
                for (int i = 0; i < typeof(LogLevel).GetEnumValues().Length; i++)
                {
                    Log("Log level " + i, (LogLevel)i);
                }
            }));

            AddPrototype(new Prototype("phys.gravity", "Adds a log entry", new Parameter[]
            {
                new Parameter("x",ParameterType.Decimal,"Gravity X",null,Parameter.MATCH_FLOAT_PATTERN),
                new Parameter("y",ParameterType.Decimal,"Gravity Y",null,Parameter.MATCH_FLOAT_PATTERN),
                new Parameter("z",ParameterType.Decimal,"Gravity Z",null,Parameter.MATCH_FLOAT_PATTERN)
            }, (command) =>
            {
                Physics.gravity = new Vector3(command.Arguments[0].GetFloat, command.Arguments[1].GetFloat, command.Arguments[2].GetFloat);
            }));

            AddPrototype(new Prototype("phys2d.gravity", "Adds a log entry", new Parameter[]
            {
                new Parameter("x",ParameterType.Decimal,"Gravity X",null,Parameter.MATCH_FLOAT_PATTERN),
                new Parameter("y",ParameterType.Decimal,"Gravity Y",null,Parameter.MATCH_FLOAT_PATTERN)
            }, (command) =>
            {
                Physics2D.gravity = new Vector2(command.Arguments[0].GetFloat, command.Arguments[1].GetFloat);
            }));

            AddPrototype(new Prototype("clear", "Clears the console", new Parameter[] { }, (command) =>
            {
                if (LocalLogger.Entries.Count > 1)
                {
                    LocalLogger.Entries.RemoveRange(1, LocalLogger.Entries.Count - 1);
                    MainWindow.ScrollLine = 0;
                }
            }));
        }

        #endregion

        /// <summary>
        /// Whether the console is enabled at all.
        /// </summary>
        public bool ConsoleEnabled { get; set; } = true;

        public Window MainWindow { get; } = new Window();

        public void AddPrototype(Prototype prototype)
        {
            foreach (Prototype p in Prototypes)
            {
                if (p.Identifier == prototype.Identifier)
                {
                    Log("Prototype \"" + prototype.Identifier + "\" has already been added! Ignoring", LogLevel.Warning);
                    return;
                }
            }

            bool atOptionalValues = false;
            for (int i = 0; i < prototype.Parameters.Length; i++)
            {
                if (prototype.Parameters[i].Default != null)
                {
                    atOptionalValues = true;
                }
                else if (atOptionalValues)
                {
                    Log("Prototype \"" + prototype.Identifier + "\" has mixed optional and required parameters. This causes confusion in the command parser. "
                    + "All optional parameters must come after all required parameters.", LogLevel.Warning);
                    Log("To fix this, move any optional parameters to the end of the parameter list.", LogLevel.Warning);
                    return;
                }
            }

            Prototypes.Add(prototype);
        }

        public Prototype GetPrototype(string identifier)
        {
            Prototype prototype = Prototypes.Single((prototype) => { return prototype.Identifier == identifier; });

            return prototype;
        }

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

                if (IsOpen)
                {
                    if ((UnityEngine.InputSystem.Keyboard.current.enterKey.wasPressedThisFrame || UnityEngine.InputSystem.Keyboard.current.numpadEnterKey.wasPressedThisFrame))
                    {
                        MainWindow.Send();
                    }

                    MainWindow.Update();
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

        /// <summary>
        /// Logs a message.
        /// </summary>
        /// <param name="message">The message.</param>
        public static void Log(object message)
        {
            Log(message, LogLevel.Log);
        }

        /// <summary>
        /// Logs a message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="logLevel">The log level.</param>
        public static void Log(object message, LogLevel logLevel)
        {
            Log(message, logLevel, null);
        }

        /// <summary>
        /// Logs a message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="logLevel">The log level.</param>
        /// <param name="color">The message color. Must be in "#FFFFFF" hex format.</param>
        public static void Log(object message, LogLevel logLevel, string color)
        {
            Instance.LocalLogger.Entries.Add(new LogEntry(message, logLevel, color));
        }

        /// <summary>
        /// Parses a command. Does not run the command.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="command">The generated command.</param>
        /// <returns>The ArgumentResult.</returns>
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
