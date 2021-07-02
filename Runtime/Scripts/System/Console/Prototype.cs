using System.Text;

namespace CGenStudios.UnityUtils
{
    public partial class Console
    {
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
    }
}
