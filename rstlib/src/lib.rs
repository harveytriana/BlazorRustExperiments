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
// -------------------------------------------------------
#[no_mangle]
pub extern "C" fn greeting() {
    println!("Hello World!");
}

// math function
// -------------------------------------------------------
#[no_mangle]
pub fn hypotenuse(x: f32, y: f32) -> f32 {
    let num = x.powi(2) + y.powi(2);
    num.powf(0.5)
}

// shared variable
// -------------------------------------------------------
static mut COUNTER: i32 = 0;

#[no_mangle]
pub extern "C" fn counter() -> i32 {
    unsafe {
        COUNTER += 1;
        return COUNTER;
    }
}

// structurs
// -------------------------------------------------------
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
// ------------------------------------------------------
use std::ffi::CStr;
use std::os::raw::c_char;

#[no_mangle]
pub extern "C" fn print_string(text_pointer: *const c_char) {
    let c_str = unsafe { CStr::from_ptr(text_pointer) };
    let r_str = c_str.to_str().unwrap();
    println!("Print from Rust: {}", r_str.to_string());
}

use std::ffi::CString;

#[no_mangle]
pub extern "C" fn string_test() -> *mut c_char {
    let s = CString::new("« Sin música, la vida sería un error »").expect("CString::new failed!");
    s.into_raw()
}

#[no_mangle]
pub extern "C" fn describe_person(age: i32) -> *mut c_char {
    let result = match age {
        a if a < 0 => "Unexpected",
        0..=12 => "Clild",
        13..=17 => "Teenager",
        18..=65 => "Adult",
        _ => "Elderly",
    };
    let encode = CString::new(result).expect("CString::new failed!");
    encode.into_raw()
}

// not FFI-safe -> &str
/*
#[no_mangle]
pub extern "C" fn describe_person(age: &i16) -> &str {
    match age {
        0..=12 => "Clild",
        13..=17 => "Teenager",
        18..=65 => "Adult",
        _ => "Elderly",
    }
}

warning: `extern` fn uses type `str`, which is not FFI-safe
  --> src\lib.rs:76:49
   |
76 | pub extern "C" fn describe_person(age: &i16) -> &str {
   |                                                 ^^^^ not FFI-safe
   |
   = note: `#[warn(improper_ctypes_definitions)]` on by default
   = help: consider using `*const u8` and a length instead
   = note: string slices have no C equivalent
*/
