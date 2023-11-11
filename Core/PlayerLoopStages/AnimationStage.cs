using Core.PlayerLoop;

namespace Core.PlayerLoopStages;

public interface IAnimatable
{
    void Animate(double time);
}
public class AnimationStage : PlayerLoopStage<IAnimatable>
{
    readonly DateTime _dateTime = DateTime.UtcNow;
    
    double _time;
    protected override void Invoke(IAnimatable subscriber)
    {
        subscriber.Animate(_time);
    }

    public override void Execute()
    {
        _time = (DateTime.UtcNow - _dateTime).TotalMilliseconds;
        base.Execute();
    }
}
