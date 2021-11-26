// CALLBACK ---------------------------------------------------------------------------------
fn operation(x: f32, f: &dyn Fn(f32) -> f32) -> f32 {
    f(x)
}
// signature for C#
#[no_mangle]
pub extern "C" fn c_operation(x: f32, callback_fn: unsafe extern "C" fn(f32) -> f32) -> f32 {
    operation(x, &|n| unsafe { callback_fn(n) })
}

// sample... operation(2, cube) = 8
#[no_mangle]
fn cube(x: f32) -> f32 {
    x * x * x
}
// sample... operation(2, square) = 8
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
pub unsafe extern "C" fn UnmanagedPrompt(notify: PromptHandler) {
    let mut i = 1;
    while i <= 10 {
        notify.expect("non-null function pointer")(i);
        // simulate
        sleep(Duration::from_millis(250));
        // return
        i += 1
    }
}
