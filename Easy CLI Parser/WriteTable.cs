using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Linq;
using System.Text.RegularExpressions;

namespace Easy_CLI_Parser
{
    internal class WriteTable
    {
        public bool convertHeaderToTitleCase = true;
        public bool showHeader = true;
        public Dictionary<int, ConsoleColor> columnColors = new Dictionary<int, ConsoleColor>();

        private string[] customColumnOrder;

        private static string convertStringToTitleCase(string input)
        {
            IEnumerable<string> words = Regex.Matches(input, "(?:^.|[A-Z])[^A-Z]*")
                .Cast<Match>()
                .Select(x => x.Value)
                .Select(x => x[0].ToString().ToUpper() + new string(x.Skip(1).ToArray()));

            return string.Join(" ", words);
        }

        public void setCustomColumnOrder(params string[] columnNames)
        {
            this.customColumnOrder = columnNames;
        }

        public void writeToConsole<T>(IEnumerable<T> values)
        {
            PropertyInfo[] properties = typeof(T).GetProperties();
            string[] prettyColumnNames;

            if (customColumnOrder == null)
            {
                properties = properties.OrderBy(x => x.Name).ToArray();
                prettyColumnNames = properties.Select(x => x.Name).ToArray();
            }
            else
            {
                properties = properties.OrderBy(x => Array.IndexOf(customColumnOrder, x.Name)).ToArray();
                prettyColumnNames = customColumnOrder;
            }

            if (convertHeaderToTitleCase)
            {
                prettyColumnNames = prettyColumnNames.Select(convertStringToTitleCase).ToArray();
            }

            string[][] cells = (from v in values
                                let c = (from p in properties
                                         select p.GetValue(v, null)?.ToString() ?? "")
                                select c.ToArray()).ToArray();

            int[] lengths;

            if (showHeader)
            {
                lengths = prettyColumnNames.Select(x => x.Length + 2).ToArray();
                if (lengths.Sum() > Console.WindowWidth)
                {
                    throw new Exception("Max width not wide enough");
                }
            }
            else
            {
                lengths = new int[prettyColumnNames.Length];
            }

            foreach (string[] row in cells)
            {
                for (int i = 0; i < row.Length; i++)
                {
                    lengths[i] = Math.Max(lengths[i], row[i].Length + 2);
                }
            }

            int diff = lengths.Sum() - Console.WindowWidth;
            if (diff > 0)
            {
                int indexOfMax = Array.IndexOf(lengths, lengths.Max());
                if (lengths[indexOfMax] < diff)
                {
                    throw new Exception("Max width not wide enough");
                }

                lengths[indexOfMax] -= diff;
            }

            if (showHeader)
            {
                for (int i = 0; i < prettyColumnNames.Length; i++)
                {
                    Console.Write(prettyColumnNames[i]);
                    Console.Write(new string(' ', lengths[i] - prettyColumnNames[i].Length));
                }

                Console.WriteLine();
                Console.WriteLine(new string('-', lengths.Sum()));
            }

            foreach (string[] row in cells)
            {
            start:

                bool needsSecondPass = false;
                for (int i = 0; i < row.Length; i++)
                {
                    row[i] = row[i].Replace("\r\n", "\n").Replace("\r", "\n");
                    if (row[i].Contains("\n"))
                    {
                        string[] lines = row[i].Split('\n');
                        for (int k = 0; k < lines.Length; k++)
                        {
                            int lineLength = lines[k].Length % (lengths[i] - 2);

                            //for full width lines, nothing to add
                            if (lineLength == 0)
                            {
                                continue;
                            }

                            int remaining = (lengths[i] - 2) - lineLength;
                            lines[k] += new string(' ', remaining);
                        }

                        row[i] = string.Join("", lines);
                    }

                    if (row[i].Length < lengths[i] - 1)
                    {
                        if(columnColors.ContainsKey(i))
                        {
                            Console.ForegroundColor = columnColors[i];
                        }
                        Console.Write(row[i]);
                        Console.ResetColor();

                        Console.Write(new string(' ', lengths[i] - row[i].Length));
                        row[i] = "";
                    }
                    else
                    {
                        if (columnColors.ContainsKey(i))
                        {
                            Console.ForegroundColor = columnColors[i];
                        }
                        Console.Write(row[i].Substring(0, lengths[i] - 2));
                        Console.Write("  ");
                        Console.ResetColor();

                        row[i] = row[i].Substring(lengths[i] - 2);
                        needsSecondPass = true;
                    }
                }

                Console.WriteLine();

                if (needsSecondPass)
                {
                    goto start;
                }
            }
        }

