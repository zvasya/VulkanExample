using System.Runtime.CompilerServices;
using Silk.NET.Vulkan;

namespace Shared;

public class UniformBuffer : CircularBuffer
{
    UniformBuffer(LogicalDevice device, HelloBuffer[] buffers) : base(device, buffers)
    {
    }

    public static UniformBuffer Create<T>(LogicalDevice device) where T: struct
    {
        return new UniformBuffer(device, CreateBuffersArray(device, HelloEngine.MAX_FRAMES_IN_FLIGHT, (ulong)Unsafe.SizeOf<T>(), BufferUsageFlags.UniformBufferBit));
    }
    
    public HelloDescriptorSets CreateDescriptorSets(HelloDescriptorPool descriptorPool, HelloDescriptorSetLayout descriptorSetLayout, ImageView textureImageView, Sampler textureSampler)
    {
        return HelloDescriptorSets.Create(descriptorPool, Device, descriptorSetLayout, Buffers, textureImageView, textureSampler);
    }
}
