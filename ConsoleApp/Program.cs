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
    }
}
#endregion