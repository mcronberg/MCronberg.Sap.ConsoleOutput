using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            else {
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

        public void SimpleHeader(string header, ConsoleColor color = ConsoleColor.Gray)
        {
            var prePost = new String('-', header.Length);

            Write(header, color);
            Write(prePost, color);
            NewLine();
        }

        public void BigHeader(string header, char headerChar = '=', ConsoleColor color = ConsoleColor.Gray)
        {
            var prePost = new String(headerChar, header.Length);
            Write(prePost, color);
            Write(header.ToUpper(), color);
            Write(prePost, color);
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

    }
}
