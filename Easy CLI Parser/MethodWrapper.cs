using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace Easy_CLI_Parser
{
    /// <summary>
    /// TODO
    /// help
    ///     intelligent to width of screen
    /// custom exception, that needs catching
    /// enums
    /// potentially report parsing exceptions for individual variables
    /// </summary>
    internal class MethodWrapper
    {
        private readonly MethodInfo methodInfo;

        public readonly string Name;
        public readonly List<string> ShortNames;
        public readonly string Description;
        public readonly List<ParameterWrapper> parameters;
        public readonly List<string> examples;

        public MethodWrapper(MethodInfo methodInfo)
        {
            this.methodInfo = methodInfo;

            Name = methodInfo.Name;
            ShortNames = methodInfo.GetCustomAttributes<CLIShortName>().Select(x => x.ShortName).ToList();
            Description = methodInfo.GetCustomAttribute<CLIDescription>()?.Description;
            parameters = methodInfo.GetParameters().Select(x => new ParameterWrapper(x)).ToList();
            examples = methodInfo.GetCustomAttributes<CLIExample>().Select(x => x.Example).ToList();
        }

        public object run(IEnumerable<string> args)
        {
            Dictionary<string, object> paramValues = parameters.ToDictionary(x => x.Name, x => x.GetDefaultValue());

            //we allow users to specify a list of values without providing variable names
            //this only works if they have not set any named variables first
            //switches and variables that have defaults are ok
            List<string> unnamedValues = new List<string>();
            bool hasSetNamedNonDefaultVariable = false;

            List<string> queue = new List<string>(args);
            while (queue.Count > 0)
            {
                string arg = queue.First();
                queue.RemoveAt(0);

                Regex variableRegex = new Regex("^(?:-*|\\/)([^-=\\/\\s][^\\s=:]*)(?:=|:)\"?(.+?)\"?$");
                Match namedVariableMatch = variableRegex.Match(arg);
                if (namedVariableMatch.Success)
                {
                    string variableName = namedVariableMatch.Groups[1].Value;
                    string value = namedVariableMatch.Groups[2].Value;

                    ParameterWrapper wrappedParam = parameters.FirstOrDefault(x => x.Name == variableName || x.ShortNames.Contains(variableName));
                    if (wrappedParam != null)
                    {
                        paramValues[wrappedParam.Name] = wrappedParam.Parse(value);

                        if (wrappedParam.HasDefaultValue() == false)
                        {
                            hasSetNamedNonDefaultVariable = true;
                        }

                        continue;
                    }
                }

                Regex switchRegex = new Regex(@"^(?:-{1,2}|\/)([^-=\/\\\s][^\s=:]*)$");
                Match switchMatch = switchRegex.Match(arg);
                if (switchMatch.Success)
                {
                    string switchName = switchMatch.Groups[1].Value;
                    if (paramValues.ContainsKey(switchName) == false)
                    {
                        throw new ParserException("unknown parameter: " + switchName);
                    }

                    if (paramValues[switchName] is bool)
                    {
                        paramValues[switchName] = !(bool)paramValues[switchName];
                    }
                    else
                    {
                        //TODO probably can use a queue after all
                        string nextArg = queue.First();
                        queue.RemoveAt(0);

                        if (variableRegex.IsMatch(nextArg) || switchRegex.IsMatch(nextArg))
                        {
                            throw new ParserException($"trying to use {arg} as a switch, but it requires a value");
                        }
                        else
                        {
                            ParameterWrapper wrappedParam = parameters.FirstOrDefault(x => x.Name == switchName || x.ShortNames.Contains(switchName));
                            paramValues[wrappedParam.Name] = wrappedParam.Parse(nextArg);
                        }
                    }
                }
                else
                {
                    unnamedValues.Add(arg);
                }
            }

            if (unnamedValues.Count > 0)
            {

                if (hasSetNamedNonDefaultVariable)
                {
                    if (paramValues.Any(x => x.Value == null) == false)
                    {
                        //if we have no parameters left to fill then the user has probably just mistyped
                        throw new ParserException($"Unknown parameter: '{unnamedValues.First()}'");
                    }
                    else
                    {
                        //otherwise, they are trying to mix named and unnamed
                        throw new ParserException("Cannot mix named and unnamed variables");
                    }
                }

                List<ParameterWrapper> nonDefaultParams = parameters.Where(x => x.HasDefaultValue() == false).ToList();
                if (nonDefaultParams.Count == 0)
                {
                    //we don't have any non default params to fill
                    //but we do have some default ones
                    //we need the number of unnamedValues to match the number of defaultValues
                    //as anything else means that we don't know which to fill in
                    List<ParameterWrapper> defaultParams = parameters.Where(x => x.HasDefaultValue()).ToList();

                    if (defaultParams.Count != unnamedValues.Count)
                    {
                        string a = defaultParams.Count == 1 ? "parameter" : "parameters";
                        string b = unnamedValues.Count == 1 ? "was" : "were";
                        throw new ParserException($"{Name} expects {defaultParams.Count} {a} but {unnamedValues.Count} {b} provided.");
                    }

                    List<bool> results = Enumerable.Zip(defaultParams, unnamedValues, (a, b) =>
                    {
                        paramValues[a.Name] = a.Parse(b);
                        return true;
                    }).ToList();

                    if (results.Any(x => x == false))
                    {
                        throw new ParserException("An unexpected error occurred when trying to parse unnamed variables");
                    }
                }
                else
                {
                    //we have non default params that need filling with unnamed values
                    //in this case, we can't mix default and non default params
                    if (nonDefaultParams.Count != unnamedValues.Count)
                    {
                        string a = nonDefaultParams.Count == 1 ? "parameter" : "parameters";
                        string b = unnamedValues.Count == 1 ? "was" : "were";
                        throw new ParserException($"{Name} expects {nonDefaultParams.Count} {a} but {unnamedValues.Count} {b} provided.");
                    }

                    List<bool> results = Enumerable.Zip(nonDefaultParams, unnamedValues, (a, b) =>
                    {
                        paramValues[a.Name] = a.Parse(b);
                        return true;
                    }).ToList();

                    if (results.Any(x => x == false))
                    {
                        throw new ParserException("An unexpected error occurred when trying to parse unnamed variables");
                    }
                }
            }

            //here we check that we have all the parameters we need to run the function
            List<string> missingParameters = paramValues.Where(x => x.Value == null).Select(x => x.Key).ToList();
            if (missingParameters.Count != 0)
            {
                throw new ParserException("Missing parameter value for: " + missingParameters.First());
            }

            object[] paramsArray = parameters.Select(x => paramValues[x.Name]).ToArray();
            return methodInfo.Invoke(null, paramsArray);
        }

        public string GetSignature()
        {
            var sigBuilder = new StringBuilder();
            sigBuilder.Append(new ParameterWrapper(methodInfo.ReturnParameter).getTypeName());
            sigBuilder.Append(' ');
  
            sigBuilder.Append(Name);

            sigBuilder.Append("(");
            
            sigBuilder.Append(string.Join(", ", parameters.Select(x => x.GetSignature())));

            sigBuilder.Append(")");
            return sigBuilder.ToString();
        }


    }
}
