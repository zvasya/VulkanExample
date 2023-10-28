using Silk.NET.Vulkan;

namespace Shared;

public unsafe class HelloRenderPass : IDisposable
{
    readonly RenderPass _renderPass;
    public RenderPass RenderPass => _renderPass;

    readonly LogicalDevice _device;
    
    HelloRenderPass(LogicalDevice device, RenderPass renderPass)
    {
        _device = device;
        _renderPass = renderPass;
    }

    public static HelloRenderPass Create(LogicalDevice device, Format imageFormat, Format depthFormat)
    {
        var renderPass = CreateRenderPass(device, imageFormat, depthFormat);
        return new HelloRenderPass(device, renderPass);
    }

    static RenderPass CreateRenderPass(LogicalDevice device, Format imageFormat, Format depthFormat)
    {
        // Attachment description
        var colorAttachment = new AttachmentDescription
        {
            Format = imageFormat,
            Samples = SampleCountFlags.Count1Bit,
            LoadOp = AttachmentLoadOp.Load,
            StoreOp = AttachmentStoreOp.Store,
            StencilLoadOp = AttachmentLoadOp.DontCare,
            StencilStoreOp = AttachmentStoreOp.DontCare,
            InitialLayout = ImageLayout.Undefined,
            FinalLayout = ImageLayout.PresentSrcKhr,
        };

        var depthAttachment = new AttachmentDescription
        {
            Format = depthFormat,
            Samples = SampleCountFlags.Count1Bit,
            LoadOp = AttachmentLoadOp.Load,
            StoreOp = AttachmentStoreOp.Store,
            StencilLoadOp = AttachmentLoadOp.DontCare,
            StencilStoreOp = AttachmentStoreOp.DontCare,
            InitialLayout = ImageLayout.Undefined,
            FinalLayout = ImageLayout.DepthStencilAttachmentOptimal,
        };
        
        // Subpasses and attachment references
        var colorAttachmentRef = new AttachmentReference
        {
            Attachment = 0,
            Layout = ImageLayout.AttachmentOptimal,
        };
        
        var depthAttachmentRef = new AttachmentReference
        {
            Attachment = 1,
            Layout = ImageLayout.DepthStencilAttachmentOptimal,
        };

        var subpass = new SubpassDescription
        {
            PipelineBindPoint = PipelineBindPoint.Graphics,
            ColorAttachmentCount = 1,
            PColorAttachments = &colorAttachmentRef,
            PDepthStencilAttachment = &depthAttachmentRef,
        };

        // Render  pass            
        var dependency = new SubpassDependency
        {
            SrcSubpass = Vk.SubpassExternal,
            DstSubpass = 0,
            SrcStageMask = PipelineStageFlags.ColorAttachmentOutputBit | PipelineStageFlags.EarlyFragmentTestsBit,
            SrcAccessMask = 0,
            DstStageMask = PipelineStageFlags.ColorAttachmentOutputBit | PipelineStageFlags.EarlyFragmentTestsBit,
            DstAccessMask = AccessFlags.ColorAttachmentWriteBit | AccessFlags.DepthStencilAttachmentWriteBit,
        };
        
        var attachments = stackalloc AttachmentDescription[] { colorAttachment, depthAttachment };

        var renderPassInfo = new RenderPassCreateInfo
        {
            SType = StructureType.RenderPassCreateInfo,
            AttachmentCount = 2,
            PAttachments = attachments,
            SubpassCount = 1,
            PSubpasses = &subpass,
            DependencyCount = 1,
            PDependencies = &dependency,
        };

        RenderPass renderPass;
        
        Helpers.CheckErrors(device.CreateRenderPass(&renderPassInfo, null, &renderPass));

        return renderPass;
    }

    public void Dispose()
    {
        _device.DestroyRenderPass(_renderPass, null);
    }
}
