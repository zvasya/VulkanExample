using CoreVideo;
using ObjCRuntime;
using Shared;

namespace mac;

[Register("VulkanViewController")]
public class VulkanViewController : NSViewController
{

    HelloEngine _engine;
    Surface _surface;
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
        
        nint fps = 60;
        var displayLink = CVDisplayLink.CreateFromDisplayId((uint) CGDisplay.MainDisplayID);
        displayLink?.SetOutputCallback(Render);
        displayLink?.Start();
    }

    CVReturn Render(CVDisplayLink displaylink, ref CVTimeStamp innow, ref CVTimeStamp inoutputtime, CVOptionFlags flagsin, ref CVOptionFlags flagsout)
    {
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

