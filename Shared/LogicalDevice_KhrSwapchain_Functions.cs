using Silk.NET.Vulkan;
using Image = Silk.NET.Vulkan.Image;
using Semaphore = Silk.NET.Vulkan.Semaphore;

// ReSharper disable UnusedMember.Global

namespace Shared;

public partial class LogicalDevice
{
    public unsafe Result AcquireNextImage(SwapchainKHR swapchain, ulong timeout, Semaphore semaphore, Fence fence, uint* pImageIndex)
    {
        return _khrSwapchain.AcquireNextImage(_device, swapchain, timeout, semaphore, fence, pImageIndex);
    }

    public Result AcquireNextImage(SwapchainKHR swapchain, ulong timeout, Semaphore semaphore, Fence fence, ref uint pImageIndex)
    {
        return _khrSwapchain.AcquireNextImage(_device, swapchain, timeout, semaphore, fence, ref pImageIndex);
    }

    public unsafe Result AcquireNextImage2(AcquireNextImageInfoKHR* pAcquireInfo, uint* pImageIndex)
    {
        return _khrSwapchain.AcquireNextImage2(_device, pAcquireInfo, pImageIndex);
    }

    public unsafe Result AcquireNextImage2(AcquireNextImageInfoKHR* pAcquireInfo, ref uint pImageIndex)
    {
        return _khrSwapchain.AcquireNextImage2(_device, pAcquireInfo, ref pImageIndex);
    }

    public unsafe Result AcquireNextImage2(in AcquireNextImageInfoKHR pAcquireInfo, uint* pImageIndex)
    {
        return _khrSwapchain.AcquireNextImage2(_device, in pAcquireInfo, pImageIndex);
    }

    public Result AcquireNextImage2(in AcquireNextImageInfoKHR pAcquireInfo, ref uint pImageIndex)
    {
        return _khrSwapchain.AcquireNextImage2(_device, in pAcquireInfo, ref pImageIndex);
    }

    public unsafe Result CreateSwapchain(SwapchainCreateInfoKHR* pCreateInfo, AllocationCallbacks* pAllocator, SwapchainKHR* pSwapchain)
    {
        return _khrSwapchain.CreateSwapchain(_device, pCreateInfo, pAllocator, pSwapchain);
    }

    public unsafe Result CreateSwapchain(SwapchainCreateInfoKHR* pCreateInfo, AllocationCallbacks* pAllocator, out SwapchainKHR pSwapchain)
    {
        return _khrSwapchain.CreateSwapchain(_device, pCreateInfo, pAllocator, out pSwapchain);
    }

    public unsafe Result CreateSwapchain(SwapchainCreateInfoKHR* pCreateInfo, in AllocationCallbacks pAllocator, SwapchainKHR* pSwapchain)
    {
        return _khrSwapchain.CreateSwapchain(_device, pCreateInfo, in pAllocator, pSwapchain);
    }

    public unsafe Result CreateSwapchain(SwapchainCreateInfoKHR* pCreateInfo, in AllocationCallbacks pAllocator, out SwapchainKHR pSwapchain)
    {
        return _khrSwapchain.CreateSwapchain(_device, pCreateInfo, in pAllocator, out pSwapchain);
    }

    public unsafe Result CreateSwapchain(in SwapchainCreateInfoKHR pCreateInfo, AllocationCallbacks* pAllocator, SwapchainKHR* pSwapchain)
    {
        return _khrSwapchain.CreateSwapchain(_device, in pCreateInfo, pAllocator, pSwapchain);
    }

    public unsafe Result CreateSwapchain(in SwapchainCreateInfoKHR pCreateInfo, AllocationCallbacks* pAllocator, out SwapchainKHR pSwapchain)
    {
        return _khrSwapchain.CreateSwapchain(_device, in pCreateInfo, pAllocator, out pSwapchain);
    }

    public unsafe Result CreateSwapchain(in SwapchainCreateInfoKHR pCreateInfo, in AllocationCallbacks pAllocator, SwapchainKHR* pSwapchain)
    {
        return _khrSwapchain.CreateSwapchain(_device, in pCreateInfo, in pAllocator, pSwapchain);
    }

    public Result CreateSwapchain(in SwapchainCreateInfoKHR pCreateInfo, in AllocationCallbacks pAllocator, out SwapchainKHR pSwapchain)
    {
        return _khrSwapchain.CreateSwapchain(_device, in pCreateInfo, in pAllocator, out pSwapchain);
    }

    public unsafe void DestroySwapchain(SwapchainKHR swapchain, AllocationCallbacks* pAllocator)
    {
        _khrSwapchain.DestroySwapchain(_device, swapchain, pAllocator);
    }

    public void DestroySwapchain(SwapchainKHR swapchain, in AllocationCallbacks pAllocator)
    {
        _khrSwapchain.DestroySwapchain(_device, swapchain, in pAllocator);
    }

    public unsafe Result GetDeviceGroupPresentCapabilities(DeviceGroupPresentCapabilitiesKHR* pDeviceGroupPresentCapabilities)
    {
        return _khrSwapchain.GetDeviceGroupPresentCapabilities(_device, pDeviceGroupPresentCapabilities);
    }

    public Result GetDeviceGroupPresentCapabilities(out DeviceGroupPresentCapabilitiesKHR pDeviceGroupPresentCapabilities)
    {
        return _khrSwapchain.GetDeviceGroupPresentCapabilities(_device, out pDeviceGroupPresentCapabilities);
    }

    public unsafe Result GetDeviceGroupSurfacePresentModes(SurfaceKHR surface, DeviceGroupPresentModeFlagsKHR* pModes)
    {
        return _khrSwapchain.GetDeviceGroupSurfacePresentModes(_device, surface, pModes);
    }

    public Result GetDeviceGroupSurfacePresentModes(SurfaceKHR surface, out DeviceGroupPresentModeFlagsKHR pModes)
    {
        return _khrSwapchain.GetDeviceGroupSurfacePresentModes(_device, surface, out pModes);
    }

    public unsafe Result GetSwapchainImages(SwapchainKHR swapchain, uint* pSwapchainImageCount, Image* pSwapchainImages)
    {
        return _khrSwapchain.GetSwapchainImages(_device, swapchain, pSwapchainImageCount, pSwapchainImages);
    }

    public unsafe Result GetSwapchainImages(SwapchainKHR swapchain, uint* pSwapchainImageCount, out Image pSwapchainImages)
    {
        return _khrSwapchain.GetSwapchainImages(_device, swapchain, pSwapchainImageCount, out pSwapchainImages);
    }

    public unsafe Result GetSwapchainImages(SwapchainKHR swapchain, ref uint pSwapchainImageCount, Image* pSwapchainImages)
    {
        return _khrSwapchain.GetSwapchainImages(_device, swapchain, ref pSwapchainImageCount, pSwapchainImages);
    }

    public Result GetSwapchainImages(SwapchainKHR swapchain, ref uint pSwapchainImageCount, out Image pSwapchainImages)
    {
        return _khrSwapchain.GetSwapchainImages(_device, swapchain, ref pSwapchainImageCount, out pSwapchainImages);
    }
}
