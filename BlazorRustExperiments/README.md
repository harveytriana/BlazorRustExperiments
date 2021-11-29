# Una guía concisa sobre cómo usar una biblioteca de Rust en Blazor WASM

## Resumen

Es posible construir un ensamblado de Rust hacía tecnologías Web basado en Flang/LLVM al estilo Emscripten y usar en Blazor WASM. 

En esta publicación voy a delinear unos pasos pasos fundamentales de cómo se debe codificar para establecer esa simbiosis entre el mundo Rust y el mundo C#.

No pretendo enseñar a usar Rust. Si no conoce este poderoso lenguaje de programación sugiero la guía documentada en las referencias 1 y 2. 

## Introducción

Sería redundante escribir sobre las cualidades de Rust, y hablar de por qué ha despertado el interés de muchos desarrolladores profesionales. Mi interés se consolida cuando sé que se puede usar como una referencia nativa en Blazor. Aquel trino de Steve Sanderson en donde dice «Usando la nueva función de dependencias nativas para Blazor WebAssembly en .NET 6, logré llamar a Rust desde C#, ¡Ambos ejecutándose en el navegador! … Ahora bien, ¿Qué podemos hacer con este nuevo poder?», fue una inspiración que me llevó a profundizar en el tema.

La relación Rust - C# está dada. La primera función externa, el clásico «Hello World!» no puede faltar. El decorador indica que es para usar externamente,

```
#[no_mangle]
pub fn hello_world() {
    println!("Hello World!");
}
```

Desde C# podemos algo como:

```
namespace ConsoleApp;
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

Donde RLIB es la ruta de la librería Rust, y existe en una clase global de constantes.

## Arquitectura del proyecto

El laboratorio consiste en una solución Blazor WASM net6 sin hospedaje, junto con la librería Rust. En la misma solución .NET agregamos un proyecto de consola net6. En el mismo folder se crea una carpeta en donde irá el proyecto Rust. De esta manera, podemos hacer referencia relativa a las rutas en las librerías sin tener que hacer copias durante el desarrollo. En general el proyecto de consola se usa como un probador ágil de código. No haré mucho énfasis en el proyecto, que bien puede estudiar en la publicación Github.

## Creación del proyecto Rust

Asumo que ya tiene el ambiente listo para programar en Rust, si no es así, en la referencia 1 encuentra los pasos. Es simple crear una biblioteca, desde la carpeta donde desea su proyecto ejecute el siguiente comando de terminal:

```
cargo new rstlib
```

Es más productivo si lo hacemos desde vscode, con la extensión «Rust Extension Pack». (Con la esperanza de que en un futuro no muy lejano podamos trabajar Rust en Visual Studio 20x). 

Después de ejecutar este comando, ahora tenemos una nueva carpeta llamada target y el archivo de configuración cargo.toml. 

Para compilar a wasm en términos de Emscripten, que es lo que entiende Wasm, necesitamos agregar al compilador Rust el soporte a Emscripten. Se instalará usando rustup. La referencia 3 indico la documentación precisa, para resumir, sólo necesita los tres primeros comandos.

```
rustup toolchain add stable
rustup target add asmjs-unknown-emscripten --toolchain stable
rustup target add wasm32-unknown-emscripten --toolchain stable
```

La compilación a emscripten necesita una librería estática, para lo cual en el archivo de configuración especificamos create-type = ["staticlib"]. 

Como una sugerencia y facilidad de desarrollo, es conveniente probar nuestro código Rust contra C#, desde una consola de C#, luego, podemos compilar para Wasm y escalar a Blazor. No obstante es importante aclarar que no todo el código C# que corre en consola es soportado de manera directa por emscripten, existen casos en los que es necesario trabajar directamente con punteros, lo cual también trato en este artículo.  

Si vamos a trabajar con una consola, entonces la librería estática no crea una DLL en la compilación, para lo cual intercambiamos el tipo de crate de estático a dinámico. Cuando esté listo para usar en wasm, cambiamos el tipo de librería a estático y compilamos. Estratégicamente dejó los dos en el mismo cargo.toml, y comentó uno o otro según el caso. Esto se puede mejorar con una macro, pero no quiero extender más allá del objetivo de este escrito.

El archivo de configuración con las características descritas es como sigue

```
[package]
name = "rstlib"
version = "0.1.0"
authors = ["Harvey Triana <harveytriana@gmail.com>"]

[lib]
name="rstlib"

