using Silk.NET.Vulkan;
using Buffer = Silk.NET.Vulkan.Buffer;

namespace Shared;

public class HelloIndexBuffer : HelloBuffer
{
    public void BindIndexBuffer(HelloCommandBuffer commandBuffer, IndexType indexType = IndexType.Uint16)
    {
        commandBuffer.CmdBindIndexBuffer(Buffer, 0, indexType);
    }
    
    public static HelloIndexBuffer Create(LogicalDevice device, ulong bufferSize)
    {
        device.CreateBuffer(bufferSize, BufferUsageFlags.TransferDstBit | BufferUsageFlags.IndexBufferBit, MemoryPropertyFlags.DeviceLocalBit, out var buffer, out var bufferMemory);
        return new HelloIndexBuffer(device, bufferSize, buffer, bufferMemory);
    }

    HelloIndexBuffer(LogicalDevice device, ulong bufferSize, Buffer buffer, DeviceMemory bufferMemory) : base(device, bufferSize, buffer, bufferMemory)
    {
    }
}
