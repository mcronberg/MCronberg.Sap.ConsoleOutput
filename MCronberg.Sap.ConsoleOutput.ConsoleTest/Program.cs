using MCronberg.Sap.ConsoleOutput.Core;
using System;

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
}
