using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Shared;
using Core;
using Core.PlayerLoop;
using Core.PlayerLoopStages;
using glTFLoader;
using glTFLoader.Schema;
using Silk.NET.Maths;
using Silk.NET.Vulkan;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using Camera = Core.Camera;
using Node = Core.Node;
using glTFNode = glTFLoader.Schema.Node;
using Image = SixLabors.ImageSharp.Image;

namespace Examples;

public class Example3
{
    readonly Surface _surface;
    readonly PlayerLoop _playerLoop;

    readonly Node _camera;
    readonly Func<string, Stream> _loader;

    readonly List<Node> _roots = new List<Node>();
    
    // struct Vertex {
    //     public Vector3 _pos;
    //     public Vector3 _normal;
    //     public Vector2 _uv;
    //     public Vector3 _color;
    //     
    //     public static VertexInputBindingDescription GetBindingDescription()
    //     {
    //         VertexInputBindingDescription bindingDescription = new()
    //         {
    //             Binding = 0,
    //             Stride = (uint)Unsafe.SizeOf<Vertex>(),
    //             InputRate = VertexInputRate.Vertex
    //         };
    //
    //         return bindingDescription;
    //     }
    //
    //     public static VertexInputAttributeDescription[] GetAttributeDescriptions()
    //     {
    //         var attributeDescriptions = new[]
    //         {
    //             new VertexInputAttributeDescription
    //             {
    //                 Binding = 0,
    //                 Location = 0,
    //                 Format = Format.R32G32B32Sfloat,
    //                 Offset = (uint)Marshal.OffsetOf<Vertex>(nameof(_pos)),
    //             },
    //             new VertexInputAttributeDescription
    //             {
    //                 Binding = 0,
    //                 Location = 1,
    //                 Format = Format.R32G32B32Sfloat,
    //                 Offset = (uint)Marshal.OffsetOf<Vertex>(nameof(_normal)),
    //             },
    //             new VertexInputAttributeDescription
    //             {
    //                 Binding = 0,
    //                 Location = 2,
    //                 Format = Format.R32G32Sfloat,
    //                 Offset = (uint)Marshal.OffsetOf<Vertex>(nameof(_uv)),
    //             },
    //             new VertexInputAttributeDescription
    //             {
    //                 Binding = 0,
    //                 Location = 3,
    //                 Format = Format.R32G32B32Sfloat,
    //                 Offset = (uint)Marshal.OffsetOf<Vertex>(nameof(_color)),
    //             },
    //         };
    //
    //         return attributeDescriptions;
    //     }
    // };

    Matrix4x4? MatrixFromArray(float[]? matrix)
    {
        return matrix is { Length: 16 }
            ? new Matrix4x4(
                matrix[0], matrix[1], matrix[2], matrix[3],
                matrix[4], matrix[5], matrix[6], matrix[7],
                matrix[8], matrix[9], matrix[10], matrix[11],
                matrix[12], matrix[13], matrix[14], matrix[15]
            )
            : null;
    }

    Vector3? Vector3FromArray(float[]? vector)
    {
        return vector is { Length: 3 }
            ? new Vector3(vector[0], vector[1], vector[2])
            : null;
    }
    
    Quaternion? QuaternionFromArray(float[]? quaternion)
    {
        return quaternion is { Length: 4 }
            ? new Quaternion(quaternion[0], quaternion[1], quaternion[2], quaternion[3])
            : null;
    }

    Node LoadNode(glTFNode gltfNode, Gltf file)
    {
        var node = new Node(gltfNode.Name);
        var matrix = MatrixFromArray(gltfNode.Matrix);

        Vector3? translation = null;
        Quaternion? rotation = null;
        Vector3? scale = null;

        if (matrix.HasValue && Matrix4x4.Decompose(matrix.Value, out var s, out var r, out var t))
        {
            translation = t;
            rotation = r;
            scale = s;
        }
        
        node.LocalPosition = Vector3FromArray(gltfNode.Translation) ?? translation ?? Vector3.Zero;
        node.LocalRotation = QuaternionFromArray(gltfNode.Rotation) ?? rotation ?? Quaternion.Identity;
        node.LocalScale = Vector3FromArray(gltfNode.Scale) ?? scale ?? Vector3.One;

        if (gltfNode.Mesh.HasValue) 
            LoadMesh(gltfNode.Mesh.Value, file, node);


        if (gltfNode.Children != null)
            foreach (var childId in gltfNode.Children)
            {
                var child = LoadNode(file.Nodes[childId], file);
                node.AddChild(child);
            }

        return node;
    }

