using Silk.NET.Vulkan;

namespace Shared;

public unsafe class HelloTexture : IDisposable
{
    HelloTexture(LogicalDevice device, HelloImage textureImage, HelloImageView textureImageView, Sampler textureSampler)
    {
        _device = device;
        _textureImage = textureImage;
        _textureImageView = textureImageView;
        _textureSampler = textureSampler;
    }

    public Sampler Sampler => _textureSampler;
    public ImageView ImageView => _textureImageView.ImageView;

    public static HelloTexture Create(LogicalDevice device, Image<Rgba32> img)
    {
        var image = CreateTextureImage(device, img);   
        var imageView = CreateTextureImageView(device, image);
        var sampler = CreateTextureSampler(device);
        return new HelloTexture(device, image, imageView, sampler);
    }

    readonly LogicalDevice _device;
    readonly HelloImage _textureImage;
    readonly HelloImageView _textureImageView;
    readonly Sampler _textureSampler;
    static HelloImage CreateTextureImage(LogicalDevice device, Image<Rgba32> img) {
    
        HelloImage textureImage;
        ulong imageSize = (ulong)(img.Width * img.Height * img.PixelType.BitsPerPixel / 8);

        using var staging = HelloBuffer.Create(device, imageSize, BufferUsageFlags.TransferSrcBit, MemoryPropertyFlags.HostVisibleBit | MemoryPropertyFlags.HostCoherentBit);
        {
            staging.Fill(dataPointer => img.CopyPixelDataTo(new Span<byte>(dataPointer, (int)imageSize)));

            textureImage = HelloImage.Create(device, (uint)img.Width, (uint)img.Height, Format.R8G8B8A8Srgb, ImageTiling.Optimal, ImageUsageFlags.TransferDstBit | ImageUsageFlags.SampledBit, MemoryPropertyFlags.DeviceLocalBit,
                ImageType.Type2D);

            textureImage.TransitionImageLayout(ImageLayout.Undefined, ImageLayout.TransferDstOptimal);
            textureImage.CopyBufferToImage(staging, (uint)img.Width, (uint)img.Height);
            textureImage.TransitionImageLayout(ImageLayout.TransferDstOptimal, ImageLayout.ShaderReadOnlyOptimal);
        }
        return textureImage;
    }

    static HelloImageView CreateTextureImageView(LogicalDevice device, HelloImage textureImage)
    {
        return HelloImageView.Create(device, textureImage, ImageAspectFlags.ColorBit);
    }

    static Sampler CreateTextureSampler(LogicalDevice device)
    {
        device.PhysicalDevice.GetPhysicalDeviceProperties(out var properties);

        SamplerCreateInfo samplerInfo = new()
        {
            SType = StructureType.SamplerCreateInfo,
            MagFilter = Filter.Linear,
            MinFilter = Filter.Linear,
            AddressModeU = SamplerAddressMode.Repeat,
            AddressModeV = SamplerAddressMode.Repeat,
            AddressModeW = SamplerAddressMode.Repeat,
            AnisotropyEnable = true,
            MaxAnisotropy = properties.Limits.MaxSamplerAnisotropy,
            BorderColor = BorderColor.IntOpaqueBlack,
            UnnormalizedCoordinates = false,
            CompareEnable = false,
            CompareOp = CompareOp.Always,
            MipmapMode = SamplerMipmapMode.Linear,
        };

        Sampler textureSampler;
        {
            if (device.CreateSampler(samplerInfo, null, &textureSampler) != Result.Success)
            {
                throw new Exception("failed to create texture sampler!");
            }
        }

        return textureSampler;
    }

    public void Dispose()
    {
        _device.DestroySampler(_textureSampler, null);
        _textureImageView.Dispose();
        
        _textureImage.Dispose();
    }
}
