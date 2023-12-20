using System.Runtime.InteropServices;
using Silk.NET.Vulkan;

namespace Shared;

public unsafe class HelloPipeline : IDisposable
{
    readonly LogicalDevice _device;
    readonly Pipeline _graphicsPipeline;

    readonly HelloPipelineLayout _pipelineLayout;
    readonly HelloDescriptorSetLayout _descriptorSetLayout;
    
    public Pipeline Pipeline => _graphicsPipeline;
    public PipelineLayout PipelineLayout => _pipelineLayout.PipelineLayout;
    public HelloDescriptorSetLayout DescriptorSetLayout => _descriptorSetLayout;
    
    HelloPipeline(LogicalDevice device, HelloDescriptorSetLayout descriptorSetLayout, HelloPipelineLayout pipelineLayout, Pipeline graphicsPipeline)
    {
        _device = device;
        _descriptorSetLayout = descriptorSetLayout;
        _pipelineLayout = pipelineLayout;
        _graphicsPipeline = graphicsPipeline;
    }

    public static HelloPipeline Create(LogicalDevice device, byte[] vertShaderCode, byte[] fragShaderCode, VertexInputBindingDescription bindingDescription, VertexInputAttributeDescription[] attributeDescriptions, HelloDescriptorSetLayout descriptorSetLayout, HelloRenderPass renderPass)
    {
        var pipelineLayout = HelloPipelineLayout.Create(device, descriptorSetLayout);
        
        var pipeline = CreatePipeline(device, vertShaderCode, fragShaderCode, bindingDescription, attributeDescriptions, renderPass, pipelineLayout);
        
        return new HelloPipeline(device, descriptorSetLayout, pipelineLayout, pipeline);
    }
    
    static ShaderModule CreateShaderModule(LogicalDevice device, byte[] code)
    {
        var createInfo = new ShaderModuleCreateInfo
        {
            SType = StructureType.ShaderModuleCreateInfo,
            CodeSize = (UIntPtr)code.Length,
        };

        fixed (byte* sourcePointer = code)
        {
            createInfo.PCode = (uint*)sourcePointer;
        }

        ShaderModule shaderModule;
        Helpers.CheckErrors(device.CreateShaderModule(&createInfo, null, &shaderModule));

        return shaderModule;
    }