    void LoadMesh(int meshId, Gltf file, Node node)
    {
        var input = file;
        var mesh = input.Meshes[meshId];
        // Iterate through all primitives of this node's mesh
        for (var i = 0; i < mesh.Primitives.Length; i++)
        {
            List<Vertex> vertexBuffer = new List<Vertex>();
            List<uint> indexBuffer = new List<uint>();
            var glTFPrimitive = mesh.Primitives[i];
            // uint firstIndex = static_cast<uint32_t>(indexBuffer.size());
            // uint vertexStart = static_cast<uint32_t>(vertexBuffer.size());
            long indexCount = 0;
            // Vertices
            {
                Span<Vector3> positionBuffer = null;
                Span<Vector3> normalsBuffer = null;
                Span<Vector2> texCoordsBuffer = null;
                int vertexCount = 0;


                // Get buffer data for vertex positions
                if (glTFPrimitive.Attributes.TryGetValue("POSITION", out var positionAccessorId))
                {
                    var accessor = input.Accessors[positionAccessorId];
                    //TODO nullcheck
                    var view = input.BufferViews[accessor.BufferView.Value];

                    if (accessor.Type != Accessor.TypeEnum.VEC3)
                        throw new Exception("Not supported Type for POSITION buffer");
                    if (accessor.ComponentType != Accessor.ComponentTypeEnum.FLOAT)
                        throw new Exception("Not supported ComponentType for POSITION buffer");

                    var buffer = file.LoadBinaryBuffer(view.Buffer, GetBytes);
                    vertexCount = accessor.Count;
                    var positionBufferByte = buffer.AsSpan(accessor.ByteOffset + view.ByteOffset, Unsafe.SizeOf<Vector3>() * vertexCount);
                    positionBuffer = MemoryMarshal.Cast<byte, Vector3>(positionBufferByte);
                }

                // Get buffer data for vertex normals
                if (glTFPrimitive.Attributes.TryGetValue("NORMAL", out var normalAccessorId))
                {
                    var accessor = input.Accessors[normalAccessorId];
                    //TODO nullcheck
                    var view = input.BufferViews[accessor.BufferView.Value];

                    if (accessor.Type != Accessor.TypeEnum.VEC3)
                        throw new Exception("Not supported Type for NORMAL buffer");
                    if (accessor.ComponentType != Accessor.ComponentTypeEnum.FLOAT)
                        throw new Exception("Not supported ComponentType for NORMAL buffer");

                    var buffer = file.LoadBinaryBuffer(view.Buffer, GetBytes);
                    var normalsBufferByte = buffer.AsSpan(accessor.ByteOffset + view.ByteOffset, Unsafe.SizeOf<Vector3>() * vertexCount);
                    normalsBuffer = MemoryMarshal.Cast<byte, Vector3>(normalsBufferByte);
                }

                // Get buffer data for vertex texture coordinates
                // glTF supports multiple sets, we only load the first one
                if (glTFPrimitive.Attributes.TryGetValue("TEXCOORD_0", out var textureAccessorId))
                {
                    var accessor = input.Accessors[textureAccessorId];
                    //TODO nullcheck
                    var view = input.BufferViews[accessor.BufferView.Value];

                    if (accessor.Type != Accessor.TypeEnum.VEC2)
                        throw new Exception("Not supported Type for TEXCOORD_0 buffer");
                    if (accessor.ComponentType != Accessor.ComponentTypeEnum.FLOAT)
                        throw new Exception("Not supported ComponentType for TEXCOORD_0 buffer");

                    var buffer = file.LoadBinaryBuffer(view.Buffer, GetBytes);
                    var normalsBufferByte = buffer.AsSpan(accessor.ByteOffset + view.ByteOffset, Unsafe.SizeOf<Vector2>() * vertexCount);
                    texCoordsBuffer = MemoryMarshal.Cast<byte, Vector2>(normalsBufferByte);
                }

                // Append data to model's vertex buffer
                for (int v = 0; v < vertexCount; v++)
                {
                    Vertex vert;
                    vert._pos = positionBuffer[v].ToGeneric();
                    // vert._normal = Vector3.Normalize(normalsBuffer != null ? normalsBuffer[v] : Vector3.Zero);
                    vert._texCoord = texCoordsBuffer != null ? texCoordsBuffer[v].ToGeneric() : Vector2.Zero.ToGeneric();
                    vert._color = Vector3.One.ToGeneric();
                    vertexBuffer.Add(vert);
                }
            }
            // Indices
            {
                //TODO nullcheck
                var accessor = input.Accessors[glTFPrimitive.Indices.Value];
                //TODO nullcheck
                var bufferView = input.BufferViews[accessor.BufferView.Value];

                if (accessor.Type != Accessor.TypeEnum.SCALAR)
                    throw new Exception("Not supported Type for Indices buffer");

                var buffer = file.LoadBinaryBuffer(bufferView.Buffer, GetBytes);
                indexCount += accessor.Count;

                // glTF supports different component types of indices
                switch (accessor.ComponentType)
                {
                    case Accessor.ComponentTypeEnum.UNSIGNED_INT:
                    {
                        var bufferByte = buffer.AsSpan(accessor.ByteOffset + bufferView.ByteOffset, Unsafe.SizeOf<uint>() * accessor.Count);
                        indexBuffer.AddRange(MemoryMarshal.Cast<byte, uint>(bufferByte));
                        break;
                    }
                    case Accessor.ComponentTypeEnum.UNSIGNED_SHORT:
                    {
                        var bufferByte = buffer.AsSpan(accessor.ByteOffset + bufferView.ByteOffset, Unsafe.SizeOf<ushort>() * accessor.Count);
                        foreach (var index in MemoryMarshal.Cast<byte, ushort>(bufferByte))
                            indexBuffer.Add(index);

                        break;
                    }

                    case Accessor.ComponentTypeEnum.UNSIGNED_BYTE:
                    {
                        var bufferByte = buffer.AsSpan(accessor.ByteOffset + bufferView.ByteOffset, Unsafe.SizeOf<byte>() * accessor.Count);
                        foreach (var index in bufferByte)
                            indexBuffer.Add(index);
                        break;
                    }

                    default:
                        throw new Exception($"Index component type {accessor.ComponentType} not supported!");
                }
            }

            var pipeline = _surface.CreatePipeLine(m_VertShaderCode, m_fragShaderCode, Vertex.GetBindingDescription(), Vertex.GetAttributeDescriptions(), GetBindings());

            var vb = _surface.CreateVertexBuffer(vertexBuffer.ToArray());
            var ib = _surface.CreateIndexBuffer(indexBuffer.ToArray());

            var renderer = new Renderer(_surface, pipeline, vb, ib, m_Texture);
            node.AddComponent(renderer);

            // Primitive primitive{};
            // primitive.firstIndex = firstIndex;
            // primitive.indexCount = indexCount;
            // primitive.materialIndex = glTFPrimitive.material;
            // node->mesh.primitives.push_back(primitive);
        }
    }

