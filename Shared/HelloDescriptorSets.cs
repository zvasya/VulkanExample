using Silk.NET.Vulkan;

namespace Shared;

public unsafe class HelloDescriptorSets
{
    readonly DescriptorSet[] _descriptorSets;

    protected HelloDescriptorSets(DescriptorSet[] descriptorSets)
    {
        _descriptorSets = descriptorSets;
    }

    public DescriptorSet this[uint index]
    {
        get => _descriptorSets[index];
    }
}
