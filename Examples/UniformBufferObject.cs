using Silk.NET.Maths;

namespace Examples;

struct UniformBufferObject
{
    public Matrix4X4<float> _model;
    public Matrix4X4<float> _view;
    public Matrix4X4<float> _proj;
}