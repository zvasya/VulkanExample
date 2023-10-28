using Silk.NET.Vulkan;

namespace Shared;

public unsafe class HelloFence : IDisposable
{
    readonly LogicalDevice _device;
    readonly Fence _fence;
    public Fence Fence => _fence;

    HelloFence(LogicalDevice device, Fence fence)
    {
        _device = device;
        _fence = fence;
    }

    public static HelloFence Create(LogicalDevice device)
    {
        var fence = CreateFence(device);
        var instance = new HelloFence(device, fence);
        return instance;
    }

    static Fence CreateFence(LogicalDevice device)
    {
        var fenceInfo = new FenceCreateInfo { SType = StructureType.FenceCreateInfo, Flags = FenceCreateFlags.SignaledBit, };

        Fence fence;
        Helpers.CheckErrors(device.CreateFence(&fenceInfo, null, &fence));
        return fence;
    }

    public void WaitForFences(bool waitAll, ulong timeout)
    {
        fixed (Fence* fencePtr = &_fence)
        {
            _device.WaitForFences(1, fencePtr, waitAll, timeout);
        }
    }

    public void ResetFences()
    {
        fixed (Fence* fencePtr = &_fence)
        {
            _device.ResetFences(1, fencePtr);
        }
    }

    public void Dispose()
    {
        _device.DestroyFence(_fence, null);
    }
}
