namespace ConsoleApp;

using static Global;

class RustCallback
{
    public delegate float Fn(float x);

    static float Square(float x) => x * x;
    static float Cube(float x) => x * x * x;

    public static void Run()
    {
        float x = 7.0F;
        WriteLineColor("\nCALLBACKS", ConsoleColor.DarkCyan);
        Console.WriteLine("Passing a C# function as parameter of a Rust function");
        Console.WriteLine("------------------------------------------------------");
        Console.WriteLine("c_operation({0}, Square)  : {1}", x, c_operation(x, Square));
        Console.WriteLine("c_operation({0}, Cube)    : {1}", x, c_operation(x, Cube));
        Console.WriteLine();
        Console.WriteLine("Passing a rust funcion as parameter of a Rust function");
        Console.WriteLine("------------------------------------------------------");
        Console.WriteLine("c_operation({0}, square)  : {1}", x, c_operation(x, square));
        Console.WriteLine("c_operation({0}, cube)    : {1}", x, c_operation(x, cube));
        //
        WriteLineColor($"execute_fn_f32({x}, cube)    : {execute_fn_f32(cube, x)}", ConsoleColor.DarkCyan);
        WriteLineColor($"execute_fn_f32({x}, cube)    : {execute_fn_f32(Cube, x)}", ConsoleColor.DarkCyan);
    }

    [DllImport(RLIB)] static extern float c_operation(float x, Fn fn);
    [DllImport(RLIB)] static extern float cube(float x);
    [DllImport(RLIB)] static extern float square(float x);
    //
    [DllImport(RLIB)] static extern float execute_fn_f32(Fn handle, float x);
}

// Advanced
unsafe class RustCallbackWasm
{
    public void Run()
    {
        WriteLineColor("\nRust Callback Wasm".ToUpper(), ConsoleColor.Cyan);

        float x = 2;
        Console.WriteLine("execute_fn_f32(*Cube, {0}) = {1}", x, execute_fn_f32((IntPtr)OnCube, x));
        Console.WriteLine("execute_fn_f32(*CubeRoot, {0}) = {1}", x, execute_fn_f32((IntPtr)OnCubeRoot, x));
    }

    const double THIRD = 1.0 / 3.0;

    [UnmanagedCallersOnly] static float Cube(float x) => x * x * x;
    [UnmanagedCallersOnly] static float CubeRoot(float x) => (float)Math.Pow(x, THIRD);

    // to handle or pointer delgate
    static readonly delegate* unmanaged<float, float> OnCube = &Cube;
    static readonly delegate* unmanaged<float, float> OnCubeRoot = &CubeRoot;

    // caviar.rs
    [DllImport(RLIB)] static extern float execute_fn_f32(IntPtr handle, float x);
}