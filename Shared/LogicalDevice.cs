using System.Runtime.InteropServices;
using Microsoft.DotNet.PlatformAbstractions;
using Silk.NET.Core.Contexts;
using Silk.NET.Core.Native;
using Silk.NET.Vulkan;
using Silk.NET.Vulkan.Extensions.KHR;
using static Shared.HelloEngine;
using Buffer = Silk.NET.Vulkan.Buffer;

namespace Shared;

public unsafe partial class LogicalDevice : IDisposable
{
    Device _device;

    KhrSwapchain _khrSwapchain = null!;

    public HelloPhysicalDevice PhysicalDevice { get; private init; } = null!;
    public HelloCommandPool CommandPool { get; private set; } = null!;

    public HelloQueue GraphicsQueue { get; private set; } = null!;
    public HelloQueue PresentQueue { get; private set; } = null!;

    public static LogicalDevice CreateLogicalDevice(HelloPhysicalDevice physicalDevice, string[] deviceExtensions)
    {
        var device = new LogicalDevice { PhysicalDevice = physicalDevice };

        device.CreateLogicalDevice(deviceExtensions);
        return device;
    }

    void CreateLogicalDevice(string[] deviceExtensions)
    {
        var indices = PhysicalDevice.FindQueueFamilies();

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
        deviceFeatures.SamplerAnisotropy = true;

        var deviceExtensionsCount = deviceExtensions.Length;
        var deviceExtensionsArray = stackalloc IntPtr[deviceExtensionsCount];
        for (var i = 0; i < deviceExtensionsCount; i++)
        {
            var extension = deviceExtensions[i];
            deviceExtensionsArray[i] = Marshal.StringToHGlobalAnsi(extension);
        }

        var createInfo = new DeviceCreateInfo
        {
            SType = StructureType.DeviceCreateInfo,
        };

        var queueCreateInfosArray = queueCreateInfos.ToArray();
        fixed (DeviceQueueCreateInfo* queueCreateInfosArrayPtr = &queueCreateInfosArray[0])
        {
            createInfo.QueueCreateInfoCount = (uint)queueCreateInfos.Count;
            createInfo.PQueueCreateInfos = queueCreateInfosArrayPtr;
        }

        createInfo.PEnabledFeatures = &deviceFeatures;
        createInfo.EnabledExtensionCount = (uint)deviceExtensions.Length;
        createInfo.PpEnabledExtensionNames = (byte**)deviceExtensionsArray;
        
        // TODO ?
        // if (Platform.EnableValidationLayers)
        // {
        //     createInfo.EnabledLayerCount = (uint)_validationLayers!.Length;
        //     createInfo.PpEnabledLayerNames = (byte**)SilkMarshal.StringArrayToPtr(_validationLayers!);
        // }
        // else
        // {
        //     createInfo.EnabledLayerCount = 0;
        // }

        fixed (Device* devicePtr = &_device)
        {
            Helpers.CheckErrors(PhysicalDevice.CreateDevice(&createInfo, null, devicePtr));
        }

        if (!VK.TryGetDeviceExtension(VK.CurrentInstance!.Value, _device, out _khrSwapchain))
        {
            throw new NotSupportedException("KHR_swapchain extension not found.");
        }

        GetDeviceQueue(indices._graphicsFamily.Value, 0, out var graphicsQueue);
        GraphicsQueue = HelloQueue.Create(graphicsQueue, _khrSwapchain);
        
        GetDeviceQueue(indices._presentFamily.Value, 0, out var presentQueue);
        PresentQueue = HelloQueue.Create(presentQueue, _khrSwapchain);

        CommandPool = HelloCommandPool.Create(this);
        
        VK.CurrentDevice = _device;
    }


    public void Dispose()
    {
        CommandPool.Dispose();
        _khrSwapchain.Dispose();
        
        VK.DestroyDevice(_device, null);
    }
    
    public void CreateBuffer(ulong size, BufferUsageFlags usage, MemoryPropertyFlags properties, out Buffer buffer, out DeviceMemory bufferMemory)
    {
        BufferCreateInfo bufferInfo = new()
        {
            SType = StructureType.BufferCreateInfo,
            Size = size,
            Usage = usage,
            SharingMode = SharingMode.Exclusive,
        };
        
        fixed (Buffer* bufferPtr = &buffer)
        {
            if (CreateBuffer(bufferInfo, null, bufferPtr) != Result.Success)
            {
                throw new Exception("failed to create vertex buffer!");
            }
        }

        GetBufferMemoryRequirements(buffer, out var memRequirements);

        MemoryAllocateInfo allocateInfo = new()
        {
            SType = StructureType.MemoryAllocateInfo,
            AllocationSize = memRequirements.Size,
            MemoryTypeIndex = PhysicalDevice.FindMemoryType(memRequirements.MemoryTypeBits, properties),
        };

        fixed (DeviceMemory* bufferMemoryPtr = &bufferMemory)
        {
            if (AllocateMemory(allocateInfo, null, bufferMemoryPtr) != Result.Success)
            {
                throw new Exception("failed to allocate vertex buffer memory!");
            }
        }

        BindBufferMemory(buffer, bufferMemory, 0);
    }
}
