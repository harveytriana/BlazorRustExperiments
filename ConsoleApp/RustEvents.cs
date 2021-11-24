// 
namespace ConsoleApp;

class RustEvents
{
    // event
    public delegate void RaiseNumber(int number);

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
