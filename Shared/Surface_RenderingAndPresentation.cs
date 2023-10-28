using Silk.NET.Vulkan;
using Semaphore = Silk.NET.Vulkan.Semaphore;

namespace Shared;

public unsafe partial class Surface
{
    bool _framebufferResized;
    HelloSemaphore[] _imageAvailableSemaphores;
    HelloSemaphore[] _renderFinishedSemaphores;
    HelloFence[] _inFlightFences;
    
    uint _currentFrame = 0;

    void CreateSyncObjects(int count)
    {
        _imageAvailableSemaphores = new HelloSemaphore[count];
        _renderFinishedSemaphores = new HelloSemaphore[count];
        _inFlightFences = new HelloFence[count];

        for (int i = 0; i < count; i++)
        {
            _imageAvailableSemaphores[i] = HelloSemaphore.Create(_device);
            _renderFinishedSemaphores[i] = HelloSemaphore.Create(_device);
            _inFlightFences[i] = HelloFence.Create(_device);
        }
    }

    public void DrawFrame()
    {
        _inFlightFences[_currentFrame].WaitForFences(true, ulong.MaxValue);

        // Acquiring and image from the swap chain
        uint imageIndex = 0;
        var result = _swapChain.AcquireNextImage(ulong.MaxValue, _imageAvailableSemaphores[_currentFrame], default, ref imageIndex);
        if (result == Result.ErrorOutOfDateKhr)
        {
            RecreateSwapChain();
            return;
        }
        else if (result != Result.Success && result != Result.SuboptimalKhr)
        {
            throw new Exception("failed to acquire swap chain image!");
        }

        foreach (var rendererNode in _renderer)
        {
            rendererNode.UpdateUniformBuffer(_currentFrame, _swapChain);
        }
        
        _inFlightFences[_currentFrame].ResetFences();

        _commandBuffers[_currentFrame].ResetCommandBuffer(CommandBufferResetFlags.None);
        
        RecordCommandBuffer(_commandBuffers[_currentFrame], _currentFrame, imageIndex);

        // Submitting the command buffer
        var imageAvailableSemaphore = _imageAvailableSemaphores[_currentFrame].Semaphore;
        var waitSemaphores = stackalloc Semaphore[] { imageAvailableSemaphore };
        var waitStages = stackalloc PipelineStageFlags[] { PipelineStageFlags.ColorAttachmentOutputBit };
        var renderFinishedSemaphore = _renderFinishedSemaphores[_currentFrame].Semaphore;
        var signalSemaphores = stackalloc Semaphore[] { renderFinishedSemaphore };
        var commandBuffer = _commandBuffers[_currentFrame].CommandBuffer;
        var commandBuffersPtr = stackalloc CommandBuffer[] { commandBuffer };

        var submitInfo = new SubmitInfo
        {
            SType = StructureType.SubmitInfo,
            WaitSemaphoreCount = 1,
            PWaitSemaphores = waitSemaphores,
            PWaitDstStageMask = waitStages,
            CommandBufferCount = 1,
            PCommandBuffers = commandBuffersPtr,
            SignalSemaphoreCount = 1,
            PSignalSemaphores = signalSemaphores,
        };

        _inFlightFences[_currentFrame].ResetFences();
        
        var fence = _inFlightFences[_currentFrame].Fence;
        var queueSubmit = _device.GraphicsQueue.QueueSubmit(1, &submitInfo, fence);
        Helpers.CheckErrors(queueSubmit);

        // Presentation
        var swapChains = stackalloc SwapchainKHR[] { _swapChain };
        var presentInfo = new PresentInfoKHR
        {
            SType = StructureType.PresentInfoKhr,
            WaitSemaphoreCount = 1,
            PWaitSemaphores = signalSemaphores,
            SwapchainCount = 1,
            PSwapchains = swapChains,
            PImageIndices = &imageIndex,
            PResults = null, // Optional
        };

        result = _device.PresentQueue.QueuePresent(&presentInfo);

        if (result == Result.ErrorOutOfDateKhr || result == Result.SuboptimalKhr || _framebufferResized)
        {
            _framebufferResized = false;
            RecreateSwapChain();
        }
        else if (result != Result.Success)
        {
            throw new Exception("failed to present swap chain image!");
        }

        _currentFrame = (_currentFrame + 1) % HelloEngine.MAX_FRAMES_IN_FLIGHT;
        Helpers.CheckErrors(_device.PresentQueue.QueueWaitIdle());
    }
    
