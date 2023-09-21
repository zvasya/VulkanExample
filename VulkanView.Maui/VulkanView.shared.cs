using Shared;
using Silk.NET.Vulkan;
using VulkanView.Maui.Views.Interfaces;
using VulkanView.Maui.Views.Primitives;

namespace VulkanView.Maui.Views;

// All the code in this file is included in all platforms.
public class VulkanView : View, IVulkanView
{
    readonly WeakEventManager eventManager = new();

    public event EventHandler ViewCreated
    {
        add => eventManager.AddEventHandler(value);
        remove => eventManager.RemoveEventHandler(value);
    }

    public event EventHandler ViewDestroyed
    {
        add => eventManager.AddEventHandler(value);
        remove => eventManager.RemoveEventHandler(value);
    }

    public event EventHandler DrawFrame
    {
        add => eventManager.AddEventHandler(value);
        remove => eventManager.RemoveEventHandler(value);
    }
    
    public event EventHandler<ChangeSizeEventArgs> ChangeSize
    {
        add => eventManager.AddEventHandler(value);
        remove => eventManager.RemoveEventHandler(value);
    }

    void IVulkanView.DrawFrame()
    {
        OnDrawFrame();
    }
    
    Func<HelloEngine, SurfaceKHR> IVulkanView.CreateSurface { get; set; }

    void IVulkanView.ViewCreated()
    {
        OnViewCreated();
    }
    
    void IVulkanView.ViewDestroyed()
    {
        OnViewDestroyed();
    }
    
    void IVulkanView.ChangeSize()
    {
        OnChangeSize();
    }

    IPlatform IVulkanView.Platform { get; set; }

    void IVulkanView.ChangeSize(int w, int h)
    {
        OnChangeSize(w, h);
    }

    internal void OnDrawFrame()
    {
        eventManager.HandleEvent(this, EventArgs.Empty, nameof(DrawFrame));
    }

    internal void OnViewCreated()
    {
        eventManager.HandleEvent(this, EventArgs.Empty, nameof(ViewCreated));
    }
    
    internal void OnViewDestroyed()
    {
        eventManager.HandleEvent(this, EventArgs.Empty, nameof(ViewDestroyed));
    }
    
    internal void OnChangeSize()
    {
        eventManager.HandleEvent(this, new ChangeSizeEventArgs(), nameof(ChangeSize));
    }

    internal void OnChangeSize(int w, int h)
    {
        eventManager.HandleEvent(this, new ChangeSizeEventArgs(w,h), nameof(ChangeSize));
    }
}

