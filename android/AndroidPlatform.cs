using Silk.NET.Vulkan.Extensions.KHR;

namespace android;

public class AndroidPlatform : Shared.IPlatform
{
    public bool EnableValidationLayers => false;
    public string[] InstanceExtensions => new[] {KhrAndroidSurface.ExtensionName};
    public string[] DeviceExtensions => Array.Empty<string>();
}
