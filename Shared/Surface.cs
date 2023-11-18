using System.Runtime.CompilerServices;
using Silk.NET.Vulkan;

namespace Shared;

public unsafe partial class Surface : IDisposable
{
    readonly HelloEngine _engine;
    readonly SurfaceKHR _surface;

    LogicalDevice _device;
    readonly bool _yInversion;
    SwapChainSupportDetails _swapChainSupport;
    SurfaceFormatKHR _surfaceFormat;
    Format _depthFormat;

    public event Action BeforeDraw;

    public Extent2D Extent2D => _swapChain.Extent;
    
    HelloRenderPass _renderPass;
    readonly List<HelloPipeline> _graphicsPipelines = new List<HelloPipeline>();

    Surface(HelloEngine engine, Func<SurfaceKHR> createSurface, bool yInversion)
    {
        _engine = engine;
        _yInversion = yInversion;
        _surface = createSurface();
    }

    public static Surface Create(HelloEngine engine, Func<SurfaceKHR> createSurface, bool yInversion = false)
    {
        var surface = new Surface(engine, createSurface, yInversion);
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
        
        _renderPass = HelloRenderPass.Create(_device, _surfaceFormat.Format, _depthFormat);
        
        CreateCommandBuffers();
        CreateSyncObjects(HelloEngine.MAX_FRAMES_IN_FLIGHT);
        
        CreateSwapChain2();
        CreateImageViews2();
        CreateDepthResources2();
        CreateFramebuffers2();
    }

    public void Update()
    {
        BeforeDraw.Invoke();
        DrawFrame();
    }

    public HelloTexture CreateTexture(Image<Rgba32> img)
    {
        var texture = HelloTexture.Create(_device, img);
        _textures.Add(texture);
        return texture;
    }

    public HelloPipeline CreatePipeLine(byte[] vertShaderCode, byte[] fragShaderCode, VertexInputBindingDescription bindingDescription, VertexInputAttributeDescription[] attributeDescriptions)
    {
        var pipeline = HelloPipeline.Create(_device, vertShaderCode, fragShaderCode, bindingDescription, attributeDescriptions, _renderPass);
        _graphicsPipelines.Add(pipeline);
        return pipeline;
    }

    public HelloIndexBuffer CreateIndexBuffer(ushort[] indices) => CreateIndexBufferInternal(indices, IndexType.Uint16);
    public HelloIndexBuffer CreateIndexBuffer(uint[] indices) => CreateIndexBufferInternal(indices, IndexType.Uint32);
    
    HelloIndexBuffer CreateIndexBufferInternal<T>(T[] indices, IndexType indexType) where T : struct
    {
        var size = (ulong)(Unsafe.SizeOf<T>() * indices.Length);
        var indexBuffer = HelloIndexBuffer.Create(_device, size, (uint)indices.Length, indexType);
        indexBuffer.FillStaging(indices, _device.CommandPool, _device.GraphicsQueue);
        return indexBuffer;
    }

    public HelloVertexBuffer CreateVertexBuffer<T>(T[] vertices) where T : struct
    {
        var size = (ulong)(Unsafe.SizeOf<T>() * vertices.Length);
        var vertexBuffer = HelloVertexBuffer.Create(_device, size);
        vertexBuffer.FillStaging(vertices, _device.CommandPool, _device.GraphicsQueue);
        return vertexBuffer;
    }

    public void RegisterCamera(CameraNode camera)
    {
        _camera = camera;
    }

    public RendererNode CreateRenderer(HelloPipeline pipeline, HelloVertexBuffer vertexBuffer, HelloIndexBuffer indexBuffer, HelloTexture texture)
    {
        var node = RendererNode.Create(_device, pipeline, vertexBuffer, indexBuffer, texture);
        _renderer.Add(node);
        node.CreateUniformBuffers(_descriptorPool);
        return node;
    }

    HelloPhysicalDevice PickPhysicalDevice()
    {
        return HelloPhysicalDevice.PickPhysicalDevice(_surface, _engine);
    }

    public void Dispose()
    {
        CleanupSwapchain();

        foreach (var graphicsPipeline in _graphicsPipelines)
            graphicsPipeline.Dispose();

        _renderPass.Dispose();

        foreach (var rendererNode in _renderer)
            rendererNode.ResetUniformBuffers();

        _descriptorPool.Dispose();

        foreach (var texture in _textures) 
            texture.Dispose();

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
