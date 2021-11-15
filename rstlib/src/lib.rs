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
pub struct Parallelepiped {
    pub length: f32,
    pub width: f32,
    pub height: f32,
}

#[no_mangle]
pub extern "C" fn get_any_parallelepiped() -> Parallelepiped {
    Parallelepiped {
        length: 1.2,
        width: 2.2,
        height: 1.9,
    }
}

#[no_mangle]
pub extern "C" fn get_parallelepiped_volume(p: Parallelepiped) -> f32 {
    let volume = p.length * p.width * p.height;
    return volume;
}

// strings
use std::ffi::CStr;
use std::os::raw::c_char;

#[no_mangle]
pub extern "C" fn hello(name: *const i8) {
    // does not suppor extended charatters
    let s: &CStr = unsafe { CStr::from_ptr(name) };
    let n = s.to_str().unwrap();

    println!("Hello '{}'!", n);
}

#[no_mangle]
pub extern "C" fn print_string(text_pointer: *const c_char) {
    let c_str = unsafe { CStr::from_ptr(text_pointer) };
    let r_str = c_str.to_str().unwrap();
    println!("{}", r_str.to_string());
}
