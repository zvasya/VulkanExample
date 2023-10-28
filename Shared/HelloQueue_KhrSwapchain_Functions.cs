using Silk.NET.Vulkan;

// ReSharper disable UnusedMember.Global

namespace Shared;

public partial class HelloQueue
{
    public unsafe Result QueuePresent(PresentInfoKHR* pPresentInfo)
    {
        return _khrSwapchain.QueuePresent(_queue, pPresentInfo);
    }

    public Result QueuePresent(in PresentInfoKHR pPresentInfo)
    {
        return _khrSwapchain.QueuePresent(_queue, in pPresentInfo);
    }
}
