using Silk.NET.Vulkan;
using Buffer = Silk.NET.Vulkan.Buffer;

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

            var vertexBuffers = new Buffer[] { _vertexBuffer };
            var offsets = new ulong[] { 0 };

            fixed (ulong* offsetsPtr = offsets)
            fixed (Buffer* vertexBuffersPtr = vertexBuffers)
            {
                _vk.CmdBindVertexBuffers(_commandBuffers[i], 0, 1, vertexBuffersPtr, offsetsPtr);
            }

            _vk.CmdBindIndexBuffer(_commandBuffers[i], _indexBuffer, 0, IndexType.Uint16);

            _vk.CmdBindDescriptorSets(_commandBuffers[i], PipelineBindPoint.Graphics, _pipelineLayout, 0, 1, _descriptorSets![i], 0, null);
            
            _vk.CmdDrawIndexed(_commandBuffers[i], (uint)_indices.Length, 1, 0, 0, 0);


            _vk.CmdEndRenderPass(this._commandBuffers[i]);

            Helpers.CheckErrors(_vk.EndCommandBuffer(this._commandBuffers[i]));
        }
    }
}
