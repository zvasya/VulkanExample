using Silk.NET.Vulkan;
using Buffer = Silk.NET.Vulkan.Buffer;

namespace Shared;

public unsafe class HelloBuffer : IDisposable
{
    readonly LogicalDevice _device;
    protected readonly DeviceMemory BufferMemory;

    public Buffer Buffer { get; }
    readonly ulong _bufferSize;

    protected HelloBuffer(LogicalDevice device, ulong bufferSize, Buffer buffer, DeviceMemory bufferMemory)
    {
        _device = device;
        _bufferSize = bufferSize;
        Buffer = buffer;
        BufferMemory = bufferMemory;
    }

    public static HelloBuffer Create(LogicalDevice device, ulong bufferSize, BufferUsageFlags bufferUsageFlags, MemoryPropertyFlags memoryPropertyFlags)
    {
        device.CreateBuffer(bufferSize, bufferUsageFlags, memoryPropertyFlags, out var buffer, out var bufferMemory);
        return new HelloBuffer(device, bufferSize, buffer, bufferMemory);
    }

    public void FillStaging<T>(T[] data, HelloCommandPool commandPool, HelloQueue queue) where T : struct
    {
        using var staging = Create(_device, _bufferSize, BufferUsageFlags.TransferSrcBit, MemoryPropertyFlags.HostVisibleBit | MemoryPropertyFlags.HostCoherentBit);
        {
            staging.Fill(data);
            CopyBuffer(staging.Buffer, Buffer, _bufferSize, commandPool, queue);
        }
    }

    public delegate void Filler(void* dataPointer);
    public void Fill(Filler fill)
    {
        void* dataPointer;
        _device.MapMemory(BufferMemory, 0, _bufferSize, 0, &dataPointer);
        fill(dataPointer);
        _device.UnmapMemory(BufferMemory);
    }
    
    public void Fill<T>(T[] data) where T : struct
    {
        Fill(dataPointer => data.AsSpan().CopyTo(new Span<T>(dataPointer, data.Length)));
    }

    public void Fill<T>(T data) where T : struct
    {
        Fill(dataPointer => new Span<T>(dataPointer, 1)[0] = data);
    }

    public DescriptorBufferInfo BufferInfo()
    {
        return new DescriptorBufferInfo
        {
            Buffer = Buffer,
            Offset = 0,
            Range = _bufferSize,
        };
    }

    static void CopyBuffer(Buffer srcBuffer, Buffer dstBuffer, ulong size, HelloCommandPool commandPool, HelloQueue queue)
    {
        var commandBuffer = commandPool.BeginSingleTimeCommands();

        BufferCopy copyRegion = new()
        {
            Size = size,
        };

        commandBuffer.CmdCopyBuffer(srcBuffer, dstBuffer, 1, copyRegion);

        commandPool.EndSingleTimeCommands(commandBuffer, queue);
    }

    public void Dispose()
    {
        _device.DestroyBuffer(Buffer, null);
        _device.FreeMemory(BufferMemory, null);
    }
}
