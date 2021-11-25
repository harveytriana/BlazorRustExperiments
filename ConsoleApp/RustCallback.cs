namespace ConsoleApp;

class RustCallback
{
    public delegate float Fn(float x);

    [DllImport(Global.RLIB)] static extern float c_operation(float x, Fn fn);
    [DllImport(Global.RLIB)] static extern float cube(float x);
    [DllImport(Global.RLIB)] static extern float square(float x);

    static float Square(float x) => x * x;
    static float Cube(float x) => x * x * x;

    public static void Run()
    {
        float x = 7.0F;

        Console.WriteLine("Passing a C# function as parameter of a Rust function");
        Console.WriteLine("------------------------------------------------------");
        Console.WriteLine("c_operation({0}, Square)  : {1}", x, c_operation(x, Square));
        Console.WriteLine("c_operation({0}, Cube)    : {1}", x, c_operation(x, Cube));
        Console.WriteLine();
        Console.WriteLine("Passing a rust funcion as parameter of a Rust function");
        Console.WriteLine("------------------------------------------------------");
        Console.WriteLine("c_operation({0}, square)  : {1}", x, c_operation(x, square));
        Console.WriteLine("c_operation({0}, cube)    : {1}", x, c_operation(x, cube));
        // Output
        // c_operation(5, Square) : 25
        // c_operation(3, Cube)   : 27
        // c_operation(5, cube)   : 125
    }
}

