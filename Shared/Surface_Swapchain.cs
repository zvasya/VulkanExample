using Silk.NET.Vulkan;

namespace Shared;

public unsafe partial class Surface
{
    HelloSwapchain _swapChain;
    readonly List<HelloTexture> _textures = new List<HelloTexture>();
    readonly List<HelloBuffer> _buffers = new List<HelloBuffer>();
    readonly List<IRendererNode> _rendererNodes = new List<IRendererNode>();
    readonly List<CircularBuffer> _circularBuffers = new List<CircularBuffer>();
    CameraNode _camera;

    void CreateSwapChain2()
    {
        _swapChainSupport = _device.PhysicalDevice.QuerySwapChainSupport(_surface);
        _surfaceFormat = HelloPhysicalDevice.ChooseSwapSurfaceFormat(_swapChainSupport._formats);
        _swapChain = HelloSwapchain.Create(_device, _surface, _swapChainSupport, _surfaceFormat);
    }

    void RecreateSwapChain()
    {
        _ = _device.DeviceWaitIdle();

        CleanupSwapchain();

        CreateSwapChain2();
        CreateImageViews2();
        CreateDepthResources2();
        CreateFramebuffers2();
    }

    void CleanupSwapchain()
    {
        _depthImageView.Dispose();
        _depthImage.Dispose();
        
        foreach (var framebuffer in _swapChainFramebuffers) 
            framebuffer.Dispose();

        foreach (var imageView in _swapChainImageViews) 
            imageView.Dispose();

        _swapChain.Dispose();
    }

    public struct SwapChainSupportDetails
    {
        public SurfaceCapabilitiesKHR _capabilities;
        public SurfaceFormatKHR[] _formats;
        public PresentModeKHR[] _presentModes;
    }
}
