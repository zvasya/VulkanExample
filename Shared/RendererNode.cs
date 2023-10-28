using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Silk.NET.Maths;
using Silk.NET.Vulkan;

namespace Shared;

public unsafe class RendererNode : IDisposable
{
    static readonly ushort[] _indices = { 0, 1, 2, 2, 3, 0 };

    static readonly Vertex[] _vertices =
    {
        new() { _pos = new Vector3D<float>(-0.5f, -0.5f, 0.0f), _color = new Vector3D<float>(1.0f, 0.0f, 0.0f), _texCoord = new Vector2D<float>(1.0f, 0.0f) },
        new() { _pos = new Vector3D<float>(-0.5f, 0.5f, 0.0f), _color = new Vector3D<float>(1.0f, 1.0f, 1.0f), _texCoord = new Vector2D<float>(0.0f, 0.0f) },
        new() { _pos = new Vector3D<float>(0.5f, 0.5f, 0.0f), _color = new Vector3D<float>(0.0f, 0.0f, 1.0f), _texCoord = new Vector2D<float>(0.0f, 1.0f) },
        new() { _pos = new Vector3D<float>(0.5f, -0.5f, 0.0f), _color = new Vector3D<float>(0.0f, 1.0f, 0.0f), _texCoord = new Vector2D<float>(1.0f, 1.0f) },
    };

    readonly DateTime _dateTime = DateTime.UtcNow;


    readonly LogicalDevice _device;
    readonly Vector3D<float> _position;

    readonly float _timeScale;

    HelloDescriptorSets? _descriptorSet;
    UniformBuffer? _uniformBuffers;
    readonly HelloIndexBuffer _indexBuffer;
    readonly HelloVertexBuffer _vertexBuffer;

    RendererNode(LogicalDevice device, float timeScale, Vector3D<float> position, HelloIndexBuffer indexBuffer, HelloVertexBuffer vertexBuffer)
    {
        _device = device;
        _timeScale = timeScale;
        _position = position;
        _indexBuffer = indexBuffer;
        _vertexBuffer = vertexBuffer;
    }

    public static RendererNode Create(LogicalDevice device,float timeScale, Vector3D<float> position)
    {
        var vertexBuffer = CreateVertexBuffer(device, _vertices);
        var indexBuffer = CreateIndexBuffer(device, _indices);
        return new RendererNode(device, timeScale, position, indexBuffer, vertexBuffer);
    }

    static HelloIndexBuffer CreateIndexBuffer(LogicalDevice device, ushort[] indices)
    {
        var size = (ulong)(Unsafe.SizeOf<ushort>() * indices.Length);
        var indexBuffer = HelloIndexBuffer.Create(device, size);
        indexBuffer.FillStaging(indices, device.CommandPool, device.GraphicsQueue);
        return indexBuffer;
    }

    static HelloVertexBuffer CreateVertexBuffer(LogicalDevice device, Vertex[] vertices)
    {
        var size = (ulong)(Unsafe.SizeOf<Vertex>() * vertices.Length);
        var vertexBuffer = HelloVertexBuffer.Create(device, size);
        vertexBuffer.FillStaging(vertices, device.CommandPool, device.GraphicsQueue);
        return vertexBuffer;
    }

    public void Draw(HelloCommandBuffer commandBuffer, HelloPipeline graphicsPipeline, uint currentImage)
    {
        // Draw
        commandBuffer.CmdBindPipeline(PipelineBindPoint.Graphics, graphicsPipeline.Pipeline);
 
        _vertexBuffer.BindVertexBuffers(commandBuffer);
        _indexBuffer.BindIndexBuffer(commandBuffer);
        
        commandBuffer.CmdBindDescriptorSets(PipelineBindPoint.Graphics, graphicsPipeline.PipelineLayout, 0, 1, _descriptorSet[currentImage], 0, null);

        commandBuffer.CmdDrawIndexed((uint)_indices.Length, 1, 0, 0, 0);

    }

