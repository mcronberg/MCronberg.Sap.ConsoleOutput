using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

// See https://github.com/Robert-McGinley/TableParser for table

namespace MCronberg.Sap.ConsoleOutput.Core
{
    public class Writer
    {
        private readonly Action<string> writer;

        public Writer(Action<string> writer = null, bool addConsole = false)
        {
            this.writer = writer;
            if (this.writer == null)
                this.writer = Console.WriteLine;
            else
            {
                if (addConsole)
                    this.writer += Console.WriteLine;
            }
        }

        private string GetJson(object obj, bool writeIndented = true)
        {
            return System.Text.Json.JsonSerializer.Serialize(obj, new System.Text.Json.JsonSerializerOptions { WriteIndented = writeIndented });
        }

        public void Write()
        {
            Write("");

        }
        public void Write(string txt, ConsoleColor color = ConsoleColor.Gray)
        {
            ConsoleColor c = Console.ForegroundColor;
            Console.ForegroundColor = color;
            writer(txt);
            Console.ForegroundColor = c;
        }

        public void FullError(Exception ex, ConsoleColor color = ConsoleColor.Gray)
        {
            BigHeader(ex.Message, '*', color);
            Write(ex.ToString(), color);
        }

        public void SimpleError(Exception ex, ConsoleColor color = ConsoleColor.Gray)
        {
            Write(ex.ToString(), color);
        }

        public void SimpleHeader(string header, ConsoleColor color = ConsoleColor.Gray, bool addNewline = false)
        {
            var prePost = new String('-', header.Length);
            Write(header, color);
            Write(prePost, color);
            if (addNewline)
                NewLine();
        }

        public void BigHeader(string header, char headerChar = '=', ConsoleColor color = ConsoleColor.Gray, bool addNewline = false)
        {
            var prePost = new String(headerChar, header.Length);
            Write(prePost, color);
            Write(header.ToUpper(), color);
            Write(prePost, color);
            if (addNewline)
                NewLine();

        }

        public void Json(object obj, ConsoleColor color = ConsoleColor.Gray)
        {
            Write(GetJson(obj, true), color);
        }

        public void WriteJsonToFile(object obj, string path)
        {
            Write("Saving json to " + path + " for inspection");
            try
            {
                string json = GetJson(obj, true);
                System.IO.File.WriteAllText(path, json);
            }
            catch (Exception ex)
            {
                throw new Exception("Error writing to " + path, ex);
            }
        }

        public void NewLine(int count = 1)
        {
            for (int i = 0; i < count; i++)
            {
                Write("");
            }
        }

        public void Table<T>(IEnumerable<T> values, bool showCounter = false, params Expression<Func<T, object>>[] valueSelectors)
        {
            TableParser.TableParserHelper tableParser = new TableParser.TableParserHelper(showCounter);
            var txt = tableParser.ToStringTable<T>(values, valueSelectors);
            Write(txt);
        }

        public void Table<T>(IEnumerable<T> values, params Expression<Func<T, object>>[] valueSelectors)
        {
            TableParser.TableParserHelper tableParser = new TableParser.TableParserHelper(false);
            var txt = tableParser.ToStringTable<T>(values, valueSelectors);
            Write(txt);
        }


        public void Table<T>(IEnumerable<T> values, string[] columnHeaders, bool showCounter = false, params Func<T, object>[] valueSelectors)
        {
            TableParser.TableParserHelper tableParser = new TableParser.TableParserHelper(showCounter) ;
            var txt = tableParser.ToStringTable<T>(values, columnHeaders, valueSelectors);
            Write(txt);
        }

        public void Table<T>(IEnumerable<T> values, string[] columnHeaders,  params Func<T, object>[] valueSelectors)
        {
            TableParser.TableParserHelper tableParser = new TableParser.TableParserHelper(false);
            var txt = tableParser.ToStringTable<T>(values, columnHeaders, valueSelectors);
            Write(txt);
        }


        public void Table<T>(IEnumerable<T> values, bool showCounter = false)
        {
            TableParser.TableParserHelper tableParser = new TableParser.TableParserHelper(showCounter);
            var txt = tableParser.ToStringTable<T>(values, new[] { "Value" }, i => i);
            Write(txt);
        }

    }

    // Inspired/copied from https://github.com/Robert-McGinley/TableParser    
    namespace TableParser
    {
        using System;
        using System.Collections.Generic;
        using System.Diagnostics;
        using System.Linq;
        using System.Linq.Expressions;
        using System.Reflection;
        using System.Text;

