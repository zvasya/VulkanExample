using Silk.NET.Maths;

namespace Shared;

public partial class Surface
{
    readonly ushort[] _indices = {
        0, 1, 2, 2, 3, 0,
    };

    readonly Vertex[] _vertices = {
        new() { _pos = new Vector2D<float>(-0.5f,-0.5f), _color = new Vector3D<float>(1.0f, 0.0f, 0.0f) },
        new() { _pos = new Vector2D<float>(-0.5f,0.5f), _color = new Vector3D<float>(1.0f, 1.0f, 1.0f) },
        new() { _pos = new Vector2D<float>(0.5f,0.5f), _color = new Vector3D<float>(0.0f, 0.0f, 1.0f) },
        new() { _pos = new Vector2D<float>(0.5f,-0.5f), _color = new Vector3D<float>(0.0f, 1.0f, 0.0f) },
    };
}
