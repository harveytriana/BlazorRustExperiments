/*
 * Welcome net6 
 */
using System.Runtime.InteropServices;
using System.Text.Json;

Console.WriteLine("Calling Rust Experiments\n");

// RustTest.Run();
RustTestStruct.Run();

Console.WriteLine("\nReady");

#region Rust Test
/// <summary>
/// How to call a Rust's code function from C# 
/// </summary>
class RustTest
{
    const string RUSTLIB = @"..\..\..\..\rstlib\target\release\librstlib.dll";

    [DllImport(RUSTLIB)] static extern void greeting();

    [DllImport(RUSTLIB)] static extern float hypotenuse(float x, float y);

    [DllImport(RUSTLIB)] static extern int counter();

    [DllImport(RUSTLIB)] static extern void print_string([MarshalAs(UnmanagedType.LPUTF8Str)] string text);
    // OR **
    // [DllImport(RUSTLIB)] static extern void print_string(byte[] utf8Text);

    [DllImport(RUSTLIB)] static extern IntPtr string_test();

    [DllImport(RUSTLIB)] static extern IntPtr describe_person(int age);

    [DllImport(RUSTLIB)] static extern IntPtr get_user(int user_id);

    record Person(int person_id, int age, string first_name, string last_name, string full_name);
    record User(int user_id, string password, Person person);

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

        // STRINGS
        // ---------------------------------------------------------
        Console.WriteLine("\nSTRINGS");
        Console.WriteLine("\nBasic");
        print_string("Hello");

        Console.WriteLine("\nTry extended characters");
        print_string("« Sin música, la vida sería un error »");

        // ** ok: but moore complexity
        // print_string("« Esto es un árbol »".Utf8Text());

        // getting a string from lib
        Console.WriteLine("\nTry to get a string from library");

        var p = string_test();
        var text = Marshal.PtrToStringUTF8(p);

        Console.WriteLine("Encode String : {0}", p);
        Console.WriteLine("Decode String : {0}", text);

        p = describe_person(18);
        text = Marshal.PtrToStringUTF8(p);
        Console.WriteLine("describe_person(18) : {0}", text);

        // COMPOSED OBJECTS
        // ---------------------------------------------------------
        Console.WriteLine("\nCOMPOSED OBJECTS");
        
        var jsPointer = get_user(79);
        var js = jsPointer.TextFromPointer() ?? string.Empty;
        var user = JsonSerializer.Deserialize<User>(js);

        Console.WriteLine("JSON data obtained from the library:\n{0}\n",  js.PrettyJson());
        Console.WriteLine("Deserialized:");
        Console.WriteLine("User identifier : {0}", user?.user_id);
        Console.WriteLine("User first name : {0}", user?.person.first_name);
        Console.WriteLine("User last name  : {0}", user?.person.last_name);
    }
}


class RustTestStruct
{
    [StructLayout(LayoutKind.Sequential)]
    struct Parallelepiped
    {
        public float length;
        public float width;
        public float height;
    }

    const string RUSTLIB = @"..\..\..\..\rstlib\target\release\rust_library.dll";

    [DllImport(RUSTLIB)] static extern Parallelepiped get_any_parallelepiped();

    [DllImport(RUSTLIB)] static extern float get_parallelepiped_volume(Parallelepiped p);

    public static void Run()
    {
        Console.WriteLine("STRUCT SAMPLE");
        // call rust functions
        var parallelepiped = get_any_parallelepiped();
        var volume = get_parallelepiped_volume(parallelepiped);
        // show it
        Console.WriteLine("Length : {0}", parallelepiped.length);
        Console.WriteLine("Width  : {0}", parallelepiped.width);
        Console.WriteLine("Height : {0}", parallelepiped.height);
        Console.WriteLine("Volume : {0:N2}", volume);
    }
}
#endregion
