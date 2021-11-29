// 
namespace ConsoleApp;

using static Global;

class RustEvents
{
    // event
    delegate void PromptHandler(int number);

    public void Run()
    {
        WriteLineColor("\nEVENTS", ConsoleColor.Cyan);
        unmanaged_prompt(OnRaiseNumber, 12);
    }

    private void OnRaiseNumber(int number)
    {
        Console.WriteLine($"arrives extern number: {number}");
    }

    // extern ----------------------------------------------------------------------
    [DllImport(RLIB)] static extern void unmanaged_prompt(PromptHandler fn, int count);
}

unsafe class RustEventsWasm
{
    readonly delegate* unmanaged<int, void> OnRaiseNumberPointer = &OnRaiseNumber;

    public void Run()
    {
        WriteLineColor("\nEVENTS", ConsoleColor.Cyan);
        unmanaged_prompt(OnRaiseNumberPointer, 12);
    }

    [UnmanagedCallersOnly]
    private static void OnRaiseNumber(int number)
    {
        Console.WriteLine($"Arrives external number: {number}");
    }

    [DllImport(RLIB)] 
    static extern void unmanaged_prompt(delegate* unmanaged<int, void> notify, int count);
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