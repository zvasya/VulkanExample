using Silk.NET.Vulkan;
using Semaphore = Silk.NET.Vulkan.Semaphore;

namespace Shared;

public unsafe partial class Surface
{
    Semaphore _imageAvailableSemaphore;
    Semaphore _renderFinishedSemaphore;
    bool _framebufferResized;

    void CreateSemaphores()
    {
        var semaphoreInfo = new SemaphoreCreateInfo
        {
            SType = StructureType.SemaphoreCreateInfo,
        };

        fixed (Semaphore* imageAvailableSemaphorePtr = &this._imageAvailableSemaphore)
        {
            Helpers.CheckErrors(_vk.CreateSemaphore(this._device, &semaphoreInfo, null, imageAvailableSemaphorePtr));
        }

        fixed (Semaphore* renderFinishedSemaphorePtr = &this._renderFinishedSemaphore)
        {
            Helpers.CheckErrors(_vk.CreateSemaphore(this._device, &semaphoreInfo, null, renderFinishedSemaphorePtr));
        }
    }

    public void DrawFrame()
    {
        // Acquiring and image from the swap chain
        uint imageIndex;
        var result = _khrSwapchain.AcquireNextImage(this._device, this._swapChain, ulong.MaxValue, this._imageAvailableSemaphore, default, &imageIndex);
        if (result == Result.ErrorOutOfDateKhr)
        {
            RecreateSwapChain();
            return;
        }
        else if (result != Result.Success && result != Result.SuboptimalKhr)
        {
            throw new Exception("failed to acquire swap chain image!");
        }

        // Submitting the command buffer
        var waitSemaphores = stackalloc Semaphore[] { this._imageAvailableSemaphore };
        var waitStages = stackalloc PipelineStageFlags[] { PipelineStageFlags.ColorAttachmentOutputBit };
        var signalSemaphores = stackalloc Semaphore[] { this._renderFinishedSemaphore };
        var commandBuffersPtr = stackalloc CommandBuffer[] { _commandBuffers[imageIndex] };

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

        Helpers.CheckErrors(_vk.QueueSubmit(this._graphicsQueue, 1, &submitInfo, default));

        // Presentation
        var swapChains = stackalloc SwapchainKHR[] { this._swapChain };
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

        result = _khrSwapchain.QueuePresent(this._presentQueue, &presentInfo);
        
        if (result == Result.ErrorOutOfDateKhr || result == Result.SuboptimalKhr || _framebufferResized)
        {
            _framebufferResized = false;
            RecreateSwapChain();
        }
        else if (result != Result.Success)
        {
            throw new Exception("failed to present swap chain image!");
        }

        Helpers.CheckErrors(_vk.QueueWaitIdle(this._presentQueue));
    }

    public void ChangeSize()
    {
        _framebufferResized = true;
        RecreateSwapChain();
        DrawFrame();
    }
}