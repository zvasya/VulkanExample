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

    public static Vector3 ToEulerAngles(this Quaternion q) {
        Vector3 angles;

        double sinX_cosp = 2.0 * (q.W * q.X + q.Y * q.Z);
        double cosX_cosp = 1.0 - 2.0 * (q.X * q.X + q.Y * q.Y);
        angles.X = (float)Math.Atan2(sinX_cosp, cosX_cosp);

        double sinY = Math.Sqrt(1.0 + 2.0 * (q.W * q.Y - q.X * q.Z));
        double cosY = Math.Sqrt(1.0 - 2.0 * (q.W * q.Y - q.X * q.Z));
        angles.Y = (float)(2.0 * Math.Atan2(sinY, cosY) - Math.PI / 2.0);

        double sinZ_cosp = 2.0 * (q.W * q.Z + q.X * q.Y);
        double cosZ_cosp = 1.0 - 2.0 * (q.Y * q.Y + q.Z * q.Z);
        angles.Z = (float)Math.Atan2(sinZ_cosp, cosZ_cosp);

        return angles;
    }
    
    public static float DegToRad(float value) => (float)(value * Math.PI / 180.0);
}
