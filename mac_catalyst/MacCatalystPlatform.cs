using Silk.NET.Vulkan.Extensions.EXT;

namespace mac_catalyst;

public class MacCatalystPlatform : Shared.IPlatform
{
    public bool EnableValidationLayers => false;
    public string[] InstanceExtensions => new[] {ExtMetalSurface.ExtensionName};
    public string[] DeviceExtensions => new[] { "VK_KHR_portability_subset" };
}
