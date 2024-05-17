using Silk.NET.Maths;
using Silk.NET.Vulkan;
using Silk.NET.Windowing;
using Examples;
using Shared;

namespace ConsoleSDL;

public class VulkanWindow
{
    IView _view;
    
    HelloEngine _engine;
    Surface _surface;
    object _example;
    
    void OnLoad()
    {
        if (_view.VkSurface is null)
        {
            Console.WriteLine("FAIL TO GET VK SURFACE!");
            return;
        }

        _view.FramebufferResize += OnFramebufferResize;

        _surface = _engine.CreateSurface(() =>
        {
            unsafe
            {
                return _view.VkSurface!.Create<AllocationCallbacks>(_engine.Instance.ToHandle(), null).ToSurface();
            }
        });
       
        _example = new Example3(
            _surface,
            Load
        );
    }
    
    void OnFramebufferResize(Vector2D<int> size)
    {
        _surface.ChangeSize();
        _view.DoRender();
    }

    void OnRender(double time)
    {
        _surface.Update();
    }

    void OnClose()
    {
        _surface.Dispose();
    }
    static Stream Load(string path)
    {
        return File.OpenRead(path);
    }
    
    public void Run()
    {
        _engine = HelloEngine.Create(new MacPlatform());
        
        _view = Window.Create(WindowOptions.DefaultVulkan);
        _view.Initialize(); // For safety the window should be initialized before querying the VkSurface
        
        OnLoad();
        _view.Render += OnRender;
        _view.Closing += OnClose;

        _view.Run();
        _view.Dispose();
    }
}