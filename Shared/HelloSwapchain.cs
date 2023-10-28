using Silk.NET.Vulkan;
using Image = Silk.NET.Vulkan.Image;

namespace Shared;

public unsafe class HelloSwapchain : IDisposable
{
    readonly SwapchainKHR _swapChain;
    readonly Extent2D _swapChainExtent;
    readonly Image[] _swapChainImages;
    readonly SurfaceTransformFlagsKHR _preTransform;

    public static implicit operator SwapchainKHR(HelloSwapchain swapchain) => swapchain._swapChain; 
    public Image[] Images => _swapChainImages;
    public Extent2D Extent => _swapChainExtent;
    public SurfaceTransformFlagsKHR PreTransform => _preTransform;

    readonly LogicalDevice _device;

    HelloSwapchain(LogicalDevice device, SwapchainKHR swapChain, Image[] swapChainImages, Extent2D extent, SurfaceTransformFlagsKHR preTransform)
    {
        _device = device;
        _swapChain = swapChain;
        _swapChainExtent = extent;
        _preTransform = preTransform;
        _swapChainImages = swapChainImages;
    }

    public static HelloSwapchain Create(LogicalDevice device, SurfaceKHR surface, Surface.SwapChainSupportDetails swapChainSupport, SurfaceFormatKHR surfaceFormat)
    {
        var extent = ChooseSwapExtent(swapChainSupport._capabilities);
        
        // var preTransform =  swapChainSupport._capabilities.CurrentTransform;
        var preTransform =  SurfaceTransformFlagsKHR.IdentityBitKhr;
        
        var imageCount = swapChainSupport._capabilities.MinImageCount + 1;
        if (swapChainSupport._capabilities.MaxImageCount > 0 && imageCount > swapChainSupport._capabilities.MaxImageCount)
        {
            imageCount = swapChainSupport._capabilities.MaxImageCount;
        }
        
        var swapChain = Init(device, surface, swapChainSupport, imageCount, surfaceFormat, extent, preTransform);
        var images = GetImages(device, swapChain, imageCount);
        
        return new HelloSwapchain(device, swapChain, images, extent, preTransform);
    }

    static SwapchainKHR Init(
        LogicalDevice device, 
        SurfaceKHR surface,
        Surface.SwapChainSupportDetails swapChainSupport, 
        uint imageCount,
        SurfaceFormatKHR surfaceFormat,
        Extent2D extent,
        SurfaceTransformFlagsKHR preTransform)
    {
        // Create SwapChain
        var presentMode = ChooseSwapPresentMode(swapChainSupport._presentModes);
        
        var createInfo = new SwapchainCreateInfoKHR
        {
            SType = StructureType.SwapchainCreateInfoKhr,
            Surface = surface,
            MinImageCount = imageCount,
            ImageFormat = surfaceFormat.Format,
            ImageColorSpace = surfaceFormat.ColorSpace,
            ImageExtent = extent,
            ImageArrayLayers = 1,
            ImageUsage = ImageUsageFlags.ColorAttachmentBit,
            PreTransform = preTransform,
        };

        var indices = device.PhysicalDevice.FindQueueFamilies();
        var queueFamilyIndices = stackalloc uint[] { indices._graphicsFamily.Value, indices._presentFamily.Value };

        if (indices._graphicsFamily != indices._presentFamily)
        {
            createInfo.ImageSharingMode = SharingMode.Concurrent;
            createInfo.QueueFamilyIndexCount = 2;
            createInfo.PQueueFamilyIndices = queueFamilyIndices;
        }
        else
        {
            createInfo.ImageSharingMode = SharingMode.Exclusive;
            createInfo.QueueFamilyIndexCount = 0; //Optional
            createInfo.PQueueFamilyIndices = null; //Optional
        }

        createInfo.CompositeAlpha = CompositeAlphaFlagsKHR.OpaqueBitKhr;
        createInfo.PresentMode = presentMode;
        createInfo.Clipped = true;
        createInfo.OldSwapchain = default;

        SwapchainKHR _swapChain;
        Helpers.CheckErrors(device.CreateSwapchain(&createInfo, null, &_swapChain));

        return _swapChain;
    }

    static Image[] GetImages(LogicalDevice device, SwapchainKHR swapChain,  uint imageCount)
    {
        // SwapChain Images
        device.GetSwapchainImages(swapChain, &imageCount, null);
        var swapChainImages = new Image[imageCount];
        fixed (Image* swapChainImagesPtr = &swapChainImages[0])
        {
            device.GetSwapchainImages(swapChain, &imageCount, swapChainImagesPtr);
        }

        return swapChainImages;
    }

    static PresentModeKHR ChooseSwapPresentMode(PresentModeKHR[] availablePresentModes)
    {
        foreach (var availablePresentMode in availablePresentModes)
        {
            if (availablePresentMode == PresentModeKHR.MailboxKhr)
            {
                return availablePresentMode;
            }
        }

        return PresentModeKHR.FifoKhr;
    }

    static Extent2D ChooseSwapExtent(SurfaceCapabilitiesKHR capabilities)
    {
        var actualExtent = new Extent2D(100, 100);
        if (capabilities.CurrentExtent.Width == uint.MaxValue)
        {
            actualExtent.Width = Math.Clamp(actualExtent.Width, capabilities.MinImageExtent.Width, capabilities.MaxImageExtent.Width);
            actualExtent.Height = Math.Clamp(actualExtent.Height, capabilities.MinImageExtent.Height, capabilities.MaxImageExtent.Height);
        }
        else
            actualExtent = capabilities.CurrentExtent;

        
        // if (capabilities.CurrentTransform.HasFlag(SurfaceTransformFlagsKHR.Rotate90BitKhr) ||
        // capabilities.CurrentTransform.HasFlag(SurfaceTransformFlagsKHR.Rotate270BitKhr))
        // {
        //     (actualExtent.Width, actualExtent.Height) = (actualExtent.Height, actualExtent.Width);
        // }

        return actualExtent;
    }

    public void Dispose()
    {
        _device.DestroySwapchain(_swapChain, null);
    }

    public Result AcquireNextImage(ulong timeout, HelloSemaphore imageAvailableSemaphore, Fence fence, ref uint imageIndex)
    {
        return _device.AcquireNextImage(_swapChain, timeout, imageAvailableSemaphore, fence,  ref imageIndex);
    }
}
