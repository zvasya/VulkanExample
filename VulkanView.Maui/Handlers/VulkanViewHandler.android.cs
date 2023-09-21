using Microsoft.Maui.Handlers;
using VulkanView.Maui.Core.Views;

namespace VulkanView.Maui.Views;

public partial class VulkanViewHandler : ViewHandler<VulkanView, MauiVulkanView>, IDisposable
{
    protected override MauiVulkanView CreatePlatformView()
    {
        return new MauiVulkanView(Context, VirtualView);
    }

    //TODO 
    // protected override void DisconnectHandler(MauiMediaElement platformView)
    // {
    //     platformView.Dispose();
    //     Dispose();
    //     base.DisconnectHandler(platformView);
    // }
}
