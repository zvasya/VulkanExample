using Silk.NET.Vulkan;
using Silk.NET.Vulkan.Extensions.KHR;
using Buffer = Silk.NET.Vulkan.Buffer;

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
        
        surface.CreateDescriptorSetLayout();

        surface.CreateGraphicsPipeline();

        surface.CreateFramebuffers();

        surface.CreateCommandPool();
        
        surface.CreateVertexBuffer();
        
        surface.CreateIndexBuffer();
        
        surface.CreateUniformBuffers();
        
        surface.CreateDescriptorPool();
        surface.CreateDescriptorSets();

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
        
        _vk!.DestroyBuffer(_device, _indexBuffer, null);
        _vk!.FreeMemory(_device, _indexBufferMemory, null);

        _vk!.DestroyBuffer(_device, _vertexBuffer, null);
        _vk!.FreeMemory(_device, _vertexBufferMemory, null);
        
        _vk!.DestroyDescriptorSetLayout(_device, _descriptorSetLayout, null);

        _vk.DestroyDevice(_device, null);
            
        _vkSurface.DestroySurface(_instance, _surface, null);
    }

    void CreateBuffer(ulong size, BufferUsageFlags usage, MemoryPropertyFlags properties, ref Buffer buffer, ref DeviceMemory bufferMemory)
    {
        BufferCreateInfo bufferInfo = new()
        {
            SType = StructureType.BufferCreateInfo,
            Size = size,
            Usage = usage,
            SharingMode = SharingMode.Exclusive,
        };

        fixed (Buffer* bufferPtr = &buffer)
        {
            if (_vk.CreateBuffer(_device, bufferInfo, null, bufferPtr) != Result.Success)
            {
                throw new Exception("failed to create vertex buffer!");
            }
        }

        MemoryRequirements memRequirements = new();
        _vk.GetBufferMemoryRequirements(_device, buffer, out memRequirements);

        MemoryAllocateInfo allocateInfo = new()
        {
            SType = StructureType.MemoryAllocateInfo,
            AllocationSize = memRequirements.Size,
            MemoryTypeIndex = FindMemoryType(memRequirements.MemoryTypeBits, properties),
        };

        fixed (DeviceMemory* bufferMemoryPtr = &bufferMemory)
        {
            if (_vk.AllocateMemory(_device, allocateInfo, null, bufferMemoryPtr) != Result.Success)
            {
                throw new Exception("failed to allocate vertex buffer memory!");
            }
        }

        _vk.BindBufferMemory(_device, buffer, bufferMemory, 0);
    }
    
    uint FindMemoryType(uint typeFilter, MemoryPropertyFlags properties)
    {
        _vk.GetPhysicalDeviceMemoryProperties(_physicalDevice, out var memProperties);

        for (var i = 0; i < memProperties.MemoryTypeCount; i++)
        {
            if ((typeFilter & (1 << i)) != 0 && (memProperties.MemoryTypes[i].PropertyFlags & properties) == properties)
            {
                return (uint)i;
            }
        }

        throw new Exception("failed to find suitable memory type!");
    }
    
    void CopyBuffer(Buffer srcBuffer, Buffer dstBuffer, ulong size)
    {
        CommandBufferAllocateInfo allocateInfo = new()
        {
            SType = StructureType.CommandBufferAllocateInfo,
            Level = CommandBufferLevel.Primary,
            CommandPool = _commandPool,
            CommandBufferCount = 1,
        };

        _vk.AllocateCommandBuffers(_device, allocateInfo, out CommandBuffer commandBuffer);

        CommandBufferBeginInfo beginInfo = new()
        {
            SType = StructureType.CommandBufferBeginInfo,
            Flags = CommandBufferUsageFlags.OneTimeSubmitBit,
        };

        _vk.BeginCommandBuffer(commandBuffer, beginInfo);

        BufferCopy copyRegion = new()
        {
            Size = size,
        };

        _vk.CmdCopyBuffer(commandBuffer, srcBuffer, dstBuffer, 1, copyRegion);

        _vk.EndCommandBuffer(commandBuffer);

        SubmitInfo submitInfo = new()
        {
            SType = StructureType.SubmitInfo,
            CommandBufferCount = 1,
            PCommandBuffers = &commandBuffer,
        };

        _vk.QueueSubmit(_graphicsQueue, 1, submitInfo, default);
        _vk.QueueWaitIdle(_graphicsQueue);

        _vk.FreeCommandBuffers(_device, _commandPool, 1, commandBuffer);
    }
}