    void RecordCommandBuffer(HelloCommandBuffer commandBuffer, uint currentFrame, uint imageIndex) {
        var beginInfo = new CommandBufferBeginInfo
        {
            SType = StructureType.CommandBufferBeginInfo,
            Flags = 0, // Optional
            PInheritanceInfo = null, // Optional
        };
        Helpers.CheckErrors(commandBuffer.BeginCommandBuffer(&beginInfo));

        // Pass
        
        var viewport = new Viewport
        {
            X = 0.0f,
            Y = 0.0f,
            Width = _swapChain.Extent.Width,
            Height = _swapChain.Extent.Height,
            MinDepth = 0.0f,
            MaxDepth = 1.0f,
        };

        var scissor = new Rect2D
        {
            Offset = new Offset2D(0, 0),
            Extent = _swapChain.Extent,
        };


        ClearAttachments(commandBuffer, _swapChainFramebuffers[imageIndex], scissor);
        
        var clearColorValue = new ClearColorValue(0.0f, 0.0f, 0.0f, 1.0f);
        var depthColorValue = new ClearDepthStencilValue(1.0f, 0);
        var clearColor = new ClearValue
        {
            Color = clearColorValue,
        };
        var clearDepthStencil = new ClearValue
        {
            DepthStencil = depthColorValue
        };

        var clearValues = stackalloc ClearValue[] { clearColor, clearDepthStencil };
        
        var renderPassInfo = new RenderPassBeginInfo
        {
            SType = StructureType.RenderPassBeginInfo,
            RenderPass = _renderPass.RenderPass,
            Framebuffer = _swapChainFramebuffers[imageIndex].Framebuffer,
            RenderArea = new Rect2D(extent: new Extent2D(_swapChain.Extent.Width,_swapChain.Extent.Height)),
            ClearValueCount = 0,
            PClearValues = clearValues,
        };
        
        commandBuffer.CmdBeginRenderPass(&renderPassInfo, SubpassContents.Inline);

        
        commandBuffer.CmdSetViewport(0, 1, &viewport);
        commandBuffer.CmdSetScissor(0, 1, &scissor);
        
        foreach (var rendererNode in _renderer) 
            rendererNode.Draw(commandBuffer, _graphicsPipeline, currentFrame);
        
        commandBuffer.CmdEndRenderPass();
        Helpers.CheckErrors(commandBuffer.EndCommandBuffer());
    }

    void ClearAttachments(HelloCommandBuffer commandBuffer, HelloFrameBuffer frameBuffer, Rect2D scissor)
    {
        var clearColorValue = new ClearColorValue(0.0f, 0.0f, 0.0f, 1.0f);
        var clearDepthStencil = new ClearDepthStencilValue(1.0f, 0);
        var clearColor = new ClearValue[]
        {
            new ClearValue{Color = clearColorValue},
            new ClearValue{DepthStencil = clearDepthStencil},
        };
        var renderPassInfo = new RenderPassBeginInfo
        {
            SType = StructureType.RenderPassBeginInfo,
            RenderPass = _renderPass.RenderPass,
            Framebuffer = frameBuffer.Framebuffer,
            RenderArea = new Rect2D(extent: new Extent2D(_swapChain.Extent.Width, _swapChain.Extent.Height)),
        };

        fixed (ClearValue* clearColorPtr = clearColor)
        {
            renderPassInfo.ClearValueCount = (uint)clearColor.Length;
            renderPassInfo.PClearValues = clearColorPtr;
        }
        
        commandBuffer.CmdBeginRenderPass(&renderPassInfo, SubpassContents.Inline);

        var colorAttachment = new ClearAttachment()
        {
            AspectMask = ImageAspectFlags.ColorBit,
            ClearValue = new ClearValue
            {
                Color = clearColorValue
            },
            ColorAttachment = 0
        };
        var depthAttachment = new ClearAttachment()
        {
            AspectMask = ImageAspectFlags.DepthBit,
            ClearValue = new ClearValue
            {
                DepthStencil = clearDepthStencil
            },
            ColorAttachment = 1
        };
        var clearAttachment = stackalloc ClearAttachment[] { colorAttachment, depthAttachment };
        var rect = new ClearRect()
        {
            LayerCount = 1,
            Rect = scissor,
        };
        var rects = stackalloc ClearRect[] { rect };

        commandBuffer.CmdClearAttachments(2, clearAttachment, 1, rects);

        commandBuffer.CmdEndRenderPass();
    }

    public void ChangeSize()
    {
        _framebufferResized = true;
        // RecreateSwapChain();
        // DrawFrame();
    }
}