    static DescriptorSetLayoutBinding[] GetBindings() =>
    [
        new DescriptorSetLayoutBinding
        {
            Binding = 0,
            DescriptorCount = 1,
            DescriptorType = DescriptorType.UniformBuffer,
            PImmutableSamplers = null,
            StageFlags = ShaderStageFlags.VertexBit,
        },
        
        new DescriptorSetLayoutBinding
        {
            Binding = 1,
            DescriptorCount = 1,
            DescriptorType = DescriptorType.CombinedImageSampler,
            PImmutableSamplers = null,
            StageFlags = ShaderStageFlags.FragmentBit,
        }
    ];

    HelloTexture m_Texture;
    byte[] m_VertShaderCode;
    byte[] m_fragShaderCode;

    
    // void LoadImages(Gltf file)
    // {
    //     // Images can be stored inside the glTF (which is the case for the sample model), so instead of directly
    //     // loading them from disk, we fetch them from the glTF loader and upload the buffers
    //     images = new Images[file.Images.Length];
    //     for (int i = 0; i < file.Images.Length; i++) {
    //         var glTfImage = file.Images[i];
    //         // Get the image data from the glTF loader
    //         var buffer = null;
    //         VkDeviceSize bufferSize = 0;
    //         bool deleteBuffer = false;
    //         // We convert RGB-only images to RGBA, as most devices don't support RGB-formats in Vulkan
    //         if (glTfImage.BufferView.Component == 3) {
    //             bufferSize = glTfImage.width * glTfImage.height * 4;
    //             buffer = new unsigned char[bufferSize];
    //             var rgba = buffer;
    //             var rgb = &glTfImage.image[0];
    //             for (int j = 0; j < glTfImage.width * glTfImage.height; ++j) {
    //                 memcpy(rgba, rgb, sizeof(unsigned char) * 3);
    //                 rgba += 4;
    //                 rgb += 3;
    //             }
    //             deleteBuffer = true;
    //         }
    //         else {
    //             buffer = &glTfImage.image[0];
    //             bufferSize = glTfImage.image.size();
    //         }
    //         // Load texture from image buffer
    //         images[i].texture.fromBuffer(buffer, bufferSize, Format.R8G8B8A8Unorm, glTfImage.width, glTfImage.height, vulkanDevice, copyQueue);
    //         // if (deleteBuffer) {
    //         //     delete[] buffer;
    //         // }
    //     }
    // }

