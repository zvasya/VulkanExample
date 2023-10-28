using Silk.NET.Vulkan;

namespace Shared;

public unsafe partial class Surface
{
    HelloFrameBuffer[] _swapChainFramebuffers;

    void CreateFramebuffers2()
    {
        _swapChainFramebuffers = new HelloFrameBuffer[_swapChainImageViews.Length];

        for (var i = 0; i < _swapChainImageViews.Length; i++) 
            _swapChainFramebuffers[i] = HelloFrameBuffer.Create(_device, new[]{_swapChainImageViews[i], _depthImageView}, _renderPass.RenderPass, _swapChain.Extent.Width, _swapChain.Extent.Height);
    }
}
