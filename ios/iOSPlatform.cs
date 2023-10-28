using Silk.NET.Vulkan.Extensions.EXT;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace ios;

public class iOSPlatform : Shared.IPlatform
{
    public bool EnableValidationLayers => false;
    public string[] InstanceExtensions => new[] {ExtMetalSurface.ExtensionName};
    public string[] DeviceExtensions => new[] { "VK_KHR_portability_subset" };

    public byte[] GetVertShader() => File.ReadAllBytes("Shaders/vert.spv");

    public byte[] GetFragShader() => File.ReadAllBytes("Shaders/frag.spv");
    
    public Image<Rgba32> GetImage()
    {
        return SixLabors.ImageSharp.Image.Load<SixLabors.ImageSharp.PixelFormats.Rgba32>("Textures/texture.jpg");
    }
}