# WebAssembly for Emscripten (.a)
# -----------------------------------------------------------------
# compile: cargo build --target wasm32-unknown-emscripten --release
# crate-type = ["staticlib"]

# Dynamic library for C# Console (.dll)
# -----------------------------------------------------------------
# compile: cargo build --release
crate-type = ["dylib"]
```

## Interoperabilidad

Este articulo esta dirigido a programadores que conocen C#, y están iniciando en Rust, o bien, ya tienen experiencia. Abordaré los casos comunes de lógica de programación con un ejemplo en cada uno. 

## Funciones

Una función matemática que retorna un tipo nativo, y usa parámetros de tipo nativos. Ejemplo, la siguiente función retorna la Hipotenusa dados los dos Catetos.

```
#[no_mangle]
pub fn hypotenuse(x: f32, y: f32) -> f32 {
    let num = x.powf(2.0) + y.powf(2.0);
    num.powf(0.5)
}
```

El código C# para usar esta función luce así:

```
[DllImport(RLIB)] static extern float hypotenuse(float x, float y);

float x = 9, y = 11, h = hypotenuse(x, y);

Console.WriteLine("From Rust Library, Hypotenuse({0}, {1}) = {2}", x, y, h);
```

Note que no es necesario usar **extern "C"** en la firma de ésta función.

## Cadenas de Texto

Trabajar con cadenas de Rust o C/C++ no es simple, ya que estos lenguajes son estrictos en cuanto a su organización en memoria. He visto varias estrategias desde C# para lidiar con ésto,algunas con demasiado código o innecesaria complejidad. Presento acá dos formas de lograr esto. 

Para iniciar este ejercicio, copie lo siguiente en [https://play.rust-lang.org/](https://play.rust-lang.org/) y ejecutelo.

```
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

Estamos enviando como parámetro un String, y recibiendo un String en el resultado ¿Cómo usar la función _reverse() _en C#? El problema se divide en dos partes, enviar y recibir Strings. Para conectar el mundo Rust con el de C# necesitamos trabajar con punteros. Envolvemos la función Rust en un tipo y tratamiento que entiende C, y así mismo C#.

```
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

Es algo complejo, pero resuelve todos los casos de intercambio de texto, aun con caracteres especiales; más adelante podremos agregar un módulo en Rust para generalizar la solución. 

Para usar esta función desde C# Tenemos dos opciones.

Primera opción, pasar el parámetro encapsulado en LPUTF8Str, y decodificar el puntero de regreso con `Marshal.PtrToStringUTF8`. Siguiendo el ejemplo,

```
[DllImport(RLIB)] 
static extern IntPtr reverse_inptr([MarshalAs(UnmanagedType.LPUTF8Str)] string text);

var quote = "« All that we see or seem is but a dream within a dream. » EAP";
var p = reverse_inptr(quote);
var quoteReversed = Marshal.PtrToStringUTF8(p);

Console.WriteLine($"Quote         : {quote}", ConsoleColor.Yellow);
Console.WriteLine($"Reverse Quote : {quoteReversed}", ConsoleColor.Yellow);
```

Segunda opción, pasar la cadena de texto como un arreglo de bytes UTF8.

```
[DllImport(RLIB)] 
static extern IntPtr reverse_inptr(byte[] utf8Text);

var quote = "« All that we see or seem is but a dream within a dream. » EAP";

var bytes = Encoding.UTF8.GetBytes(quote);
var p = reverse_inptr(bytes);
var quoteReversed = Marshal.PtrToStringUTF8(p);

Console.WriteLine($"Quote         : {quote}");
Console.WriteLine($"Reverse Quote : {quoteReversed}");
```

En ambos caso el resultado es el mismo:

```
Quote         : « All that we see or seem is but a dream within a dream. » EAP
Reverse Quote : PAE » .maerd a nihtiw maerd a tub si mees ro ees ew taht llA «
```

¿Cuándo usar uno u otro? Opino que el criterio es como prefiera. Se puede simplificar uno u otro caso con extensiones C#; en el proyecto se muestra de esa manera.

El uso de esta estrategia para intercambiar cadenas de texto en Blazor WASM es el mismo, no se requiere tratamiento especial.

## Estructuras

Definamos una estructura cualquiera, con tipos nativos, y un método que devuelva una instancia de ella, y, así mismo, una función que use esta estructura en uno de sus parámetros:

```
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

