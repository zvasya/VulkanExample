using VulkanView.Maui.Views.Interfaces;

namespace VulkanView.Maui.Views;

public partial class VulkanViewHandler
{
    public static IPropertyMapper<VulkanView, VulkanViewHandler> PropertyMapper = new PropertyMapper<VulkanView, VulkanViewHandler>(ViewMapper)
    {
        
    };
    
    
    public static CommandMapper<VulkanView, VulkanViewHandler> CommandMapper = new(ViewCommandMapper);
    
    public VulkanViewHandler() : base(PropertyMapper, CommandMapper)
    {
    }

    public VulkanViewHandler(IPropertyMapper? mapper = null, CommandMapper? commandMapper = null)
        : base(mapper ?? PropertyMapper, commandMapper ?? CommandMapper)
    {
    }
    
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Releases the unmanaged resources used by the <see cref="MediaElement"/> and optionally releases the managed resources.
    /// </summary>
    /// <param name="disposing"><see langword="true"/> to release both managed and unmanaged resources; <see langword="false"/> to release only unmanaged resources.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
        }
    }
}
