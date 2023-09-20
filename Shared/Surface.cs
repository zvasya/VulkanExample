using Silk.NET.Vulkan;
using Silk.NET.Vulkan.Extensions.KHR;

namespace Shared;

public unsafe partial class Surface : IDisposable
{
    readonly KhrSurface _vkSurface;
    readonly Vk _vk;
    readonly SurfaceKHR _surface;
    readonly Instance _instance;
    readonly HelloEngine _engine;

    Surface(HelloEngine engine, Func<SurfaceKHR> createSurface)
    {
        _engine = engine;
        _vkSurface = engine._vkSurface;
        _instance = engine._instance;
        _vk = engine._vk!;

        _surface = createSurface();
    }

    public static Surface Create(HelloEngine engine, Func<SurfaceKHR> createSurface)
    {
        var surface = new Surface(engine, createSurface);

        surface.PickPhysicalDevice();

        surface.CreateLogicalDevice();

        surface.CreateSwapChain();

        surface.CreateImageViews();

        surface.CreateRenderPass();

        surface.CreateGraphicsPipeline();

        surface.CreateFramebuffers();

        surface.CreateCommandPool();

        surface.CreateCommandBuffers();

        surface.CreateSemaphores();

        return surface;
    }

    public void Dispose()
    {
        _vk.DestroySemaphore(_device, _renderFinishedSemaphore, null);
        _vk.DestroySemaphore(_device, _imageAvailableSemaphore, null);

        _vk.DestroyCommandPool(_device, _commandPool, null);

        CleanupSwapchain();

        _vk.DestroyDevice(_device, null);
            
        _vkSurface.DestroySurface(_instance, _surface, null);
    }
}
