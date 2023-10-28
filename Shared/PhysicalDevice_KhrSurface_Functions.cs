using Silk.NET.Core;
using Silk.NET.Vulkan;
using static Shared.HelloEngine;

// ReSharper disable UnusedMember.Global

namespace Shared;

public partial class HelloPhysicalDevice
{
    public unsafe Result GetPhysicalDeviceSurfaceCapabilities(SurfaceKHR surface, SurfaceCapabilitiesKHR* pSurfaceCapabilities)
    {
        return VkSurface.GetPhysicalDeviceSurfaceCapabilities(_physicalDevice, surface, pSurfaceCapabilities);
    }

    public Result GetPhysicalDeviceSurfaceCapabilities(SurfaceKHR surface, out SurfaceCapabilitiesKHR pSurfaceCapabilities)
    {
        return VkSurface.GetPhysicalDeviceSurfaceCapabilities(_physicalDevice, surface, out pSurfaceCapabilities);
    }

    public unsafe Result GetPhysicalDeviceSurfaceFormats(SurfaceKHR surface, uint* pSurfaceFormatCount, SurfaceFormatKHR* pSurfaceFormats)
    {
        return VkSurface.GetPhysicalDeviceSurfaceFormats(_physicalDevice, surface, pSurfaceFormatCount, pSurfaceFormats);
    }

    public unsafe Result GetPhysicalDeviceSurfaceFormats(SurfaceKHR surface, uint* pSurfaceFormatCount, out SurfaceFormatKHR pSurfaceFormats)
    {
        return VkSurface.GetPhysicalDeviceSurfaceFormats(_physicalDevice, surface, pSurfaceFormatCount, out pSurfaceFormats);
    }

    public unsafe Result GetPhysicalDeviceSurfaceFormats(SurfaceKHR surface, ref uint pSurfaceFormatCount, SurfaceFormatKHR* pSurfaceFormats)
    {
        return VkSurface.GetPhysicalDeviceSurfaceFormats(_physicalDevice, surface, ref pSurfaceFormatCount, pSurfaceFormats);
    }

    public Result GetPhysicalDeviceSurfaceFormats(SurfaceKHR surface, ref uint pSurfaceFormatCount, out SurfaceFormatKHR pSurfaceFormats)
    {
        return VkSurface.GetPhysicalDeviceSurfaceFormats(_physicalDevice, surface, ref pSurfaceFormatCount, out pSurfaceFormats);
    }

    public unsafe Result GetPhysicalDeviceSurfacePresentModes(SurfaceKHR surface, uint* pPresentModeCount, PresentModeKHR* pPresentModes)
    {
        return VkSurface.GetPhysicalDeviceSurfacePresentModes(_physicalDevice, surface, pPresentModeCount, pPresentModes);
    }

    public unsafe Result GetPhysicalDeviceSurfacePresentModes(SurfaceKHR surface, uint* pPresentModeCount, out PresentModeKHR pPresentModes)
    {
        return VkSurface.GetPhysicalDeviceSurfacePresentModes(_physicalDevice, surface, pPresentModeCount, out pPresentModes);
    }

    public unsafe Result GetPhysicalDeviceSurfacePresentModes(SurfaceKHR surface, ref uint pPresentModeCount, PresentModeKHR* pPresentModes)
    {
        return VkSurface.GetPhysicalDeviceSurfacePresentModes(_physicalDevice, surface, ref pPresentModeCount, pPresentModes);
    }

    public Result GetPhysicalDeviceSurfacePresentModes(SurfaceKHR surface, ref uint pPresentModeCount, out PresentModeKHR pPresentModes)
    {
        return VkSurface.GetPhysicalDeviceSurfacePresentModes(_physicalDevice, surface, ref pPresentModeCount, out pPresentModes);
    }

    public unsafe Result GetPhysicalDeviceSurfaceSupport(uint queueFamilyIndex, SurfaceKHR surface, Bool32* pSupported)
    {
        return VkSurface.GetPhysicalDeviceSurfaceSupport(_physicalDevice, queueFamilyIndex, surface, pSupported);
    }

    public Result GetPhysicalDeviceSurfaceSupport(uint queueFamilyIndex, SurfaceKHR surface, out Bool32 pSupported)
    {
        return VkSurface.GetPhysicalDeviceSurfaceSupport(_physicalDevice, queueFamilyIndex, surface, out pSupported);
    }
}
