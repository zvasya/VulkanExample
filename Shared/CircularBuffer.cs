using Silk.NET.Vulkan;

namespace Shared;

public class CircularBuffer : IDisposable
{
    protected readonly LogicalDevice Device;

    protected readonly HelloBuffer[] Buffers;

    protected CircularBuffer(LogicalDevice device, HelloBuffer[] buffers)
    {
        Device = device;
        Buffers = buffers;
    }

    public HelloBuffer this[uint index]
    {
        get { return Buffers[index]; }
    }

    public static CircularBuffer Create(LogicalDevice device, int count, ulong bufferSize, BufferUsageFlags bufferUsageFlags)
    {
        return new CircularBuffer(device, CreateBuffersArray(device, count, bufferSize, bufferUsageFlags));
    }

    protected static HelloBuffer[] CreateBuffersArray(LogicalDevice device, int count, ulong bufferSize, BufferUsageFlags bufferUsageFlags)
    {
        var buffers = new HelloBuffer[count];

        for (var i = 0; i < count; i++) 
            buffers[i] = HelloBuffer.Create(device, bufferSize, bufferUsageFlags, MemoryPropertyFlags.HostVisibleBit | MemoryPropertyFlags.HostCoherentBit);
        
        return buffers;
    }

    public void Dispose()
    {
        foreach (var buffer in Buffers)
            buffer.Dispose();
    }
}
