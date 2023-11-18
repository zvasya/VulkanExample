using System.Runtime.CompilerServices;
using Silk.NET.Vulkan;
using Buffer = Silk.NET.Vulkan.Buffer;

namespace Shared;

public class HelloIndexBuffer : HelloBuffer
{
    public uint IndicesCount { get; }

    public IndexType IndexType { get; }

    public void BindIndexBuffer(HelloCommandBuffer commandBuffer)
    {
        commandBuffer.CmdBindIndexBuffer(Buffer, 0, IndexType);
    }
    
    public static HelloIndexBuffer Create(LogicalDevice device, ulong bufferSize, uint indicesCount, IndexType indexType)
    {
        device.CreateBuffer(bufferSize, BufferUsageFlags.TransferDstBit | BufferUsageFlags.IndexBufferBit, MemoryPropertyFlags.DeviceLocalBit, out var buffer, out var bufferMemory);
        return new HelloIndexBuffer(device, bufferSize, indicesCount, indexType, buffer, bufferMemory);
    }

    HelloIndexBuffer(LogicalDevice device, ulong bufferSize, uint indicesCount, IndexType indexType, Buffer buffer, DeviceMemory bufferMemory) : base(device, bufferSize, buffer, bufferMemory)
    {
        IndicesCount = indicesCount;
        IndexType = indexType;
    }
}
