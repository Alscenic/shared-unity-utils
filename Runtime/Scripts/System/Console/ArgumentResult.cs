namespace CGenStudios.UnityUtils
{
    public partial class Console
    {
        /// <summary>
        /// The result of a parsed argument.
        /// </summary>
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
    }
}