    static Pipeline CreatePipeline(LogicalDevice device, byte[] vertShaderCode, byte[] fragShaderCode, VertexInputBindingDescription bindingDescription, VertexInputAttributeDescription[] attributeDescriptions, HelloRenderPass renderPass, HelloPipelineLayout pipelineLayout)
    {
        Pipeline graphicsPipeline;
        var vertShaderModule = CreateShaderModule(device, vertShaderCode);
        var fragShaderModule = CreateShaderModule(device, fragShaderCode);

        var vertShaderStageInfo = new PipelineShaderStageCreateInfo
        {
            SType = StructureType.PipelineShaderStageCreateInfo,
            Stage = ShaderStageFlags.VertexBit,
            Module = vertShaderModule,
            PName = (byte*)Marshal.StringToHGlobalAnsi("main"),
        };

        var fragShaderStageInfo = new PipelineShaderStageCreateInfo
        {
            SType = StructureType.PipelineShaderStageCreateInfo,
            Stage = ShaderStageFlags.FragmentBit,
            Module = fragShaderModule,
            PName = (byte*)Marshal.StringToHGlobalAnsi("main"),
        };

        var shaderStages = stackalloc PipelineShaderStageCreateInfo[] { vertShaderStageInfo, fragShaderStageInfo };
        
        fixed (VertexInputAttributeDescription* attributeDescriptionsPtr = attributeDescriptions)
        {
            // Vertex Input
            var vertexInputInfo = new PipelineVertexInputStateCreateInfo
            {
                SType = StructureType.PipelineVertexInputStateCreateInfo,
                VertexBindingDescriptionCount = 1,
                PVertexBindingDescriptions = &bindingDescription,
                VertexAttributeDescriptionCount = (uint)attributeDescriptions.Length,
                PVertexAttributeDescriptions = attributeDescriptionsPtr,
            };

            // Input assembly
            var inputAssembly = new PipelineInputAssemblyStateCreateInfo
            {
                SType = StructureType.PipelineInputAssemblyStateCreateInfo,
                Topology = PrimitiveTopology.TriangleList,
                PrimitiveRestartEnable = false
            };

            var viewportState = new PipelineViewportStateCreateInfo
            {
                SType = StructureType.PipelineViewportStateCreateInfo,
                ViewportCount = 1,
                ScissorCount = 1,
            };

            // Rasterizer
            var rasterizer = new PipelineRasterizationStateCreateInfo
            {
                SType = StructureType.PipelineRasterizationStateCreateInfo,
                DepthClampEnable = false,
                RasterizerDiscardEnable = false,
                PolygonMode = PolygonMode.Fill,
                LineWidth = 1.0f,
                CullMode = CullModeFlags.BackBit,
                // FrontFace = FrontFace.Clockwise,
                FrontFace = FrontFace.CounterClockwise,
                DepthBiasEnable = false,
                DepthBiasConstantFactor = 0.0f, // Optional
                DepthBiasClamp = 0.0f, // Optional
                DepthBiasSlopeFactor = 0.0f, // Optional
            };

            var multisampling = new PipelineMultisampleStateCreateInfo
            {
                SType = StructureType.PipelineMultisampleStateCreateInfo,
                SampleShadingEnable = false,
                RasterizationSamples = SampleCountFlags.Count1Bit,
                MinSampleShading = 1.0f, // Optional
                PSampleMask = null, // Optional
                AlphaToCoverageEnable = false, // Optional
                AlphaToOneEnable = false, // Optional
            };

            // Depth and Stencil testing
            var depthStencil = new PipelineDepthStencilStateCreateInfo()
            {
                SType = StructureType.PipelineDepthStencilStateCreateInfo,
                DepthTestEnable = true,
                DepthWriteEnable = true,
                DepthCompareOp = CompareOp.Less,
                DepthBoundsTestEnable = false,
                StencilTestEnable = false,
            };

            // Color blending
            var colorBlendAttachment = new PipelineColorBlendAttachmentState
            {
                ColorWriteMask = ColorComponentFlags.RBit |
                                 ColorComponentFlags.GBit |
                                 ColorComponentFlags.BBit |
                                 ColorComponentFlags.ABit,
                BlendEnable = false,
                SrcColorBlendFactor = BlendFactor.One, // Optional
                DstColorBlendFactor = BlendFactor.Zero, // Optional
                ColorBlendOp = BlendOp.Add, // Optional
                SrcAlphaBlendFactor = BlendFactor.One, // Optional
                DstAlphaBlendFactor = BlendFactor.Zero, // Optional
                AlphaBlendOp = BlendOp.Add, // Optional
            };

            var colorBlending = new PipelineColorBlendStateCreateInfo
            {
                SType = StructureType.PipelineColorBlendStateCreateInfo,
                LogicOpEnable = false,
                LogicOp = LogicOp.Copy, // Optional
                AttachmentCount = 1,
                PAttachments = &colorBlendAttachment,
            };

            colorBlending.BlendConstants[0] = 0.0f; // Optional
            colorBlending.BlendConstants[1] = 0.0f; // Optional
            colorBlending.BlendConstants[2] = 0.0f; // Optional
            colorBlending.BlendConstants[3] = 0.0f; // Optional

            var dynamicStates = stackalloc DynamicState[] {
                DynamicState.Viewport,
                DynamicState.Scissor,
            };

            var dynamicState = new PipelineDynamicStateCreateInfo
            {
                SType = StructureType.PipelineDynamicStateCreateInfo,
                DynamicStateCount = 2,
                PDynamicStates = dynamicStates,
            };
            
            var pipelineInfo = new GraphicsPipelineCreateInfo
            {
                SType = StructureType.GraphicsPipelineCreateInfo,
                StageCount = 2,
                PStages = shaderStages,
                PVertexInputState = &vertexInputInfo,
                PInputAssemblyState = &inputAssembly,
                PViewportState = &viewportState,
                PRasterizationState = &rasterizer,
                PMultisampleState = &multisampling,
                PDepthStencilState = &depthStencil,
                PColorBlendState = &colorBlending,
                PDynamicState = &dynamicState, // Optional
                Layout = pipelineLayout.PipelineLayout,
                RenderPass = renderPass.RenderPass,
                Subpass = 0,
                BasePipelineHandle = default, // Optional
                BasePipelineIndex = -1, // Optional
            };

            Helpers.CheckErrors(device.CreateGraphicsPipelines(default, 1, &pipelineInfo, null, &graphicsPipeline));
        }

        device.DestroyShaderModule(fragShaderModule, null);
        device.DestroyShaderModule(vertShaderModule, null);

        return graphicsPipeline;
    }
    
    public void Dispose()
    {
        _pipelineLayout.Dispose();
        _descriptorSetLayout.Dispose();
        
        _device.DestroyPipeline(_graphicsPipeline, null);
    }
}
