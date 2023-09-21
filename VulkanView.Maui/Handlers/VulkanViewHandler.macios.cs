using Microsoft.Maui.Handlers;
using VulkanView.Maui.Core.Views.iOS;

namespace VulkanView.Maui.Views;

public partial class VulkanViewHandler : ViewHandler<VulkanView, MauiVulkanView>
{
    protected override MauiVulkanView CreatePlatformView()
    {
        return new MauiVulkanView(VirtualView);
    }
}