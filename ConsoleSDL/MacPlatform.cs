// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using Silk.NET.Vulkan.Extensions.EXT;

namespace ConsoleSDL;

public class MacPlatform : Shared.IPlatform
{
    public bool EnableValidationLayers => false;
    public string[] InstanceExtensions => new[] { ExtMetalSurface.ExtensionName, "VK_KHR_portability_enumeration" };
    public string[] DeviceExtensions => new[] { "VK_KHR_portability_subset" };
}
