using Silk.NET.Windowing.Sdl.iOS;
using iosSDL;

SilkMobile.RunApp([], strings =>
{
    var triangle = new VulkanView();
    triangle.Run();
});
