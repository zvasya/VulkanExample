// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using Silk.NET.Vulkan.Extensions.EXT;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace mac;

public class MacPlatform : Shared.IPlatform
{
    public bool EnableValidationLayers => false;
    public string[] InstanceExtensions => new[] { ExtMetalSurface.ExtensionName, "VK_KHR_portability_enumeration" };
    public string[] DeviceExtensions => new[] { "VK_KHR_portability_subset" };

    public byte[] GetVertShader() => File.ReadAllBytes("Contents/Resources/Shaders/vert.spv");

    public byte[] GetFragShader() => File.ReadAllBytes("Contents/Resources/Shaders/frag.spv");

    public Image<Rgba32> GetImage()
    {
        return SixLabors.ImageSharp.Image.Load<SixLabors.ImageSharp.PixelFormats.Rgba32>("Contents/Resources/Textures/texture.jpg");
    }
}
