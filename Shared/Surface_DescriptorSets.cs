﻿namespace Shared;

public partial class Surface
{
    HelloDescriptorPool _descriptorPool;
    
    void CreateDescriptorPool()
    {
        _descriptorPool = HelloDescriptorPool.Create(_device, HelloEngine.MAX_FRAMES_IN_FLIGHT);
    }
}
