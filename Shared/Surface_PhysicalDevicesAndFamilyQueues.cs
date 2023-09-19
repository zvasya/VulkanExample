using Silk.NET.Core;
using Silk.NET.Core.Native;
using Silk.NET.Vulkan;

namespace Shared;

public unsafe partial class Surface
{
    PhysicalDevice _physicalDevice;

    void PickPhysicalDevice()
    {
        uint deviceCount = 0;
        Helpers.CheckErrors(_vk.EnumeratePhysicalDevices(_instance, &deviceCount, null));
        if (deviceCount == 0)
        {
            throw new Exception("Failed to find GPUs with Vulkan support!");
        }

        var devices = stackalloc PhysicalDevice[(int)deviceCount];
        Helpers.CheckErrors(_vk.EnumeratePhysicalDevices(_instance, &deviceCount, devices));

        for (var i = 0; i < deviceCount; i++)
        {
            var device = devices[i];
            if (this.IsPhysicalDeviceSuitable(device))
            {
                this._physicalDevice = device;
                break;
            }
        }

        if (this._physicalDevice.Handle == default)
        {
            throw new Exception("failed to find a suitable GPU!");
        }
    }

    bool IsPhysicalDeviceSuitable(PhysicalDevice physicalDevice)
    {
        var indices = this.FindQueueFamilies(physicalDevice);

        var extensionsSupported = this.CheckPhysicalDeviceExtensionSupport(physicalDevice);

        var swapChainAdequate = false;
        if (extensionsSupported)
        {
            var swapChainSupport = this.QuerySwapChainSupport(physicalDevice);
            swapChainAdequate = swapChainSupport._formats.Length != 0 && swapChainSupport._presentModes.Length != 0;
        }

        return indices.IsComplete() && extensionsSupported && swapChainAdequate;
    }

    bool CheckPhysicalDeviceExtensionSupport(PhysicalDevice physicalDevice)
    {
        uint extensionCount = 0;
        Helpers.CheckErrors(_vk.EnumerateDeviceExtensionProperties(physicalDevice, (string)null!, ref extensionCount, null));

        var availableExtensions = stackalloc ExtensionProperties[(int)extensionCount];
        Helpers.CheckErrors(_vk.EnumerateDeviceExtensionProperties(physicalDevice, (string)null!, &extensionCount, availableExtensions));

        var requiredExtensions = new HashSet<string>(_deviceExtensions);

        for(var i = 0; i < extensionCount; i++)
        {
            var extension = availableExtensions[i];
            requiredExtensions.Remove(SilkMarshal.PtrToString(new IntPtr(extension.ExtensionName), NativeStringEncoding.LPTStr)!);
        }

        return requiredExtensions.Count == 0;
    }

    public struct QueueFamilyIndices
    {
        public uint? _graphicsFamily;
        public uint? _presentFamily;

        public bool IsComplete()
        {
            return _graphicsFamily.HasValue && _presentFamily.HasValue;
        }
    }

    QueueFamilyIndices FindQueueFamilies(PhysicalDevice physicalDevice)
    {
        QueueFamilyIndices indices = default;

        uint queueFamilyCount = 0;
        _vk.GetPhysicalDeviceQueueFamilyProperties(physicalDevice, &queueFamilyCount, null);

        var queueFamilies = stackalloc QueueFamilyProperties[(int)queueFamilyCount];
        _vk.GetPhysicalDeviceQueueFamilyProperties(physicalDevice, &queueFamilyCount, queueFamilies);

        for (uint i = 0; i < queueFamilyCount; i++)
        {
            var queueFamily = queueFamilies[i];
            if ((queueFamily.QueueFlags & QueueFlags.GraphicsBit) != 0)
            {                    
                indices._graphicsFamily = i;
            }

            Bool32 presentSupport = false;
                
            Helpers.CheckErrors(_vkSurface.GetPhysicalDeviceSurfaceSupport(physicalDevice, i, _surface, &presentSupport));

            if (presentSupport)
            {
                indices._presentFamily = i;
            }

            if (indices.IsComplete())
            {
                break;
            }
        }

        return indices;
    }
}
