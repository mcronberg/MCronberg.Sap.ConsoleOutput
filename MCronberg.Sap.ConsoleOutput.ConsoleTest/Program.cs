using MCronberg.Sap.ConsoleOutput.Core;
using System;
using System.Collections.Generic;

namespace MCronberg.Sap.ConsoleOutput.ConsoleTest
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Writer w = new Writer();

            w.BigHeader("test");
            w.BigHeader("test", '-');
            w.BigHeader("test", '-', ConsoleColor.Red);

            w.SimpleHeader("test");
            w.SimpleHeader("test", ConsoleColor.Red);

            w.Write("test");
            w.Write("test", ConsoleColor.Red);
            w.NewLine();
            w.Json(new { a = 1, b = "b" });

            Exception e = new ArgumentException();
            w.FullError(e);
            w.FullError(e, ConsoleColor.Red);
            w.SimpleError(e);
            w.SimpleError(e, ConsoleColor.Red);

            w.Table<Test>(Test.GetTest(), i => i.A, i => i.B);
            w.Table<Test>(Test.GetTest(), showCounter: true, i => i.A, i => i.B);
            List<int> lst1 = new List<int>() { 4, 5, 1, 6, 1, 1, 1, 1, 1, 1, 1, 1 };
            w.Table(lst1, true);
            w.Table(values: lst1, columnHeaders: new[] { "i" }, showCounter: true, i => i);

            if (System.IO.Directory.Exists(@"c:\temp"))
            {
                w = new Writer(t => System.IO.File.AppendAllText(@"c:\temp\test.txt", t + "\r\n"));
                w.BigHeader("Text to file");
                w.BigHeader("Text to file");

                w = new Writer(t => System.IO.File.AppendAllText(@"c:\temp\test.txt", t + "\r\n"), true);
                w.BigHeader("Text to file and console");
                w.BigHeader("Text to file and console");

            }

        }
    }

    class Test
    {
        public int A { get; set; }
        public string B { get; set; }

        public static List<Test> GetTest()
        {
            List<Test> lst = new List<Test>();
            lst.Add(new Test { A = 1, B = "a" });
            lst.Add(new Test { A = 2, B = "b" });
            lst.Add(new Test { A = 3, B = "c" });
            return lst;
        }
    }
}
