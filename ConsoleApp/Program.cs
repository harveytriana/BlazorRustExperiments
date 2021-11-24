using ConsoleApp;

Console.WriteLine("CALLING RUST EXPERIMENTS\n");

RustMiscellany.Run();
RustAndStruct.Run();
RustAndJson.Run();
RustCallback.Run();

new RustEvents().Run();

Console.WriteLine("\nReady");
