using System.Numerics;
using Core;
using Core.PlayerLoopStages;
using Silk.NET.Maths;

namespace Examples;

public class TwistRotator : Component, IUpdateble
{
    public required float Speed { get; set; }
    
    protected override void OnConnect()
    {
        base.OnConnect();
        UpdateStage.Register(this);
    }

    protected override void OnDisconnect()
    {
        base.OnDisconnect();
        UpdateStage.Unregister(this);
    }

    public void Update(double time)
    {
        SceneNode!.LocalRotation = Quaternion.CreateFromAxisAngle(Vector3.UnitZ, (float)Math.Sin(Scalar.DegreesToRadians(time * Speed) ));
    }
}
