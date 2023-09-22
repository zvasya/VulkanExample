using System.Runtime.CompilerServices;
using Silk.NET.Vulkan;

namespace Shared;

public unsafe partial class Surface
{
    DescriptorPool _descriptorPool;
    DescriptorSet[]? _descriptorSets;

    void CreateDescriptorPool()
    {
        DescriptorPoolSize poolSize = new()
        {
            Type = DescriptorType.UniformBuffer, DescriptorCount = (uint)_swapChainImages.Length,
        };


        DescriptorPoolCreateInfo poolInfo = new()
        {
            SType = StructureType.DescriptorPoolCreateInfo,
            PoolSizeCount = 1,
            PPoolSizes = &poolSize,
            MaxSets = (uint)_swapChainImages.Length,
        };

        fixed (DescriptorPool* descriptorPoolPtr = &_descriptorPool)
        {
            if (_vk.CreateDescriptorPool(_device, poolInfo, null, descriptorPoolPtr) != Result.Success)
            {
                throw new Exception("failed to create descriptor pool!");
            }
        }
    }

    void CreateDescriptorSets()
    {
        var layouts = new DescriptorSetLayout[_swapChainImages.Length];
        Array.Fill(layouts, _descriptorSetLayout);

        fixed (DescriptorSetLayout* layoutsPtr = layouts)
        {
            DescriptorSetAllocateInfo allocateInfo = new()
            {
                SType = StructureType.DescriptorSetAllocateInfo,
                DescriptorPool = _descriptorPool,
                DescriptorSetCount = (uint)_swapChainImages.Length,
                PSetLayouts = layoutsPtr,
            };

            _descriptorSets = new DescriptorSet[_swapChainImages.Length];
            fixed (DescriptorSet* descriptorSetsPtr = _descriptorSets)
            {
                if (_vk.AllocateDescriptorSets(_device, allocateInfo, descriptorSetsPtr) != Result.Success)
                {
                    throw new Exception("failed to allocate descriptor sets!");
                }
            }
        }


        for (var i = 0; i < _swapChainImages.Length; i++)
        {
            DescriptorBufferInfo bufferInfo = new()
            {
                Buffer = _uniformBuffers![i], Offset = 0, Range = (ulong)Unsafe.SizeOf<UniformBufferObject>(),
            };

            WriteDescriptorSet descriptorWrite = new()
            {
                SType = StructureType.WriteDescriptorSet,
                DstSet = _descriptorSets[i],
                DstBinding = 0,
                DstArrayElement = 0,
                DescriptorType = DescriptorType.UniformBuffer,
                DescriptorCount = 1,
                PBufferInfo = &bufferInfo,
            };

            _vk.UpdateDescriptorSets(_device, 1, descriptorWrite, 0, null);
        }
    }
}
