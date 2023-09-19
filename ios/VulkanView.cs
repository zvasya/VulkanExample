using CoreAnimation;
using ObjCRuntime;

namespace ios;

[Register("VulkanView")]
public class VulkanView : UIView
{
    public VulkanView (IntPtr handle) : base (handle)
    {
    }
    
    [Export ("layerClass")]
    public static Class LayerClass () {
        return new Class (typeof (CAMetalLayer));
    }
}