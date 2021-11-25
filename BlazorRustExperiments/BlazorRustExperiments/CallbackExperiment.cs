using System.Runtime.InteropServices;

namespace BlazorRustExperiments;

public unsafe class CallbackExperiment
{
    // echo to consumer
    public delegate Task EchoHandler(string message);
    public static EchoHandler Echo;

    static readonly delegate* unmanaged<int, void> OnRaiseNumberPointer = &OnRaiseNumber;

    public static void Run()
    {
        Console.WriteLine("\nRunning C++");

        // call rust method
        UnmanagedPrompt((IntPtr)OnRaiseNumberPointer);
        // ** theory
        // UnmanagedPrompt(OnRaiseNumberPointer);
    }

    [UnmanagedCallersOnly]
    private static void OnRaiseNumber(int number)
    {
        Echo?.Invoke($"Arrives external number: {number}");
    }

    // extern ---------------------------------------------------------------
    [DllImport(Global.RLIB)] static extern void UnmanagedPrompt(IntPtr notify);
    // ** theory
    // [DllImport(Global.RLIB)] static extern void UnmanagedPrompt(delegate *unmanaged<int,void> notify);
}