        public string Write<T>(IEnumerable<T> values, int maxWidth)
        {
            PropertyInfo[] properties = typeof(T).GetProperties();
            string[] prettyColumnNames;

            if (customColumnOrder == null)
            {
                properties = properties.OrderBy(x => x.Name).ToArray();
                prettyColumnNames = properties.Select(x => x.Name).ToArray();
            }
            else
            {
                properties = properties.OrderBy(x => Array.IndexOf(customColumnOrder, x.Name)).ToArray();
                prettyColumnNames = customColumnOrder;
            }

            if (convertHeaderToTitleCase)
            {
                prettyColumnNames = prettyColumnNames.Select(convertStringToTitleCase).ToArray();
            }

            string[][] cells = (from v in values
                                let c = (from p in properties
                                         select p.GetValue(v, null)?.ToString() ?? "")
                                select c.ToArray()).ToArray();

            int[] lengths;

            if(showHeader)
            {
                lengths = prettyColumnNames.Select(x => x.Length + 2).ToArray();
                if (lengths.Sum() > maxWidth)
                {
                    throw new Exception("Max width not wide enough");
                }
            }
            else
            {
                lengths = new int[prettyColumnNames.Length];
            }

            foreach (string[] row in cells)
            {
                for (int i = 0; i < row.Length; i++)
                {
                    lengths[i] = Math.Max(lengths[i], row[i].Length + 2);
                }
            }

            int diff = lengths.Sum() - maxWidth;
            if (diff > 0)
            {
                int indexOfMax = Array.IndexOf(lengths, lengths.Max());
                if(lengths[indexOfMax] < diff)
                {
                    throw new Exception("Max width not wide enough");
                }

                lengths[indexOfMax] -= diff;
            }

            StringBuilder sb = new StringBuilder();

            if(showHeader)
            {
                for (int i = 0; i < prettyColumnNames.Length; i++)
                {
                    sb.Append(prettyColumnNames[i]);
                    sb.Append(new string(' ', lengths[i] - prettyColumnNames[i].Length));
                }

                sb.AppendLine();
                sb.AppendLine(new string('-', lengths.Sum()));
            }
            
            foreach (string[] row in cells)
            {
                start:

                bool needsSecondPass = false;
                for (int i = 0; i < row.Length; i++)
                {
                    row[i] = row[i].Replace("\r\n", "\n").Replace("\r", "\n");
                    if(row[i].Contains("\n"))
                    {
                        string[] lines = row[i].Split('\n');
                        for (int k = 0; k < lines.Length; k++)
                        {
                            int lineLength = lines[k].Length % (lengths[i] - 2);

                            //for full width lines, nothing to add
                            if(lineLength == 0)
                            {
                                continue;
                            }

                            int remaining = (lengths[i] - 2) - lineLength;
                            lines[k] += new string(' ', remaining);
                        }

                        row[i] = string.Join("", lines);
                    }

                    if(row[i].Length < lengths[i] - 1)
                    {
                        sb.Append(row[i]);
                        sb.Append(new string(' ', lengths[i] - row[i].Length));
                        row[i] = "";
                    }
                    else
                    {
                        sb.Append(row[i].Substring(0, lengths[i] - 2));
                        sb.Append("  ");
                        row[i] = row[i].Substring(lengths[i] - 2);
                        needsSecondPass = true;
                    }
                }

                sb.AppendLine();

                if(needsSecondPass)
                {
                    goto start;
                }
            }

            return sb.ToString();
        }
    }
}
