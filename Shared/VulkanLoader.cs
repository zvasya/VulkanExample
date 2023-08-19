using System.Runtime.InteropServices;
using Silk.NET.Vulkan;

namespace Shared;

public class VulkanLoader
{
    static readonly string[] CommonExtensions = {
            "VK_KHR_surface",
        };

    public static unsafe uint CreateVulkan(IEnumerable<string> platformExtensions)
    {
        var appInfo = new ApplicationInfo
        {
            SType = StructureType.ApplicationInfo,
            PApplicationName = (byte*)System.Runtime.InteropServices.Marshal.StringToHGlobalAnsi("Hello Triangle"),
            ApplicationVersion = Vk.MakeVersion(1, 0, 0),
            PEngineName = (byte*)System.Runtime.InteropServices.Marshal.StringToHGlobalAnsi("No Engine"),
            EngineVersion = Vk.MakeVersion(1, 0, 0),
            ApiVersion = Vk.MakeVersion(1, 2, 0),
        };

        InstanceCreateInfo createInfo = default;
        createInfo.SType = StructureType.InstanceCreateInfo;
        createInfo.PApplicationInfo = &appInfo;
        var extensions = CommonExtensions.Concat(platformExtensions).ToArray();
        var extensionsToBytesArray = stackalloc IntPtr[extensions.Length];
        for (var i = 0; i < extensions.Length; i++)
        {
            extensionsToBytesArray[i] = Marshal.StringToHGlobalAnsi(extensions[i]);
        }
        createInfo.EnabledExtensionCount = (uint)extensions.Length;
        createInfo.PpEnabledExtensionNames = (byte**)extensionsToBytesArray;

        createInfo.Flags = extensions.Contains("VK_KHR_portability_enumeration") ? InstanceCreateFlags.EnumeratePortabilityBitKhr : InstanceCreateFlags.None;
        createInfo.EnabledLayerCount = 0;
        createInfo.PNext = null;

        var vk = Vk.GetApi(createInfo, out var instance);
        var deviceCount = 0u;

        vk.EnumeratePhysicalDevices(instance, &deviceCount, null);
        Console.WriteLine($"[VULKAN_LOADER] deviceCount = {deviceCount}");

        return deviceCount;
    }
}