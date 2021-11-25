// 
namespace ConsoleApp;

class RustEvents
{
    // event
    delegate void RaiseNumber(int number);

    public void Run()
    {
        Console.WriteLine("\nRunning Sample Rust");

        // call a rust method
        UnmanagedPrompt(OnRaiseNumber);
    }

    private void OnRaiseNumber(int number)
    {
        Console.WriteLine($"arrives extern number: {number}");
    }

    // extern ----------------------------------------------------------------------
    [DllImport(App.RLIB)] static extern void UnmanagedPrompt(RaiseNumber cppCallback);
}

unsafe class RustEventsWasm
{
    readonly delegate* unmanaged<int, void> OnRaiseNumberPointer = &OnRaiseNumber;

    public void Run()
    {
        Console.WriteLine("\nRunning Sample Rust");

        // call rust method
        UnmanagedPrompt((IntPtr)OnRaiseNumberPointer);
    }

    [UnmanagedCallersOnly]
    private static void OnRaiseNumber(int number)
    {
        Console.WriteLine($"Arrives external number: {number}");
    }

    // extern ---------------------------------------------------------------
    [DllImport(App.RLIB)] static extern void UnmanagedPrompt(IntPtr notify);
    // theory
    // [DllImport(CLIB)] static extern void UnmanagedPrompt(delegate *unmanaged<int,void> cppCallback);
}