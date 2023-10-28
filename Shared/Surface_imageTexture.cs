using Silk.NET.Vulkan;

namespace Shared;

public unsafe partial class Surface
{
    HelloImage _textureImage;
    HelloImageView _textureImageView;
    Sampler _textureSampler;
    void CreateTextureImage(Image<Rgba32> img) {
    
        ulong imageSize = (ulong)(img.Width * img.Height * img.PixelType.BitsPerPixel / 8);

        using var staging = HelloBuffer.Create(_device, imageSize, BufferUsageFlags.TransferSrcBit, MemoryPropertyFlags.HostVisibleBit | MemoryPropertyFlags.HostCoherentBit);
        {
            staging.Fill(dataPointer => img.CopyPixelDataTo(new Span<byte>(dataPointer, (int)imageSize)));

            _textureImage = HelloImage.Create(_device, (uint)img.Width, (uint)img.Height, Format.R8G8B8A8Srgb, ImageTiling.Optimal, ImageUsageFlags.TransferDstBit | ImageUsageFlags.SampledBit, MemoryPropertyFlags.DeviceLocalBit,
                ImageType.Type2D);

            _textureImage.TransitionImageLayout(ImageLayout.Undefined, ImageLayout.TransferDstOptimal);
            _textureImage.CopyBufferToImage(staging, (uint)img.Width, (uint)img.Height);
            _textureImage.TransitionImageLayout(ImageLayout.TransferDstOptimal, ImageLayout.ShaderReadOnlyOptimal);
        }
    
    }

    private void CreateTextureImageView()
    {
        _textureImageView = HelloImageView.Create(_device, _textureImage, ImageAspectFlags.ColorBit);
    }

    private void CreateTextureSampler()
    {
        _device.PhysicalDevice.GetPhysicalDeviceProperties(out var properties);

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

        fixed (Sampler* textureSamplerPtr = &_textureSampler)
        {
            if (_device.CreateSampler(samplerInfo, null, textureSamplerPtr) != Result.Success)
            {
                throw new Exception("failed to create texture sampler!");
            }
        }
    }
}
