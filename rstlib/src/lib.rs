/*
----------------------------------------------------
Interoperability exercise
COMPILE
.dll
cargo build --release

.a (WebAssembly)
cargo build --target wasm32-unknown-emscripten --release
----------------------------------------------------
*/
// basic test
#[no_mangle]
pub extern "C" fn greeting() {
    println!("Hello World!");
}

// math function
#[no_mangle]
pub fn hypotenuse(x: f32, y: f32) -> f32 {
    let num = x.powi(2) + y.powi(2);
    num.powf(0.5)
}

// shared variable
static mut COUNTER: i32 = 0;

#[no_mangle]
pub extern "C" fn counter() -> i32 {
    unsafe {
        COUNTER += 1;
        return COUNTER;
    }
}

// structurs
#[repr(C)]
pub struct ShapeStruct {
    pub length: f32,
    pub width: f32,
    pub height: f32,
}

#[no_mangle]
pub extern "C" fn get_simple_struct() -> ShapeStruct {
    ShapeStruct {
        length: 1.2,
        width: 2.2,
        height: 1.9,
    }
}
