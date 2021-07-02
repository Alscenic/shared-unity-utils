using System.Text;
using UnityEngine;

namespace CGenStudios.UnityUtils
{
    public partial class Console
    {
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

            public string Body => (string.IsNullOrEmpty(Color) ? " " : " <color=" + Color + ">") + Message.ToString().Split('\n')[0] + (string.IsNullOrEmpty(Color) ? "" : "</color>");

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
                return Prefix + Body;
            }
        }
    }
}
