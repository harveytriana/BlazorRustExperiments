using System.Runtime.InteropServices;

namespace BlazorRustExperiments;

//TODO works?
public unsafe class Temporary
{
    delegate int Fn(int number);
    delegate* unmanaged<int, int> _handleCube = &Cube;

    // works in console, in wasm is complex
    // static int Square(int number) => number * number;
    // static int Cube(int number) => number * number * number;


    [UnmanagedCallersOnly]
    private static int Cube(int number)
    {
        return number * number * number;
    }

}

