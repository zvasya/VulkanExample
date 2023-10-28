using CoreAnimation;
using Examples;
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
    Surface surface;
    Example1 _example;

    [Export("viewDidLoad")]
    public override void ViewDidLoad()
    {
        base.ViewDidLoad();
        // Perform any additional setup after loading the view, typically from a nib.
        
        View!.ContentScaleFactor = UIScreen.MainScreen.NativeScale;

        _engine = HelloEngine.Create(new iOSPlatform());
        surface = _engine.CreateSurface(() =>
        {
            HelloEngine.CreateMetalSurface(_engine, View.Layer.GetHandle(), out var surfaceKhr);
            return surfaceKhr;
        });
        
        _example = new Example1(
            surface,
            GetVertShader,
            GetFragShader,
            GetImage1,
            GetImage2
        );
        
        nint fps = 60;
        _displayLink = CADisplayLink.Create(RenderLoop);
        _displayLink.PreferredFramesPerSecond = fps;
        _displayLink.AddToRunLoop(NSRunLoop.Current, NSRunLoopMode.Default);
    }

    static byte[] GetVertShader() => File.ReadAllBytes("Shaders/vert.spv");

    static byte[] GetFragShader() => File.ReadAllBytes("Shaders/frag.spv");

    static Image<Rgba32> GetImage1() => Image.Load<Rgba32>("Textures/texture.jpg");

    static Image<Rgba32> GetImage2() => Image.Load<Rgba32>("Textures/texture2.jpg");

    void RenderLoop()
    {
        _example.Update();
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
