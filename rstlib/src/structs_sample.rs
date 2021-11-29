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
    Parallelepiped {
        /* random for example */
        length: 1.7,
        width: 2.2,
        height: 1.9,
    }
}

#[no_mangle]
pub extern "C" fn get_parallelepiped_volume(p: Parallelepiped) -> f32 {
    let volume = p.length * p.width * p.height;
    volume
}

// work around for wasm --------------------------------------------------
static mut _P: Parallelepiped = Parallelepiped {
    length: 0.0,
    width: 0.0,
    height: 0.0,
};

#[no_mangle]
pub unsafe extern "C" fn get_parallelepiped_ptr() -> *const Parallelepiped {
    _P = get_parallelepiped();
    &_P
}
