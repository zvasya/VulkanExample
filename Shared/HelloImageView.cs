using Silk.NET.Vulkan;
using Image = Silk.NET.Vulkan.Image;

namespace Shared;

public unsafe class HelloImageView : IDisposable
{
    readonly ImageView _imageView;

    readonly LogicalDevice _device;
    public ImageView ImageView => _imageView;
    HelloImageView(LogicalDevice device, ImageView imageView)
    {
        _device = device;
        _imageView = imageView;
    }
    
    public static HelloImageView Create(LogicalDevice device, HelloImage image, ImageAspectFlags imageAspect)
    {
        var imageView = CreateImageView(device, image, imageAspect);
        return new HelloImageView(device, imageView);
    }
    
    public static HelloImageView Create(LogicalDevice device, Image image, Format format, ImageAspectFlags imageAspect)
    {
        var imageView = CreateImageView(device, image, format, imageAspect);
        return new HelloImageView(device, imageView);
    }

    static ImageView CreateImageView(LogicalDevice device, HelloImage image, ImageAspectFlags aspectMask)
    {
        var createInfo = image.GetImageViewCreateInfo(aspectMask);

        ImageView imageView;
        
        Helpers.CheckErrors(device.CreateImageView(&createInfo, null, &imageView));
        return imageView;
    }

    static ImageView CreateImageView(LogicalDevice device, Image image, Format format, ImageAspectFlags aspectMask)
    {
        var createInfo = new ImageViewCreateInfo();
        createInfo.SType = StructureType.ImageViewCreateInfo;
        createInfo.Image = image;
        createInfo.ViewType = ImageViewType.Type2D;
        createInfo.Format = format;
        createInfo.Components.R = ComponentSwizzle.Identity;
        createInfo.Components.G = ComponentSwizzle.Identity;
        createInfo.Components.B = ComponentSwizzle.Identity;
        createInfo.Components.A = ComponentSwizzle.Identity;
        createInfo.SubresourceRange.AspectMask = aspectMask;
        createInfo.SubresourceRange.BaseMipLevel = 0;
        createInfo.SubresourceRange.LevelCount = 1;
        createInfo.SubresourceRange.BaseArrayLayer = 0;
        createInfo.SubresourceRange.LayerCount = 1;

        ImageView imageView;
        Helpers.CheckErrors(device.CreateImageView(&createInfo, null, &imageView));
        return imageView;
    }

    public void Dispose()
    {
        _device.DestroyImageView(_imageView, null);
    }
}
