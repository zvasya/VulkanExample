using Silk.NET.Vulkan.Extensions.KHR;
using Xamarin.Essentials;

namespace android;

public class AndroidPlatform : Shared.IPlatform
{
    public bool EnableValidationLayers => false;
    public string[] InstanceExtensions => new[] {KhrAndroidSurface.ExtensionName};
    public string[] DeviceExtensions => Array.Empty<string>();
    public byte[] GetVertShader() => Read("Shaders/vert.spv");
    public byte[] GetFragShader() => Read("Shaders/frag.spv");

    byte[] Read(string fileName)
    {
        using var stream = FileSystem.OpenAppPackageFileAsync(fileName);
        using var reader = new BinaryReader(stream.Result);
        return reader.ReadBytes(10000);
    }
}
