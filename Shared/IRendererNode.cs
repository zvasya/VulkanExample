namespace Shared;

public interface IRenderer
{
    void Draw(HelloCommandBuffer commandBuffer);
    void BeforeDraw();
}