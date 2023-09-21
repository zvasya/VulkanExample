namespace VulkanView.Maui.Views;

public static class AppBuilderExtension
{
    public static MauiAppBuilder UseMauiVulkanView(this MauiAppBuilder builder)
    {
        builder.ConfigureMauiHandlers(h =>
        {
            h.AddHandler<VulkanView, VulkanViewHandler>();
        });

        return builder;
    }
}