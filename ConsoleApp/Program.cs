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

    [DllImport(RUSTLIB)] static extern Parallelepiped get_any_parallelepiped();

    [DllImport(RUSTLIB)] static extern float get_parallelepiped_volume(Parallelepiped p);

    [DllImport(RUSTLIB)] static extern void hello(string name);


    [StructLayout(LayoutKind.Sequential)]
    public struct Parallelepiped
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

        Console.WriteLine("\nSTRUCT SAMPLE");
        // call rust functions
        var parallelepiped = get_any_parallelepiped();
        var volume = get_parallelepiped_volume(parallelepiped);
        // show it
        Console.WriteLine("Length : {0}", parallelepiped.length);
        Console.WriteLine("Width  : {0}", parallelepiped.width);
        Console.WriteLine("Height : {0}", parallelepiped.height);
        Console.WriteLine("Volume : {0:N2}", volume);

        // strings
        Console.WriteLine("\nSTRINGS");
        hello("Ozzy");

        //TODO accept utf8, eg « Ozzy »
    }
}
#endregion