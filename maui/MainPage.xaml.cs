using Examples;
using Shared;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using VulkanView.Maui.Views.Interfaces;
using VulkanView.Maui.Views.Primitives;
using Image = SixLabors.ImageSharp.Image;

namespace maui;

public partial class MainPage : ContentPage
{
    HelloEngine _engine;
    Surface _surface;
    object _example;

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
        _example = new Example3(_surface, Load);
    }

    static Stream Load(string path)
    {
        return FileSystem.OpenAppPackageFileAsync(Path.Combine("Assets", path)).Result;
    }

    void vulkanView_OnViewDestroyed(object sender, EventArgs e)
    {
        _surface.Dispose();
    }

    void vulkanView_DrawFrame(object sender, EventArgs e)
    {
        _surface.Update();
    }

    void vulkanView_OnChangeSize(object sender, ChangeSizeEventArgs e)
    {
        _surface?.ChangeSize();
    }
}