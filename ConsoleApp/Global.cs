// 
namespace ConsoleApp;

public static class Global
{
    public const string RLIB = @"..\..\..\..\rstlib\target\release\rstlib.dll";

    public static void WriteLineColor(string text, ConsoleColor color)
    {
        Console.ForegroundColor = color;
        Console.WriteLine(text);
        Console.ForegroundColor = ConsoleColor.Gray;
    }
}



