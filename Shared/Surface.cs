using Silk.NET.Maths;
using Silk.NET.Vulkan;

namespace Shared;

public unsafe partial class Surface : IDisposable
{
    readonly HelloEngine _engine;
    readonly SurfaceKHR _surface;

    LogicalDevice _device;
    SwapChainSupportDetails _swapChainSupport;
    SurfaceFormatKHR _surfaceFormat;
    Format _depthFormat;
    
    HelloRenderPass _renderPass;

    Surface(HelloEngine engine, Func<SurfaceKHR> createSurface)
    {
        _engine = engine;
        _surface = createSurface();
    }

    public static Surface Create(HelloEngine engine, Func<SurfaceKHR> createSurface)
    {
        var surface = new Surface(engine, createSurface);
        surface.Init();
        return surface;
    }

    void Init()
    {
        var physicalDevice = PickPhysicalDevice();
        _device = physicalDevice.CreateLogicalDevice();

        _swapChainSupport = _device.PhysicalDevice.QuerySwapChainSupport(_surface);
        _surfaceFormat = HelloPhysicalDevice.ChooseSwapSurfaceFormat(_swapChainSupport._formats);
        _depthFormat = _device.PhysicalDevice.FindDepthFormat();
        CreateDescriptorPool();
        
        // @@@ EXTERNAL
        _renderPass = HelloRenderPass.Create(_device, _surfaceFormat.Format, _depthFormat);
        _graphicsPipeline = HelloPipeline.Create(_device, _engine.Platform.GetVertShader(), _engine.Platform.GetFragShader(), _renderPass);
        
        CreateCommandBuffers();
        CreateSyncObjects(HelloEngine.MAX_FRAMES_IN_FLIGHT);
        
        
        CreateSwapChain2();
        CreateImageViews2();
        CreateDepthResources2();
        CreateFramebuffers2();

        using (var img = _engine.Platform.GetImage())
        {
            CreateTextureImage(img);   
        }
        CreateTextureImageView();
        CreateTextureSampler();
        
        _renderer.Add(RendererNode.Create(_device, 4, new Vector3D<float>(0,0,0.5f) ));
        _renderer.Add(RendererNode.Create(_device, 2, Vector3D<float>.Zero ));
        _renderer.Add(RendererNode.Create(_device, 3, new Vector3D<float>(0,0,-0.5f) ));
        _renderer.Add(RendererNode.Create(_device, 5, new Vector3D<float>(0,0,1.0f) ));
        _renderer.Add(RendererNode.Create(_device, 1, new Vector3D<float>(0,0,-1.0f) ));
        
        foreach (var rendererNode in _renderer) 
            rendererNode.CreateUniformBuffers(_descriptorPool, _graphicsPipeline.DescriptorSetLayout, _textureImageView.ImageView, _textureSampler);
    }

    HelloPhysicalDevice PickPhysicalDevice()
    {
        return HelloPhysicalDevice.PickPhysicalDevice(_surface, _engine);
    }

    public void Dispose()
    {
        CleanupSwapchain();

        _graphicsPipeline.Dispose();
        _renderPass.Dispose();

        foreach (var rendererNode in _renderer)
            rendererNode.ResetUniformBuffers();

        _descriptorPool.Dispose();

        _device.DestroySampler(_textureSampler, null);
        _textureImageView.Dispose();

        _textureImage.Dispose();

        foreach (var rendererNode in _renderer)
        {
            rendererNode.Dispose();
        }
        
        for (var i = 0; i < _renderFinishedSemaphores.Length; i++)
        {
            _renderFinishedSemaphores[i].Dispose();
            _imageAvailableSemaphores[i].Dispose();
            _inFlightFences[i].Dispose();
        }

        _device.Dispose();

        _engine.DestroySurface(_surface, null);
    }
}
