namespace Shared;

public interface IPlatform
{
    bool EnableValidationLayers { get; }

    string[] InstanceExtensions { get; }
    string[] DeviceExtensions { get; }
}
