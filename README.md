# Rust in Blazor WebAssembly

*A concise guide on how to use a Rust library in Blazor WASM*

## Summary

It is possible to build a Rust assembly to Emscripten-based Web technologies and use it in Blazor WASM. In this post I am going to outline a basic outline of how to code to establish that symbiosis between the Rust world and the C# world.

I do not claim to teach how to use Rust. If you do not know this programming language I suggest the guide documented in references 1 and 2.

## Introduction

It would be redundant to write about the qualities of Rust, and talk about why it has piqued the interest of many professional developers. My interest is solidified when I know that it can be used as a native reference in Blazor. That Steve Sanderson tweet where he says: *Using the new native dependencies feature for Blazor WebAssembly in .NET6, I was able to call Rust from C#, both running in the browser! … Now, what can we do with this new power?*, was an inspiration that led me to delve into the subject.

The Rust - C# relation is given. The first external function, the classic *Hello World!* it can not miss. The decorator indicates that it is to be used externally,

```csharp
#[no_mangle]
pub fn hello_world() {
    println!("Hello World!");
}
```

From C# we can use this function through Platform Invocation (P/Invoke), something like:

```csharp
using System.Runtime.InteropServices;
using static Global;

class RustMiscellany
{
    [DllImport(RLIB)] static extern void hello_world();

    public static void Run()
    {
        hello_world();
    }
}
```

Where RLIB is the path of the Rust library, and it exists in a global class of constants.

## Project architecture

The lab consists of a non-hosted Blazor WASM net6 solution, along with the Rust library. In the same .NET solution we add a net6 console project. In the same folder a folder is created where the Rust project will go. In this way, we can make relative reference to the paths in the libraries without having to make a copy during development. In general the console project is used as an agile code tester. I will not put much emphasis on the project, which you may well study in the Github post.

## Rust project creation

I assume that you already have the environment ready to program in Rust, if not, in reference 1 you will find the steps. It is simple to create a library, from the folder where you want your project run the following terminal command:

```
cargo new rstlib
```

After executing this command we have the base files of a Rust project. It is more productive if we do it from *vscode*,  with the *Rust Extension Pack* extension. (In the hope that in the not too distant future we can work on Rust from the Visual Studio 20x IDE).

To compile to wasm in Emscripten terms, which is what Wasm understands, we need to add Emscripten support to the Rust compiler. It will be installed using rustup. Reference 3 indicated the precise documentation, to summarize, you only need the first three commands.

```
rustup toolchain add stable
rustup target add asmjs-unknown-emscripten --toolchain stable
rustup target add wasm32-unknown-emscripten --toolchain stable
```

Compiling to emscripten requires a static library, for which we specify create-type = [staticlib] in the configuration file.

As a suggestion and ease of development, it is convenient to test our Rust code with C# support, from a C# console, then we can compile for Wasm and scale to Blazor. However, it is important to clarify that not all the C# code that runs in the console is directly supported by emscripten, there are cases in which it is necessary to work directly with pointers, which I also deal with in this article.

If we are going to work with a console, then the static library does not create a DLL on compilation, for which we exchange the type of crate from static to dynamic. When it's ready to use in wasm, we change the library type to static and compile. Strategically he left the two in the same position toml, and commented on one or the other depending on the case. This can be improved with a macro, but I don't want to extend beyond the scope of this writing.

The configuration file with the characteristics described is as follows

```python
[package]
name = "rstlib"
version = "0.1.0"
authors = ["Harvey Triana<harveytriana@gmail.com> "]

[lib]
name = "rstlib"

# WebAssembly for Emscripten (.a)
# ------------------------------------------------- ----------------
# compile: cargo build --target wasm32-unknown-emscripten --release
# crate-type = ["staticlib"]

# Dynamic library for C# Console (.dll)
# ------------------------------------------------- ----------------
# compile: cargo build --release
crate-type = ["dylib"]
```

## Interoperability

This article is aimed at programmers who know C#, and are starting with Rust, or already have experience. I'll cover the common programming logic cases with an example on each.

## Functions

A mathematical function that returns a native type, and uses native type parameters. Example, the following function returns the Hypotenuse given the two Legs.

```csharp
#[no_mangle]
pub fn hypotenuse(x: f32, y: f32) -> f32 {
    let num = x.powf(2.0) + y.powf(2.0);
    num.powf(0.5)
}
```

The C# code to use this function looks like this:

