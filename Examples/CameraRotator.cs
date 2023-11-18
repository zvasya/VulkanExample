using System.Numerics;
using Core;
using Core.PlayerLoopStages;

namespace Examples;

public class CameraRotator : Component, IUpdateble
{
    public required float Speed { get; set; }
    public required Vector3 Axis { get; set; }
    
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
        SceneNode!.LocalRotation = Quaternion.CreateFromAxisAngle(Axis, (float) (time * Speed) );
    }
}
