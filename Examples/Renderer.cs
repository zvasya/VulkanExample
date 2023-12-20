using System.Numerics;
using Core;
using Core.PlayerLoopStages;
using Shared;
using Silk.NET.Maths;
using Silk.NET.Vulkan;
using Surface = Shared.Surface;

namespace Examples;

public class Renderer : Component, IRenderable, IRendererNode
{
    readonly Surface _surface;
    readonly HelloPipeline _pipeline;
    readonly HelloVertexBuffer _vertexBuffer;
    readonly HelloIndexBuffer _indexBuffer;
    readonly HelloTexture _texture;
    UniformBuffer _uniformBuffers;
    public Matrix4x4 WorldMatrix4X4 { get; set; }
    
    readonly List<HelloDescriptorSets> _descriptorSet = new List<HelloDescriptorSets>();


    public Renderer(Surface surface, HelloPipeline pipeline, HelloVertexBuffer vertexBuffer, HelloIndexBuffer indexBuffer, HelloTexture texture)
    {
        _surface = surface;
        _pipeline = pipeline;
        _vertexBuffer = vertexBuffer;
        _indexBuffer = indexBuffer;
        _texture = texture;
        _uniformBuffers = _surface.CreateUniformBuffer<UniformBufferObject>();
    }

    protected override void OnConnect()
    {
        base.OnConnect();
        _descriptorSet.Add( _uniformBuffers.CreateDescriptorSets(_surface.DescriptorPool, _pipeline.DescriptorSetLayout, 0));
        _descriptorSet.Add(_texture.CreateDescriptorSets(_surface.DescriptorPool, _pipeline.DescriptorSetLayout, 1));
        
        _surface.RegisterRenderer(this);
        
        RenderableStage.Register(this);
    }
    
    
    public void Draw(HelloCommandBuffer commandBuffer, uint currentImage)
    {
        // Draw
        commandBuffer.CmdBindPipeline(PipelineBindPoint.Graphics, _pipeline.Pipeline);

        _vertexBuffer.BindVertexBuffers(commandBuffer);
        _indexBuffer.BindIndexBuffer(commandBuffer);

        
        //TODO CHECK null -> 0
        foreach (var set in _descriptorSet) 
            commandBuffer.CmdBindDescriptorSets(PipelineBindPoint.Graphics, _pipeline.PipelineLayout, 0, 1, set[currentImage], 0, 0);
        
        commandBuffer.CmdDrawIndexed(_indexBuffer.IndicesCount, 1, 0, 0, 0);
    }
    

    protected override void OnDisconnect()
    {
        base.OnDisconnect();
        _indexBuffer.Dispose();
        _vertexBuffer.Dispose();
        RenderableStage.Unregister(this);
    }

    public void BeforeRender()
    {
        WorldMatrix4X4 = SceneNode!.WorldMatrix4X4;
    }

    public void BeforeDraw(CameraNode camera, uint currentImage)
    {
        UniformBufferObject ubo = new()
        {
            _model = WorldMatrix4X4.ToGeneric(),
            _view = camera.ViewMatrix,
            _proj = camera.Projection,
        };

        _uniformBuffers[currentImage].Fill(ubo);
    }
}
