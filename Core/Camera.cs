using System.Numerics;
using Core.PlayerLoopStages;
using Silk.NET.Maths;
using Shared;

namespace Core;

public class Camera : Component, IRenderable
{
    readonly Surface? _surface;
    CameraNode? _cameraNode = null;
    public float FieldOfView { get; set; } = 45;
    public bool Orthographic { get; set; } = false;
    public float OrthographicSize { get; set; } = 5;
    public float NearPlane { get; set; } = 0.1f;
    public float FarPlane { get; set; } = 100.0f;
    
    public Camera(Surface surface)
    {
        _surface = surface;
    }

    protected override void OnConnect()
    {
        base.OnConnect();
        
        _cameraNode = new CameraNode();
        _surface!.RegisterCamera(_cameraNode);
        
        RenderableStage.Register(this);
    }

    protected override void OnDisconnect()
    {
        base.OnDisconnect();
        
        RenderableStage.Unregister(this);
    }

    public void BeforeRender()
    {
        var cameraNode = _cameraNode!;
        
        Matrix4x4.Invert(SceneNode!.WorldMatrix4X4, out var view);
        cameraNode.ViewMatrix = view.ToGeneric();
        var extent2D = _surface!.Extent2D;
        var aspect = (float)extent2D.Width / extent2D.Height;
        cameraNode.Projection = Orthographic
            ? Matrix4x4.CreateOrthographic(OrthographicSize * aspect, OrthographicSize, NearPlane, FarPlane).ToGeneric()
            : Matrix4x4.CreatePerspectiveFieldOfView(Scalar.DegreesToRadians(FieldOfView), aspect, NearPlane, FarPlane).ToGeneric();
        
    }
}
