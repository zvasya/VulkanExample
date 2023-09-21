using Shared;
using Silk.NET.Vulkan;

namespace VulkanView.Maui.Views.Interfaces;

public interface IVulkanView : IView
{
    void ViewCreated();
    void ViewDestroyed();
    void DrawFrame();
    void ChangeSize(int w, int h);
    void ChangeSize();

    IPlatform Platform { get; set; }  
    
    Func<HelloEngine, SurfaceKHR> CreateSurface { get; set; }
}
