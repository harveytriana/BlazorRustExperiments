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

#[repr(C)]
pub struct SampleStruct {
    pub field_one: i16,
    pub field_two: i32,
}

#[no_mangle]
pub extern "C" fn get_simple_struct() -> SampleStruct {
    SampleStruct {
        field_one: 1,
        field_two: 2,
    }
}
