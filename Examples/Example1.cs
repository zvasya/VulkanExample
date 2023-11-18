using System.Numerics;
using Shared;
using Core;
using Core.PlayerLoop;
using Core.PlayerLoopStages;
using Silk.NET.Maths;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Examples;

public class Example1
{
    readonly Surface _surface;
    readonly PlayerLoop _playerLoop;
    readonly Node _camera;
    readonly Node _cameraRoot;
    readonly Node _renderer1;
    readonly Node _renderer2;
    readonly Node _renderer3;
    readonly Node _renderer4;
    readonly Node _renderer5;

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

        _playerLoop = CreatePlayerLoop();
        _surface.BeforeDraw += _playerLoop.Run;
        
        var pipeline = _surface.CreatePipeLine(vertexShader(), fragmentShader(), Vertex.GetBindingDescription(), Vertex.GetAttributeDescriptions());
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

        _cameraRoot = new Node("camera_root");
        _cameraRoot.AddComponent(new CameraRotator() { Speed = 1.0f/5000.0f, Axis = Vector3.UnitZ});
        
        _camera = new Node("camera");
        var camera = new Camera(_surface);
        _camera.AddComponent(camera);
        
        _camera.LocalPosition = new Vector3(2, 2, 2);
        _camera.LocalRotation = CreateFromYawPitchRoll(DegToRad(54.7f), 0, DegToRad(135f));

        _cameraRoot.AddChild(_camera);
        
        _renderer1 = new Node("renderer1");
        var renderer1 = new Renderer(_surface,pipeline, vb, ib, texture);
        _renderer1.AddComponent(renderer1);
        _renderer1.LocalPosition = new Vector3(0, 0, 0.5f);
        _renderer1.AddComponent(new TwistRotator {Speed = 1.0f/40f});
        
        _renderer2 = new Node("renderer2");
        var renderer2 = new Renderer(_surface, pipeline, vb, ib, texture);
        _renderer2.AddComponent(renderer2);
        _renderer2.LocalPosition = Vector3.Zero;
        _renderer2.AddComponent(new TwistRotator {Speed = 1.0f/30f});
        
        _renderer3 = new Node("renderer3");
        var renderer3 = new Renderer(_surface, pipeline, vb, ib, texture);
        _renderer3.AddComponent(renderer3);
        _renderer3.LocalPosition = new Vector3(0, 0, -0.5f);
        _renderer3.AddComponent(new TwistRotator {Speed = 1.0f/20f});
        
        _renderer4 = new Node("renderer4");
        var renderer4 = new Renderer(_surface, pipeline, vb, ib, texture2);
        _renderer4.AddComponent(renderer4);
        _renderer4.LocalPosition = new Vector3(0, 0, 1.0f);
        _renderer1.AddComponent(new TwistRotator {Speed = 1.0f/50f});
        
        _renderer5 = new Node("renderer5");
        var renderer5 = new Renderer(_surface, pipeline, vb, ib, texture);
        _renderer5.AddComponent(renderer5);
        _renderer5.LocalPosition = new Vector3(0, 0, -1.0f);
        _renderer5.AddComponent(new TwistRotator {Speed = 1.0f/50f});
    }

    static PlayerLoop CreatePlayerLoop()
    {
        var playerLoop = new PlayerLoop();
        playerLoop.Add(new UpdateStage());
        playerLoop.Add(new RenderableStage());
        return playerLoop;
    }
}
