namespace ConsoleApp;

using static Global;

class RustCallback
{
    delegate float Fn(float x);

    static float Square(float x) => x * x;
    static float Cube(float x) => x * x * x;

    public static void Run()
    {
        float x = 7.0F;
        WriteLineColor("\nCALLBACKS", ConsoleColor.DarkCyan);
        Console.WriteLine("Passing a C# function as parameter of a Rust function");
        Console.WriteLine("------------------------------------------------------");
        Console.WriteLine("execute(Square, {0})  : {1}", x, execute_fn_f32(Square,x));
        Console.WriteLine("execute(Cube, {0})    : {1}", x, execute_fn_f32(Cube, x));
        Console.WriteLine();
        Console.WriteLine("Passing a rust funcion as parameter of a Rust function");
        Console.WriteLine("------------------------------------------------------");
        Console.WriteLine("execute(square, {0})  : {1}", x, execute_fn_f32(square, x));
        Console.WriteLine("execute(cube, {0})    : {1}", x, execute_fn_f32(cube, x));
    }
    // references
    [DllImport(RLIB)] static extern float execute_fn_f32(Fn handle, float x);
    [DllImport(RLIB)] static extern float cube(float x);
    [DllImport(RLIB)] static extern float square(float x);
}

/// <summary>
/// For wasm, Emscripten works with pointers in callbacks
/// </summary>
unsafe class RustCallbackWasm
{
    public void Run()
    {
        WriteLineColor("\nRust Callback Wasm".ToUpper(), ConsoleColor.Cyan);

        float x = 2;
        Console.WriteLine("execute(*Cube, {0})     : {1}", x, execute_fn_f32((IntPtr)OnCube, x));
        Console.WriteLine("execute(*CubeRoot, {0}) : {1}", x, execute_fn_f32((IntPtr)OnCubeRoot, x));
    }

    const double THIRD = 1.0 / 3.0;

    [UnmanagedCallersOnly] static float Cube(float x) => x * x * x;
    [UnmanagedCallersOnly] static float CubeRoot(float x) => (float)Math.Pow(x, THIRD);

    // to handle or pointer delgate
    static readonly delegate* unmanaged<float, float> OnCube = &Cube;
    static readonly delegate* unmanaged<float, float> OnCubeRoot = &CubeRoot;

    [DllImport(RLIB)] static extern float execute_fn_f32(IntPtr handle, float x);
}