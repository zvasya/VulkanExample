using System.Numerics;
using Silk.NET.Maths;

namespace Shared;

public class Node
{
    public Vector3 _position = Vector3.Zero;
    public Quaternion _rotation = Quaternion.Identity;
    public Vector3 _scale = Vector3.One;

    protected Matrix4x4 TRS()
    {
        return Matrix4x4.CreateScale(_scale) * Matrix4x4.CreateFromQuaternion(_rotation) * Matrix4x4.CreateTranslation(_position);
    }
}
