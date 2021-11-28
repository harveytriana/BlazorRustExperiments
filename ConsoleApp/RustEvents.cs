// 
namespace ConsoleApp;

using static Global;

class RustEvents
{
    // event
    delegate void RaiseNumber(int number);

    public void Run()
    {
        WriteLineColor("\nEVENTS", ConsoleColor.Cyan);
        // call a rust method
        UnmanagedPrompt(OnRaiseNumber);
    }

    private void OnRaiseNumber(int number)
    {
        Console.WriteLine($"arrives extern number: {number}");
    }

    // extern ----------------------------------------------------------------------
    [DllImport(RLIB)] static extern void UnmanagedPrompt(RaiseNumber fn);
}

unsafe class RustEventsWasm
{
    readonly delegate *unmanaged<int, void> OnRaiseNumberPointer = &OnRaiseNumber;

    public void Run()
    {
        WriteLineColor("\nEVENTS", ConsoleColor.Cyan);
        // call rust method
        UnmanagedPrompt(OnRaiseNumberPointer);
    }

    [UnmanagedCallersOnly]
    private static void OnRaiseNumber(int number)
    {
        Console.WriteLine($"Arrives external number: {number}");
    }

    // extern ---------------------------------------------------------------
    // [DllImport(App.RLIB)] static extern void UnmanagedPrompt(IntPtr notify);
    // **
    [DllImport(RLIB)] static extern void UnmanagedPrompt(delegate *unmanaged<int,void> notify);
}

class RustEvents2
{
    delegate void Notify(int number);
    delegate int Fn(int x);

    public void Run()
    {
        WriteLineColor("\nEVENTS", ConsoleColor.Cyan);
        // call a rust method
        get_serie(OnNotify, Cube, 1, 10);
    }

    int Cube(int x)
    {
        return x * x * x;
    }

    private void OnNotify(int number)
    {
        Console.WriteLine($"arrives extern number: {number}");
    }

    // extern ----------------------------------------------------------------------
    [DllImport(RLIB)] static extern void get_serie(Notify notify, Fn fn, int x1, int x2);
}