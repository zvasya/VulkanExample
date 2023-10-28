using Silk.NET.Vulkan;

namespace Shared;

public partial class Surface
{
    HelloImage _depthImage;
    HelloImageView _depthImageView;
        
    void CreateDepthResources2() {
        
        var depthFormat = _device.PhysicalDevice.FindDepthFormat();
        
        _depthImage = HelloImage.Create(_device, _swapChain.Extent.Width, _swapChain.Extent.Height, depthFormat, ImageTiling.Optimal, ImageUsageFlags.DepthStencilAttachmentBit, MemoryPropertyFlags.DeviceLocalBit,  ImageType.Type2D);
        _depthImageView = HelloImageView.Create(_device, _depthImage, ImageAspectFlags.DepthBit);
    }
}
