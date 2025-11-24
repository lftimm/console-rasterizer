# console-rasterizer
## Summary
This project is my companion while reading Peter Shirley's "Fundamentals of Computer Graphics, 2nd Edition" textbook.

The scene compiled by default has two cubes rendered in wireframe from an obj file.
There is also a obj file of a teapot included in the repo that can be rendered by changing the path in the code.
The rendering is done in the terminal using ASCII characters. 

## Controls
- W, A, S, D : Move camera
- Q, E : Rotate camera
- Space, C: Move camera up and down

## Running 
Although it should be in theory cross platform, I have only managed to get it working on Windows.
Tested it on Fedora Linux, it doesn't render the scene.
It might be a terminal emulator issue (tested with Kitty and gnome-terminal) or how dotnet handles console output on linux.

[![Video demo](https://img.youtube.com/vi/lPetBtIs9k0/0.jpg)](https://www.youtube.com/watch?v=lPetBtIs9k0)

```bash
git clone https://github.com/lftimm/console-rasterizer
cd console-rasterizer/ConsoleRasterizer
dotnet run 
```

## Notable features
Some things i did here:

- OpenGL-like rendering pipeline (Vertex, Tesselation, Fragment)
- Triangle Rasterization using Barycentric Coordinates
- Own Linear Alegbra Infraestructure, Matrix4 and Vector3 
- Post processing with box filter
- Clean architecture respecting best practices
- Simple Event System for non interruptive keyboard input handling
- Obj rendering with a helper class

## Final notes
There were things left out due to time constraints and scope creep.
For example:
- Most notably, the Z buffer the book teaches is not implemented here;
- Lighting is done by only checking Z values;
- Clipping issues when the geometry goes out of view;

I enjoyed how the ascii is being rendered, using the simple map from lighting valeus (calculated based on Z);
There was no need to implement my own math module. System.Numerics has SIMD ready Vector and Matrix types.
My arbitrary choice of implementing was for learning-sake, but it took down development time and performance.

## Further improvements
- Fixing any bugs
- Adding the Z Buffer
- Implementing a proper lighting model, Phong Lighting for example
- Adding textures
- Maybe adding colors using escape codes or the built-in tools in the Console class

---
When using it be mindful to your terminal's fontsize and window size.
Enjoy !!

