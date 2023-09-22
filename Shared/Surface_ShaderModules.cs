using System.Runtime.InteropServices;
using Silk.NET.Vulkan;

namespace Shared;

public unsafe partial class Surface
{
    PipelineLayout _pipelineLayout;
    Pipeline _graphicsPipeline;

    ShaderModule CreateShaderModule(byte[] code)
    {
        var createInfo = new ShaderModuleCreateInfo();
        createInfo.SType = StructureType.ShaderModuleCreateInfo;
        createInfo.CodeSize = (UIntPtr)code.Length;

        fixed (byte* sourcePointer = code)
        {
            createInfo.PCode = (uint*)sourcePointer;
        }

        ShaderModule shaderModule;
        Helpers.CheckErrors(_vk.CreateShaderModule(_device, &createInfo, null, &shaderModule));

        return shaderModule;
    }

    void CreateGraphicsPipeline()
    {
        var vertShaderCode = _engine.Platform.GetVertShader();
        var fragShaderCode = _engine.Platform.GetFragShader();

        var vertShaderModule = this.CreateShaderModule(vertShaderCode);
        var fragShaderModule = this.CreateShaderModule(fragShaderCode);

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

        var bindingDescription = Vertex.GetBindingDescription();
        var attributeDescriptions = Vertex.GetAttributeDescriptions();
        
        fixed (VertexInputAttributeDescription* attributeDescriptionsPtr = attributeDescriptions)
        fixed (DescriptorSetLayout* descriptorSetLayoutPtr = &_descriptorSetLayout)
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
            var inputAssembly = new PipelineInputAssemblyStateCreateInfo { SType = StructureType.PipelineInputAssemblyStateCreateInfo, Topology = PrimitiveTopology.TriangleList, PrimitiveRestartEnable = false, };

            // Viewports and scissors
            var viewport = new Viewport
            {
                X = 0.0f,
                Y = 0.0f,
                Width = _swapChainExtent.Width,
                Height = _swapChainExtent.Height,
                MinDepth = 0.0f,
                MaxDepth = 1.0f,
            };

            var scissor = new Rect2D { Offset = new Offset2D(0, 0), Extent = _swapChainExtent, };

            var viewportState = new PipelineViewportStateCreateInfo
            {
                SType = StructureType.PipelineViewportStateCreateInfo,
                ViewportCount = 1,
                PViewports = &viewport,
                ScissorCount = 1,
                PScissors = &scissor,
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
                FrontFace = FrontFace.Clockwise,
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
            //VkPipelineDepthStencilStateCreateInfo

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

            var pipelineLayoutInfo = new PipelineLayoutCreateInfo
            {
                SType = StructureType.PipelineLayoutCreateInfo,
                SetLayoutCount = 1,
                PSetLayouts = descriptorSetLayoutPtr,
                // PushConstantRangeCount = 0, // Optional
                // PPushConstantRanges = null, // Optional
            };

            fixed (PipelineLayout* pipelineLayoutPtr = &_pipelineLayout)
            {
                Helpers.CheckErrors(_vk.CreatePipelineLayout(_device, &pipelineLayoutInfo, null, pipelineLayoutPtr));
            }

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
                PDepthStencilState = null, // Optional
                PColorBlendState = &colorBlending,
                PDynamicState = null, // Optional
                Layout = this._pipelineLayout,
                RenderPass = this._renderPass,
                Subpass = 0,
                BasePipelineHandle = default, // Optional
                BasePipelineIndex = -1, // Optional
            };

            fixed (Pipeline* graphicsPipelinePtr = &this._graphicsPipeline)
            {
                Helpers.CheckErrors(_vk.CreateGraphicsPipelines(this._device, default, 1, &pipelineInfo, null, graphicsPipelinePtr));
            }
        }

        _vk.DestroyShaderModule(_device, fragShaderModule, null);
        _vk.DestroyShaderModule(_device, vertShaderModule, null);
    }
}
