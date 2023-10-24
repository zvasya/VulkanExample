using CoreAnimation;
using ObjCRuntime;
namespace mac;

[Register("VulkanView")]
public class VulkanView : NSView
{
    public VulkanView() : base()
    {
    }

    public VulkanView(NativeHandle handle) : base(handle) 
    { }

    public VulkanView (IntPtr handle) : base (handle)
    {
    }

    CAMetalLayer? _layer;

    public override bool WantsUpdateLayer => true;

    public override CALayer MakeBackingLayer()
    {
        _layer ??= new CAMetalLayer();
        var viewScale = ConvertSizeToBacking(new CGSize(1.0f, 1.0f));
        _layer.ContentsScale = (float)Math.Min(viewScale.Width.Value, viewScale.Height.Value);
        return _layer;
    }

    [Export ("layerClass")]
    public static Class LayerClass () {
        return new Class (typeof (CAMetalLayer));
    }
}
