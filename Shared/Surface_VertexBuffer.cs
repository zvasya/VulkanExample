using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Silk.NET.Maths;
using Silk.NET.Vulkan;
using Buffer = Silk.NET.Vulkan.Buffer;

namespace Shared;

public unsafe partial class Surface
{
    struct Vertex
    {
        public Vector2D<float> _pos;
        public Vector3D<float> _color;

        public static VertexInputBindingDescription GetBindingDescription()
        {
            VertexInputBindingDescription bindingDescription = new()
            {
                Binding = 0,
                Stride = (uint)Unsafe.SizeOf<Vertex>(),
                InputRate = VertexInputRate.Vertex,
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
                    Format = Format.R32G32Sfloat,
                    Offset = (uint)Marshal.OffsetOf<Vertex>(nameof(_pos)),
                },
                new VertexInputAttributeDescription
                {
                    Binding = 0,
                    Location = 1,
                    Format = Format.R32G32B32Sfloat,
                    Offset = (uint)Marshal.OffsetOf<Vertex>(nameof(_color)),
                },
            };

            return attributeDescriptions;
        }
    }

    Buffer _vertexBuffer;
    DeviceMemory _vertexBufferMemory;
    
    void CreateVertexBuffer()
    {
        var bufferSize = (ulong)(Unsafe.SizeOf<Vertex>() * _vertices.Length);

        Buffer stagingBuffer = default;
        DeviceMemory stagingBufferMemory = default;
        CreateBuffer(bufferSize, BufferUsageFlags.TransferSrcBit, MemoryPropertyFlags.HostVisibleBit | MemoryPropertyFlags.HostCoherentBit, ref stagingBuffer, ref stagingBufferMemory);

        void* data;
        _vk.MapMemory(_device, stagingBufferMemory, 0, bufferSize, 0, &data);
        _vertices.AsSpan().CopyTo(new Span<Vertex>(data, _vertices.Length));
        _vk.UnmapMemory(_device, stagingBufferMemory);

        CreateBuffer(bufferSize, BufferUsageFlags.TransferDstBit | BufferUsageFlags.VertexBufferBit, MemoryPropertyFlags.DeviceLocalBit, ref _vertexBuffer, ref _vertexBufferMemory);

        CopyBuffer(stagingBuffer, _vertexBuffer, bufferSize);

        _vk.DestroyBuffer(_device, stagingBuffer, null);
        _vk.FreeMemory(_device, stagingBufferMemory, null);
    }
}
