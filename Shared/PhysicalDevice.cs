using Silk.NET.Core;
using Silk.NET.Core.Native;
using Silk.NET.Vulkan;
using Silk.NET.Vulkan.Extensions.KHR;
using static Shared.HelloEngine;

namespace Shared;

public unsafe partial class HelloPhysicalDevice
{
    readonly string[] _deviceExtensions = { KhrSwapchain.ExtensionName };
    
    PhysicalDevice _physicalDevice;

    readonly HelloEngine _engine;
    readonly SurfaceKHR _surface;

    HelloPhysicalDevice(HelloEngine engine, SurfaceKHR surface)
    {
        _engine = engine;
        _surface = surface;
    }

    public static HelloPhysicalDevice PickPhysicalDevice(SurfaceKHR surface, HelloEngine engine)
    {
        var device = new HelloPhysicalDevice (engine, surface);
        device.PickPhysicalDevice();

        return device;
    }

    public LogicalDevice CreateLogicalDevice()
    {
        return LogicalDevice.CreateLogicalDevice(this, _deviceExtensions.Union(_engine.Platform.DeviceExtensions).ToArray());
    }

    void PickPhysicalDevice()
    {
        uint deviceCount = 0;
        Helpers.CheckErrors(_engine.EnumeratePhysicalDevices(&deviceCount, null));
        if (deviceCount == 0)
        {
            throw new Exception("Failed to find GPUs with Vulkan support!");
        }

        var devices = stackalloc PhysicalDevice[(int)deviceCount];
        Helpers.CheckErrors(_engine.EnumeratePhysicalDevices(&deviceCount, devices));

        for (var i = 0; i < deviceCount; i++)
        {
            var device = devices[i];
            if (IsPhysicalDeviceSuitable(device))
            {
                _physicalDevice = device;
                break;
            }
        }

        if (_physicalDevice.Handle == default)
        {
            throw new Exception("failed to find a suitable GPU!");
        }
    }

    bool IsPhysicalDeviceSuitable(PhysicalDevice physicalDevice)
    {
        var indices = FindQueueFamilies(physicalDevice);

        var extensionsSupported = CheckPhysicalDeviceExtensionSupport(physicalDevice);

        var swapChainAdequate = false;
        if (extensionsSupported)
        {
            var swapChainSupport = physicalDevice.QuerySwapChainSupport(_surface);
            swapChainAdequate = swapChainSupport._formats.Length != 0 && swapChainSupport._presentModes.Length != 0;
        }
        
        PhysicalDeviceFeatures supportedFeatures;
        VK.GetPhysicalDeviceFeatures(physicalDevice, &supportedFeatures);

        return indices.IsComplete() && extensionsSupported && swapChainAdequate && supportedFeatures.SamplerAnisotropy;
    }

    bool CheckPhysicalDeviceExtensionSupport(PhysicalDevice physicalDevice)
    {
        uint extensionCount = 0;
        Helpers.CheckErrors(physicalDevice.EnumerateDeviceExtensionProperties(null!, &extensionCount, null));

        var availableExtensions = stackalloc ExtensionProperties[(int)extensionCount];
        Helpers.CheckErrors(physicalDevice.EnumerateDeviceExtensionProperties(null!, &extensionCount, availableExtensions));

        var requiredExtensions = new HashSet<string>(_deviceExtensions);

        for (var i = 0; i < extensionCount; i++)
        {
            var extension = availableExtensions[i];
            requiredExtensions.Remove(SilkMarshal.PtrToString(new IntPtr(extension.ExtensionName), NativeStringEncoding.LPTStr)!);
        }

        return requiredExtensions.Count == 0;
    }

    public QueueFamilyIndices FindQueueFamilies()
    {
        return FindQueueFamilies(_physicalDevice);
    }

    QueueFamilyIndices FindQueueFamilies(PhysicalDevice physicalDevice)
    {
        QueueFamilyIndices indices = default;

        uint queueFamilyCount = 0;
        VK.GetPhysicalDeviceQueueFamilyProperties(physicalDevice, &queueFamilyCount, null);

        var queueFamilies = stackalloc QueueFamilyProperties[(int)queueFamilyCount];
        VK.GetPhysicalDeviceQueueFamilyProperties(physicalDevice, ref queueFamilyCount, queueFamilies);

        for (uint i = 0; i < queueFamilyCount; i++)
        {
            var queueFamily = queueFamilies[i];
            if ((queueFamily.QueueFlags & QueueFlags.GraphicsBit) != 0)
            {
                indices._graphicsFamily = i;
            }

            Bool32 presentSupport = false;

            Helpers.CheckErrors(VkSurface.GetPhysicalDeviceSurfaceSupport(physicalDevice, i, _surface, &presentSupport));

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

    public uint FindMemoryType(uint typeFilter, MemoryPropertyFlags properties)
    {
        GetPhysicalDeviceMemoryProperties(out var memProperties);

        for (var i = 0; i < memProperties.MemoryTypeCount; i++)
        {
            if ((typeFilter & (1 << i)) != 0 && (memProperties.MemoryTypes[i].PropertyFlags & properties) == properties)
            {
                return (uint)i;
            }
        }

        throw new Exception("failed to find suitable memory type!");
    }

    Format FindSupportedFormat(Format[] candidates, ImageTiling tiling, FormatFeatureFlags features)
    {
        foreach (var format in candidates)
        {
            FormatProperties props;
            GetPhysicalDeviceFormatProperties(format, &props);

            if (tiling == ImageTiling.Linear && (props.LinearTilingFeatures & features) == features) {
                return format;
            } else if (tiling == ImageTiling.Optimal && (props.OptimalTilingFeatures & features) == features) {
                return format;
            }
        }

        throw new Exception("failed to find supported format!");
    }

    public Format FindDepthFormat()
    {
        return FindSupportedFormat(
            new[] { Format.D32Sfloat, Format.D32SfloatS8Uint, Format.D24UnormS8Uint },
            ImageTiling.Optimal,
            FormatFeatureFlags.DepthStencilAttachmentBit
        );
    }

    public Surface.SwapChainSupportDetails QuerySwapChainSupport(SurfaceKHR surface)
    {
        return _physicalDevice.QuerySwapChainSupport(surface);
    }
    
    public static SurfaceFormatKHR ChooseSwapSurfaceFormat(SurfaceFormatKHR[] availableFormats)
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

    public struct QueueFamilyIndices
    {
        public uint? _graphicsFamily;
        public uint? _presentFamily;

        public bool IsComplete()
        {
            return _graphicsFamily.HasValue && _presentFamily.HasValue;
        }
    }
}
