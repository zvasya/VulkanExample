using Core.PlayerLoopStages;

namespace Core.Animation;

public class SimpleAnimator : Component, IAnimatable
{
    public Clip? Animation { get; init; }
    public PlayMode Mode { get; init; }
    readonly Dictionary<AnimationBinding, Action<float>> _bindings = new Dictionary<AnimationBinding, Action<float>>();
    double? _previousTime;
    double _currentTime;
    bool _bound;
    float _duration;

    public enum PlayMode
    {
        HOLD,
        LOOP,
        PING_PONG,
    }

    protected override void OnConnect()
    {
        base.OnConnect();
        AnimationStage.Register(this);
        if (Animation != null)
            Bind();
    }

    protected override void OnDisconnect()
    {
        base.OnDisconnect();
        AnimationStage.Unregister(this);
    }


    public void Bind()
    {
        if (Animation == null)
            throw new Exception("Animation is null");

        if (SceneNode == null)
            throw new Exception("SceneNode is null");

        _bindings.Clear();
        _duration = 0;
        foreach (var track in Animation.Tracks)
        {
            _duration = Math.Max(track.Curve.Duration, _duration);
            if (SceneNode.TryFindChildByPath(track.Binding.Path, out var node))
            {
                _bindings.Add(track.Binding, node!.GetSetterAnim(track.Binding.Property));
            }
        }

        _bound = true;
    }

    public void Animate(double time)
    {
        if (Animation == null || !_bound)
            return;

        _previousTime ??= time;

        _currentTime += (time - _previousTime.Value) / 1000.0;
        var t = Mode switch
        {
            PlayMode.LOOP => _currentTime % _duration,
            PlayMode.PING_PONG => Math.Abs((_currentTime + _duration) % (_duration * 2.0) - _duration),
            PlayMode.HOLD => Math.Clamp(_currentTime, 0, _duration),
            _ => Math.Clamp(_currentTime, 0, _duration),
        };
        _previousTime = time;

        var values = Animation.Evaluate((float)t);
        foreach (var (path, value) in values)
        {
            if (_bindings.TryGetValue(path, out var f))
                f(value);
        }
    }
}
