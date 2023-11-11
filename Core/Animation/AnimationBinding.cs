namespace Core.Animation;

public readonly struct AnimationBinding
{
    public readonly string Path;
    public readonly string Property;

    public AnimationBinding(string path, string property)
    {
        Path = path;
        Property = property;
    }
}
