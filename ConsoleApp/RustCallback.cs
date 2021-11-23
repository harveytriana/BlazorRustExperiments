class RustCallback
{
    const string RSTLIB = @"..\..\..\..\rstlib\target\release\rstlib.dll";

    public delegate int Fn(int number);

    [DllImport(RSTLIB)] static extern int c_operation(int number, Fn fn);
    [DllImport(RSTLIB)] static extern int cube(int number);
    [DllImport(RSTLIB)] static extern int square(int number);

    static int Square(int number) => number * number;
    static int Cube(int number) => number * number * number;

    public static void Run()
    {
        Console.WriteLine("Passing a C# function as parameter of a Rust function");
        Console.WriteLine("c_operation(5, Square)  : {0}", c_operation(5, Square));
        Console.WriteLine("c_operation(3, Cube)    : {0}", c_operation(3, Cube));
        Console.WriteLine("------------------------------------------------------");
        Console.WriteLine("Passing a rust funcion as parameter of a Rust function");
        Console.WriteLine("c_operation(5, square)  : {0}", c_operation(5, square));
        Console.WriteLine("c_operation(3, cube)    : {0}", c_operation(3, cube));

        // Output
        // c_operation(5, Square) : 25
        // c_operation(3, Cube)   : 27
        // c_operation(5, cube)   : 125
    }
}

