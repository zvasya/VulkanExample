namespace Core.Animation;

public readonly struct Track
{
    public readonly AnimationBinding Binding;
    public readonly Curve Curve;

    public Track(AnimationBinding binding, Curve curve)
    {
        Binding = binding;
        Curve = curve;
    }
}
