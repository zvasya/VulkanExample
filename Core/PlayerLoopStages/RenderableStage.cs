using Core.PlayerLoop;

namespace Core.PlayerLoopStages;

public interface IRenderable
{
    void BeforeRender();
}

public class RenderableStage : PlayerLoopStage<IRenderable>
{
    protected override void Invoke(IRenderable subscriber)
    {
        subscriber.BeforeRender();
    }
}
