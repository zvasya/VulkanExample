namespace Core.Animation;

public readonly struct Keyframe
{
    public readonly float Time;
    public readonly float Value;
    public readonly float InTangent;
    public readonly float OutTangent;

    public Keyframe(float time, float value, float inTangent, float outTangent)
    {
        Time = time;
        Value = value;
        InTangent = inTangent;
        OutTangent = outTangent;
    }
}
