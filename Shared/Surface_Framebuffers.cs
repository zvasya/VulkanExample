using Silk.NET.Vulkan;

namespace Shared;

public unsafe partial class Surface
{
    Framebuffer[] _swapChainFramebuffers;

    void CreateFramebuffers()
    {
        this._swapChainFramebuffers = new Framebuffer[this._swapChainImageViews.Length];

        for (var i = 0; i < this._swapChainImageViews.Length; i++)
        {

            var framebufferInfo = new FramebufferCreateInfo();
            framebufferInfo.SType = StructureType.FramebufferCreateInfo;
            framebufferInfo.RenderPass = _renderPass;
            framebufferInfo.AttachmentCount = 1;

            fixed (ImageView* attachments = &_swapChainImageViews[i])
            {
                framebufferInfo.PAttachments = attachments;
            }

            framebufferInfo.Width = _swapChainExtent.Width;
            framebufferInfo.Height = _swapChainExtent.Height;
            framebufferInfo.Layers = 1;

            fixed (Framebuffer* swapChainFramebufferPtr = &this._swapChainFramebuffers[i])
            {
                Helpers.CheckErrors(_vk.CreateFramebuffer(_device, &framebufferInfo, null, swapChainFramebufferPtr));
            }
        }
    }
}
