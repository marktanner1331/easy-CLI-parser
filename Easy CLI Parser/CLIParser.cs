using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace Easy_CLI_Parser
{
    /// <summary>
    /// TODO: array parsing
    /// case insensitivity
    /// enums
    /// 
    /// </summary>
    public static class CLIParser
    {
        private static Assembly customAssembly;
        public static bool includeClassNames = false;

        public static void run() => run(Environment.GetCommandLineArgs().Skip(1));

        public static void run(params string[] args) => run((IEnumerable<string>)args);

        public static void run(IEnumerable<string> args)
        {
            if (args.Count() == 0)
            {
                help("");
                return;
            }

            string commandName = args.First();

            MethodWrapper method = (from m in getAllCLIMethods()
                                    where m.Name.ToLower() == commandName.ToLower()
                                       || m.ShortNames.Any(x => x.ToLower() == commandName.ToLower())
                                    select m).FirstOrDefault();

            if (method == null)
            {
                Console.WriteLine("Unknown command: " + commandName);
                return;
            }

            try
            {
                object o = method.run(args.Skip(1));
                if (o != null)
                {
                    if(o is IEnumerable list)
                    {
                        IEnumerator enumerator = list.GetEnumerator();
                        while(enumerator.MoveNext())
                        {
                            Console.WriteLine(enumerator.Current);
                        }
                    }
                    else
                    {
                        Console.WriteLine(o);
                    }
                }
            }
            catch (ParserException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// help for class based commands
        /// </summary>
        [CLIMethod]
        [CLIDescription("Shows a list of possible commands, or help on a specific command")]
        [CLIShortName("?")]
        private static void help(string className = "", string command = "")
        {
            if(className == "" && command == "")
            {
                string assemblyName = Path.GetFileNameWithoutExtension(Assembly.GetEntryAssembly().Location);
                Console.WriteLine($"For help on a specific command, type: {assemblyName} help [class] [command]");
                Console.WriteLine();

                List<Type> types = getAllCLIClasses();
                types = types.OrderBy(x => x.Name).ToList();
            }
        }

        /// <summary>
        /// help for non class based commands
        /// </summary>
        [CLIMethod]
        [CLIDescription("Shows a list of possible commands, or help on a specific command")]
        [CLIShortName("?")]
        private static void help(string command = "")
        {
            if (command == "")
            {
                string assemblyName = Path.GetFileNameWithoutExtension(Assembly.GetEntryAssembly().Location);

                Console.WriteLine("HELP");
                Console.Write("   type: ");

                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write(assemblyName.ToLower() + " help ");

                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write("COMMAND");

                Console.ResetColor();

                Console.Write($" for help on a specific command");
                Console.ResetColor();

                Console.WriteLine();
                Console.WriteLine();

                List<MethodWrapper> methods = getAllCLIMethods();
                methods = methods.OrderBy(x => x.Name).ToList();

                Console.WriteLine("COMMANDS");
                
                WriteTable writeTable = new WriteTable();
                writeTable.columnColors[0] = ConsoleColor.Yellow;
                writeTable.showHeader = false;
                writeTable.convertHeaderToTitleCase = false;
                writeTable.setCustomColumnOrder("CommandName", "Description");
                writeTable.writeToConsole(methods.Select(x => new
                {
                    CommandName = "   " + x.Name,
                    Description = x.Description
                }));
            }
            else
            {
                MethodWrapper method = (from m in getAllCLIMethods()
                                        where m.Name == command
                                           || m.ShortNames.Contains(command)
                                        select m).FirstOrDefault();

                if (method == null)
                {
                    throw new ParserException($"Unknown command: '{command}'");
                }

                Console.WriteLine("DESCRIPTION");
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("   " + method.Description);
                Console.ResetColor();

                Console.WriteLine();

                Console.WriteLine("SIGNATURE");
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("   " + method.GetSignature());
                Console.ResetColor();

                if (method.examples.Any())
                {
                    Console.WriteLine();
                    if(method.examples.Count == 1)
                    {
                        Console.WriteLine("EXAMPLE");
                    }
                    else
                    {
                        Console.WriteLine("EXAMPLES");
                    }

                    Console.ForegroundColor = ConsoleColor.Yellow;
                    foreach (string example in method.examples)
                    {
                        Console.WriteLine("   " + example);
                    }
                    Console.ResetColor();
                }

                Console.WriteLine();

                Console.WriteLine("PARAMETERS");
                foreach(ParameterWrapper parameter in method.parameters)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("   " + parameter.Name + ":");
                    Console.ResetColor();

                    Console.Write("      type: ");
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine(parameter.getTypeName());
                    Console.ResetColor();

                    if(parameter.ShortNames.Any())
                    {
                        Console.Write("      shortname: ");
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Console.WriteLine(string.Join(", ", parameter.ShortNames));
                        Console.ResetColor();
                    }

                    if(string.IsNullOrEmpty(parameter.Description) == false)
                    {
                        Console.Write("      description: ");
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Console.WriteLine(parameter.Description);
                        Console.ResetColor();
                    }
                }

                //WriteTable writeTable = new WriteTable();
                //writeTable.convertHeaderToTitleCase = false;
                //writeTable.setCustomColumnOrder("ParameterName", "Type", "ShortName", "Description");
                //writeTable.writeToConsole(method.parameters.Select(x => new
                //{
                //    ParameterName = x.Name,
                //    Type = x.getTypeName(),
                //    ShortName = string.Join("\n", x.ShortNames),
                //    Description = x.Description,
                //}));
            }
        }

        private static List<Type> getAllCLIClasses()
        {
            Assembly assembly = customAssembly ?? Assembly.GetEntryAssembly();

            List<Type> types = (from type in assembly.GetTypes()
                                           from method in type.GetMethods(
                                             BindingFlags.Public |
                                             BindingFlags.NonPublic |
                                             BindingFlags.Static)
                                           where method.GetCustomAttribute<CLIMethod>() != null
                                           select type).Distinct().ToList();

            return types;
        }

        private static List<MethodWrapper> getAllCLIMethods()
        {
            Assembly assembly = customAssembly ?? Assembly.GetEntryAssembly();

            List<MethodWrapper> methods = (from type in assembly.GetTypes()
                                           from method in type.GetMethods(
                                             BindingFlags.Public |
                                             BindingFlags.NonPublic |
                                             BindingFlags.Static)
                                           where method.GetCustomAttribute<CLIMethod>() != null
                                           select new MethodWrapper(method)).ToList();

            MethodInfo helpMethod = typeof(CLIParser).GetMethods(
                BindingFlags.Public |
                BindingFlags.NonPublic |
                BindingFlags.Static)
                .Where(x => x.Name == "help")
                .Where(x => x.GetParameters().Length == (includeClassNames ? 2 : 1))
                .FirstOrDefault();

            methods.Add(new MethodWrapper(helpMethod));

            List<string> names = methods.Select(x => x.Name).ToList();
            names.AddRange(methods.SelectMany(x => x.ShortNames));

            List<string> duplicates = names.GroupBy(x => x).Where(x => x.Count() > 1).Select(x => x.Key).ToList();
            if (duplicates.Count > 0)
            {
                throw new ParserException($"Error, multiple commands called '{duplicates.First()}'");
            }

            return methods;
        }

        public static void setCustomAssembly(Assembly assembly)
        {
            customAssembly = assembly;
        }
    }
}
