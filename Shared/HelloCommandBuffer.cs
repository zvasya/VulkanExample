using Silk.NET.Vulkan;

namespace Shared;

public unsafe partial class HelloCommandBuffer : IDisposable
{
    readonly CommandBuffer _commandBuffer;
    readonly HelloCommandPool? _commandPool;
    public uint currentImage = 0;

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
    
    public void CmdBindDescriptorSets(PipelineBindPoint pipelineBindPoint, PipelineLayout layout, uint firstSet, uint descriptorSetCount,
        in HelloDescriptorSets descriptorSets, uint dynamicOffsetCount, in uint pDynamicOffsets)
    {
        CmdBindDescriptorSets(pipelineBindPoint, layout, firstSet, descriptorSetCount, descriptorSets[currentImage], dynamicOffsetCount, pDynamicOffsets);
    }
}