        internal class TableParserHelper
        {
            private readonly bool showCounter;

            public TableParserHelper(bool showCounter = false)
            {
                this.showCounter = showCounter;
            }
            public string ToStringTable<T>(IEnumerable<T> values, string[] columnHeaders, params Func<T, object>[] valueSelectors)
            {
                return ToStringTable(values.ToArray(), columnHeaders, valueSelectors);
            }

            public string ToStringTable<T>(T[] values, string[] columnHeaders, params Func<T, object>[] valueSelectors)
            {
                

                var arrValues = new string[values.Length + 1, valueSelectors.Length];

                // Fill headers
                for (int colIndex = 0; colIndex < arrValues.GetLength(1); colIndex++)
                {
                    arrValues[0, colIndex] = columnHeaders[colIndex];
                }

                // Fill table rows
                for (int rowIndex = 1; rowIndex < arrValues.GetLength(0); rowIndex++)
                {
                    for (int colIndex = 0; colIndex < arrValues.GetLength(1); colIndex++)
                    {
                        object value = valueSelectors[colIndex].Invoke(values[rowIndex - 1]);

                        arrValues[rowIndex, colIndex] = value != null ? value.ToString() : "null";
                    }
                }

                return ToStringTable(arrValues);
            }

            public string ToStringTable(string[,] arrValues)
            {
                int[] maxColumnsWidth = GetMaxColumnsWidth(arrValues);
                int countWidth = arrValues.GetLength(0).ToString().Length;
                int addShowCounter = showCounter ? countWidth +2 : 0;
                var headerSpliter = new string('-', maxColumnsWidth.Sum(i => i + 3) - 1 + addShowCounter+1);

                var sb = new StringBuilder();
                int count = -1;
                for (int rowIndex = 0; rowIndex < arrValues.GetLength(0); rowIndex++)
                {
                    if (showCounter)
                    {
                        sb.Append(" | ");
                        if (rowIndex == 0)
                            sb.Append("#" + new string(' ', countWidth -1));                         
                        else
                            sb.Append(count.ToString().PadRight(countWidth));
                    }

                    for (int colIndex = 0; colIndex < arrValues.GetLength(1); colIndex++)
                    {
                        // Print cell
                        string cell = arrValues[rowIndex, colIndex];
                        cell = cell.PadRight(maxColumnsWidth[colIndex]);
                        sb.Append(" | ");
                        sb.Append(cell);
                    }
                    // Print end of line
                    sb.Append(" | ");
                    sb.AppendLine();

                    // Print splitter
                    if (rowIndex == 0)
                    {
                        sb.AppendFormat(" |{0}| ", headerSpliter);
                        sb.AppendLine();
                    }
                    count++;
                }

                return sb.ToString();
            }

            private int[] GetMaxColumnsWidth(string[,] arrValues)
            {
                var maxColumnsWidth = new int[arrValues.GetLength(1)];
                for (int colIndex = 0; colIndex < arrValues.GetLength(1); colIndex++)
                {
                    for (int rowIndex = 0; rowIndex < arrValues.GetLength(0); rowIndex++)
                    {
                        int newLength = arrValues[rowIndex, colIndex].Length;
                        int oldLength = maxColumnsWidth[colIndex];

                        if (newLength > oldLength)
                        {
                            maxColumnsWidth[colIndex] = newLength;
                        }
                    }
                }

                return maxColumnsWidth;
            }

            public string ToStringTable<T>(IEnumerable<T> values, params Expression<Func<T, object>>[] valueSelectors)
            {
                var headers = valueSelectors.Select(func => GetProperty(func).Name).ToArray();
                var selectors = valueSelectors.Select(exp => exp.Compile()).ToArray();
                return ToStringTable(values, headers, selectors);
            }

            private PropertyInfo GetProperty<T>(Expression<Func<T, object>> expresstion)
            {
                if (expresstion.Body is UnaryExpression)
                {
                    if ((expresstion.Body as UnaryExpression).Operand is MemberExpression)
                    {
                        return ((expresstion.Body as UnaryExpression).Operand as MemberExpression).Member as PropertyInfo;
                    }
                }

                if ((expresstion.Body is MemberExpression))
                {
                    return (expresstion.Body as MemberExpression).Member as PropertyInfo;
                }
                return null;
            }
        }
    }
}
