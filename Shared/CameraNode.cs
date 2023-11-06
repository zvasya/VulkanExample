using Silk.NET.Maths;

namespace Shared;

public class CameraNode
{
    public Matrix4X4<float> ViewMatrix { get; set; }
    public Matrix4X4<float> Projection { get; set; }
}
