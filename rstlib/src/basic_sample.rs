// basic test
// -------------------------------------------------------
#[no_mangle]
pub fn hello_world() {
    println!("Hello World!");
}

// math function
// -------------------------------------------------------
#[no_mangle]
pub fn hypotenuse(x: f32, y: f32) -> f32 {
    let num = x.powf(2.0) + y.powf(2.0);
    num.powf(0.5)
}

// ~closure
// -------------------------------------------------------
static mut COUNTER: i32 = 0;

#[no_mangle]
pub fn counter() -> i32 {
    unsafe {
        COUNTER += 1;
        return COUNTER;
    }
}

#[no_mangle]
pub extern "C" fn hello_world2() {
    println!("Hello World!");
}
