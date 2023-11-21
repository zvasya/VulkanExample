using System.Diagnostics.CodeAnalysis;
using Silk.NET.Maths;
using System.Numerics;
using Silk.NET.Vulkan;

namespace Shared;

public unsafe class RendererNode : IDisposable
{
    readonly LogicalDevice _device;

    readonly HelloPipeline _pipeline;
    readonly HelloTexture _texture;

    HelloDescriptorSets _descriptorSet;
    UniformBuffer _uniformBuffers;
    readonly HelloIndexBuffer _indexBuffer;
    readonly HelloVertexBuffer _vertexBuffer;

    public Matrix4x4 WorldMatrix4X4 { get; set; }

    RendererNode(LogicalDevice device, HelloPipeline pipeline, HelloTexture texture, HelloIndexBuffer indexBuffer, HelloVertexBuffer vertexBuffer)
    {
        _device = device;
        _pipeline = pipeline;
        _texture = texture;
        _indexBuffer = indexBuffer;
        _vertexBuffer = vertexBuffer;
    }

    public static RendererNode Create(LogicalDevice device, HelloPipeline pipeline, HelloVertexBuffer vertexBuffer, HelloIndexBuffer indexBuffer, HelloTexture texture)
    {
        return new RendererNode(device, pipeline, texture, indexBuffer, vertexBuffer);
    }

    

    public void Draw(HelloCommandBuffer commandBuffer, uint currentImage)
    {
        // Draw
        commandBuffer.CmdBindPipeline(PipelineBindPoint.Graphics, _pipeline.Pipeline);
 
        _vertexBuffer.BindVertexBuffers(commandBuffer);
        _indexBuffer.BindIndexBuffer(commandBuffer);
        
        commandBuffer.CmdBindDescriptorSets(PipelineBindPoint.Graphics, _pipeline.PipelineLayout, 0, 1, _descriptorSet[currentImage], 0, null);

        commandBuffer.CmdDrawIndexed(_indexBuffer.IndicesCount, 1, 0, 0, 0);
    }

    public void Dispose()
    {
        _indexBuffer.Dispose();
        _vertexBuffer.Dispose();
    }


    public void CreateUniformBuffers(HelloDescriptorPool descriptorPool)
    {
        _uniformBuffers = UniformBuffer.Create<UniformBufferObject>(_device);
        _descriptorSet = _uniformBuffers.CreateDescriptorSets(descriptorPool, _pipeline.DescriptorSetLayout, _texture.ImageView, _texture.Sampler);
    }

    public void UpdateUniformBuffer(uint currentImage, HelloSwapchain swapChain, CameraNode camera)
    {
        var preRotation = swapChain.PreTransform switch
        {
            SurfaceTransformFlagsKHR.Rotate90BitKhr => Matrix4x4.CreateRotationZ(0.5f),
            SurfaceTransformFlagsKHR.Rotate180BitKhr => Matrix4x4.CreateRotationZ(1f),
            SurfaceTransformFlagsKHR.Rotate270BitKhr => Matrix4x4.CreateRotationZ(1.5f),
            _ => Matrix4x4.Identity
        };
        UniformBufferObject ubo = new()
        {
            _model = WorldMatrix4X4.ToGeneric(),
            _view = camera.ViewMatrix,
            _proj = camera.Projection,
        };

        _uniformBuffers[currentImage].Fill(ubo);
    }


    public void ResetUniformBuffers()
    {
        _uniformBuffers.Dispose();
    }

    [SuppressMessage("ReSharper", "NotAccessedField.Local")]
    struct UniformBufferObject
    {
        public Matrix4X4<float> _model;
        public Matrix4X4<float> _view;
        public Matrix4X4<float> _proj;
    }
}