El código C# para leer una estructura, y pasar una estructura como parámetro es el siguiente:

```
/// C# 10
record struct Parallelepiped(float length, float width, float height);

[DllImport(RLIB)] static extern Parallelepiped get_parallelepiped();
[DllImport(RLIB)] static extern float get_parallelepiped_volume(Parallelepiped p);

var parallelepiped = get_parallelepiped();
var volume = get_parallelepiped_volume(parallelepiped);

// can create the struct in C# and use as parameter in Rust
get_parallelepiped_volume(new Parallelepiped(1, 2, 3)));
```

Noté que se está empleando un **record struct**, en vez de una clásico struct; menos código, mayor productividad. Igualmente puede crear la estructura clásica y usarlo acá, no obstante debe agregar un decorador [StructLayout(LayoutKind.Sequential)].

### Trabajo Circundante para usar en WASM

Podemos pasar la estructura como parámetro, tal como se describe en el párrafo anterior, pero no recibirla de la misma manera. El compilado Emscripten no deja disponibilidad para eso; se genera una excepción de «discrepancia de firma de función». No obstante, estratégicamente lo podemos resolver a través de punteros.

Agregamos una función en Rust que envuelve la función con retorno de una dirección de memoria, y usamos una variable estática en el módulo. 

```
// work around for wasm --------------------------------------------------
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

En C#:

```
[DllImport(RLIB)] static extern IntPtr get_parallelepiped_ptr();

var ptr = get_parallelepiped_ptr();
var p = Marshal.PtrToStructure<Parallelepiped>(ptr);

Console.WriteLine("Pointer     : {0}", ptr);
Console.WriteLine("Deferenced  : {0}", p);
```

Esta solución es efectiva, aunque se debe tener precaución, ya que se requiere la existencia de una variable estática en la librería. Aunque, por tratarse de Wasm, el único cliente es el visualizador. Como mencione, es una solución estratégica, - Si sugieres una mejor solución, excelente; por favor compartela.

_La complejidad radica en que <code>get_parallelepiped_ptr</code>() devuelve una dirección de la instancia local de la estructura pero su tiempo de vida termina con el retorno de la función. Ese es un comportamiento indefinido en C, C++, o Rust.</em>

## Transferencia JSON

La transferencia JSON resuelve toda complejidad de tipos, aún de tipos con profundidad anidada. Para usar JSON en Rust necesitamos un librería externa o crate externo, para esto agregamos la dependencia al archivo de configuración:

```
[dependencies]
serde = { version = "1.0.126", features = ["derive"] }
serde_json = "1.0.64"
```

Usar JSON en Rust es simple. Creé una estructura anidada para demostrarlo. Veamos el ejemplo.

```
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

Como podrá ver, tan solo es transferencia y lectura de texto, unos decoradores, más el uso de la herramienta de serialización / deserialización de Rust.

En C# podemos usar una clase, hasta con las convenciones de codificación de C#. 

```
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

## Devoluciones de llamada

Usaré el caso clásico en donde a una función otra función como parámetro. Ese parámetro es un delegado que permite apuntar a diferentes funciones que cumple la firma del delegado. Por ejemplo, una función f(x) para hacer un cálculo matemático, por ejemplo, Cuadrado, Cubo, Raíz Cúbica, y así sucesivamente. Estas funciones de primer orden pueden ser escritas en Rust o en C#.

```
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

Del lado C#, usamos esto de la siguiente manera:

```
[DllImport(RLIB)] static extern float execute_fn(Fn handle, float x);
[DllImport(RLIB)] static extern float cube(float x);
[DllImport(RLIB)] static extern float square(float x);

delegate float Fn(float x);

float x = 7.0F;
Console.WriteLine("Passing a rust function as parameter of a Rust function");
Console.WriteLine("execute(square, {0})  : {1}", x, execute_fn(square, x));
Console.WriteLine("execute(cube, {0})    : {1}", x, execute_fn(cube, x));
```

Ahora, si deseamos pasar una función C# hacia la librería Rust, también es posible. Voy a escribir las mismas funciones, pero del lado de C# e invocarlas indirectamente en Rust.

```
static float Square(float x) => x * x;
static float Cube(float x) => x * x * x;

float x = 7.0F;
Console.WriteLine("Passing a C# function as parameter of a Rust function");
Console.WriteLine("execute(Square, {0})  : {1}", x, execute_fn(Square, x));
Console.WriteLine("execute(Cube, {0})    : {1}", x, execute_fn(Cube, x));
```

