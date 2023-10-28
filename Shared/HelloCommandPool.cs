using Silk.NET.Vulkan;

namespace Shared;

public unsafe class HelloCommandPool : IDisposable
{
    readonly CommandPool _commandPool;

    readonly LogicalDevice _device;

    HelloCommandPool(LogicalDevice device, CommandPool commandPool)
    {
        _device = device;
        _commandPool = commandPool;
    }
    
    public static HelloCommandPool Create(LogicalDevice device)
    {
        var commandPool = CreateCommandPool(device);
        return new HelloCommandPool(device, commandPool);
    }

    HelloCommandBuffer CreateCommandBuffer()
    {
        var allocInfo = new CommandBufferAllocateInfo
        {
            SType = StructureType.CommandBufferAllocateInfo,
            CommandPool = _commandPool,
            Level = CommandBufferLevel.Primary,
            CommandBufferCount = 1,
        };
        
        Helpers.CheckErrors(_device.AllocateCommandBuffers(in allocInfo, out var commandBuffer));
        
        return HelloCommandBuffer.Create(commandBuffer, this);
    }

    public HelloCommandBufferArray CreateCommandBuffers(int count)
    {
        var allocInfo = new CommandBufferAllocateInfo
        {
            SType = StructureType.CommandBufferAllocateInfo,
            CommandPool = _commandPool,
            Level = CommandBufferLevel.Primary,
            CommandBufferCount = (uint)count,
        };
        
        var commandBuffers = new CommandBuffer[count];
        Helpers.CheckErrors(_device.AllocateCommandBuffers(in allocInfo, out commandBuffers[0]));
        
        return HelloCommandBufferArray.Create(commandBuffers, this);
    }

    static CommandPool CreateCommandPool(LogicalDevice device)
    {
        var queueFamilyIndices = device.PhysicalDevice.FindQueueFamilies();

        var poolInfo = new CommandPoolCreateInfo
        {
            SType = StructureType.CommandPoolCreateInfo,
            QueueFamilyIndex = queueFamilyIndices._graphicsFamily.Value,
            Flags = CommandPoolCreateFlags.ResetCommandBufferBit, // Optional,
        };

        CommandPool commandPool;
        
        Helpers.CheckErrors(device.CreateCommandPool(&poolInfo, null, &commandPool));

        return commandPool;
    }

    public void FreeCommandBuffers(uint count, CommandBuffer* commandBuffers)
    {
        _device.FreeCommandBuffers(_commandPool, count, commandBuffers);
    }
    
    public void FreeCommandBuffer(in CommandBuffer commandBuffer)
    {
        _device.FreeCommandBuffers(_commandPool, 1, commandBuffer);
    }
    
    public HelloCommandBuffer BeginSingleTimeCommands()
    {
        var commandBuffer = CreateCommandBuffer();

        CommandBufferBeginInfo beginInfo = new()
        {
            SType = StructureType.CommandBufferBeginInfo,
            Flags = CommandBufferUsageFlags.OneTimeSubmitBit,
        };

        commandBuffer.BeginCommandBuffer(beginInfo);

        return commandBuffer;
    }

    public void EndSingleTimeCommands(HelloCommandBuffer commandBuffer, HelloQueue queue)
    {
        commandBuffer.EndCommandBuffer();

        var cmdBuffer = commandBuffer.CommandBuffer;
        SubmitInfo submitInfo = new()
        {
            SType = StructureType.SubmitInfo,
            CommandBufferCount = 1,
            PCommandBuffers = &cmdBuffer,
        };

        queue.QueueSubmit(1, submitInfo, default);
        queue.QueueWaitIdle();
        
        commandBuffer.Dispose();
    }

    public void Dispose()
    {
        _device.DestroyCommandPool(_commandPool, null);
    }
}
