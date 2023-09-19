namespace Shared;

public interface IPlatform
{
    bool EnableValidationLayers { get; }

    string[] RequiredExtensions { get; }
    
    byte[] GetVertShader();
    byte[] GetFragShader();
}
