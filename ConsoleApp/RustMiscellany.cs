﻿/// <summary>
/// How to call a Rust's code function from C# 
/// </summary>
class RustMiscellany
{
    const string RUSTLIB = @"..\..\..\..\rstlib\target\release\rstlib.dll";

    [DllImport(RUSTLIB)] static extern void hello_world();

    [DllImport(RUSTLIB)] static extern float hypotenuse(float x, float y);

    [DllImport(RUSTLIB)] static extern int counter();

    [DllImport(RUSTLIB)] static extern void print_string([MarshalAs(UnmanagedType.LPUTF8Str)] string text);
    // OR **
    // [DllImport(RUSTLIB)] static extern void print_string(byte[] utf8Text);

    [DllImport(RUSTLIB)] static extern IntPtr get_some_string();

    [DllImport(RUSTLIB)] static extern IntPtr describe_person(int age);

    [DllImport(RUSTLIB)] static extern IntPtr get_user(int user_id);

    public static void Run()
    {
        Console.ForegroundColor = ConsoleColor.DarkYellow;
        hello_world(); // rust writes in console
        Console.ForegroundColor = ConsoleColor.Gray;

        // rust executes a math formula
        Console.WriteLine("\nCALLING MATH FUNCTION");
        float x = 9, y = 11, h = hypotenuse(x, y);

        Console.WriteLine("Hypotenuse({0}, {1}) = {2}", x, y, h);

        Console.WriteLine("\nCOUNTER FROM CLOSURE", "green");
        Console.WriteLine("Counter: {0}", counter());
        Console.WriteLine("Counter: {0}", counter());
        Console.WriteLine("Counter: {0}", counter());

        // STRINGS
        // ---------------------------------------------------------
        Console.WriteLine("\nEXCHANGING STRINGS");

        Console.WriteLine("Send a string to Rust (with extended characters)");
        Console.ForegroundColor = ConsoleColor.DarkYellow;
        print_string("« Sin música, la vida sería un error »");
        Console.ForegroundColor = ConsoleColor.Gray;

        // ** Another approach, but moore complexity
        // print_string("« Esto es un árbol »".Utf8Text());

        // getting a string from lib
        Console.WriteLine("\nGETTING STRINGS");

        var ptr = get_some_string();
        var text = Marshal.PtrToStringUTF8(ptr);

        Console.WriteLine("String Pointer : {0}", ptr);
        Console.WriteLine("Dereference    : {0}", text);

        Console.WriteLine("\nString function: ");
        int age = 18;
        ptr = describe_person(age);
        text = ptr.TextFromPointer(); // with extension
        // OR
        // text = Marshal.PtrToStringUTF8(ptr);
         
        Console.WriteLine("describe_person(age: {0}) : {1}",age, text);

    }
}

