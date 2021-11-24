namespace ConsoleApp;

class RustAndStruct
{
    // C# 10
    record struct Parallelepiped(float length, float width, float height);
    /*
    Before C# 10
    [StructLayout(LayoutKind.Sequential)]
    struct Parallelepiped
    {
        public float length {get ;set; };
        public float width {get ;set; };
        public float height {get ;set; };
    }
    */

    [DllImport(App.RLIB)] static extern Parallelepiped get_parallelepiped();

    [DllImport(App.RLIB)] static extern float get_parallelepiped_volume(Parallelepiped p);
    
    [DllImport(App.RLIB)] static extern IntPtr get_parallelepiped_ptr();

    public static void Run()
    {
        Console.WriteLine("\nSTRUCT SAMPLE");
        Console.WriteLine("\nGetting a Rust's struct:");
        // call rust functions
        var parallelepiped = get_parallelepiped();
        var volume = get_parallelepiped_volume(parallelepiped);

        // show it
        Console.WriteLine("Length : {0}", parallelepiped.length);
        Console.WriteLine("Width  : {0}", parallelepiped.width);
        Console.WriteLine("Height : {0}", parallelepiped.height);

        Console.WriteLine("\nCalling a Rust fuction wih a C# struct as parameter:");
        Console.WriteLine("Volume : {0:N2}", volume);

        Console.WriteLine("\nUsing Pointers");
        var pointer = get_parallelepiped_ptr();
        var p = Marshal.PtrToStructure<Parallelepiped>(pointer);
        Console.WriteLine("Deferenced object: {0}", p);
    }
}
