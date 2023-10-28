using Silk.NET.Vulkan;
using Silk.NET.Vulkan.Extensions.KHR;

namespace Shared;

public partial class HelloQueue
{
    readonly KhrSwapchain _khrSwapchain;
    readonly Queue _queue;

    HelloQueue(Queue queue, KhrSwapchain khrSwapchain)
    {
        _queue = queue;
        _khrSwapchain = khrSwapchain;
    }

    public static HelloQueue Create(Queue queue, KhrSwapchain khrSwapchain)
    {
        return new HelloQueue(queue, khrSwapchain);
    }
}
