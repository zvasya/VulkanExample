using System.Runtime.InteropServices;
using Silk.NET.Core.Contexts;
using Silk.NET.Vulkan;
using Silk.NET.Vulkan.Extensions.KHR;

namespace Shared;

public unsafe partial class Surface
{
    readonly string[] _deviceExtensions = {
        "VK_KHR_swapchain",
        "VK_KHR_portability_subset",
    };

    Device _device;
    Queue _graphicsQueue;
    Queue _presentQueue;
        
    KhrSwapchain _khrSwapchain;

    void CreateLogicalDevice()
    {
        var indices = this.FindQueueFamilies(_physicalDevice);

        var queueCreateInfos = new List<DeviceQueueCreateInfo>();
        var uniqueQueueFamilies = new HashSet<uint> { indices._graphicsFamily.Value, indices._presentFamily.Value };

        var queuePriority = 1.0f;
        foreach (var queueFamily in uniqueQueueFamilies)
        {
            var queueCreateInfo = new DeviceQueueCreateInfo
            {
                SType = StructureType.DeviceQueueCreateInfo,
                QueueFamilyIndex = queueFamily,
                QueueCount = 1,
                PQueuePriorities = &queuePriority,
            };
            queueCreateInfos.Add(queueCreateInfo);
        }

        PhysicalDeviceFeatures deviceFeatures = default;

        var deviceExtensionsCount = _deviceExtensions.Length;
        var deviceExtensionsArray = stackalloc IntPtr[deviceExtensionsCount];
        for (var i = 0; i < deviceExtensionsCount; i++)
        {
            var extension = _deviceExtensions[i];
            deviceExtensionsArray[i] = Marshal.StringToHGlobalAnsi(extension);
        }

        var createInfo = new DeviceCreateInfo();
        createInfo.SType = StructureType.DeviceCreateInfo;

        var queueCreateInfosArray = queueCreateInfos.ToArray();
        fixed (DeviceQueueCreateInfo* queueCreateInfosArrayPtr = &queueCreateInfosArray[0])
        {
            createInfo.QueueCreateInfoCount = (uint)queueCreateInfos.Count;
            createInfo.PQueueCreateInfos = queueCreateInfosArrayPtr;

        }

        createInfo.PEnabledFeatures = &deviceFeatures;
        createInfo.EnabledExtensionCount = (uint)_deviceExtensions.Length;
        createInfo.PpEnabledExtensionNames = (byte**)deviceExtensionsArray;

        fixed (Device* devicePtr = &_device)
        {
            Helpers.CheckErrors(_vk.CreateDevice(_physicalDevice, &createInfo, null, devicePtr));
        }

        fixed (Queue* graphicsQueuePtr = &_graphicsQueue)
        {
            _engine._vk.GetDeviceQueue(_device, indices._graphicsFamily.Value, 0, graphicsQueuePtr);
        }

        fixed(Queue* presentQueuePtr = &_presentQueue)
        {
            _engine. _vk.GetDeviceQueue(_device, indices._presentFamily.Value, 0, presentQueuePtr); // TODO queue index 0 ?¿?¿
        }

        _engine._vk.CurrentDevice = _device;

        
        //TODO: Rework on _vk.TryGetDeviceExtension
        _khrSwapchain = new KhrSwapchain(new LamdaNativeContext(x => _vk.GetDeviceProcAddr(_device, x)));
        // if (!_vk.TryGetDeviceExtension(_instance, device, out khrSwapchain))
        // {
        //     throw new NotSupportedException("KHR_swapchain extension not found.");
        // }
    }
}
