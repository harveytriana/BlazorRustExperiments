
class RustStruct
{
    [StructLayout(LayoutKind.Sequential)]
    struct Parallelepiped
    {
        public float length;
        public float width;
        public float height;
    }

    const string RUSTLIB = @"..\..\..\..\rstlib\target\release\rstlib.dll";

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