```csharp
[DllImport(RLIB)] static extern float hypotenuse(float x, float y);

float x = 9, y = 11, h = hypotenuse(x, y);

Console.WriteLine("From Rust Library, Hypotenuse({0}, {1}) = {2}", x, y, h);
```

Note that it is not necessary to use **extern "C"** in the signature of this function.

## Text Strings

Working with C/C++/Rust strings is not simple, as these languages are strict about their organization in memory. I've seen various strategies since C# to deal with this, some with too much code or unnecessary complexity. We work with pointers. I present here two ways to solve this.

To start this exercise, copy the following to [Play Rust](https://play.rust-lang.org/) and run it.

```csharp
fn main() {
    let quote = String::from("« All that we see or seem is but a dream within a dream. » EAP");
    println!("{}", quote);
    println!("{}", reverse(quote));
}

fn reverse(text: String) -> String {
    let s = text.chars().rev().collect::<String>();
    s
}
```

We are sending a String as a parameter, and receiving a String in the result How to use the ```reverse()``` function in C#? The problem is divided into two parts, sending and receiving Strings. To connect the Rust world with the C# world we need to work with pointers. We wrap the Rust function in a type and treatment that understands C, and likewise C#.

```csharp
use std::ffi::CStr;
use std::ffi::CString;
use std::os::raw::c_char;

fn reverse(text: String) -> String {
    let s = text.chars().rev().collect::<String>();
    s
}
// C wrap
#[no_mangle]
pub extern "C" fn reverse_inptr(text_pointer: *const c_char) -> *const c_char {
    let c_str = unsafe { CStr::from_ptr(text_pointer) };
    let r_str = c_str.to_str().unwrap();
    let text = r_str.to_string();
    let reversed = reverse(text);
    let raw = CString::new(reversed).expect("CString::new failed!");
    raw.into_raw()
}
```

It is somewhat complex, but it solves all cases of text exchange, even with special characters; later we can add a module in Rust to generalize the solution.

To use this function from C# We have two options.

First option, pass the encapsulated parameter in LPUTF8Str, and decode the return pointer with Marshal.PtrToStringUTF8. Following the example,

```csharp
[DllImport(RLIB)]
static extern IntPtr reverse_inptr([MarshalAs(UnmanagedType.LPUTF8Str)] string text);

var quote = "« All that we see or seem is but a dream within a dream. » EAP";
var p = reverse_inptr(quote);
var quoteReversed = Marshal.PtrToStringUTF8(p);

Console.WriteLine($"Quote         : {quote}", ConsoleColor.Yellow);
Console.WriteLine($"Reverse Quote : {quoteReversed}", ConsoleColor.Yellow);
```

Second option, pass the text string as a UTF8 byte array.

```csharp
[DllImport(RLIB)]
static extern IntPtr reverse_inptr(byte[] utf8Text);

var quote = "« All that we see or seem is but a dream within a dream. » EAP";

var bytes = Encoding.UTF8.GetBytes(quote);
var p = reverse_inptr(bytes);
var quoteReversed = Marshal.PtrToStringUTF8(p);

Console.WriteLine($"Quote         : {quote}");
Console.WriteLine($"Reverse Quote : {quoteReversed}");
```

In both cases the result is the same:

```
Quote: «All that we see or seem is but a dream within a dream. »EAP
Reverse Quote: PAE ».maerd a nihtiw maerd a tub si mees ro ees ew taht llA«
```

When to use one or the other? I think the criteria is as you prefer. One or the other case can be simplified with C# extensions; in the project it is displayed that way.

The use of this strategy for exchanging text strings in Blazor WASM is the same, no special handling is required.

## Structures

Let's define any structure, with native types, and a method that returns an instance of it, and, likewise, a function that uses this structure in one of its parameters:

```csharp
#[repr(C)]
pub struct Parallelepiped {
    pub length: f32,
    pub width: f32,
    pub height: f32,
}

#[no_mangle]
pub extern "C" fn get_parallelepiped() -> Parallelepiped {
    Parallelepiped {// random for example
        length: 1.7,
        width: 2.2,
        height: 1.9,
    }
}

#[no_mangle]
pub extern "C" fn get_parallelepiped_volume(p: Parallelepiped) -> f32 {
    let volume = p.length * p.width * p.height;
    volume
}
```

The C# code to read a structure, and pass a structure as a parameter is the following:

```csharp
/// C# 10
record struct Parallelepiped(float length, float width, float height);

[DllImport(RLIB)] static extern Parallelepiped get_parallelepiped();
[DllImport(RLIB)] static extern float get_parallelepiped_volume(Parallelepiped p);

var parallelepiped = get_parallelepiped();
var volume = get_parallelepiped_volume(parallelepiped);

// can create the struct in C# and use as parameter in Rust
get_parallelepiped_volume(new Parallelepiped(1, 2, 3)));
```

I noticed that a **record struct** is being used, instead of a classic struct; less code, higher productivity. You can also create the classic structure and use it here, however you must add a [StructLayout (LayoutKind.Sequential)] decorator.

### Surround Work to use in WASM

We can pass the structure as a parameter, as described in the previous paragraph, but not receive it in the same way. The compiled Emscripten leaves no availability for that; a "function signature mismatch" exception is thrown. However, strategically we can solve it through pointers.

We add a function in Rust that wraps the function with return of a memory address, and we use a static variable in the module.

```csharp
static mut _P: Parallelepiped = Parallelepiped {
    length: 0.0,
    width: 0.0,
    height: 0.0,
};

#[no_mangle]
pub unsafe extern "C" fn get_parallelepiped_ptr() -> *const Parallelepiped {
    _P = get_parallelepiped();
    &_P
}
```

In C#:

```csharp
[DllImport(RLIB)] static extern IntPtr get_parallelepiped_ptr();

var ptr = get_parallelepiped_ptr();
var p = Marshal.PtrToStructure<Parallelepiped>(ptr);

Console.WriteLine("Pointer     : {0}", ptr);
Console.WriteLine("Deferenced  : {0}", p);
```

This solution is effective, although caution must be exercised, since the existence of a static variable in the library is required. Although, since it is Wasm, the only client is the viewer. As I mentioned, it is a strategic solution, - If you suggest a better solution, excellent; please share it.

> The complexity is that `get_parallelepiped_ptr()` returns an address of the local instance of the structure but its lifetime ends with the return of the function. That is undefined behavior in C, C++, or Rust.

## JSON transfer

JSON pass-through solves all type complexity, even types with nested depth. To use JSON in Rust we need an external library or external crate, for this we add the dependency to the configuration file, Cargo.toml:

```
[dependencies]
serde = {version = "1.0.126", features = ["derive"]}
serde_json = "1.0.64"
```

Using JSON in Rust is simple. I created a nested structure to demonstrate it. Let's look at the example.

```csharp
extern crate serde;
extern crate serde_json;

use serde::{Deserialize, Serialize};
use std::ffi::CStr;
use std::ffi::CString;
use std::os::raw::c_char;

// STRUT SAMPLE
#[derive(Serialize, Deserialize, Debug)]
pub struct Person {
    person_id: i32,
    first_name: String,
    last_name: String,
    age: i32,
}
impl Person {
    fn full_name(&self) -> String {
        let s = format!("{} {}", self.first_name, self.last_name);
        s
    }
}

#[derive(Serialize, Deserialize, Debug)]
pub struct User {
    user_id: i32,
    password: String,
    person: Person,
}
// RETURN JSON
#[no_mangle]
pub fn get_user(user_id: i32) -> *const c_char {
    // dummy entity
    let user = User {
        user_id: user_id, // simulate
        password: "hashed password".to_string(),
        person: Person {
            person_id: 79296125,
            first_name: "Karl".to_string(),
            last_name: "Sagan".to_string(),
            age: 33,
        },
    };
    let json = serde_json::to_string(&user).unwrap();
    // to C#
    let encode = CString::new(json).expect("CString::new failed!");
    encode.into_raw()
}
// JSON AS ARGUMENT
#[no_mangle]
pub fn post_user(json_pointer: *const c_char) {
    let c_str = unsafe { CStr::from_ptr(json_pointer) };
    let r_str = c_str.to_str().unwrap();
    let json = r_str.to_string();

    //deserialize and print
    let user: User = serde_json::from_str(&json).unwrap();

    println!("User is a Rust's Structure:");
    println!("{:?}", user);
    //  do something...
}
```

As you can see, it is just transferring and reading text, some decorators, plus the use of Rust's serialization / deserialization tool.

In C# we can use a class, even with the coding conventions of C#.

```csharp
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ConsoleApp;
using static Global;

class RustJson
{
    [DllImport(RLIB)]
    static extern IntPtr get_user(int user_id);
    [DllImport(RLIB)]
    static extern void post_user([MarshalAs(UnmanagedType.LPWStr)] string userJson);

    class Person
    {
        [JsonPropertyName("person_id")]  public int Id { get; set; }
        [JsonPropertyName("first_name")] public string? FirstName { get; set; }
        [JsonPropertyName("last_name")]  public string? LastName { get; set; }
        [JsonPropertyName("age")]        public int Age { get; set; }
    }
    class User
    {
        [JsonPropertyName("user_id")]    public int Id { get; set; }
        [JsonPropertyName("person")]     public Person? Person { get; set; }
        [JsonPropertyName("password")]   public string? Password { get; set; }
    }

    public static void Run()
    {
        Console.WriteLine("\nCOMPOSED OBJECTS\n");

        var jsPointer = get_user(79);
        var js = jsPointer.TextFromPointer() ?? string.Empty;
        var user = JsonSerializer.Deserialize<User>(js);

        Console.WriteLine("JSON data obtained from the library:\n{0}\n", js.PrettyJson());
        Console.WriteLine("Deserialized:");
        Console.WriteLine("User identifier : {0}", user?.Id);
        ...
        // sending json
        var alisson = new User {
            Id = 789456123,
            Password = "hashed password",
            Person = new Person {
                Id = 123,
                FirstName = "Alisson Johana",
                LastName = "Triana",
                Age = 18,
            }
        };
        var alissonJson = JsonSerializer.Serialize(alisson);

        Console.WriteLine("\nWill send:\n{0}\n", alissonJson.PrettyJson());
        try {
            post_user(alissonJson);
        }
        catch { }
    }
}
```

## Callbacks

I will use the classic case where to a function another function as a parameter. That parameter is a delegate that allows you to point to different functions that the delegate's signature fulfills. For example, a function f(x) to do a mathematical calculation, for example, Square, Cube, Cube Root, and so on. These first-order functions can be written in Rust or in C#.

```csharp
// floats: f(x). Use: execute(<delegate function>, x)
#[no_mangle]
pub extern "C" fn execute_fn(
    // callback
    callback: extern "C" fn(f32) -> f32,
    // parameters
    x: f32,
) -> f32 {
    let y = callback(x);
    y
}

#[no_mangle]
fn cube(x: f32) -> f32 {
    x * x * x
}

#[no_mangle]
fn square(x: f32) -> f32 {
    x * x
}
```

On the C# side, we use this as follows:

```csharp
[DllImport(RLIB)] static extern float execute_fn(Fn handle, float x);
[DllImport(RLIB)] static extern float cube(float x);
[DllImport(RLIB)] static extern float square(float x);

delegate float Fn(float x);

float x = 7.0F;
Console.WriteLine("Passing a rust function as parameter of a Rust function");
Console.WriteLine("execute(square, {0})  : {1}", x, execute_fn(square, x));
Console.WriteLine("execute(cube, {0})    : {1}", x, execute_fn(cube, x));
```

Now, if we want to pass a C# function to the Rust library, it is also possible. I'm going to write the same functions, but on the C# side and invoke them indirectly in Rust.

```csharp
static float Square(float x) => x * x;
static float Cube(float x) => x * x * x;

float x = 7.0F;
Console.WriteLine("Passing a C# function as parameter of a Rust function");
Console.WriteLine("execute(Square, {0})  : {1}", x, execute_fn(Square, x));
Console.WriteLine("execute(Cube, {0})    : {1}", x, execute_fn(Cube, x));
```

### Surround Work to use in WASM

While we can pass a Rust function to the Rust method from C# in Wasm, it is not allowed to pass a C# function directly. The solution again is to work with pointers, in this case it is more advanced because we need to work in unsafe mode in C#, and decorate the function with [UnmanagedCallersOnly]. Basically we convert the delegate to a memory address, which we pass to the Rust method.

```csharp
unsafe {
[DllImport(RLIB)] static extern float execute_fn(IntPtr handle, float x);

[UnmanagedCallersOnly] static float Cube(float x) => x * x * x;
static readonly delegate* unmanaged<float, float> OnCube = &Cube;

float x = 2;
Console.WriteLine("execute(*Cube, {x}): {execute_fn((IntPtr)OnCube, x)}");
}
```

## Events

The events involve delegates. That is, the case is similar to the previous one. In this example, I am going to write a function in Rust that receives a notification function and invokes it every certain time, the caller receives the data inadvertently.

```csharp
use std::thread::sleep;
use std::time::Duration;

// event (~> delegate in C#)
pub type PromptHandler = Option<unsafe extern "C" fn(_: i32) -> ()>;

#[no_mangle]
pub unsafe extern "C" fn unmanaged_prompt(
    // callback
    notify: PromptHandler,
    count: i32,
) {
    let mut i = 1;
    while i <= count {
        notify.expect("non-null function pointer")(i);
        // simulate
        sleep(Duration::from_millis(200));
        // return
        i += 1
    }
}
```

In C#:

```csharp
delegate void PromptHandler(int number);

public void Run()
{
    unmanaged_prompt(OnRaiseNumber, 12);
}

private void OnRaiseNumber(int number)
{
    Console.WriteLine($"arrives extern number: {number}");
}

// extern
[DllImport(RLIB)] static extern void unmanaged_prompt(PromptHandler fn, int count);
```

### Surround Work to use in WASM

As we already know, we cannot pass C# methods or functions directly to a Rust library compiled for Wasm. The strategy again uses pointers, this case being equivalent to the previous one.

```csharp
unsafe class RustEventsWasm
{
    readonly delegate* unmanaged<int, void> OnRaiseNumberPointer = &OnRaiseNumber;

    public void Run()
    {
        unmanaged_prompt((IntPtr)OnRaiseNumberPointer, 12);
    }

    [UnmanagedCallersOnly]
    private static void OnRaiseNumber(int number)
    {
        Console.WriteLine($"Arrives external number: {number}");
    }

    [DllImport(RLIB)] static extern void unmanaged_prompt(IntPtr notify, int count);
}
```

In Blazor, the case imposes one more challenge, and that is that the strategy necessarily uses static, which from a page that is not static requires some special treatment. One way is to isolate the logic in a static class, and via an event notify the page. Cautions should be taken in handling this scheme so as not to leave lost links.

## Blazor

We compile for Emscripten, for this we change the type of library to static and compile with the corresponding command. This generates a .a file which is the one we add as a native reference to the Blazor project. See the comments in the Cargo.toml file for details.

The Blazor project file contains the following directives:

```
<PropertyGroup>
<AllowUnsafeBlocks>true</AllowUnsafeBlocks>
<WasmBuildNative>true</WasmBuildNative>
</PropertyGroup>

<ItemGroup>
<NativeFileReference Include="..\..\rstlib\target\wasm32-unknown-emscripten\release\librstlib.a" />
</ItemGroup>
```

The path of the native reference corresponds to the fact that it exists in the same directory as the Blazor solution, and we will not have to be moving the librstlib.a file.

In each case treated here I have provided an example in Blazor, distributed in the pages of the SPA. In the special cases of unsafe code, I created a class with the statics, and through an event you can read responses from the events or callbacks. In general it would be unusual if we would want to pass functions from C# to Rust in Wasm, however here we show how it can be done. You can check the repository here: [BlazorRustExperiments](https://github.com/harveytriana/BlazorRustExperiments)

The path of the native reference corresponds to the fact that it exists in the same directory as the Blazor solution, and we will not have to be moving the librstlib.a file. Furthermore, I add that path to a global constants file,

```csharp
namespace BlazorRustExperiments;

public static class Global
{
    public const string RLIB = "librstlib";
}
```

To improve the design, add the image that represents Rust on the cover, and the logo. To present results I used a Blazor component written to emulate a console output.

![Index](https://github.com/harveytriana/BlazorRustExperiments/blob/master/Screens/index.png)

The Board component driving results:

![Board](https://github.com/harveytriana/BlazorRustExperiments/blob/master/Screens/bz_cb.png)

The development possibilities are immense, we work naturally from C#, calling Rust code. Simply great.

## Conclusions

How far will this new power take us? The evolution of Blazor is certainly overwhelming, however, the characteristic of native dependencies is notoriously relevant. Since Denis Roth's blog post, *Native dependency support for Blazor WebAssembly applications*, Blazor developers know this is going to take us a long way.

It is certainly not difficult to use Rust code in C#, in this article I will demonstrate several fundamental aspects of how that relationship can be established. An exhaustive mathematical process that should be executed on the client, can be solved from a native library with high performance.

---

### References

1. [Rust](https://www.rust-lang.org/)
2. [Rust first steps](https://docs.microsoft.com/en-us/learn/paths/rust-first-steps/)
3. [Rust and WebAssembly](https://www.hellorust.com/setup/emscripten/)
4. [Emscripten](https://emscripten.org/)
5. [Platform Invoke](https://docs.microsoft.com/en-us/dotnet/standard/native-interop/pinvoke)

---

Published: 2021-11-29

**Source**
- https://www.blazorspread.net/blogview/rust-in-blazor-webassembly
- Spanish: https://www.blazorspread.net/blogview/rust-en-blazor-webassembly
