using Silk.NET.Vulkan.Extensions.EXT;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace ios;

public class iOSPlatform : Shared.IPlatform
{
    public bool EnableValidationLayers => false;
    public string[] InstanceExtensions => new[] {ExtMetalSurface.ExtensionName};
    public string[] DeviceExtensions => new[] { "VK_KHR_portability_subset" };
}
