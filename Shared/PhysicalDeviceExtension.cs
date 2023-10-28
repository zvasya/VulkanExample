using Silk.NET.Vulkan;

namespace Shared;

public static unsafe class PhysicalDeviceExtension
{
    public static Surface.SwapChainSupportDetails QuerySwapChainSupport(this PhysicalDevice physicalDevice, SurfaceKHR surface)
    {
        Surface.SwapChainSupportDetails details = default;

        // Capabilities
        Helpers.CheckErrors(HelloEngine.VkSurface.GetPhysicalDeviceSurfaceCapabilities(physicalDevice, surface, &details._capabilities));

        // Formats
        uint formatCount;
        Helpers.CheckErrors(HelloEngine.VkSurface.GetPhysicalDeviceSurfaceFormats(physicalDevice, surface, &formatCount, null));

        if (formatCount != 0)
        {
            details._formats = new SurfaceFormatKHR[formatCount];
            fixed (SurfaceFormatKHR* formatsPtr = &details._formats[0])
            {
                Helpers.CheckErrors(HelloEngine.VkSurface.GetPhysicalDeviceSurfaceFormats(physicalDevice, surface, &formatCount, formatsPtr));
            }
        }

        // Present Modes
        uint presentModeCount;
        Helpers.CheckErrors(HelloEngine.VkSurface.GetPhysicalDeviceSurfacePresentModes(physicalDevice, surface, &presentModeCount, null));

        if (presentModeCount != 0)
        {
            details._presentModes = new PresentModeKHR[presentModeCount];
            fixed (PresentModeKHR* presentModesPtr = &details._presentModes[0])
            {
                Helpers.CheckErrors(HelloEngine.VkSurface.GetPhysicalDeviceSurfacePresentModes(physicalDevice, surface, &presentModeCount, presentModesPtr));
            }
        }

        return details;
    }

    public static Result EnumerateDeviceExtensionProperties(
        this PhysicalDevice physicalDevice,
        string pLayerName,
        uint* pPropertyCount,
        ExtensionProperties* pProperties)
    {
        return HelloEngine.VK.EnumerateDeviceExtensionProperties(physicalDevice, pLayerName, pPropertyCount, pProperties);
    }
}
