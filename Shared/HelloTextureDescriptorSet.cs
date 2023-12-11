using Silk.NET.Vulkan;

namespace Shared;

public unsafe class HelloTextureDescriptorSet : HelloDescriptorSets
{
    protected HelloTextureDescriptorSet(DescriptorSet[] descriptorSets) : base(descriptorSets)
    {
    }
    
    public static HelloTextureDescriptorSet Create(HelloDescriptorPool descriptorPool, LogicalDevice device, HelloDescriptorSetLayout descriptorSetLayout, HelloTexture texture, int count, uint dstBinding)
    {
        var descriptorSets = descriptorPool.CreateDescriptorSets(descriptorSetLayout, count);

        for (var i = 0; i < count; i++)
        {
            var imageInfo = new DescriptorImageInfo
            {
                ImageLayout = ImageLayout.ShaderReadOnlyOptimal,
                ImageView = texture.ImageView,
                Sampler = texture.Sampler,
            };

            var descriptorWrites = new WriteDescriptorSet[]
            {
                new()
                {
                    SType = StructureType.WriteDescriptorSet,
                    DstSet = descriptorSets[i],
                    DstBinding = dstBinding,
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
        
        return new HelloTextureDescriptorSet(descriptorSets);
    }
}