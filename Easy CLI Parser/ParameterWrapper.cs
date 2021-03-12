using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Linq;
using System.Runtime.InteropServices;

namespace Easy_CLI_Parser
{
    internal class ParameterWrapper
    {
        private readonly ParameterInfo parameterInfo;

        public readonly string Name;
        public readonly List<string> ShortNames;
        public readonly string Description;
        public Type Type => parameterInfo.ParameterType;

        static ParameterWrapper()
        {

        }

        public ParameterWrapper(ParameterInfo parameterInfo)
        {
            this.parameterInfo = parameterInfo;

            Name = parameterInfo.Name;
            ShortNames = parameterInfo.GetCustomAttributes<CLIShortName>().Select(x => x.ShortName).ToList();
            Description = parameterInfo.GetCustomAttribute<CLIDescription>()?.Description;
        }

        public string GetSignature()
        {
            string s = getTypeName();
            s += " ";
            s += Name;

            if (parameterInfo.HasDefaultValue)
            {
                string defaultValue = parameterInfo.DefaultValue.ToString();
                if(defaultValue == "")
                {
                    defaultValue = "\"\"";
                }

                s += "=" + defaultValue;
            }

            return s;
        }

        public string getTypeName() => TypeName(Type);

        /// <summary>
        /// Get full type name with full namespace names
        /// from: https://stackoverflow.com/a/13318056
        /// </summary>
        /// <param name="type">Type. May be generic or nullable</param>
        /// <returns>Full type name, fully qualified namespaces</returns>
        private static string TypeName(Type type)
        {
            var nullableType = Nullable.GetUnderlyingType(type);
            if (nullableType != null)
                return nullableType.Name + "?";

            if (!(type.IsGenericType && type.Name.Contains('`')))
            {
                string typeName = type.Name;
                if (typeName.StartsWith("System."))
                {
                    typeName = typeName.Substring(7);
                }
                switch (typeName)
                {
                    case "String": return "string";
                    case "Int32": return "int";
                    case "Double": return "double";
                    case "Float": return "float";
                    case "Decimal": return "decimal";
                    case "Object": return "object";
                    case "Void": return "void";
                    default: return typeName;
                }
            }

            var sb = new StringBuilder(type.Name.Substring(0,
            type.Name.IndexOf('`'))
            );
            sb.Append('<');
            var first = true;
            foreach (var t in type.GetGenericArguments())
            {
                if (!first)
                    sb.Append(',');
                sb.Append(TypeName(t));
                first = false;
            }
            sb.Append('>');
            return sb.ToString();
        }

        public object GetDefaultValue()
        {
            if (parameterInfo.HasDefaultValue)
            {
                return parameterInfo.DefaultValue;
            }
            else if (parameterInfo.ParameterType == typeof(bool))
            {
                return false;
            }
            else
            {
                return null;
            }
        }

        public object Parse(string input) => Parser.Parse(parameterInfo.ParameterType, input);

        public bool HasDefaultValue()
        {
            if (parameterInfo.HasDefaultValue)
            {
                return true;
            }
            else if (parameterInfo.ParameterType == typeof(bool))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
