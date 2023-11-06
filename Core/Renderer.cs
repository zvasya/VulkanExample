using Core.PlayerLoopStages;
using Shared;
using Surface = Shared.Surface;

namespace Core;

public class Renderer : Component, IRenderable
{
    RendererNode? _rendererNode;
    readonly Surface _surface;
    readonly HelloPipeline _pipeline;
    readonly HelloVertexBuffer _vertexBuffer;
    readonly HelloIndexBuffer _indexBuffer;
    readonly HelloTexture _texture;


    public Renderer(Surface surface, HelloPipeline pipeline, HelloVertexBuffer vertexBuffer, HelloIndexBuffer indexBuffer, HelloTexture texture)
    {
        _surface = surface;
        _pipeline = pipeline;
        _vertexBuffer = vertexBuffer;
        _indexBuffer = indexBuffer;
        _texture = texture;
    }

    protected override void OnConnect()
    {
        base.OnConnect();
        _rendererNode = _surface.CreateRenderer(_pipeline, _vertexBuffer, _indexBuffer, _texture);
        RenderableStage.Register(this);
    }

    protected override void OnDisconnect()
    {
        base.OnDisconnect();
        _rendererNode!.Dispose();
        RenderableStage.Unregister(this);
    }

    public void BeforeRender()
    {
        _rendererNode!.WorldMatrix4X4 = SceneNode!.WorldMatrix4X4;
    }
}
