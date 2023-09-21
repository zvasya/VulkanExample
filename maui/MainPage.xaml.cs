using Shared;
using VulkanView.Maui.Views.Interfaces;
using VulkanView.Maui.Views.Primitives;

namespace maui;

public partial class MainPage : ContentPage
{
    HelloEngine _engine;
    Surface _surface;

	public MainPage()
	{
		InitializeComponent();
    }

    void OnCounterClicked(object sender, EventArgs e)
	{
		SemanticScreenReader.Announce(CounterBtn.Text);
	}

    void vulkanView_ViewCreated(object sender, EventArgs e)
    {
        var v = vulkanView as IVulkanView;
        _engine = HelloEngine.Create(v.Platform);
        _surface = _engine.CreateSurface(() => v.CreateSurface(_engine));
    }

    void vulkanView_OnViewDestroyed(object sender, EventArgs e)
    {
        _surface.Dispose();
    }

    void vulkanView_DrawFrame(object sender, EventArgs e)
    {
        _surface.DrawFrame();
    }

    void vulkanView_OnChangeSize(object sender, ChangeSizeEventArgs e)
    {
        _surface?.ChangeSize();
    }
}


