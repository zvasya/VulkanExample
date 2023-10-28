using Silk.NET.Vulkan;

namespace Shared;

public unsafe class HelloFrameBuffer : IDisposable
{
    readonly Framebuffer _framebuffer;

    readonly LogicalDevice _device;

    public Framebuffer Framebuffer => _framebuffer;

    HelloFrameBuffer(LogicalDevice device, Framebuffer framebuffer)
    {
        _device = device;
        _framebuffer = framebuffer;
    }

    public static HelloFrameBuffer Create(LogicalDevice device, HelloImageView[] images, RenderPass renderPass, uint width, uint height)
    {
        var framebuffer = CreateFramebuffer(device, images, renderPass, width, height);
        return new HelloFrameBuffer(device, framebuffer);
    }

    static Framebuffer CreateFramebuffer(LogicalDevice device, HelloImageView[] images, RenderPass renderPass, uint width, uint height)
    {
        var imageView = stackalloc ImageView[images.Length];
        for (var i = 0; i < images.Length; i++)
        {
            imageView[i] = images[i].ImageView;
        }

        var framebufferInfo = new FramebufferCreateInfo
        {
            SType = StructureType.FramebufferCreateInfo,
            RenderPass = renderPass,
            AttachmentCount = (uint)images.Length,
            PAttachments = imageView,
            Width = width,
            Height = height,
            Layers = 1,
        };

        device.CreateFramebuffer(&framebufferInfo, null, out var framebuffer);
        return framebuffer;
    }

    public void Dispose()
    {
        _device.DestroyFramebuffer(_framebuffer, null);
    }
}
