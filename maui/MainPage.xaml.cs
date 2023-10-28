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
    Example1 _example;

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
        _example = new Example1(_surface, GetVertShader, GetFragShader, GetImage1, GetImage2);
    }
    
    static byte[] GetVertShader() => Read("Shaders/vert.spv");
    static byte[] GetFragShader() => Read("Shaders/frag.spv");

    static Image<Rgba32> GetImage1()
    {
        using var stream = FileSystem.OpenAppPackageFileAsync("Textures/texture.jpg");
        return Image.Load<Rgba32>(stream.Result);
    }
    
    static Image<Rgba32> GetImage2()
    {
        using var stream = FileSystem.OpenAppPackageFileAsync("Textures/texture2.jpg");
        return Image.Load<Rgba32>(stream.Result);
    }
    
    static byte[] Read(string fileName)
    {
        using var stream = FileSystem.OpenAppPackageFileAsync(fileName);
        using var memStream = new MemoryStream();
        stream.Result.CopyTo(memStream);
        return memStream.ToArray();
    }

    void vulkanView_OnViewDestroyed(object sender, EventArgs e)
    {
        _surface.Dispose();
    }

    void vulkanView_DrawFrame(object sender, EventArgs e)
    {
        _example.Update();
        _surface.DrawFrame();
    }

    void vulkanView_OnChangeSize(object sender, ChangeSizeEventArgs e)
    {
        _surface?.ChangeSize();
    }
}


