using System.Runtime.CompilerServices;
using Silk.NET.Vulkan;
using Buffer = Silk.NET.Vulkan.Buffer;

namespace Shared;

public class HelloIndexBuffer : HelloBuffer
{
    public uint IndicesCount { get; }

    public void BindIndexBuffer(HelloCommandBuffer commandBuffer, IndexType indexType = IndexType.Uint16)
    {
        commandBuffer.CmdBindIndexBuffer(Buffer, 0, indexType);
    }
    
    public static HelloIndexBuffer Create(LogicalDevice device, uint indicesCount)
    {
        var size = (ulong)(Unsafe.SizeOf<ushort>() * indicesCount);
        device.CreateBuffer(size, BufferUsageFlags.TransferDstBit | BufferUsageFlags.IndexBufferBit, MemoryPropertyFlags.DeviceLocalBit, out var buffer, out var bufferMemory);
        return new HelloIndexBuffer(device, size, indicesCount, buffer, bufferMemory);
    }

    HelloIndexBuffer(LogicalDevice device, ulong bufferSize, uint indicesCount, Buffer buffer, DeviceMemory bufferMemory) : base(device, bufferSize, buffer, bufferMemory)
    {
        IndicesCount = indicesCount;
    }
}
