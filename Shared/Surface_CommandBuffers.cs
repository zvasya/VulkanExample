using Silk.NET.Vulkan;

namespace Shared;

public unsafe partial class Surface
{
    CommandPool _commandPool;
    CommandBuffer[] _commandBuffers;

    void CreateCommandPool()
    {
        var queueFamilyIndices = this.FindQueueFamilies(this._physicalDevice);

        var poolInfo = new CommandPoolCreateInfo
        {
            SType = StructureType.CommandPoolCreateInfo,
            QueueFamilyIndex = queueFamilyIndices._graphicsFamily.Value,
            Flags = 0, // Optional,
        };

        fixed (CommandPool* commandPoolPtr = &this._commandPool)
        {
            Helpers.CheckErrors(_vk.CreateCommandPool(_device, &poolInfo, null, commandPoolPtr));
        }
    }

    void CreateCommandBuffers()
    {
        this._commandBuffers = new CommandBuffer[this._swapChainFramebuffers.Length];

        var allocInfo = new CommandBufferAllocateInfo
        {
            SType = StructureType.CommandBufferAllocateInfo,
            CommandPool = _commandPool,
            Level = CommandBufferLevel.Primary,
            CommandBufferCount = (uint)_commandBuffers.Length,
        };

        fixed (CommandBuffer* commandBuffersPtr = &this._commandBuffers[0])
        {
            Helpers.CheckErrors(_vk.AllocateCommandBuffers(this._device, &allocInfo, commandBuffersPtr));
        }

        // Begin
        for (uint i = 0; i < this._commandBuffers.Length; i++)
        {
            var beginInfo = new CommandBufferBeginInfo
            {
                SType = StructureType.CommandBufferBeginInfo,
                Flags = 0, // Optional
                PInheritanceInfo = null, // Optional
            };

            Helpers.CheckErrors(_vk.BeginCommandBuffer(this._commandBuffers[i], &beginInfo));

            // Pass
            var clearColor = new ClearValue
            {
                Color = new ClearColorValue(0.0f, 0.0f, 0.0f, 1.0f),
            };

            var renderPassInfo = new RenderPassBeginInfo
            {
                SType = StructureType.RenderPassBeginInfo,
                RenderPass = this._renderPass,
                Framebuffer = this._swapChainFramebuffers[i],
                RenderArea = new Rect2D(extent: new Extent2D(_swapChainExtent.Width,_swapChainExtent.Height)),
                ClearValueCount = 1,
                PClearValues = &clearColor,
            };

            _vk.CmdBeginRenderPass(this._commandBuffers[i], &renderPassInfo, SubpassContents.Inline);

            // Draw
            _vk.CmdBindPipeline(this._commandBuffers[i], PipelineBindPoint.Graphics, this._graphicsPipeline);

            _vk.CmdDraw(this._commandBuffers[i], 3, 1, 0, 0);

            _vk.CmdEndRenderPass(this._commandBuffers[i]);

            Helpers.CheckErrors(_vk.EndCommandBuffer(this._commandBuffers[i]));
        }
    }
}
