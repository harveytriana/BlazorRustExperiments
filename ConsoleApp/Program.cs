/*
 * Welcome net6 
 */
using System.Runtime.InteropServices;

Console.WriteLine("Calling Rust Experiments\n");

RustTest.Run();

Console.WriteLine("\nReady");

#region Rust Test
/// <summary>
/// How to call a Rust's code function from C# 
/// </summary>
class RustTest
{
    const string RUSTLIB = @"..\..\..\..\rstlib\target\release\rust_library.dll";

    [DllImport(RUSTLIB)] static extern void greeting();

    [DllImport(RUSTLIB)] static extern float hypotenuse(float x, float y);

    [DllImport(RUSTLIB)] static extern int counter();

    [DllImport(RUSTLIB)] static extern ShapeStruct get_simple_struct();

    [StructLayout(LayoutKind.Sequential)]
    public struct ShapeStruct
    {
        public float length;
        public float width;
        public float height;
    }

    public static void Run()
    {
        Console.WriteLine("greetings subrutine:");
        greeting(); // rust writes in console

        // rust executes a math formula
        float x = 9, y = 11, h = hypotenuse(x, y);

        Console.WriteLine("\nHypotenuse({0}, {1}) = {2}", x, y, h);

        Console.WriteLine("\nCOUNTER");
        Console.WriteLine("{0}", counter());
        Console.WriteLine("{0}", counter());
        Console.WriteLine("{0}", counter());

        Console.WriteLine("\nREAD STRUCT");
        var shape = get_simple_struct();
        Console.WriteLine("Length : {0}", shape.length);
        Console.WriteLine("Width  : {0}", shape.width);
        Console.WriteLine("Height : {0}", shape.height);
    }
}
#endregion