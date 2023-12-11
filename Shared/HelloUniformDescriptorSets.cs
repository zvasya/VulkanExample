using Silk.NET.Vulkan;

namespace Shared;

public unsafe class HelloUniformDescriptorSets : HelloDescriptorSets
{
    protected HelloUniformDescriptorSets(DescriptorSet[] descriptorSets) : base(descriptorSets)
    {
    }
    
    public static HelloUniformDescriptorSets Create(HelloDescriptorPool descriptorPool, LogicalDevice device, HelloDescriptorSetLayout descriptorSetLayout, HelloBuffer[] uniformBuffers, uint dstBinding)
    {
        var count = uniformBuffers.Length;
        var descriptorSets = descriptorPool.CreateDescriptorSets(descriptorSetLayout, count);
        for (var i = 0; i < count; i++)
        {
            var bufferInfo = uniformBuffers[i].BufferInfo();
            var descriptorWrites = new WriteDescriptorSet[]
            {
                new()
                {
                    SType = StructureType.WriteDescriptorSet,
                    DstSet = descriptorSets[i],
                    DstBinding = dstBinding,
                    DstArrayElement = 0,
                    DescriptorType = DescriptorType.UniformBuffer,
                    DescriptorCount = 1,
                    PBufferInfo = &bufferInfo,
                }
            };

            fixed (WriteDescriptorSet* descriptorWritesPtr = descriptorWrites)
            {
                device.UpdateDescriptorSets((uint)descriptorWrites.Length, descriptorWritesPtr, 0, null);
            }
        }
        
        return new HelloUniformDescriptorSets(descriptorSets);
    }
}