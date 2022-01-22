using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTargetCameraState : CameraState
{
    [SerializeField]
    private GameObject TargetObj;

    public FollowTargetCameraState(GameObject Target)
    {
        TargetObj = Target;
    }

    override public void Enter(CameraController2D Controller)
    {
    }

    override public void Exit(CameraController2D Controller)
    {
    }

    public override CameraState Update(CameraController2D Controller)
    {
        Vector3 camPos = Controller.CameraObj.transform.position;
        Vector3 targetPos = TargetObj.transform.position;
        Controller.CameraObj.transform.position = new Vector3(targetPos.x, targetPos.y, camPos.z);
        return null;
    }
}
