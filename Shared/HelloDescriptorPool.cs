using Silk.NET.Vulkan;

namespace Shared;

public unsafe class HelloDescriptorPool : IDisposable
{
    readonly DescriptorPool _descriptorPool;

    readonly LogicalDevice _device;
    HelloDescriptorPool(LogicalDevice device, DescriptorPool descriptorPool)
    {
        _device = device;
        _descriptorPool = descriptorPool;
    }
    

    public static HelloDescriptorPool Create(LogicalDevice device, uint count)
    {
        var descriptorPool = CreateDescriptorPool(device, count);
        return new HelloDescriptorPool(device, descriptorPool);
    }

    static DescriptorPool CreateDescriptorPool(LogicalDevice device, uint count)
    {
        const uint Size = 512;

        var uboCount = count * Size;
        var imagesCount = count * Size;
        DescriptorPoolSize[] poolSizes = 
        {
            new()
            {
                Type = DescriptorType.UniformBuffer,
                DescriptorCount = uboCount,
            },
            new()
            {
                Type = DescriptorType.CombinedImageSampler,
                DescriptorCount = imagesCount,
            },
        };

        uint descriptorTypeCount = (uint)poolSizes.Length;

        uint totalPoolSets = (uint)poolSizes.Sum(descriptorPoolSize => descriptorPoolSize.DescriptorCount);
        
        fixed (DescriptorPoolSize* poolSizePtr = &poolSizes[0])
        {
            DescriptorPoolCreateInfo poolInfo = new()
            {
                SType = StructureType.DescriptorPoolCreateInfo,
                PoolSizeCount = descriptorTypeCount,
                PPoolSizes = poolSizePtr,
                MaxSets = totalPoolSets,
            };

            DescriptorPool descriptorPool;
            {
                if (device.CreateDescriptorPool(poolInfo, null, &descriptorPool) != Result.Success)
                {
                    throw new Exception("failed to create descriptor pool!");
                }
            }

            return descriptorPool;
        }
    }

    public DescriptorSet[] CreateDescriptorSets(HelloDescriptorSetLayout descriptorSetLayout, int count)
    {
        var layouts = descriptorSetLayout.ToArray(count);
        
        var descriptorSets = new DescriptorSet[count];
        
        fixed (DescriptorSetLayout* layoutsPtr = layouts)
        {
            DescriptorSetAllocateInfo allocateInfo = new()
            {
                SType = StructureType.DescriptorSetAllocateInfo,
                DescriptorPool = _descriptorPool,
                DescriptorSetCount = (uint)count,
                PSetLayouts = layoutsPtr,
            };
            
            fixed (DescriptorSet* descriptorSetsPtr = descriptorSets)
            {
                if (_device.AllocateDescriptorSets(allocateInfo, descriptorSetsPtr) != Result.Success)
                {
                    throw new Exception("failed to allocate descriptor sets!");
                }
            }
        }

        return descriptorSets;
    }

    public void Dispose()
    {
        _device.DestroyDescriptorPool(_descriptorPool, null);
    }
}
