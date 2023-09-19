using Silk.NET.Vulkan.Extensions.EXT;

namespace ios;

public class iOSPlatform : Shared.IPlatform
{
    public bool EnableValidationLayers => false;
    public string[] RequiredExtensions => new[] {ExtMetalSurface.ExtensionName};

    public byte[] GetVertShader() => File.ReadAllBytes("Shaders/vert.spv");

    public byte[] GetFragShader() => File.ReadAllBytes("Shaders/frag.spv");
}
