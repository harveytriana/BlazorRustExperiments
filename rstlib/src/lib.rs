#![allow(dead_code)]
/*
----------------------------------------------------
Interoperability exercise
COMPILE for dll
cargo build --release

COMPILE for WebAssembly
cargo build --target wasm32-unknown-emscripten --release
----------------------------------------------------
*/
// basic test
// -------------------------------------------------------
#[no_mangle]
pub fn greeting() {
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
pub fn counter() -> i32 {
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
pub fn print_string(text_pointer: *const c_char) {
    let c_str = unsafe { CStr::from_ptr(text_pointer) };
    let r_str = c_str.to_str().unwrap();
    println!("Print from Rust: {}", r_str.to_string());
}

use std::ffi::CString;

#[no_mangle]
pub fn string_test() -> *mut c_char {
    let s = CString::new("« Sin música, la vida sería un error »").expect("CString::new failed!");
    s.into_raw()
}

#[no_mangle]
pub fn describe_person(age: i32) -> *mut c_char {
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
// NOTE
// not FFI-safe -> &str
/*
#[no_mangle]
pub extern "C" fn describe_person(age: &i16) -> &str {
...
}
*/

// Objects
// ------------------------------------------------------
// class
extern crate serde;
extern crate serde_json;

use serde::{Deserialize, Serialize};

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

#[no_mangle]
// return -> User lock by not FFI-safe. Then we return JSON
// Then it is solved by returning JSON, and thus all the complexity is eliminated
pub fn get_user(user_id: i32) -> *mut c_char {
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
    let encode = CString::new(json).expect("CString::new failed!");
    encode.into_raw()
}
