using Silk.NET.Vulkan;
using Buffer = Silk.NET.Vulkan.Buffer;

namespace Shared;

public unsafe class HelloVertexBuffer : HelloBuffer
{
    public void BindVertexBuffers(HelloCommandBuffer commandBuffer)
    {
        var vertexBuffers = new[] { Buffer };
        var offsets = new ulong[] { 0 };

        fixed (ulong* offsetsPtr = offsets)
        fixed (Buffer* vertexBuffersPtr = vertexBuffers)
        {
            commandBuffer.CmdBindVertexBuffers(0, 1, vertexBuffersPtr, offsetsPtr);
        }
        
    }
    
    public static HelloVertexBuffer Create(LogicalDevice device, ulong bufferSize)
    {
        device.CreateBuffer(bufferSize, BufferUsageFlags.TransferDstBit | BufferUsageFlags.VertexBufferBit, MemoryPropertyFlags.DeviceLocalBit, out var buffer, out var bufferMemory);
        return new HelloVertexBuffer(device, bufferSize, buffer, bufferMemory);
    }

    HelloVertexBuffer(LogicalDevice device, ulong bufferSize, Buffer buffer, DeviceMemory bufferMemory) : base(device, bufferSize, buffer, bufferMemory)
    {
    }
}
