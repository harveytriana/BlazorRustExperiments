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

    [DllImport(RUSTLIB)] static extern void print_string([MarshalAs(UnmanagedType.LPUTF8Str)] string text);
    // OR **
    // [DllImport(RUSTLIB)] static extern void print_string(byte[] utf8Text);

    [DllImport(RUSTLIB)] static extern IntPtr string_test();

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
        Console.WriteLine("\nBasic");
        print_string("Hello");

        Console.WriteLine("\nTry extended characters");
        print_string("« Sin música, la vida sería un error »");

        // ** ok: but moore complexity
        // print_string("« Esto es un árbol »".Utf8Text());

        // getting a string from lib
        Console.WriteLine("\nTry to get a string from library");
        
        var encodeText = string_test();
        var text = Marshal.PtrToStringUTF8(encodeText);

        Console.WriteLine("Encode String : {0}", encodeText);
        Console.WriteLine("Decode String : {0}", text); 
    }
}
#endregion

// ok
//static class Extensions
//{
//    public static byte[] Utf8Text(this string text)
//    {
//        return Encoding.UTF8.GetBytes(text);
//    }
//}