### Trabajo Circundante para usar en WASM

Mientras podemos pasar una función Rust al método Rust desde C# en Wasm, no se permite pasar directamente una función C#. La solución nuevamente es trabajar con punteros, en este caso es más avanzado porque necesitamos trabajar en modo unsafe en C#, y decorar la función con [UnmanagedCallersOnly]. Básicamente el delegado lo convertimos a una dirección de memoria, que pasamos al método Rust.

```
unsafe {
[DllImport(RLIB)] static extern float execute_fn(IntPtr handle, float x);

[UnmanagedCallersOnly] static float Cube(float x) => x * x * x;
static readonly delegate* unmanaged<float, float> OnCube = &Cube;

float x = 2;
Console.WriteLine("execute(*Cube, {x}): {execute_fn((IntPtr)OnCube, x)}");
}
```

## Eventos

Los eventos involucran delegados. Es decir, el caso es similar al anterior. En este ejemplo, voy a escribir una función en Rust que reciba una función de notificación y la invoque cada determinado tiempo, el invocador recibe el dato desatendidamente.

```
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

En C#:

```
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

### Trabajo Circundante para usar en WASM

Como ya sabemos, no podemos pasar métodos o funciones C# directamente a un librería Rust compilada para Wasm. La estrategia nuevamente recurre a punteros, siendo este caso equivalente al anterior.

```
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
    // extern ---------------------------------------------------------------
    [DllImport(RLIB)] static extern void unmanaged_prompt(IntPtr notify, int count);
}
```

En Blazor, el caso impone un reto más, y es que la estrategia usa necesariamente estáticos, lo cual desde una página que no es estática requiere cierto tratamiento especial. Una forma es aislar la lógica en una clase estática, y mediante un evento notificar a la página. Se deben tener precauciones en manejar este esquema para no dejar vínculos perdidos.

# Blazor

Compilamos para Emscripten, para esto cambiamos el tipo de librería a estático y compilamos con el comando correspondiente. Esto genera un archivo .a que es el que agregamos como referencia nativa al proyecto Blazor. Vea los comentarios del archivo Cargo.toml para los detalles.

El archivo de proyecto de Blazor contiene las siguientes directivas:

```
<PropertyGroup>
  <AllowUnsafeBlocks>true</AllowUnsafeBlocks>
  <WasmBuildNative>true</WasmBuildNative>
</PropertyGroup>

<ItemGroup>
  <NativeFileReference Include="..\..\rstlib\target\wasm32-unknown-emscripten\release\librstlib.a" />
</ItemGroup>
```

La ruta de la referencia nativa corresponde a que existe en el mismo directorio de la solución Blazor, y no tendremos que estar moviendo el archivo librstlib.a

En cada caso tratado acá he dispuesto un ejemplo en Blazor, repartido en las páginas del SPA. En los casos especiales de código unsafe, creé una clase con los estáticos, y a través de un evento se pueden leer respuestas de los eventos o devoluciones de llamadas. En general sería poco usual que desearíamos pasar funciones de C# a Rust en Wasm, no obstante acá se demuestra como se puede hacer. Puede consultar el repositorio acá: [BlazorRustExperiments](https://github.com/harveytriana/BlazorRustExperiments)

# Conclusiones

¿Hasta dónde nos llevará este nuevo poder? La evolución de Blazor es ciertamente abrumadora, no obstante, la característica de las dependencias nativas es notoriamente relevante. Desde el anuncio de Denis Roth en su blog, bajo el título «Soporte de dependencias nativas para aplicaciones Blazor WebAssembly» los programadores de Blazor sabemos que esto nos va a llevar lejos. 

Ciertamente no es difícil usar código Rust en C#, en este artículo demostraré varios aspectos fundamentales de cómo se puede establecer esa relación. Un proceso matemático exhaustivo que debiera ejecutarse en el cliente, se puede resolver desde una librería nativa con un alto desempeño. 

---

### Referencias

1. [Rust]([https://www.rust-lang.org/](https://www.rust-lang.org/))
2. [Take your first steps with Rust]([https://docs.microsoft.com/en-us/learn/paths/rust-first-steps/](https://docs.microsoft.com/en-us/learn/paths/rust-first-steps/))
3. [Rust and WebAssembly](https://www.hellorust.com/setup/emscripten/)
4. [Emscripten ]([https://emscripten.org/]