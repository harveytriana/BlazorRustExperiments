namespace ConsoleApp;

using static Global;

/// <summary>
/// How to call a Rust's code function from C# 
/// </summary>
class RustMiscellany
{
    [DllImport(RLIB)] static extern void hello_world();
    [DllImport(RLIB)] static extern float hypotenuse(float x, float y);
    [DllImport(RLIB)] static extern int counter();
    [DllImport(RLIB)] static extern void print_string([MarshalAs(UnmanagedType.LPUTF8Str)] string text);
    // OR **
    // [DllImport(App.RLIB)] static extern void print_string(byte[] utf8Text);
    [DllImport(RLIB)] static extern IntPtr get_some_string();
    [DllImport(RLIB)] static extern IntPtr describe_person(int age);

    public static void Run()
    {
        Console.ForegroundColor = ConsoleColor.DarkYellow;
        hello_world(); // rust writes in console
        Console.ForegroundColor = ConsoleColor.Gray;

        // rust executes a math formula
        Console.WriteLine("\nCALLING MATH FUNCTION");
        float x = 9, y = 11, h = hypotenuse(x, y);

        Console.WriteLine("From Rust Library, Hypotenuse({0}, {1}) = {2}", x, y, h);

        Console.WriteLine("\nCOUNTER FROM CLOSURE");
        Console.WriteLine("Counter: {0}", counter());
        Console.WriteLine("Counter: {0}", counter());
        Console.WriteLine("Counter: {0}", counter());

        // STRINGS
        // ---------------------------------------------------------
        Console.WriteLine("\nEXCHANGING STRINGS");

        Console.WriteLine("Send a string to Rust (with extended characters)");
        Console.ForegroundColor = ConsoleColor.DarkYellow;
        print_string("■ Hello from Blazor");
        Console.ForegroundColor = ConsoleColor.Gray;

        // ** Another approach, but moore complexity
        // print_string("« Esto es un árbol »".Utf8Text());

        // getting a string from lib
        Console.WriteLine("\nGETTING STRINGS");

        var ptr = get_some_string();
        var text = Marshal.PtrToStringUTF8(ptr);

        Console.WriteLine("String Pointer : {0}", ptr);
        Console.WriteLine("Dereferenced   : {0}", text);

        Console.WriteLine("\nString function:");
        int age = 18;
        ptr = describe_person(age);
         
        Console.WriteLine("describe_person(age: {0}) : {1}",age, ptr.TextFromPointer());
    }
}

