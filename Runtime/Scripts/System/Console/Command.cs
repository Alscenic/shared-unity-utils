namespace CGenStudios.UnityUtils
{
    public partial class Console
    {
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
    }
}
