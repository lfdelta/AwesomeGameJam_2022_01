using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NullCameraState : CameraState
{
    override public void EnterState(CameraController2D Controller)
    {
    }

    override public void ExitState(CameraController2D Controller)
    {
    }

    public override CameraState UpdateState(CameraController2D Controller)
    {
        return null;
    }
}
