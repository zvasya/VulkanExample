using System.Collections;
using Silk.NET.Vulkan;

namespace Shared;

public unsafe class HelloCommandBufferArray : IDisposable
{
    readonly HelloCommandBuffer[] _buffers;
    readonly CommandBuffer[] _commandBuffers;
    readonly HelloCommandPool _commandPool;
        
    HelloCommandBufferArray(CommandBuffer[] commandBuffers, HelloCommandPool commandPool)
    {
        _commandBuffers = commandBuffers;
        _commandPool = commandPool;
        _buffers = new HelloCommandBuffer[_commandBuffers.Length];
        for (var i = 0; i < _commandBuffers.Length; i++)
        {
            var commandBuffer = _commandBuffers[i];
            _buffers[i] = HelloCommandBuffer.Create(commandBuffer, null!);
        }
    }

    public static HelloCommandBufferArray Create(CommandBuffer[] commandBuffers, HelloCommandPool commandPool)
    {
        return new HelloCommandBufferArray(commandBuffers, commandPool);
    }
    void ReleaseUnmanagedResources()
    {
        fixed (CommandBuffer* buffers = _commandBuffers)
        {
            _commandPool.FreeCommandBuffers((uint)_commandBuffers.Length, buffers);
        }
    }

    public void Dispose()
    {
        ReleaseUnmanagedResources();
        foreach (var commandBuffer in _buffers) 
            GC.SuppressFinalize(commandBuffer);
        GC.SuppressFinalize(this);
    }

    
    ~HelloCommandBufferArray()
    {
        ReleaseUnmanagedResources();
    }

    public int Count => _buffers.Length;

    public HelloCommandBuffer this[uint i] => _buffers[i];
}
