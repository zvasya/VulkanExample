using CoreAnimation;
using mac_catalyst;
using ObjCRuntime;
using Shared;

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
    Surface surface;

    [Export("viewDidLoad")]
    public override void ViewDidLoad()
    {
        base.ViewDidLoad();
        // Perform any additional setup after loading the view, typically from a nib.
        
        View!.ContentScaleFactor = UIScreen.MainScreen.NativeScale;

        _engine = HelloEngine.Create(new MacCatalystPlatform());
        surface = _engine.CreateSurface(() =>
        {
            _engine.CreateMetalSurface(View.Layer.GetHandle(), out var surfaceKhr);
            return surfaceKhr;
        });
        
        nint fps = 60;
        _displayLink = CADisplayLink.Create(RenderLoop);
        _displayLink.PreferredFramesPerSecond = fps;
        _displayLink.AddToRunLoop(NSRunLoop.Current, NSRunLoopMode.Default);
    }

    void RenderLoop()
    {
        _engine.DrawFrame();
    }

    public override void ViewDidLayoutSubviews()
    {
        base.ViewDidLayoutSubviews();
        surface.ChangeSize();
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