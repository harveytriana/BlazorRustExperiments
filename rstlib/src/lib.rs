#![allow(dead_code)]
/*
----------------------------------------------------------
Interoperability exercise
COMPILE for dll
1. Change to crate-type = ["dylib"] in Cargo.toml (see the comments)
2. cargo build --release

COMPILE for WebAssembly
1. Change to crate-type = ["staticlib"] in Cargo.toml (see the comments)
2. cargo build --target wasm32-unknown-emscripten --release


https://doc.rust-lang.org/cargo/commands/cargo-build.html
----------------------------------------------------------
*/
extern crate serde;
extern crate serde_json;

mod basic_sample;
mod callbacks_sample;
mod closure_sample;
mod json_sample;
mod strings_sample;
mod structs_sample;
