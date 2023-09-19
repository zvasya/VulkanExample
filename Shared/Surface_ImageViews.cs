using Silk.NET.Vulkan;

namespace Shared;

public unsafe partial class Surface
{
    ImageView[] _swapChainImageViews;

    void CreateImageViews()
    {
        _swapChainImageViews = new ImageView[_swapChainImages.Length];

        for (var i = 0; i < _swapChainImages.Length; i++)
        {
            var createInfo = new ImageViewCreateInfo();
            createInfo.SType = StructureType.ImageViewCreateInfo;
            createInfo.Image = _swapChainImages[i];
            createInfo.ViewType = ImageViewType.Type2D;
            createInfo.Format = _swapChainImageFormat;
            createInfo.Components.R = ComponentSwizzle.Identity;
            createInfo.Components.G = ComponentSwizzle.Identity;
            createInfo.Components.B = ComponentSwizzle.Identity;
            createInfo.Components.A = ComponentSwizzle.Identity;
            createInfo.SubresourceRange.AspectMask = ImageAspectFlags.ColorBit;
            createInfo.SubresourceRange.BaseMipLevel = 0;
            createInfo.SubresourceRange.LevelCount = 1;
            createInfo.SubresourceRange.BaseArrayLayer = 0;
            createInfo.SubresourceRange.LayerCount = 1;

            fixed (ImageView* swapChainImageViewPtr = &_swapChainImageViews[i])
            {
                Helpers.CheckErrors(_vk.CreateImageView(_device, &createInfo, null, swapChainImageViewPtr));
            }
        }
    }
}
