# MCronberg.Sap.ConsoleOutput

MCronberg.Sap.ConsoleOutput is a Simple As Possible Console Output package. It can be used to write text to console (and file).

Use it as:

```csharp
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
```

It will write this to console (without color):

```
====
TEST
====

----
TEST
----

----
TEST
----

test
----

test
----

test
test

{
  "a": 1,
  "b": "b"
}
**********************************************
VALUE DOES NOT FALL WITHIN THE EXPECTED RANGE.
**********************************************

System.ArgumentException: Value does not fall within the expected range.
**********************************************
VALUE DOES NOT FALL WITHIN THE EXPECTED RANGE.
**********************************************

System.ArgumentException: Value does not fall within the expected range.
System.ArgumentException: Value does not fall within the expected range.
System.ArgumentException: Value does not fall within the expected range.
========================
TEXT TO FILE AND CONSOLE
========================

========================
TEXT TO FILE AND CONSOLE
========================
```
