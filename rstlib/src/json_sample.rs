extern crate serde;
extern crate serde_json;

use serde::{Deserialize, Serialize};
use std::ffi::CStr;
use std::ffi::CString;
use std::os::raw::c_char;

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

//
#[no_mangle]
pub fn get_user(user_id: i32) -> *mut c_char {
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

#[no_mangle]
pub fn post_user(json_pointer: *const c_char) {
    let c_str = unsafe { CStr::from_ptr(json_pointer) };
    let r_str = c_str.to_str().unwrap();
    let json = r_str.to_string();

    //deserialize and print
    let user: User = serde_json::from_str(&json).unwrap();

    println!("User is a Rust's Structure:");
    println!("{:?}", user);
}
