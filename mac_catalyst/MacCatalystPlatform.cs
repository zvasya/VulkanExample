// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
using Silk.NET.Vulkan.Extensions.EXT;

namespace mac_catalyst;

public class MacCatalystPlatform : Shared.IPlatform
{
    public bool EnableValidationLayers => false;
    public string[] RequiredExtensions => new[] {ExtMetalSurface.ExtensionName};

    public byte[] GetVertShader() => File.ReadAllBytes("Contents/Resources/Shaders/vert.spv");

    public byte[] GetFragShader() => File.ReadAllBytes("Contents/Resources/Shaders/frag.spv");
}
