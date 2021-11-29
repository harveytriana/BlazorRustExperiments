/*
 * BlazorSpread's Article
*/
//! based on Flang / LLVM in the Emscripten style of Alon Zakai.

using System.Runtime.InteropServices;

namespace BlazorRustExperiments;

using static Global;

public delegate Task EchoHandler(string message);

public unsafe class CallbackEvent
{
    public static EchoHandler Echo;

    static readonly delegate* unmanaged<int, void> OnRaiseNumberPointer = &OnRaiseNumber;

    public static void RunEvents(int count)
    {
        Console.WriteLine("\nRunning Rust...");
        unmanaged_prompt((IntPtr)OnRaiseNumberPointer, count);
    }

    [UnmanagedCallersOnly]
    private static void OnRaiseNumber(int number)
    {
        Echo?.Invoke($"Arrives external number: {number}");
    }

    // extern -----------------------------------------------------------------------------
    [DllImport(RLIB)] static extern void unmanaged_prompt(IntPtr notify, int count);
}

unsafe class ExecuteFunctions
{
    public static EchoHandler Echo;

    public static void Run()
    {
        Echo?.Invoke("Passing a C# fucntion as delegate pointer to Rust");
        float x = 2;
        Echo?.Invoke(string.Format("execute(square, {0})    : {1}", x, execute_fn_f32((IntPtr)OnCube, x)));
        Echo?.Invoke(string.Format("execute(*CubeRoot, {0}) : {1}", x, execute_fn_f32((IntPtr)OnCubeRoot, x)));
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