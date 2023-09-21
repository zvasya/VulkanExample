using Silk.NET.Vulkan.Extensions.EXT;

namespace ios;

public class iOSPlatform : Shared.IPlatform
{
    public bool EnableValidationLayers => false;
    public string[] InstanceExtensions => new[] {ExtMetalSurface.ExtensionName};
    public string[] DeviceExtensions => new[] { "VK_KHR_portability_subset" };

    public byte[] GetVertShader() => File.ReadAllBytes("Shaders/vert.spv");

    public byte[] GetFragShader() => File.ReadAllBytes("Shaders/frag.spv");
}
