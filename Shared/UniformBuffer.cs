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
    
    public HelloDescriptorSets CreateDescriptorSets(HelloDescriptorPool descriptorPool, HelloDescriptorSetLayout descriptorSetLayout, uint dstBinding)
    {
        var descriptors = new DescriptorBufferInfo[Buffers.Length];
        for (var i = 0; i < Buffers.Length; i++)
        {
            var buffer = Buffers[i];
            descriptors[i] = buffer.BufferInfo();
        }

        return HelloUniformDescriptorSets.Create(descriptorPool, Device, descriptorSetLayout, Buffers, dstBinding);
    }
}
