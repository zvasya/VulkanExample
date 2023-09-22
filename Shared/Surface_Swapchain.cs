using Silk.NET.Vulkan;

namespace Shared;

public unsafe partial class Surface
{
    SwapchainKHR _swapChain;
    Image[] _swapChainImages;
    Format _swapChainImageFormat;
    Extent2D _swapChainExtent;

    public struct SwapChainSupportDetails
    {
        public SurfaceCapabilitiesKHR _capabilities;
        public SurfaceFormatKHR[] _formats;
        public PresentModeKHR[] _presentModes;
    }

    SwapChainSupportDetails QuerySwapChainSupport(PhysicalDevice physicalDevice)
    {
        SwapChainSupportDetails details = default;

        // Capabilities
        Helpers.CheckErrors(_vkSurface.GetPhysicalDeviceSurfaceCapabilities(physicalDevice, _surface, &details._capabilities));

        // Formats
        uint formatCount;
        Helpers.CheckErrors(_vkSurface.GetPhysicalDeviceSurfaceFormats(physicalDevice, _surface, &formatCount, null));

        if (formatCount != 0)
        {
            details._formats = new SurfaceFormatKHR[formatCount];
            fixed (SurfaceFormatKHR* formatsPtr = &details._formats[0])
            {
                Helpers.CheckErrors(_vkSurface.GetPhysicalDeviceSurfaceFormats(physicalDevice, _surface, &formatCount, formatsPtr));
            }
        }

        // Present Modes
        uint presentModeCount;
        Helpers.CheckErrors(_vkSurface.GetPhysicalDeviceSurfacePresentModes(physicalDevice, _surface, &presentModeCount, null));

        if (presentModeCount != 0)
        {
            details._presentModes = new PresentModeKHR[presentModeCount];
            fixed (PresentModeKHR* presentModesPtr = &details._presentModes[0])
            {
                Helpers.CheckErrors(_vkSurface.GetPhysicalDeviceSurfacePresentModes(physicalDevice, _surface, &presentModeCount, presentModesPtr));
            }
        }

        return details;
    }

    SurfaceFormatKHR ChooseSwapSurfaceFormat(SurfaceFormatKHR[] availableFormats)
    {
        foreach (var availableFormat in availableFormats)
        {
            if (availableFormat.Format == Format.B8G8R8A8Srgb && availableFormat.ColorSpace == ColorSpaceKHR.SpaceSrgbNonlinearKhr)
            {
                return availableFormat;
            }
        }

        return availableFormats[0];
    }

    PresentModeKHR ChooseSwapPresentMode(PresentModeKHR[] availablePresentModes)
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

    Extent2D ChooseSwapExtent(SurfaceCapabilitiesKHR capabilities)
    {
        if (capabilities.CurrentExtent.Width != uint.MaxValue)
        {
            return capabilities.CurrentExtent;
        }

        var actualExtent = new Extent2D(100, 100);

        if (capabilities.MaxImageExtent.Width < 1 || capabilities.MaxImageExtent.Height < 1)
        {
            _framebufferResized = true;
        }

        actualExtent.Width = Math.Max(capabilities.MinImageExtent.Width, Math.Min(capabilities.MaxImageExtent.Width, actualExtent.Width));
        actualExtent.Height = Math.Max(capabilities.MinImageExtent.Height, Math.Min(capabilities.MaxImageExtent.Height, actualExtent.Height));

        return actualExtent;
    }

    void CreateSwapChain()
    {
        // Create SwapChain
        var swapChainSupport = this.QuerySwapChainSupport(this._physicalDevice);

        var surfaceFormat = this.ChooseSwapSurfaceFormat(swapChainSupport._formats);
        var presentMode = this.ChooseSwapPresentMode(swapChainSupport._presentModes);
        var extent = this.ChooseSwapExtent(swapChainSupport._capabilities);

        var imageCount = swapChainSupport._capabilities.MinImageCount + 1;
        if (swapChainSupport._capabilities.MaxImageCount > 0 && imageCount > swapChainSupport._capabilities.MaxImageCount)
        {
            imageCount = swapChainSupport._capabilities.MaxImageCount;
        }

        var createInfo = new SwapchainCreateInfoKHR();
        createInfo.SType = StructureType.SwapchainCreateInfoKhr;
        createInfo.Surface = _surface;
        createInfo.MinImageCount = imageCount;
        createInfo.ImageFormat = surfaceFormat.Format;
        createInfo.ImageColorSpace = surfaceFormat.ColorSpace;
        createInfo.ImageExtent = extent;
        createInfo.ImageArrayLayers = 1;
        createInfo.ImageUsage = ImageUsageFlags.ColorAttachmentBit;

        var indices = this.FindQueueFamilies(this._physicalDevice);
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
        createInfo.PreTransform = swapChainSupport._capabilities.CurrentTransform;
        createInfo.CompositeAlpha = CompositeAlphaFlagsKHR.OpaqueBitKhr;
        createInfo.PresentMode = presentMode;
        createInfo.Clipped = true;
        createInfo.OldSwapchain = default;

        fixed (SwapchainKHR* swapChainPtr = &_swapChain)
        {
            Helpers.CheckErrors(_khrSwapchain.CreateSwapchain(_device, &createInfo, null, swapChainPtr));
        }

        // SwapChain Images
        _khrSwapchain.GetSwapchainImages(_device, _swapChain, &imageCount, null);
        this._swapChainImages = new Image[imageCount];
        fixed (Image* swapChainImagesPtr = &this._swapChainImages[0])
        {
            _khrSwapchain.GetSwapchainImages(_device, _swapChain, &imageCount, swapChainImagesPtr);
        }

        this._swapChainImageFormat = surfaceFormat.Format;
        this._swapChainExtent = extent;
    }

    void RecreateSwapChain()
    {
        _ = _vk.DeviceWaitIdle(_device);

        CleanupSwapchain();

        CreateSwapChain();
        CreateImageViews();
        CreateRenderPass();
        CreateGraphicsPipeline();
        CreateFramebuffers();
        CreateUniformBuffers();
        CreateDescriptorPool();
        CreateDescriptorSets();
        CreateCommandBuffers();
    }

    void CleanupSwapchain()
    {
        foreach (var framebuffer in _swapChainFramebuffers) 
            _vk.DestroyFramebuffer(_device, framebuffer, null);

        fixed (CommandBuffer* buffers = _commandBuffers) 
            _vk.FreeCommandBuffers(_device, _commandPool, (uint)_commandBuffers.Length, buffers);

        _vk.DestroyPipeline(_device, _graphicsPipeline, null);
        _vk.DestroyPipelineLayout(_device, _pipelineLayout, null);
        _vk.DestroyRenderPass(_device, _renderPass, null);

        foreach (var imageView in _swapChainImageViews) 
            _vk.DestroyImageView(_device, imageView, null);

        _khrSwapchain.DestroySwapchain(_device, _swapChain, null);
        
        for (var i = 0; i < _swapChainImages.Length; i++)
        {
            _vk.DestroyBuffer(_device, _uniformBuffers![i], null);
            _vk.FreeMemory(_device, _uniformBuffersMemory![i], null);
        }
        
        _vk.DestroyDescriptorPool(_device, _descriptorPool, null);
    }

}
