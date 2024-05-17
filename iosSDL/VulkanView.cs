using Silk.NET.Maths;
using Silk.NET.Vulkan;
using Silk.NET.Windowing;
using Examples;
using Shared;

namespace iosSDL;

public class VulkanView
{
    IView _window;
    
    HelloEngine _engine;
    Surface surface;
    object _example;
    
    void OnLoad()
    {
        if (_window.VkSurface is null)
        {
            Console.WriteLine("FAIL TO GET VK SURFACE!");
            return;
        }

        _window.FramebufferResize += OnFramebufferResize;

        surface = _engine.CreateSurface(() =>
        {
            unsafe
            {
                return _window.VkSurface!.Create<AllocationCallbacks>(_engine.Instance.ToHandle(), null).ToSurface();
            }
        });
       
        _example = new Example3(
            surface,
            Load
        );
    }
    
    void OnFramebufferResize(Vector2D<int> size)
    {
        surface.ChangeSize();
        _window.DoRender();
    }

    void OnRender(double time)
    {
        surface.Update();
    }

    void OnClose()
    {
        surface.Dispose();
    }
    static Stream Load(string path)
    {
        return File.OpenRead(path);
    }
    
    public void Run()
    {
        _engine = HelloEngine.Create(new iOSPlatform());
        Window.PrioritizeSdl();
        _window = Window.GetView(ViewOptions.DefaultVulkan);
        
        _window.Load += OnLoad;
        _window.Render += OnRender;
        _window.Closing += OnClose;

        _window.Run();
        _window.Dispose();
    }
}