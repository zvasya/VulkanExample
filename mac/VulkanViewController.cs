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
    object _example;
    CVDisplayLink? _displayLink;
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

        _example = new Example3(
            _surface,
            Load
            );
        
        nint fps = 60;
        
        _displayLink = CVDisplayLink.CreateFromDisplayId((uint) CGDisplay.MainDisplayID);
        _displayLink?.SetOutputCallback(Render);
        _displayLink?.Start();
    }
    
    static Stream Load(string path)
    {
	    return File.OpenRead(Path.Combine("Contents", "Resources", path));
    }
    
    CVReturn Render(CVDisplayLink displayLink, ref CVTimeStamp innow, ref CVTimeStamp inoutputtime, CVOptionFlags flagsin, ref CVOptionFlags flagsout)
    {
        _surface.Update();
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

