global using static Examples.QuaternionUtils;
using System.Numerics;

namespace Examples;

public static class QuaternionUtils
{
    public static Quaternion CreateFromYawPitchRoll(float roll, float pitch, float yaw)// roll (x), pitch (y), yaw (z)
    {
        var sz = MathF.Sin(yaw * 0.5f);
        var cz = MathF.Cos(yaw * 0.5f);
        var sx = MathF.Sin(roll * 0.5f);
        var cx = MathF.Cos(roll * 0.5f);
        var sy = MathF.Sin(pitch * 0.5f);
        var cy = MathF.Cos(pitch * 0.5f);

        Quaternion result;
        result.X = cy * sx * cz - sy * cx * sz;
        result.Y = sy * cx * cz + cy * sx * sz;
        result.Z = cy * cx * sz - sy * sx * cz;
        result.W = cy * cx * cz + sy * sx * sz;

        return result;
    }
    
    public static float DegToRad(float value) => (float)(value * Math.PI / 180.0);
}
