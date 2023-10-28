using Silk.NET.Vulkan;

namespace Shared;

public unsafe partial class HelloCommandBuffer : IDisposable
{
    readonly CommandBuffer _commandBuffer;
    readonly HelloCommandPool? _commandPool;

    public CommandBuffer CommandBuffer => _commandBuffer;
    
    HelloCommandBuffer(CommandBuffer commandBuffer, HelloCommandPool commandPool)
    {
        _commandBuffer = commandBuffer;
        _commandPool = commandPool;
    }

    public static HelloCommandBuffer Create(CommandBuffer commandBuffer, HelloCommandPool commandPool)
    {
        return new HelloCommandBuffer(commandBuffer, commandPool);
    }

    public void Dispose()
    {
        _commandPool?.FreeCommandBuffer(_commandBuffer);
    }
}
