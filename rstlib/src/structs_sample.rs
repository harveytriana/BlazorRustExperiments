// structurs
// -------------------------------------------------------
#[repr(C)]
pub struct Parallelepiped {
    pub length: f32,
    pub width: f32,
    pub height: f32,
}

#[no_mangle]
// Not use in Emscripten's wasm : (
pub extern "C" fn get_parallelepiped() -> Parallelepiped {
    // dummy
    Parallelepiped {
        length: 1.7,
        width: 2.2,
        height: 1.9,
    }
}

// return a static struct in a pointer
static mut _P: Parallelepiped = Parallelepiped {
    length: 0.0,
    width: 0.0,
    height: 0.0,
};

#[no_mangle]
pub extern "C" fn get_parallelepiped_ptr() -> *mut Parallelepiped {
    unsafe {
        // dummy
        _P.length = 1.7;
        _P.width = 2.2;
        _P.height = 1.9;
        &mut _P
    }
}

#[no_mangle]
pub extern "C" fn get_parallelepiped_volume(p: Parallelepiped) -> f32 {
    let volume = p.length * p.width * p.height;
    return volume;
}
