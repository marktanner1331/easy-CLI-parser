using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Linq;
using System.Runtime.InteropServices;

namespace Easy_CLI_Parser
{
internal static class Parser
{
    //for types that don't have a Parse method or a usable constructor
    public static Dictionary<Type, Func<string, object>> parsers;

    static Parser()
    {
        parsers = new Dictionary<Type, Func<string, object>>();
        parsers[typeof(string)] = x => x;
    }

    public static void RegisterCustomParser<T>(Func<string, T> parser)
    {
        parsers[typeof(T)] = new Func<string, object>(x => parser(x));
    }

    public static object Parse<T>(string input) => Parse(typeof(T), input);

    public static object Parse(Type type, string input)
    {
        if(parsers.ContainsKey(type) == false)
        {
            parsers[type] = getParser(type);
        }

        return parsers[type](input);
    }

    private static Func<string, object> getParser(Type type)
    {
        MethodInfo parse = (from m in type.GetMethods(BindingFlags.Static | BindingFlags.Public)
                            where m.Name == "Parse"
                            let parameters = m.GetParameters()
                            where parameters.Length == 1
                            let param1 = parameters.First()
                            where param1.ParameterType == typeof(string)
                            where m.ReturnType == type
                            select m).FirstOrDefault();

        if (parse != null)
        {
            return x => parse.Invoke(null, new object[] { x });
        }

        MethodInfo tryParse = (from m in type.GetMethods(BindingFlags.Static | BindingFlags.Public)
                                where m.Name == "TryParse"
                                let parameters = m.GetParameters()
                                where parameters.Length == 2
                                let param1 = parameters.First()
                                where param1.ParameterType == typeof(string)
                                let param2 = parameters.Skip(1).First()
                                where param2.GetCustomAttribute<OutAttribute>() != null
                                where m.ReturnType == typeof(bool)
                                select m).FirstOrDefault();

        if (tryParse != null)
        {
            return x =>
            {
                object[] parameters = new object[] { x, null };
                bool success = (bool)tryParse.Invoke(null, parameters);
                if (success)
                {
                    return parameters[1];
                }
                else
                {
                    throw new ParserException("Input was not in the correct format");
                }
            };
        }

        ConstructorInfo constructor = (from c in type.GetConstructors()
                                        where c.IsPublic
                                        let parameters = c.GetParameters()
                                        where parameters.Length == 1
                                        let param1 = parameters.First()
                                        where param1.ParameterType == typeof(string)
                                        select c).FirstOrDefault();

        if (constructor != null)
        {
            return x => constructor.Invoke(new object[] { x });
        }

        throw new ParserException("No parser found for type: " + type);
    }
}
}
