using CoreVideo;
using Examples;
using ObjCRuntime;
using Shared;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace mac;

[Register("VulkanViewController")]
public class VulkanViewController : NSViewController
{
    HelloEngine _engine;
    Surface _surface;
    Example1 _example;
	protected VulkanViewController (NativeHandle handle) : base (handle)
	{
		// This constructor is required if the view controller is loaded from a xib or a storyboard.
		// Do not put any initialization here, use ViewDidLoad instead.
	}
    
    public override void ViewDidLoad ()
	{
		base.ViewDidLoad ();

        this.View.WantsLayer = true;		// Back the view with a layer created by the makeBackingLayer method.
        
        _engine = HelloEngine.Create(new MacPlatform());
        
        _surface = _engine.CreateSurface(() =>
        {
            HelloEngine.CreateMetalSurface(_engine, View.Layer.GetHandle(), out var surfaceKhr);
            return surfaceKhr;
        });

        _example = new Example1(
            _surface,
            GetVertShader,
            GetFragShader,
            GetImage1,
            GetImage2
            );
        
        nint fps = 60;
        var displayLink = CVDisplayLink.CreateFromDisplayId((uint) CGDisplay.MainDisplayID);
        displayLink?.SetOutputCallback(Render);
        displayLink?.Start();
    }

    static byte[] GetVertShader() => File.ReadAllBytes("Contents/Resources/Shaders/vert.spv");

    static byte[] GetFragShader() => File.ReadAllBytes("Contents/Resources/Shaders/frag.spv");

    static Image<Rgba32> GetImage1() => Image.Load<Rgba32>("Contents/Resources/Textures/texture.jpg");

    static Image<Rgba32> GetImage2() => Image.Load<Rgba32>("Contents/Resources/Textures/texture2.jpg");
    
    CVReturn Render(CVDisplayLink displayLink, ref CVTimeStamp innow, ref CVTimeStamp inoutputtime, CVOptionFlags flagsin, ref CVOptionFlags flagsout)
    {
        _example.Update();
        _engine.DrawFrame();
        
        return CVReturn.Success;
    }

    public override void ViewDidLayout()
    {
        base.ViewDidLayout();
        _surface.ChangeSize();
    }

    public override NSObject RepresentedObject {
		get => base.RepresentedObject;
		set {
			base.RepresentedObject = value;

			// Update the view, if already loaded.
		}
	}
}

