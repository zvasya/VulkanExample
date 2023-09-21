using System.Runtime.InteropServices;
using Silk.NET.Core;
using Silk.NET.Core.Native;
using Silk.NET.Vulkan;
using Silk.NET.Vulkan.Extensions.EXT;
using Silk.NET.Vulkan.Extensions.KHR;

namespace Shared;

public unsafe partial class HelloEngine : IDisposable
{
    internal readonly IPlatform Platform;

    internal Instance _instance;
    internal Vk? _vk;
    internal KhrSurface? _vkSurface;

    DebugUtils? _debug;

    string[]? _validationLayers;
    readonly string[] _instanceExtensions = { KhrSurface.ExtensionName, ExtDebugUtils.ExtensionName };

    readonly string[][] _validationLayerNamesPriorityList =
    {
        new[] { "VK_LAYER_KHRONOS_validation" },
        new[] { "VK_LAYER_LUNARG_standard_validation" },
        new[] { "VK_LAYER_GOOGLE_threading", "VK_LAYER_LUNARG_parameter_validation", "VK_LAYER_LUNARG_object_tracker", "VK_LAYER_LUNARG_core_validation", "VK_LAYER_GOOGLE_unique_objects" },
    };

    HelloEngine(IPlatform platform)
    {
        Platform = platform;
    }

    public static HelloEngine Create(IPlatform platform)
    {
        var engine = new HelloEngine(platform);
        engine.Init();
        engine._debug = new DebugUtils(engine);
        return engine;
    }


    public void CreateMetalSurface(IntPtr pLayer, out SurfaceKHR surface)
    {
        var createInfo = new MetalSurfaceCreateInfoEXT { SType = StructureType.MetalSurfaceCreateInfoExt, PNext = null, Flags = 0, PLayer = (IntPtr*)pLayer };

        if (!_vk!.TryGetInstanceExtension<ExtMetalSurface>(_instance, out var extMetalSurface))
        {
            throw new NotSupportedException("extMetalSurface extension not found.");
        }

        extMetalSurface.CreateMetalSurface(_instance, &createInfo, null, out surface);
    }

    public void CreateAndroidSurface(IntPtr window, out SurfaceKHR surface)
    {
        var createInfo = new AndroidSurfaceCreateInfoKHR
        {
            SType = StructureType.AndroidSurfaceCreateInfoKhr,
            PNext = null,
            Flags = 0,
            Window = (IntPtr*)window,
        };

        if (!_vk!.TryGetInstanceExtension<KhrAndroidSurface>(_instance, out var khrAndroidSurface))
        {
            throw new NotSupportedException("khrAndroidSurface extension not found.");
        }

        Helpers.CheckErrors(khrAndroidSurface.CreateAndroidSurface(_instance, &createInfo, null, out surface));
    }

    readonly List<Surface> _surfaces = new();

    public Surface CreateSurface(Func<SurfaceKHR> factory)
    {
        var surface = Surface.Create(this, factory);
        _surfaces.Add(surface);
        return surface;
    }

    void Init()
    {
        _vk = Vk.GetApi();

        if (Platform.EnableValidationLayers)
        {
            _validationLayers = GetOptimalValidationLayers();
            if (_validationLayers is null)
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
        _vk.EnumerateInstanceExtensionProperties((string)null!, &availableExtensionCount, null);

        var availableExtensionProperties = new ExtensionProperties[availableExtensionCount];
        fixed (ExtensionProperties* instanceExtensionPtr = availableExtensionProperties)
            _vk.EnumerateInstanceExtensionProperties((string)null!, &availableExtensionCount, instanceExtensionPtr);

        var availableExtension = availableExtensionProperties.Select(property => SilkMarshal.PtrToString(new IntPtr(property.ExtensionName), NativeStringEncoding.LPTStr)).ToArray();

        var extensions = Platform.InstanceExtensions;
        var requestedExtension = extensions.Concat(_instanceExtensions).Intersect(availableExtension).ToArray();

        createInfo.EnabledExtensionCount = (uint)requestedExtension.Length;
        createInfo.PpEnabledExtensionNames = (byte**)SilkMarshal.StringArrayToPtr(requestedExtension);

        if (Platform.EnableValidationLayers)
        {
            createInfo.EnabledLayerCount = (uint)_validationLayers!.Length;
            createInfo.PpEnabledLayerNames = (byte**)SilkMarshal.StringArrayToPtr(_validationLayers!);
        }
        else
        {
            createInfo.EnabledLayerCount = 0;
            createInfo.PNext = null;
        }

        if (availableExtension.Contains("VK_KHR_portability_enumeration"))
            createInfo.Flags = InstanceCreateFlags.EnumeratePortabilityBitKhr;

        fixed (Instance* instance = &_instance)
        {
            if (_vk.CreateInstance(&createInfo, null, instance) != Result.Success)
            {
                throw new Exception("Failed to create instance!");
            }
        }

        _vk.CurrentInstance = _instance;

        if (!_vk.TryGetInstanceExtension(_instance, out _vkSurface))
        {
            throw new NotSupportedException("KHR_surface extension not found.");
        }

        Marshal.FreeHGlobal((nint)appInfo.PApplicationName);
        Marshal.FreeHGlobal((nint)appInfo.PEngineName);

        if (Platform.EnableValidationLayers)
        {
            SilkMarshal.Free((nint)createInfo.PpEnabledLayerNames);
        }
    }

    string[]? GetOptimalValidationLayers()
    {
        var layerCount = 0u;
        _vk!.EnumerateInstanceLayerProperties(&layerCount, (LayerProperties*)0);

        var availableLayers = new LayerProperties[layerCount];
        fixed (LayerProperties* availableLayersPtr = availableLayers)
        {
            _vk.EnumerateInstanceLayerProperties(&layerCount, availableLayersPtr);
        }

        var availableLayerNames = availableLayers.Select(availableLayer => Marshal.PtrToStringAnsi((nint)availableLayer.LayerName)).ToArray();
        foreach (var validationLayerNameSet in _validationLayerNamesPriorityList)
        {
            if (validationLayerNameSet.All(validationLayerName => availableLayerNames.Contains(validationLayerName)))
            {
                return validationLayerNameSet;
            }
        }

        return null;
    }

    public void DrawFrame()
    {
        foreach (var surface in _surfaces)
        {
            surface.DrawFrame();
        }
    }


    public void Dispose()
    {
        foreach (var t in _surfaces)
            t.Dispose();
        
        _debug?.Dispose();
        _vk!.DestroyInstance(_instance, null);
    }
}
