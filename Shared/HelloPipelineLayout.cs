using Silk.NET.Vulkan;

namespace Shared;

public unsafe class HelloPipelineLayout : IDisposable
{
    readonly PipelineLayout _pipelineLayout;
    public PipelineLayout PipelineLayout => _pipelineLayout;

    readonly LogicalDevice _device;
    
    HelloPipelineLayout(LogicalDevice device, PipelineLayout pipelineLayout)
    {
        _device = device;
        _pipelineLayout = pipelineLayout;
    }

    public static HelloPipelineLayout Create(LogicalDevice device, HelloDescriptorSetLayout descriptorSetLayout)
    {
        var pipelineLayout = CreatePipelineLayout(device, descriptorSetLayout);
        return new HelloPipelineLayout(device, pipelineLayout);
    }

    static PipelineLayout CreatePipelineLayout(LogicalDevice device, HelloDescriptorSetLayout descriptorSetLayout)
    {
        var pipelineLayoutInfo = descriptorSetLayout.GetPipelineLayoutCreateInfo();


        PipelineLayout pipelineLayout;
        Helpers.CheckErrors(device.CreatePipelineLayout(&pipelineLayoutInfo, null, &pipelineLayout));
        return pipelineLayout;
    }

    public void Dispose()
    {
        _device.DestroyPipelineLayout(_pipelineLayout, null);
    }

}
