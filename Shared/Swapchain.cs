// using Silk.NET.Vulkan;
//
// namespace Shared;
//
// public unsafe class Swapchain
// {
//     SwapchainKHR _swapChain;
//     
//     Extent2D _swapChainExtent;
//     Format _swapChainImageFormat;
//     Image[] _swapChainImages;
//     
//     HelloPhysicalDevice _physicalDevice;
//     LogicalDevice _device;
//     SurfaceKHR _surface;
//     
//     Swapchain(HelloPhysicalDevice physicalDevice, SurfaceKHR surface, LogicalDevice device)
//     {
//         _physicalDevice = physicalDevice;
//         _device = device;
//         _surface = surface;
//     }
//
//     public static Swapchain Create(HelloPhysicalDevice physicalDevice, SurfaceKHR surface, LogicalDevice device)
//     {
//         var swapchain = new Swapchain(physicalDevice, surface, device);
//         swapchain.Init();
//         return swapchain;
//     }
//
//     void Init()
//     {
//         var swapChainSupport = _physicalDevice.QuerySwapChainSupport(_surface);
//
//         var surfaceFormat = ChooseSwapSurfaceFormat(swapChainSupport._formats);
//         var presentMode = ChooseSwapPresentMode(swapChainSupport._presentModes);
//         var extent = ChooseSwapExtent(swapChainSupport._capabilities);
//
//         var imageCount = swapChainSupport._capabilities.MinImageCount + 1;
//         if (swapChainSupport._capabilities.MaxImageCount > 0 && imageCount > swapChainSupport._capabilities.MaxImageCount)
//         {
//             imageCount = swapChainSupport._capabilities.MaxImageCount;
//         }
//
//         var createInfo = new SwapchainCreateInfoKHR
//         {
//             SType = StructureType.SwapchainCreateInfoKhr,
//             Surface = _surface,
//             MinImageCount = imageCount,
//             ImageFormat = surfaceFormat.Format,
//             ImageColorSpace = surfaceFormat.ColorSpace,
//             ImageExtent = extent,
//             ImageArrayLayers = 1,
//             ImageUsage = ImageUsageFlags.ColorAttachmentBit,
//         };
//
//         var indices = _physicalDevice.FindQueueFamilies();
//         var queueFamilyIndices = stackalloc uint[] { indices._graphicsFamily.Value, indices._presentFamily.Value };
//
//         if (indices._graphicsFamily != indices._presentFamily)
//         {
//             createInfo.ImageSharingMode = SharingMode.Concurrent;
//             createInfo.QueueFamilyIndexCount = 2;
//             createInfo.PQueueFamilyIndices = queueFamilyIndices;
//         }
//         else
//         {
//             createInfo.ImageSharingMode = SharingMode.Exclusive;
//             createInfo.QueueFamilyIndexCount = 0; //Optional
//             createInfo.PQueueFamilyIndices = null; //Optional
//         }
//
//         createInfo.PreTransform = swapChainSupport._capabilities.CurrentTransform;
//         createInfo.CompositeAlpha = CompositeAlphaFlagsKHR.OpaqueBitKhr;
//         createInfo.PresentMode = presentMode;
//         createInfo.Clipped = true;
//         createInfo.OldSwapchain = default;
//
//         fixed (SwapchainKHR* swapChainPtr = &_swapChain)
//         {
//             Helpers.CheckErrors(_device.CreateSwapchain(&createInfo, null, swapChainPtr));
//         }
//
//         // SwapChain Images
//         _device.GetSwapchainImages(_swapChain, &imageCount, null);
//         _swapChainImages = new Image[imageCount];
//         fixed (Image* swapChainImagesPtr = &_swapChainImages[0])
//         {
//             _device.GetSwapchainImages(_swapChain, &imageCount, swapChainImagesPtr);
//         }
//
//         _swapChainImageFormat = surfaceFormat.Format;
//         _swapChainExtent = extent;
//     }
//     
//     SurfaceFormatKHR ChooseSwapSurfaceFormat(SurfaceFormatKHR[] availableFormats)
//     {
//         foreach (var availableFormat in availableFormats)
//         {
//             if (availableFormat.Format == Format.B8G8R8A8Srgb && availableFormat.ColorSpace == ColorSpaceKHR.SpaceSrgbNonlinearKhr)
//             {
//                 return availableFormat;
//             }
//         }
//
//         return availableFormats[0];
//     }
//
//     PresentModeKHR ChooseSwapPresentMode(PresentModeKHR[] availablePresentModes)
//     {
//         foreach (var availablePresentMode in availablePresentModes)
//         {
//             if (availablePresentMode == PresentModeKHR.MailboxKhr)
//             {
//                 return availablePresentMode;
//             }
//         }
//
//         return PresentModeKHR.FifoKhr;
//     }
//
//     Extent2D ChooseSwapExtent(SurfaceCapabilitiesKHR capabilities)
//     {
//         if (capabilities.CurrentExtent.Width != uint.MaxValue)
//         {
//             return capabilities.CurrentExtent;
//         }
//
//         var actualExtent = new Extent2D(100, 100);
//
//         if (capabilities.MaxImageExtent.Width < 1 || capabilities.MaxImageExtent.Height < 1)
//         {
//             _framebufferResized = true;
//         }
//
//         actualExtent.Width = Math.Max(capabilities.MinImageExtent.Width, Math.Min(capabilities.MaxImageExtent.Width, actualExtent.Width));
//         actualExtent.Height = Math.Max(capabilities.MinImageExtent.Height, Math.Min(capabilities.MaxImageExtent.Height, actualExtent.Height));
//
//         return actualExtent;
//     }
// }
