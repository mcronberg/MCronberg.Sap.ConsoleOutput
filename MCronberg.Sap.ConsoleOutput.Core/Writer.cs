using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
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

        public void Clear()
        {
            Console.Clear();
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
            TableParser.TableParserHelper tableParser = new TableParser.TableParserHelper(showCounter);
            var txt = tableParser.ToStringTable<T>(values, columnHeaders, valueSelectors);
            Write(txt);
        }

        public void Table<T>(IEnumerable<T> values, string[] columnHeaders, params Func<T, object>[] valueSelectors)
        {
            TableParser.TableParserHelper tableParser = new TableParser.TableParserHelper(false);
            var txt = tableParser.ToStringTable<T>(values, columnHeaders, valueSelectors);
            Write(txt);
        }


        public void Table<T>(IEnumerable<T> values, bool showCounter = false)
        {
            TableParser.TableParserHelper tableParser = new TableParser.TableParserHelper(showCounter);
            if (typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance).Length > 1)
            {
                var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
                string[,] v = new string[values.Count() + 1, properties.Count()];
                int row = 0, col = 0;
                foreach (var propery in properties)
                {
                    v[row, col++] = propery.Name;
                }
                row++;
                col = 0;
                foreach (var value in values)
                {
                    foreach (var propery in properties)
                    {
                        v[row, col++] = propery.GetValue(value) == null ? "" : propery.GetValue(value).ToString();
                    }
                    row++;
                    col = 0;
                }
                var txt = tableParser.ToStringTable(v);
                Write(txt);

            }
            else
            {
                var txt = tableParser.ToStringTable<T>(values, new[] { "Value" }, i => i);
                Write(txt);
            }
        }

    }
}
