namespace Core.Animation;

public class Clip
{
    public required List<Track> Tracks { get; init; }

    public IEnumerable<(AnimationBinding, float)> Evaluate(float time)
    {
        return Tracks.Select(track => (track.Binding, track.Curve.Evaluate(time)));
    }
}
