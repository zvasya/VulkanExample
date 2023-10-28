using Silk.NET.Vulkan;
using Semaphore = Silk.NET.Vulkan.Semaphore;

namespace Shared;

public unsafe class HelloSemaphore : IDisposable
{
    readonly Semaphore _semaphore;
    public Semaphore Semaphore => _semaphore;
    readonly LogicalDevice _device;
    
    HelloSemaphore(LogicalDevice device, Semaphore semaphore)
    {
        _device = device;
        _semaphore = semaphore;
    }

    public static HelloSemaphore Create(LogicalDevice device)
    {
        var semaphore = CreateSemaphore(device);
        return new HelloSemaphore(device, semaphore);
    }

    static Semaphore CreateSemaphore(LogicalDevice _device)
    {
        var semaphoreInfo = new SemaphoreCreateInfo
        {
            SType = StructureType.SemaphoreCreateInfo,
        };

        Semaphore semaphore;
        {
            Helpers.CheckErrors(_device.CreateSemaphore(&semaphoreInfo, null, &semaphore));
        }
        
        return semaphore;
    }
    
    public static implicit operator Semaphore(HelloSemaphore s) => s._semaphore;

    public void Dispose()
    {
        _device.DestroySemaphore(_semaphore, null);
    }
}