    //FlightHelmet.gltf
    void LoadGlTFFile(Stream stream)
    {
        Gltf file;
        if (!stream.CanSeek)
        {
            using var memStream = new MemoryStream();
            stream.CopyTo(memStream);
            file = Interface.LoadModel(memStream);
        }
        else
            file = Interface.LoadModel(stream);

        // LoadImages(file);
        // LoadMaterials(file);
        // LoadTextures(file);
        
        var scene = file.Scenes[file.Scene ?? 0];
        foreach (var nodeID in scene.Nodes)
        {
            var node = LoadNode(file.Nodes[nodeID], file);
            _roots.Add(node);
        }

    }

    public Example3(Surface surface, Func<string, Stream> assetLoader)
    {
        _loader = assetLoader;
        _surface = surface;

        _playerLoop = CreatePlayerLoop();
        _surface.BeforeDraw += _playerLoop.Run;
        
        using (var img = GetImage("Textures/grey.png"))
        {
            m_Texture = _surface.CreateTexture(img);
        }
        
        m_VertShaderCode = GetVertexShader();
        m_fragShaderCode = GetFragmentShader();

        using (var file = _loader("Models/deer.gltf"))
        {
            LoadGlTFFile(file);
        }
        var body = _roots.FirstOrDefault(n => n.Name == "Body");
        if (body != null)
            body.AddComponent(new CameraRotator() { Speed = 1.0f/5000.0f, Axis = Vector3.UnitY});
        var cameraSceneRoot = _roots.FirstOrDefault(n => n.Name == "Camera");
        var cameraRoot = new Node("camera");
        
        if (cameraSceneRoot != null)
        {
            cameraSceneRoot.AddChild(cameraRoot);
        }
        else
        {
            cameraRoot.LocalPosition = new Vector3(2, 2, 5);
            cameraRoot.LocalRotation = CreateFromYawPitchRoll(DegToRad(-10f), DegToRad(25f),0);
        }

        _camera = new Node("camera");
        var camera = new Camera(_surface) {FieldOfView = 60};
        _camera.AddComponent(camera);
        cameraRoot.AddChild(_camera);
    }
    
    byte[] GetFragmentShader() => GetBytes("Shaders/frag.spv");
    byte[] GetVertexShader() => GetBytes("Shaders/vert.spv");

    Image<Rgba32> GetImage(string path)
    {
        using var stream = _loader(path);
        return Image.Load<Rgba32>(stream);
    }

    byte[] GetBytes(string path)
    {
        using var stream = _loader(path);
        using var memStream = new MemoryStream();
        stream.CopyTo(memStream);
        return memStream.ToArray();
    }


    static PlayerLoop CreatePlayerLoop()
    {
        var playerLoop = new PlayerLoop();
        playerLoop.Add(new UpdateStage());
        playerLoop.Add(new AnimationStage());
        playerLoop.Add(new RenderableStage());
        return playerLoop;
    }
}
