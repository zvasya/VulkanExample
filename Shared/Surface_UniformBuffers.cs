using System.Runtime.CompilerServices;
using Silk.NET.Maths;
using Silk.NET.Vulkan;
using Buffer = Silk.NET.Vulkan.Buffer;

namespace Shared;

public unsafe partial class Surface
{
    Buffer[]? _uniformBuffers;
    DeviceMemory[]? _uniformBuffersMemory;
    readonly DateTime _dateTime = DateTime.UtcNow;

    struct UniformBufferObject
    {
        public Matrix4X4<float> _model;
        public Matrix4X4<float> _view;
        public Matrix4X4<float> _proj;
    }

    void CreateUniformBuffers()
    {
        var bufferSize = (ulong)Unsafe.SizeOf<UniformBufferObject>();

        _uniformBuffers = new Buffer[_swapChainImages.Length];
        _uniformBuffersMemory = new DeviceMemory[_swapChainImages.Length];

        for (var i = 0; i < _swapChainImages.Length; i++)
        {
            CreateBuffer(bufferSize, BufferUsageFlags.UniformBufferBit,
                MemoryPropertyFlags.HostVisibleBit | MemoryPropertyFlags.HostCoherentBit, ref _uniformBuffers[i],
                ref _uniformBuffersMemory[i]);
        }
    }

    void UpdateUniformBuffer(uint currentImage)
    {
        //Silk Window has timing information so we are skipping the time code.

        var time = (DateTime.UtcNow - _dateTime).TotalMilliseconds / 10f;

        // UniformBufferObject ubo = new()
        // {
        //     model = Matrix4X4<float>.Identity,
        //     view = Matrix4X4<float>.Identity,
        //     proj = Matrix4X4<float>.Identity,
        // };

        var angle = (float)Math.Sin(Scalar.DegreesToRadians(time) / 2f);
        var rotation = Matrix4X4.CreateFromAxisAngle(new Vector3D<float>(0, 0, 1), angle);
        UniformBufferObject ubo = new()
        {
            _model = Matrix4X4<float>.Identity * rotation,
            _view =
                Matrix4X4.CreateLookAt(new Vector3D<float>(2, 2, 2), new Vector3D<float>(0, 0, 0),
                    new Vector3D<float>(0, 0, 1)),
            _proj = Matrix4X4.CreatePerspectiveFieldOfView(Scalar.DegreesToRadians(45.0f),
                (float)_swapChainExtent.Width / _swapChainExtent.Height, 0.1f, 10.0f),
        };
        ubo._proj.M22 *= -1;


        void* data;
        _vk.MapMemory(_device, _uniformBuffersMemory![currentImage], 0, (ulong)Unsafe.SizeOf<UniformBufferObject>(), 0,
            &data);
        new Span<UniformBufferObject>(data, 1)[0] = ubo;
        _vk.UnmapMemory(_device, _uniformBuffersMemory![currentImage]);
    }
}
