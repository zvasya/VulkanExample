using Silk.NET.Vulkan;

namespace Shared;

public unsafe partial class Surface
{
    DescriptorSetLayout _descriptorSetLayout;
    
    void CreateDescriptorSetLayout()
        {
            DescriptorSetLayoutBinding uboLayoutBinding = new()
            {
                Binding = 0,
                DescriptorCount = 1,
                DescriptorType = DescriptorType.UniformBuffer,
                PImmutableSamplers = null,
                StageFlags = ShaderStageFlags.VertexBit,
            };
    
            DescriptorSetLayoutCreateInfo layoutInfo = new()
            {
                SType = StructureType.DescriptorSetLayoutCreateInfo,
                BindingCount = 1,
                PBindings = &uboLayoutBinding,
            };
    
            fixed (DescriptorSetLayout* descriptorSetLayoutPtr = &_descriptorSetLayout)
            {
                if (_vk.CreateDescriptorSetLayout(_device, layoutInfo, null, descriptorSetLayoutPtr) != Result.Success)
                {
                    throw new Exception("failed to create descriptor set layout!");
                }
            }
        }
}
