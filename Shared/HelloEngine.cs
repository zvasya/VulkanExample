using System.Runtime.InteropServices;
using Silk.NET.Core;
using Silk.NET.Core.Native;
using Silk.NET.Vulkan;
using Silk.NET.Vulkan.Extensions.EXT;
using Silk.NET.Vulkan.Extensions.KHR;

namespace Shared;

public unsafe partial class HelloEngine : IDisposable
{
    internal static Vk VK = null!;
    internal static KhrSurface VkSurface = null!;
    static readonly string[] _instanceExtensions = { KhrSurface.ExtensionName, ExtDebugUtils.ExtensionName };

    public const int MAX_FRAMES_IN_FLIGHT = 2;
    readonly List<Surface> _surfaces = new();

    static readonly string[][] _validationLayerNamesPriorityList =
    {
        new[] { "VK_LAYER_KHRONOS_validation" },
        new[] { "VK_LAYER_LUNARG_standard_validation" },
        new[] { "VK_LAYER_GOOGLE_threading", "VK_LAYER_LUNARG_parameter_validation", "VK_LAYER_LUNARG_object_tracker", "VK_LAYER_LUNARG_core_validation", "VK_LAYER_GOOGLE_unique_objects" },
    };

    internal readonly IPlatform Platform;

    DebugUtils? _debug;

    readonly Instance _instance;

    HelloEngine(IPlatform platform, Instance instance)
    {
        Platform = platform;
        _instance = instance;
    }


    public void Dispose()
    {
        foreach (var t in _surfaces)
        {
            t.Dispose();
        }

        _debug?.Dispose();
        DestroyInstance(null);
    }

    public static HelloEngine Create(IPlatform platform)
    {
        var instance = Init(platform, _instanceExtensions, _validationLayerNamesPriorityList, out var validationLayers);
        var engine = new HelloEngine(platform, instance);
        engine._debug = new DebugUtils(engine);
        return engine;
    }


    public static void CreateMetalSurface(HelloEngine engine, IntPtr pLayer, out SurfaceKHR surface)
    {
        var createInfo = new MetalSurfaceCreateInfoEXT { SType = StructureType.MetalSurfaceCreateInfoExt, PNext = null, Flags = 0, PLayer = (IntPtr*)pLayer };

        if (!VK.TryGetInstanceExtension<ExtMetalSurface>(engine._instance, out var extMetalSurface))
        {
            throw new NotSupportedException("extMetalSurface extension not found.");
        }

        extMetalSurface.CreateMetalSurface(engine._instance, &createInfo, null, out surface);
    }

    public static void CreateAndroidSurface(HelloEngine engine, IntPtr window, out SurfaceKHR surface)
    {
        var createInfo = new AndroidSurfaceCreateInfoKHR
        {
            SType = StructureType.AndroidSurfaceCreateInfoKhr, PNext = null, Flags = 0, Window = (IntPtr*)window,
        };

        if (!VK.TryGetInstanceExtension<KhrAndroidSurface>(engine._instance, out var khrAndroidSurface))
        {
            throw new NotSupportedException("khrAndroidSurface extension not found.");
        }

        Helpers.CheckErrors(khrAndroidSurface.CreateAndroidSurface(engine._instance, &createInfo, null, out surface));
    }

    public Surface CreateSurface(Func<SurfaceKHR> factory)
    {
        var surface = Surface.Create(this, factory);
        _surfaces.Add(surface);
        return surface;
    }

    static Instance Init(IPlatform platform, string[] instanceExtensions, string[][] validationLayerNamesPriorityList, out string[]? validationLayers)
    {
        VK = Vk.GetApi();

        validationLayers = null;
        if (platform.EnableValidationLayers)
        {
            validationLayers = GetOptimalValidationLayers(validationLayerNamesPriorityList);
            if (validationLayers is null)
            {
                throw new NotSupportedException("Validation layers requested, but not available!");
            }
        }

        var appInfo = new ApplicationInfo
        {
            SType = StructureType.ApplicationInfo,
            PApplicationName = (byte*)Marshal.StringToHGlobalAnsi("Hello Triangle"),
            ApplicationVersion = new Version32(1, 0, 0),
            PEngineName = (byte*)Marshal.StringToHGlobalAnsi("No Engine"),
            EngineVersion = new Version32(1, 0, 0),
            ApiVersion = Vk.Version12,
        };

        var createInfo = new InstanceCreateInfo { SType = StructureType.InstanceCreateInfo, PApplicationInfo = &appInfo };

        uint availableExtensionCount;
        VK.EnumerateInstanceExtensionProperties((string)null!, &availableExtensionCount, null);

        var availableExtensionProperties = new ExtensionProperties[availableExtensionCount];
        fixed (ExtensionProperties* instanceExtensionPtr = availableExtensionProperties)
        {
            VK.EnumerateInstanceExtensionProperties((string)null!, &availableExtensionCount, instanceExtensionPtr);
        }

        var availableExtension = availableExtensionProperties.Select(property => SilkMarshal.PtrToString(new IntPtr(property.ExtensionName), NativeStringEncoding.LPTStr)).ToArray();

        var extensions = platform.InstanceExtensions;
        var requestedExtension = extensions.Concat(instanceExtensions).Intersect(availableExtension).ToArray();

        createInfo.EnabledExtensionCount = (uint)requestedExtension.Length;
        createInfo.PpEnabledExtensionNames = (byte**)SilkMarshal.StringArrayToPtr(requestedExtension);

        if (validationLayers != null)
        {
            createInfo.EnabledLayerCount = (uint)validationLayers!.Length;
            createInfo.PpEnabledLayerNames = (byte**)SilkMarshal.StringArrayToPtr(validationLayers!);
        }
        else
        {
            createInfo.EnabledLayerCount = 0;
            createInfo.PNext = null;
        }

        if (availableExtension.Contains("VK_KHR_portability_enumeration"))
        {
            createInfo.Flags = InstanceCreateFlags.EnumeratePortabilityBitKhr;
        }

        Instance instance;
        {
            if (VK.CreateInstance(&createInfo, null, &instance) != Result.Success)
            {
                throw new Exception("Failed to create instance!");
            }
        }

        VK.CurrentInstance = instance;

        if (!VK.TryGetInstanceExtension(instance, out VkSurface))
        {
            throw new NotSupportedException("KHR_surface extension not found.");
        }

        Marshal.FreeHGlobal((nint)appInfo.PApplicationName);
        Marshal.FreeHGlobal((nint)appInfo.PEngineName);

        if (platform.EnableValidationLayers)
        {
            SilkMarshal.Free((nint)createInfo.PpEnabledLayerNames);
        }

        return instance;
    }

    static string[]? GetOptimalValidationLayers(string[][] validationLayerNamesPriorityList)
    {
        var layerCount = 0u;
        VK.EnumerateInstanceLayerProperties(&layerCount, (LayerProperties*)0);

        var availableLayers = new LayerProperties[layerCount];
        fixed (LayerProperties* availableLayersPtr = availableLayers)
        {
            VK.EnumerateInstanceLayerProperties(&layerCount, availableLayersPtr);
        }

        var availableLayerNames = availableLayers.Select(availableLayer => Marshal.PtrToStringAnsi((nint)availableLayer.LayerName)).ToArray();
        foreach (var validationLayerNameSet in validationLayerNamesPriorityList)
        {
            if (validationLayerNameSet.All(validationLayerName => availableLayerNames.Contains(validationLayerName)))
            {
                return validationLayerNameSet;
            }
        }

        return null;
    }
}
