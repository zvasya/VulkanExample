using System.Runtime.InteropServices;
using Silk.NET.Vulkan;
using Silk.NET.Vulkan.Extensions.EXT;

namespace Shared;

public unsafe partial class HelloEngine
{
    internal class DebugUtils : IDisposable
    {
        readonly ExtDebugUtils? _debugUtils;
        readonly HelloEngine _helloEngine;
        DebugUtilsMessengerEXT _debugMessenger;

        public DebugUtils(HelloEngine helloEngine)
        {
            _helloEngine = helloEngine;

            if (!_helloEngine.Platform.EnableValidationLayers)
            {
                return;
            }

            if (!VK.TryGetInstanceExtension(_helloEngine._instance, out _debugUtils))
            {
                return;
            }

            var createInfo = new DebugUtilsMessengerCreateInfoEXT();
            PopulateDebugMessengerCreateInfo(ref createInfo);

            fixed (DebugUtilsMessengerEXT* debugMessenger = &_debugMessenger)
            {
                if (_debugUtils!.CreateDebugUtilsMessenger
                        (_helloEngine._instance, &createInfo, null, debugMessenger) != Result.Success)
                {
                    throw new Exception("Failed to create debug messenger.");
                }
            }
        }

        public void Dispose()
        {
            _debugUtils?.DestroyDebugUtilsMessenger(_helloEngine._instance, _debugMessenger, null);
        }

        void PopulateDebugMessengerCreateInfo(ref DebugUtilsMessengerCreateInfoEXT createInfo)
        {
            createInfo.SType = StructureType.DebugUtilsMessengerCreateInfoExt;
            createInfo.MessageSeverity = DebugUtilsMessageSeverityFlagsEXT.VerboseBitExt |
                                         DebugUtilsMessageSeverityFlagsEXT.WarningBitExt |
                                         DebugUtilsMessageSeverityFlagsEXT.ErrorBitExt;
            createInfo.MessageType = DebugUtilsMessageTypeFlagsEXT.GeneralBitExt |
                                     DebugUtilsMessageTypeFlagsEXT.PerformanceBitExt |
                                     DebugUtilsMessageTypeFlagsEXT.ValidationBitExt;
            createInfo.PfnUserCallback = (DebugUtilsMessengerCallbackFunctionEXT)DebugCallback;
        }

        uint DebugCallback(DebugUtilsMessageSeverityFlagsEXT messageSeverity, DebugUtilsMessageTypeFlagsEXT messageTypes, DebugUtilsMessengerCallbackDataEXT* pCallbackData, void* pUserData)
        {
            if (messageSeverity > DebugUtilsMessageSeverityFlagsEXT.VerboseBitExt)
            {
                Console.WriteLine
                    ($"{messageSeverity} {messageTypes}" + Marshal.PtrToStringAnsi((nint)pCallbackData->PMessage));
            }

            return Vk.False;
        }
    }
}
