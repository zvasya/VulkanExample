using Silk.NET.Vulkan;

namespace Shared;

public unsafe class HelloDescriptorSetLayout : IDisposable
{
    DescriptorSetLayout _descriptorSetLayout;
    readonly LogicalDevice _device;
    
    HelloDescriptorSetLayout(LogicalDevice device, DescriptorSetLayout descriptorSetLayout)
    {
        _device = device;
        _descriptorSetLayout = descriptorSetLayout;
    }

    public PipelineLayoutCreateInfo GetPipelineLayoutCreateInfo()
    {
        fixed (DescriptorSetLayout* descriptorSetLayoutPtr = &_descriptorSetLayout)
        {
            return new PipelineLayoutCreateInfo
            {
                SType = StructureType.PipelineLayoutCreateInfo,
                SetLayoutCount = 1,
                PSetLayouts = descriptorSetLayoutPtr,
                // PushConstantRangeCount = 0, // Optional
                // PPushConstantRanges = null, // Optional
            };
        }
    }

    public static HelloDescriptorSetLayout Create(LogicalDevice device)
    {
        var descriptorSetLayout = CreateDescriptorSetLayout(device);
        return new HelloDescriptorSetLayout(device, descriptorSetLayout);
    }

    static DescriptorSetLayout CreateDescriptorSetLayout(LogicalDevice device)
    {
        DescriptorSetLayoutBinding uboLayoutBinding = new()
        {
            Binding = 0,
            DescriptorCount = 1,
            DescriptorType = DescriptorType.UniformBuffer,
            PImmutableSamplers = null,
            StageFlags = ShaderStageFlags.VertexBit,
        };

        DescriptorSetLayoutBinding samplerLayoutBinding = new()
        {
            Binding = 1,
            DescriptorCount = 1,
            DescriptorType = DescriptorType.CombinedImageSampler,
            PImmutableSamplers = null,
            StageFlags = ShaderStageFlags.FragmentBit,
        };

        var bindings = stackalloc DescriptorSetLayoutBinding[] { uboLayoutBinding, samplerLayoutBinding  };
        DescriptorSetLayoutCreateInfo layoutInfo = new()
        {
            SType = StructureType.DescriptorSetLayoutCreateInfo,
            BindingCount = 2,
            PBindings = bindings,
        };

        DescriptorSetLayout descriptorSetLayout;
        if (device.CreateDescriptorSetLayout(layoutInfo, null, &descriptorSetLayout) != Result.Success)
        {
            throw new Exception("failed to create descriptor set layout!");
        }
        
        return descriptorSetLayout;
    }

    public DescriptorSetLayout[] ToArray(int count)
    {
        var layouts = new DescriptorSetLayout[count];
        Array.Fill(layouts, _descriptorSetLayout);
        return layouts;
    }

    public void Dispose()
    {
        _device.DestroyDescriptorSetLayout(_descriptorSetLayout, null);
    }
}
