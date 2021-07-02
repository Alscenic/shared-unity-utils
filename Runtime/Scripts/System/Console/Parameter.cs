using System.Text;
using System.Text.RegularExpressions;

namespace CGenStudios.UnityUtils
{
    public partial class Console
    {
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

            public const string MATCH_HEX_PATTERN = @"^(?i)[0-9A-F]{6}$";

            public const string MATCH_HEXCOLOR_PATTERN = @"^(?i)#[0-9A-F]{6}$";

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
    }
}
