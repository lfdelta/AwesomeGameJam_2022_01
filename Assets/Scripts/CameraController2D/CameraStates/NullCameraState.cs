using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NullCameraState : CameraState
{
    override public void Enter(CameraController2D Controller)
    {
    }

    override public void Exit(CameraController2D Controller)
    {
    }

    public override CameraState Update(CameraController2D Controller)
    {
        return null;
    }
}
