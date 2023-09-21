using CoreAnimation;
using Foundation;
using ObjCRuntime;
using Shared;
using Silk.NET.Vulkan;
using Silk.NET.Vulkan.Extensions.EXT;
using UIKit;
using VulkanView.Maui.Views.Interfaces;

namespace VulkanView.Maui.Core.Views.iOS;

// All the code in this file is only included on iOS.
public class MauiVulkanView : UIView
{
    IVulkanView _vulkanView;
    public MauiVulkanView (IVulkanView vulkanView)
    {
        _vulkanView = vulkanView;
        _vulkanView.Platform = new iOSPlatform();
        _vulkanView.CreateSurface = CreateSurface;
    }
    
    SurfaceKHR CreateSurface(HelloEngine engine)
    {
        HelloEngine.CreateMetalSurface(engine, Layer.GetHandle(), out var surfaceKhr);
        return surfaceKhr;
    }

    public MauiVulkanView() : base()
    {
    }

    bool firstTimeLoad = true;
    CADisplayLink _displayLink;

    public override void LayoutSubviews()
    {
        base.LayoutSubviews();
        _vulkanView.ChangeSize();
    }

    public override void MovedToSuperview()
    {
        base.MovedToSuperview();
        
        if (firstTimeLoad)
        {
            _vulkanView.ViewCreated();

            float fps = 60;
            
            _displayLink = CADisplayLink.Create(RenderLoop);
            // _displayLink.FrameInterval = (60f / fps);
            _displayLink.AddToRunLoop(NSRunLoop.Current, NSRunLoopMode.Default);
            
            firstTimeLoad = false;
        }
    }

    
    public void RenderLoop()
    {
        _vulkanView.DrawFrame();
    }
    
    [Export ("layerClass")]
    public static Class LayerClass () {
        return new Class (typeof (CAMetalLayer));
    }
}

public class iOSPlatform : Shared.IPlatform
{
    public bool EnableValidationLayers => false;
    public string[] InstanceExtensions => new[] {ExtMetalSurface.ExtensionName};
    public string[] DeviceExtensions => new[] { "VK_KHR_portability_subset" };

    public byte[] GetVertShader() => Read("Shaders/vert.spv");
    public byte[] GetFragShader() => Read("Shaders/frag.spv");

    byte[] Read(string fileName)
    {
        using var stream = FileSystem.OpenAppPackageFileAsync(fileName);
        using var reader = new BinaryReader(stream.Result);
        return reader.ReadBytes(10000);
    }
}
