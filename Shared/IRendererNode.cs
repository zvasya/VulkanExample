namespace Shared;

public  interface IRendererNode
{
    void Draw(HelloCommandBuffer commandBuffer, uint currentImage);
    void BeforeDraw(CameraNode cameraNode, uint currentImage);
}