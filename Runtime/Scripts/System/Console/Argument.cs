using System.Collections.Generic;
using System.Text;

namespace CGenStudios.UnityUtils
{
    public partial class Console
    {
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

            public double GetDouble
            {
                get
                {
                    if (IsFloat)
                    {
                        return double.Parse(Input);
                    }

                    return 0.0d;
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

            public long GetLong
            {
                get
                {
                    if (IsInt)
                    {
                        return long.Parse(Input);
                    }

                    return 0;
                }
            }

            //
            // Static methods

            public static ArgumentResult FromInput(Prototype prototype, string input, out Argument[] args)
            {
                input = input.Trim();

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
                        else if (!(input[i] == '\\' && i < input.Length - 1 && input[i + 1] == '"'))
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
    }
}
