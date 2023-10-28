using Silk.NET.Core;
using Silk.NET.Vulkan;

// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBePrivate.Global

namespace Shared;

public partial class HelloEngine
{
    public unsafe void DestroyInstance(AllocationCallbacks* pAllocator)
    {
        VK.DestroyInstance(_instance, pAllocator);
    }

    public void DestroyInstance(in AllocationCallbacks pAllocator)
    {
        VK.DestroyInstance(_instance, in pAllocator);
    }

    public unsafe Result EnumeratePhysicalDeviceGroups(uint* pPhysicalDeviceGroupCount, PhysicalDeviceGroupProperties* pPhysicalDeviceGroupProperties)
    {
        return VK.EnumeratePhysicalDeviceGroups(_instance, pPhysicalDeviceGroupCount, pPhysicalDeviceGroupProperties);
    }

    public unsafe Result EnumeratePhysicalDeviceGroups(uint* pPhysicalDeviceGroupCount, ref PhysicalDeviceGroupProperties pPhysicalDeviceGroupProperties)
    {
        return VK.EnumeratePhysicalDeviceGroups(_instance, pPhysicalDeviceGroupCount, ref pPhysicalDeviceGroupProperties);
    }

    public unsafe Result EnumeratePhysicalDeviceGroups(ref uint pPhysicalDeviceGroupCount, PhysicalDeviceGroupProperties* pPhysicalDeviceGroupProperties)
    {
        return VK.EnumeratePhysicalDeviceGroups(_instance, ref pPhysicalDeviceGroupCount, pPhysicalDeviceGroupProperties);
    }

    public Result EnumeratePhysicalDeviceGroups(ref uint pPhysicalDeviceGroupCount, ref PhysicalDeviceGroupProperties pPhysicalDeviceGroupProperties)
    {
        return VK.EnumeratePhysicalDeviceGroups(_instance, ref pPhysicalDeviceGroupCount, ref pPhysicalDeviceGroupProperties);
    }

    public unsafe Result EnumeratePhysicalDevices(uint* pPhysicalDeviceCount, PhysicalDevice* pPhysicalDevices)
    {
        return VK.EnumeratePhysicalDevices(_instance, pPhysicalDeviceCount, pPhysicalDevices);
    }

    public unsafe Result EnumeratePhysicalDevices(uint* pPhysicalDeviceCount, ref PhysicalDevice pPhysicalDevices)
    {
        return VK.EnumeratePhysicalDevices(_instance, pPhysicalDeviceCount, ref pPhysicalDevices);
    }

    public unsafe Result EnumeratePhysicalDevices(ref uint pPhysicalDeviceCount, PhysicalDevice* pPhysicalDevices)
    {
        return VK.EnumeratePhysicalDevices(_instance, ref pPhysicalDeviceCount, pPhysicalDevices);
    }

    public Result EnumeratePhysicalDevices(ref uint pPhysicalDeviceCount, ref PhysicalDevice pPhysicalDevices)
    {
        return VK.EnumeratePhysicalDevices(_instance, ref pPhysicalDeviceCount, ref pPhysicalDevices);
    }

    public unsafe PfnVoidFunction GetInstanceProcAddr(byte* pName)
    {
        return VK.GetInstanceProcAddr(_instance, pName);
    }

    public PfnVoidFunction GetInstanceProcAddr(in byte pName)
    {
        return VK.GetInstanceProcAddr(_instance, in pName);
    }

    public PfnVoidFunction GetInstanceProcAddr(string pName)
    {
        return VK.GetInstanceProcAddr(_instance, pName);
    }
}
