using Silk.NET.Vulkan;

namespace Shared;

public partial class Surface
{
    HelloImageView[] _swapChainImageViews;

    void CreateImageViews2()
    {
        _swapChainImageViews = new HelloImageView[_swapChain.Images.Length];

        for (var i = 0; i < _swapChain.Images.Length; i++)
        {
            _swapChainImageViews[i] = HelloImageView.Create(_device, _swapChain.Images[i], _surfaceFormat.Format, ImageAspectFlags.ColorBit); 
        }
    }
}
