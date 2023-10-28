using Silk.NET.Vulkan;
using Buffer = Silk.NET.Vulkan.Buffer;
using Image = Silk.NET.Vulkan.Image;

namespace Shared;

public unsafe class HelloImage : IDisposable
{
    readonly LogicalDevice _device;
    readonly Image _image;
    readonly DeviceMemory _imageMemory;
    readonly ImageType _imageType;
    readonly Format _format;

    public Image Image => _image;

    HelloImage(LogicalDevice device, ImageType imageType, Format format, Image image, DeviceMemory imageMemory)
    {
        _device = device;
        _imageType = imageType;
        _format = format;
        _image = image;
        _imageMemory = imageMemory;
    }

    public static HelloImage Create(LogicalDevice device, uint width, uint height, Format format, ImageTiling tiling, ImageUsageFlags usage, MemoryPropertyFlags properties, ImageType imageType)
    {
        var image = CreateImage(device, imageType, format, width, height, tiling, usage);
        var memory = CreateDeviceMemory(device, image, properties);
        var instance = new HelloImage(device, imageType, format, image, memory);
        return instance;
    }

    static Image CreateImage(LogicalDevice device, ImageType imageType, Format format, uint width, uint height, ImageTiling tiling, ImageUsageFlags usage)
    {
        var imageInfo = new ImageCreateInfo
        {
            SType = StructureType.ImageCreateInfo,
            ImageType = imageType,
            Extent = new Extent3D
            {
                Width = width,
                Height = height,
                Depth = 1,
            },
            MipLevels = 1,
            ArrayLayers = 1,
            Format = format,
            Tiling = tiling,
            InitialLayout = ImageLayout.Undefined,
            Usage = usage,
            Samples = SampleCountFlags.Count1Bit,
            SharingMode = SharingMode.Exclusive,
        };


        Image image;
        if (device.CreateImage(&imageInfo, null, &image) != Result.Success)
            throw new Exception("faild to create image!");
        
        return image;
    }
    
    static DeviceMemory CreateDeviceMemory(LogicalDevice device, Image image, MemoryPropertyFlags properties)
    {
        MemoryRequirements memRequirements;
        device.GetImageMemoryRequirements(image, &memRequirements);

        var allocInfo = new MemoryAllocateInfo
        {
            SType = StructureType.MemoryAllocateInfo,
            AllocationSize = memRequirements.Size,
            MemoryTypeIndex = device.PhysicalDevice.FindMemoryType(memRequirements.MemoryTypeBits, properties),
        };

        DeviceMemory _imageMemory;
        Helpers.CheckErrors(device.AllocateMemory(&allocInfo, null, &_imageMemory));

        device.BindImageMemory(image, _imageMemory, 0);
        return _imageMemory;
    }

    public void TransitionImageLayout(ImageLayout oldLayout, ImageLayout newLayout) {
        var commandBuffer = _device.CommandPool.BeginSingleTimeCommands();

        var barrier = new ImageMemoryBarrier();
        barrier.SType = StructureType.ImageMemoryBarrier;
        barrier.OldLayout = oldLayout;
        barrier.NewLayout = newLayout;
        barrier.SrcQueueFamilyIndex = Vk.QueueFamilyIgnored;
        barrier.DstQueueFamilyIndex = Vk.QueueFamilyIgnored;
        barrier.Image = _image;
        barrier.SubresourceRange.AspectMask = ImageAspectFlags.ColorBit;
        barrier.SubresourceRange.BaseMipLevel = 0;
        barrier.SubresourceRange.LevelCount = 1;
        barrier.SubresourceRange.BaseArrayLayer = 0;
        barrier.SubresourceRange.LayerCount = 1;

        PipelineStageFlags sourceStage;
        PipelineStageFlags destinationStage;

        if (oldLayout == ImageLayout.Undefined && newLayout == ImageLayout.TransferDstOptimal) {
            barrier.SrcAccessMask = 0;
            barrier.DstAccessMask = AccessFlags.TransferWriteBit;

            sourceStage = PipelineStageFlags.TopOfPipeBit;
            destinationStage = PipelineStageFlags.TransferBit;
        } else if (oldLayout == ImageLayout.TransferDstOptimal && newLayout == ImageLayout.ShaderReadOnlyOptimal) {
            barrier.SrcAccessMask = AccessFlags.TransferWriteBit;
            barrier.DstAccessMask = AccessFlags.ShaderReadBit;

            sourceStage = PipelineStageFlags.TransferBit;
            destinationStage = PipelineStageFlags.FragmentShaderBit;
        } else {
            throw new Exception("unsupported layout transition!");
        }

        commandBuffer.CmdPipelineBarrier(
            sourceStage,
            destinationStage,
            0,
            0, 
            null,
            0,
            null,
            1, 
            &barrier
        );

        _device.CommandPool.EndSingleTimeCommands(commandBuffer, _device.GraphicsQueue);
    }
    
    public void CopyBufferToImage(HelloBuffer buffer,uint width, uint height)
    {
        var commandBuffer = _device.CommandPool.BeginSingleTimeCommands();

        BufferImageCopy region = new()
        {
            BufferOffset = 0,
            BufferRowLength = 0,
            BufferImageHeight = 0,
            ImageSubresource =
            {
                AspectMask = ImageAspectFlags.ColorBit,
                MipLevel = 0,
                BaseArrayLayer = 0,
                LayerCount = 1,
            },
            ImageOffset = new Offset3D(0, 0, 0),
            ImageExtent = new Extent3D(width, height, 1),

        };

        commandBuffer.CmdCopyBufferToImage(buffer.Buffer, _image, ImageLayout.TransferDstOptimal, 1, region);

        _device.CommandPool.EndSingleTimeCommands(commandBuffer, _device.GraphicsQueue);
    }
        
    public ImageViewCreateInfo GetImageViewCreateInfo(ImageAspectFlags aspectMask)
    {
        var viewType = _imageType switch
        {
            ImageType.Type1D => ImageViewType.Type1D,
            ImageType.Type2D => ImageViewType.Type2D,
            ImageType.Type3D => ImageViewType.Type3D,
            _ => ImageViewType.Type2D
        };
        var createInfo = new ImageViewCreateInfo();
        createInfo.SType = StructureType.ImageViewCreateInfo;
        createInfo.Image = _image;
        createInfo.ViewType = viewType;
        createInfo.Format = _format;
        createInfo.Components.R = ComponentSwizzle.Identity;
        createInfo.Components.G = ComponentSwizzle.Identity;
        createInfo.Components.B = ComponentSwizzle.Identity;
        createInfo.Components.A = ComponentSwizzle.Identity;
        createInfo.SubresourceRange.AspectMask = aspectMask;
        createInfo.SubresourceRange.BaseMipLevel = 0;
        createInfo.SubresourceRange.LevelCount = 1;
        createInfo.SubresourceRange.BaseArrayLayer = 0;
        createInfo.SubresourceRange.LayerCount = 1;
        
        return createInfo;
    }

    public void Dispose()
    {
        _device.DestroyImage(_image, null);
        _device.FreeMemory(_imageMemory, null);
    }
}
