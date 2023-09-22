using System.Runtime.CompilerServices;
using Silk.NET.Vulkan;
using Buffer = Silk.NET.Vulkan.Buffer;

namespace Shared;


public unsafe partial class Surface
{
    Buffer _indexBuffer;
    DeviceMemory _indexBufferMemory;
    
    void CreateIndexBuffer()
    {
        var bufferSize = (ulong)(Unsafe.SizeOf<ushort>() * _indices.Length);

        Buffer stagingBuffer = default;
        DeviceMemory stagingBufferMemory = default;
        CreateBuffer(bufferSize, BufferUsageFlags.TransferSrcBit, MemoryPropertyFlags.HostVisibleBit | MemoryPropertyFlags.HostCoherentBit, ref stagingBuffer, ref stagingBufferMemory);

        void* data;
        _vk.MapMemory(_device, stagingBufferMemory, 0, bufferSize, 0, &data);
        _indices.AsSpan().CopyTo(new Span<ushort>(data, _indices.Length));
        _vk.UnmapMemory(_device, stagingBufferMemory);

        CreateBuffer(bufferSize, BufferUsageFlags.TransferDstBit | BufferUsageFlags.IndexBufferBit, MemoryPropertyFlags.DeviceLocalBit, ref _indexBuffer, ref _indexBufferMemory);

        CopyBuffer(stagingBuffer, _indexBuffer, bufferSize);

        _vk.DestroyBuffer(_device, stagingBuffer, null);
        _vk.FreeMemory(_device, stagingBufferMemory, null);
    }
}
