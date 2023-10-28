using Silk.NET.Vulkan;

namespace Shared;

public unsafe class HelloDescriptorSets
{
    readonly DescriptorSet[] _descriptorSets;

    HelloDescriptorSets(DescriptorSet[] descriptorSets)
    {
        _descriptorSets = descriptorSets;
    }

    public static HelloDescriptorSets Create(HelloDescriptorPool descriptorPool, LogicalDevice device, HelloDescriptorSetLayout descriptorSetLayout, HelloBuffer[] uniformBuffers, ImageView textureImageView, Sampler textureSampler)
    {
        var count = uniformBuffers.Length;
        var descriptorSets = descriptorPool.CreateDescriptorSets(descriptorSetLayout, count);

        for (var i = 0; i < count; i++)
        {
            var bufferInfo = uniformBuffers[i].BufferInfo();
            
            var imageInfo = new DescriptorImageInfo
            {
                ImageLayout = ImageLayout.ShaderReadOnlyOptimal,
                ImageView = textureImageView,
                Sampler = textureSampler,
            };

            var descriptorWrites = new WriteDescriptorSet[]
            {
                new()
                {
                    SType = StructureType.WriteDescriptorSet,
                    DstSet = descriptorSets[i],
                    DstBinding = 0,
                    DstArrayElement = 0,
                    DescriptorType = DescriptorType.UniformBuffer,
                    DescriptorCount = 1,
                    PBufferInfo = &bufferInfo,
                },
                new()
                {
                    SType = StructureType.WriteDescriptorSet,
                    DstSet = descriptorSets[i],
                    DstBinding = 1,
                    DstArrayElement = 0,
                    DescriptorType = DescriptorType.CombinedImageSampler,
                    DescriptorCount = 1,
                    PImageInfo = &imageInfo,
                }
            };

            fixed (WriteDescriptorSet* descriptorWritesPtr = descriptorWrites)
            {
                device.UpdateDescriptorSets((uint)descriptorWrites.Length, descriptorWritesPtr, 0, null);
            }
        }
        
        return new HelloDescriptorSets(descriptorSets);
    }

    public DescriptorSet this[uint index]
    {
        get => _descriptorSets[index];
    }
}
