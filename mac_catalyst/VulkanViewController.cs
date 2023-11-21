using CoreAnimation;
using Examples;
using mac_catalyst;
using ObjCRuntime;
using Shared;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace ios;

[Register("VulkanViewController")]
public class VulkanViewController : UIViewController
{
    public VulkanViewController()
    {
    }

    public VulkanViewController (IntPtr handle) : base (handle)
    {
        _displayLink = null;
    }

    CADisplayLink? _displayLink;
    HelloEngine _engine;
    Surface _surface;
    object _example; 

    [Export("viewDidLoad")]
    public override void ViewDidLoad()
    {
        base.ViewDidLoad();
        // Perform any additional setup after loading the view, typically from a nib.
        
        View!.ContentScaleFactor = UIScreen.MainScreen.NativeScale;

        _engine = HelloEngine.Create(new MacCatalystPlatform());
        _surface = _engine.CreateSurface(() =>
        {
            HelloEngine.CreateMetalSurface(_engine, View.Layer.GetHandle(), out var surfaceKhr);
            return surfaceKhr;
        });
        _example = new Example3(_surface, Load);
        
        nint fps = 60;
        _displayLink = CADisplayLink.Create(RenderLoop);
        _displayLink.PreferredFramesPerSecond = fps;
        _displayLink.AddToRunLoop(NSRunLoop.Current, NSRunLoopMode.Default);
    }

    static Stream Load(string path)
    {
        return File.OpenRead(Path.Combine("Contents", "Resources", path));
    }
    
    void RenderLoop()
    {
        _surface.Update();
    }

    public override void ViewDidLayoutSubviews()
    {
        base.ViewDidLayoutSubviews();
        _surface.ChangeSize();
    }

    public override void ViewWillTransitionToSize(CGSize toSize, IUIViewControllerTransitionCoordinator coordinator)
    {
        base.ViewWillTransitionToSize(toSize, coordinator);
    }

    public override void DidReceiveMemoryWarning()
    {
        base.DidReceiveMemoryWarning();
        // Release any cached data, images, etc that aren't in use.
    }
}