    public void Dispose()
    {
        _indexBuffer.Dispose();
        _vertexBuffer.Dispose();
    }


    public void CreateUniformBuffers(HelloDescriptorPool descriptorPool, HelloDescriptorSetLayout descriptorSetLayout, ImageView textureImageView, Sampler textureSampler)
    {
        _uniformBuffers = UniformBuffer.Create<UniformBufferObject>(_device);
        _descriptorSet = _uniformBuffers.CreateDescriptorSets(descriptorPool, descriptorSetLayout, textureImageView, textureSampler);
    }

    public void UpdateUniformBuffer(uint currentImage, HelloSwapchain swapChain)
    {
        var time = (DateTime.UtcNow - _dateTime).TotalMilliseconds / 10f;

        var angle = (float)Math.Sin(Scalar.DegreesToRadians(time) / _timeScale);
        var rotation = Matrix4X4.CreateFromAxisAngle(new Vector3D<float>(0, 0, 1), angle);

        var preRotation = swapChain.PreTransform switch
        {
            SurfaceTransformFlagsKHR.Rotate90BitKhr => Matrix4X4.CreateRotationZ(0.5f),
            SurfaceTransformFlagsKHR.Rotate180BitKhr => Matrix4X4.CreateRotationZ(1f),
            SurfaceTransformFlagsKHR.Rotate270BitKhr => Matrix4X4.CreateRotationZ(1.5f),
            _ => Matrix4X4<float>.Identity
        };
        UniformBufferObject ubo = new()
        {
            // _model = Matrix4X4<float>.Identity * rotation,
            _model = Matrix4X4.CreateTranslation(_position) * rotation,
            _view =
                Matrix4X4.CreateLookAt(new Vector3D<float>(2, 2, 2), new Vector3D<float>(0, 0, 0),
                    new Vector3D<float>(0, 0, 1)),
            _proj = Matrix4X4.CreatePerspectiveFieldOfView(Scalar.DegreesToRadians(45.0f),
                (float)swapChain.Extent.Width / swapChain.Extent.Height, 0.1f, 10.0f),
        };
        ubo._proj.M22 *= -1;

        _uniformBuffers[currentImage].Fill(ubo);
    }


    public void ResetUniformBuffers()
    {
        _uniformBuffers.Dispose();
    }

    public struct Vertex
    {
        public Vector3D<float> _pos;
        public Vector3D<float> _color;
        public Vector2D<float> _texCoord;

        public static VertexInputBindingDescription GetBindingDescription()
        {
            VertexInputBindingDescription bindingDescription = new()
            {
                Binding = 0,
                Stride = (uint)Unsafe.SizeOf<Vertex>(),
                InputRate = VertexInputRate.Vertex
            };

            return bindingDescription;
        }

        public static VertexInputAttributeDescription[] GetAttributeDescriptions()
        {
            var attributeDescriptions = new[]
            {
                new VertexInputAttributeDescription
                {
                    Binding = 0,
                    Location = 0,
                    Format = Format.R32G32B32Sfloat,
                    Offset = (uint)Marshal.OffsetOf<Vertex>(nameof(_pos)),
                },
                new VertexInputAttributeDescription
                {
                    Binding = 0,
                    Location = 1,
                    Format = Format.R32G32B32Sfloat,
                    Offset = (uint)Marshal.OffsetOf<Vertex>(nameof(_color)),
                },
                new VertexInputAttributeDescription
                {
                    Binding = 0,
                    Location = 2,
                    Format = Format.R32G32Sfloat,
                    Offset = (uint)Marshal.OffsetOf<Vertex>(nameof(_texCoord)),
                },
            };

            return attributeDescriptions;
        }
    }

    [SuppressMessage("ReSharper", "NotAccessedField.Local")]
    struct UniformBufferObject
    {
        public Matrix4X4<float> _model;
        public Matrix4X4<float> _view;
        public Matrix4X4<float> _proj;
    }
}
