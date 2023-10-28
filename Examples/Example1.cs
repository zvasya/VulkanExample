using System.Numerics;
using Shared;
using Silk.NET.Maths;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Examples;

public class Example1
{
    readonly Surface _surface;
    readonly CameraNode _camera;
    readonly RendererNode _renderer1;
    readonly RendererNode _renderer2;
    readonly RendererNode _renderer3;
    readonly RendererNode _renderer4;
    readonly RendererNode _renderer5;

    readonly DateTime _dateTime = DateTime.UtcNow;

    readonly Vertex[] _vertices =
    {
        new() { _pos = new Vector3D<float>(-0.5f, -0.5f, 0.0f), _color = new Vector3D<float>(1.0f, 0.0f, 0.0f), _texCoord = new Vector2D<float>(0.0f, 1.0f) },
        new() { _pos = new Vector3D<float>(-0.5f, 0.5f, 0.0f), _color = new Vector3D<float>(1.0f, 1.0f, 1.0f), _texCoord = new Vector2D<float>(0.0f, 0.0f) },
        new() { _pos = new Vector3D<float>(0.5f, 0.5f, 0.0f), _color = new Vector3D<float>(0.0f, 0.0f, 1.0f), _texCoord = new Vector2D<float>(1.0f, 0.0f) },
        new() { _pos = new Vector3D<float>(0.5f, -0.5f, 0.0f), _color = new Vector3D<float>(0.0f, 1.0f, 0.0f), _texCoord = new Vector2D<float>(1.0f, 1.0f) },
    };

    readonly ushort[] _indices = { 0, 1, 2, 2, 3, 0 };


    public Example1(Surface surface, Func<byte[]> vertexShader, Func<byte[]> fragmentShader, Func<Image<Rgba32>> image1, Func<Image<Rgba32>> image2)
    {
        _surface = surface;

        _camera = new CameraNode();
        _surface.RegisterCamera(_camera);
        _camera._position = new Vector3(2, 2, 2);
        _camera._rotation = CreateFromYawPitchRoll(DegToRad(54.7f), 0, DegToRad(135f));

        var pipeline = _surface.CreatePipeLine(vertexShader(), fragmentShader(), Vertex.GetBindingDescription1(), Vertex.GetAttributeDescriptions1());
        HelloTexture texture;
        using (var img = image1())
        {
            texture = _surface.CreateTexture(img);
        }

        HelloTexture texture2;
        using (var img = image2())
        {
            texture2 = _surface.CreateTexture(img);
        }

        var vb = _surface.CreateVertexBuffer(_vertices);
        var ib = _surface.CreateIndexBuffer(_indices);

        _renderer1 = _surface.CreateRenderer(pipeline, vb, ib, texture);
        _renderer1._position = new Vector3(0, 0, 0.5f);
        _renderer2 = _surface.CreateRenderer(pipeline, vb, ib, texture2);
        _renderer2._position = Vector3.Zero;
        _renderer3 = _surface.CreateRenderer(pipeline, vb, ib, texture);
        _renderer3._position = new Vector3(0, 0, -0.5f);
        ;
        _renderer4 = _surface.CreateRenderer(pipeline, vb, ib, texture2);
        _renderer4._position = new Vector3(0, 0, 1.0f);
        _renderer5 = _surface.CreateRenderer(pipeline, vb, ib, texture);
        _renderer5._position = new Vector3(0, 0, -1.0f);
    }

    public void Update()
    {
        var time = (DateTime.UtcNow - _dateTime).TotalMilliseconds / 10f;

        _renderer1._rotation = Quaternion.CreateFromAxisAngle(Vector3.UnitZ, (float)Math.Sin(Scalar.DegreesToRadians(time) / 4));
        _renderer2._rotation = Quaternion.CreateFromAxisAngle(Vector3.UnitZ, (float)Math.Sin(Scalar.DegreesToRadians(time) / 3));
        _renderer3._rotation = Quaternion.CreateFromAxisAngle(Vector3.UnitZ, (float)Math.Sin(Scalar.DegreesToRadians(time) / 2));
        _renderer4._rotation = Quaternion.CreateFromAxisAngle(Vector3.UnitZ, (float)Math.Sin(Scalar.DegreesToRadians(time) / 5));
        _renderer5._rotation = Quaternion.CreateFromAxisAngle(Vector3.UnitZ, (float)Math.Sin(Scalar.DegreesToRadians(time) / 1));
    }
}
