using System.Numerics;
using Silk.NET.Maths;

namespace Shared;

public class CameraNode : Node
{
    public float FieldOfView { get; set; } = 45f;
    
    public Matrix4x4 View()
    {
        Matrix4x4.Invert(TRS(), out var view);
        return view;
    }

    public Matrix4x4 Projection(float aspect)
    {
        return Matrix4x4.CreatePerspectiveFieldOfView(Scalar.DegreesToRadians(FieldOfView), aspect, 0.1f, 100.0f);
    }
}
