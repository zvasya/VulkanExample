using Silk.NET.Vulkan;

namespace Shared;

public unsafe partial class Surface
{
    HelloCommandBufferArray _commandBuffers;

    void CreateCommandBuffers()
    {
        _commandBuffers = _device.CommandPool.CreateCommandBuffers(HelloEngine.MAX_FRAMES_IN_FLIGHT);
    }
}
