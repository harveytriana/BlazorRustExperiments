// basic test
// -------------------------------------------------------
#[no_mangle]
pub fn hello_world() {
    println!("Hello World!"); // rust writes in console
}

// math function
// -------------------------------------------------------
#[no_mangle]
pub fn hypotenuse(x: f32, y: f32) -> f32 {
    let num = x.powi(2) + y.powi(2);
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
