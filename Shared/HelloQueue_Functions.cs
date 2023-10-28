using Silk.NET.Vulkan;
using static Shared.HelloEngine;

namespace Shared;

public partial class HelloQueue
{
    public unsafe Result QueueSubmit2(
        uint submitCount,
        SubmitInfo2* pSubmits,
        Fence fence)
    {
        return VK.QueueSubmit2(
            _queue,
            submitCount,
            pSubmits,
            fence);
    }

    public Result QueueSubmit2(
        uint submitCount,
        in SubmitInfo2 pSubmits,
        Fence fence)
    {
        return VK.QueueSubmit2(
            _queue,
            submitCount,
            in pSubmits,
            fence);
    }


    public unsafe Result QueueBindSparse(
        uint bindInfoCount,
        BindSparseInfo* pBindInfo,
        Fence fence)
    {
        return VK.QueueBindSparse(
            _queue,
            bindInfoCount,
            pBindInfo,
            fence);
    }

    public Result QueueBindSparse(
        uint bindInfoCount,
        in BindSparseInfo pBindInfo,
        Fence fence)
    {
        return VK.QueueBindSparse(
            _queue,
            bindInfoCount,
            in pBindInfo,
            fence);
    }

    public unsafe Result QueueSubmit(
        uint submitCount,
        SubmitInfo* pSubmits,
        Fence fence)
    {
        return VK.QueueSubmit(
            _queue,
            submitCount,
            pSubmits,
            fence);
    }

    public Result QueueSubmit(
        uint submitCount,
        in SubmitInfo pSubmits,
        Fence fence)
    {
        return VK.QueueSubmit(
            _queue,
            submitCount,
            in pSubmits,
            fence);
    }


    public Result QueueWaitIdle()
    {
        return VK.QueueWaitIdle(
            _queue);
    }
}
