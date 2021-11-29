// CALLBACK ---------------------------------------------------------------------------------
// floats: f(x). Use: execute(<delegate function>, x)
#[no_mangle]
pub extern "C" fn execute_fn_f32(
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

//  EVENTS -----------------------------------------------------------
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

// Other sample
#[no_mangle]
pub extern "C" fn get_serie(
    // callbak
    notify: extern "C" fn(i32) -> i32,
    // callback
    callback: extern "C" fn(i32) -> i32,
    // parameters
    x1: i32,
    x2: i32,
) {
    let mut x = x1;
    while x <= x2 {
        // execute in the caller
        let y = callback(x);
        notify(y);
        sleep(Duration::from_millis(200));
        // return
        x += 1;
    }
}
