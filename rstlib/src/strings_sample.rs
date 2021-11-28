// strings
// ------------------------------------------------------
use std::ffi::CStr;
use std::ffi::CString;
use std::os::raw::c_char;

#[no_mangle]
pub fn print_string(text_pointer: *const c_char) {
    let c_str = unsafe { CStr::from_ptr(text_pointer) };
    let r_str = c_str.to_str().unwrap();
    println!("Print from Rust: {}", r_str.to_string());
}

#[no_mangle]
pub fn get_some_string() -> *mut c_char {
    let s = CString::new("« Solo sé que nada sé » Sócrates").expect("CString::new failed!");
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

// OTHERS SAMPLES ------------------------------------------
fn reverse(text: String) -> String {
    let s = text.chars().rev().collect::<String>();
    s
}
// C wrap
#[no_mangle]
pub extern "C" fn reverse_inptr(text_pointer: *const c_char) -> *mut c_char {
    let c_str = unsafe { CStr::from_ptr(text_pointer) };
    let r_str = c_str.to_str().unwrap();
    let text = r_str.to_string();
    let reversed = reverse(text);
    let raw = CString::new(reversed).expect("CString::new failed!");
    raw.into_raw()
}
