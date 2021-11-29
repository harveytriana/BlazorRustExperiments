namespace ConsoleApp;

using static Global;

class RustStruct
{
    // C# 10
    record struct Parallelepiped(float length, float width, float height);
    /*
    // C# 
    [StructLayout(LayoutKind.Sequential)]
    struct Parallelepiped
    {
        public float length {get ;set; };
        public float width {get ;set; };
        public float height {get ;set; };
    }
    */

    [DllImport(RLIB)] static extern Parallelepiped get_parallelepiped();
    [DllImport(RLIB)] static extern IntPtr get_parallelepiped_ptr();
    [DllImport(RLIB)] static extern float get_parallelepiped_volume(Parallelepiped p);

    public static void Run()
    {
        WriteLineColor("STRUCT SAMPLE", ConsoleColor.White);
        Console.WriteLine("\nGetting a Rust's struct:");
        // call rust functions
        var parallelepiped = get_parallelepiped();
        var volume = get_parallelepiped_volume(parallelepiped);

        Console.WriteLine("Deferenced object: {0}", parallelepiped);
        Console.WriteLine("\nCalling a Rust fuction wih a C# struct as parameter:");
        Console.WriteLine("Volume       : {0:N2}", volume);
        // struct create from C#
        Console.WriteLine("Volume Other : {0:N2}", get_parallelepiped_volume(new Parallelepiped(1, 2, 3)));

        WriteLineColor("\nUsing Pointers", ConsoleColor.Cyan);
        var ptr = get_parallelepiped_ptr();
        var p = Marshal.PtrToStructure<Parallelepiped>(ptr);
        Console.WriteLine("Pointer     : {0}", ptr);
        Console.WriteLine("Deferenced  : {0}", p);
    }
}
