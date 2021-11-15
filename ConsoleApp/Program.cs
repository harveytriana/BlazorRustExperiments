/*
 * Welcome net6 
 */
using System.Runtime.InteropServices;
using System.Text;

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

    [DllImport(RUSTLIB, CharSet = CharSet.Unicode)] static extern void print_string(byte[] utf8Text);

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

        Console.WriteLine("\nTry extended characters");
        try {
            // hello("« Sabbath »");
            print_string(Encoding.UTF8.GetBytes("« Sabbath »"));
        }
        catch 
        {
            // Try extended characters
            // thread '<unnamed>' panicked at 'called `Result::unwrap()` on an `Err` value: Utf8Error { valid_up_to: 0, error_len: Some(1) }', src\lib.rs:75:32
            // note: run with `RUST_BACKTRACE = 1` environment variable to display a backtrace
        }

        //TODO must accept utf8, eg « Ozzy »
        // maybe: https://stackoverflow.com/questions/66582380/pass-string-from-c-sharp-to-rust-using-ffi
    }
}
#endregion