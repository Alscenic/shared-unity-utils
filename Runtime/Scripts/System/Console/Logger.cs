using System.Collections.Generic;

namespace CGenStudios.UnityUtils
{
    public partial class Console
    {
        public class Logger
        {
            public List<LogEntry> Entries { get; } = new List<LogEntry>();
        }
    }
}
