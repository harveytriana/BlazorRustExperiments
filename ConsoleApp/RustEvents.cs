// 
namespace ConsoleApp;

class RustEvents
{
    // event
    public delegate void ECallback(int number);

    public void Run()
    {
        Console.WriteLine("\nRunning Sample Rust");

        // call a rust method
        UnmanagedPrompt(DoSomenthingWithNumber);
    }

    private void DoSomenthingWithNumber(int number)
    {
        Console.WriteLine($"arrives extern number: {number}");
    }

    // extern ----------------------------------------------------------------------
    [DllImport(App.RLIB)] static extern void UnmanagedPrompt(ECallback cppCallback);
}
