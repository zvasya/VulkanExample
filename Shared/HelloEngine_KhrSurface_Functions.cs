using Silk.NET.Vulkan;

// ReSharper disable UnusedMember.Global

namespace Shared;

public partial class HelloEngine
{
    public unsafe void DestroySurface(SurfaceKHR surface, AllocationCallbacks* pAllocator)
    {
        VkSurface.DestroySurface(_instance, surface, pAllocator);
    }

    public void DestroySurface(SurfaceKHR surface, in AllocationCallbacks pAllocator)
    {
        VkSurface.DestroySurface(_instance, surface, in pAllocator);
    }
}
