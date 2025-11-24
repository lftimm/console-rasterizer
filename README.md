# console-rasterizer
## Summary
A C# implementation of a simple software rasterizer that renders 3D scenes in the terminal using ASCII characters.
This project is my companion while reading Peter Shirley's "Fundamentals of Computer Graphics, 2nd Edition" textbook, first 10 chapters.

### Video-demo (clickable)
[![Video demo](https://img.youtube.com/vi/lPetBtIs9k0/0.jpg)](https://www.youtube.com/watch?v=lPetBtIs9k0)


## Notable features
Some things i did here were:

- OpenGL-like rendering pipeline (Vertex, Tesselation, Fragment)
- Triangle Rasterization using Barycentric Coordinates
- Own Linear Alegbra Infraestructure, Matrix4 and Vector3 
- Post processing with box filter blur
- Clean architecture respecting best practices
- Simple Event System for non interruptive keyboard input handling
- Obj rendering with a helper class

## Final notes
I enjoyed how the ascii is being rendered, using the simple map from lighting valeus (calculated based on Z);
There was no need to implement my own math module. System.Numerics has SIMD ready Vector and Matrix types.

I liked reading the book, it's a great introduction to computer graphics. All the algorithms presented in pseudo-code were fairly straight forward to implement.

## Running 
The scene compiled by default has two cubes rendered in wireframe from an obj file.
There is also a obj file of a teapot included in the repo that can be rendered by changing the path in the code.

```bash
git clone https://github.com/lftimm/console-rasterizer
cd console-rasterizer/ConsoleRasterizer
dotnet run 
```

## Controls
- W, A, S, D : Move camera
- Q, E : Rotate camera
- Space, C: Move camera up and down


### Cross Platform
Although it should in theory be cross platform, I have only managed to get it working on Windows.
It might be a terminal emulator issue on linux (i tested with Kitty and gnome-terminal on Fedora Linux) or how dotnet handles the output stream.

## Further improvements
- Fixing any missed bugs
- Adding the Z Buffer
- Implementing a proper lighting model, Phong Lighting for example
- Adding textures
- Maybe adding colors using escape codes or the built-in tools in the Console class

---
When using it be mindful to your terminal's fontsize and window size.
Enjoy !!

