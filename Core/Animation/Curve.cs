namespace Core.Animation;

public class Curve
{
    readonly List<Keyframe> _keyframes = new List<Keyframe>();

    public void AddKeyframe(Keyframe keyframe)
    {
        _keyframes.Add(keyframe);
    }

    public float Duration => _keyframes[^1].Time;

    public float Evaluate(float time)
    {
        var i = 0;
        if (time <= _keyframes[i].Time)
            return _keyframes[i].Value;
        i++;

        while (i < _keyframes.Count - 1 && _keyframes[i].Time < time)
        {
            i++;
        }

        var keyframeFrom = _keyframes[i-1];
        var keyframeTo = _keyframes[i];
        var dt = (time - keyframeFrom.Time) / (keyframeTo.Time - keyframeFrom.Time);

        return (float)Hermite(keyframeFrom.Value, keyframeFrom.OutTangent, keyframeTo.Value, keyframeTo.InTangent, dt);
        
        // return (float)Lerp(keyframeFrom.value, keyframeTo.value, dt);
    }
    
    static double Hermite(double from, double fromTangent, double to, double toTangent, double amount)
    {   
        if (amount <= 0.0)
            return from;
        
        if (amount >= 1.0)
            return to;

        var sSquared = amount * amount;
        var sCubed = sSquared * amount;

        return (2.0 * from + fromTangent - 2.0 * to + toTangent) * sCubed +
               (- 3.0 * from - 2.0 * fromTangent + 3.0 * to - toTangent) * sSquared +
               fromTangent * amount +
               from;
        
    }
    
    static double Lerp(double from, double to, double amount) => from * (1.0 - amount) + to * amount;
}
