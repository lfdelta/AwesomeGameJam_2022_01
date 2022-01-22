using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InterpToObjectCameraState : CameraState
{
    private Vector2 StartPos;
    private GameObject TargetObj;
    private float StartTime;
    private float EndTime;

    public InterpToObjectCameraState(GameObject Target, float InterpTime)
    {
        TargetObj = Target;
        StartTime = Time.time;
        EndTime = StartTime + InterpTime;
    }

    override public void EnterState(CameraController2D Controller)
    {
        Vector3 camPos = Controller.CameraObj.transform.position;
        StartPos = new Vector2(camPos.x, camPos.y);
    }

    override public void ExitState(CameraController2D Controller)
    {
    }

    public override CameraState UpdateState(CameraController2D Controller)
    {
        float alpha = (Time.time - StartTime) / (EndTime - StartTime);
        Vector3 camPos = Controller.CameraObj.transform.position;
        Vector3 targetPos3D = TargetObj.transform.position;
        Vector2 targetPos = new Vector2(targetPos3D.x, targetPos3D.y);
        if (alpha >= 1.0f)
        {
            Controller.CameraObj.transform.position = new Vector3(targetPos.x, targetPos.y, camPos.z);
            return Controller.DefaultState;
        }
        else
        {
            Vector2 interpPos = (alpha * StartPos) + ((1.0f - alpha) * targetPos);
            Controller.CameraObj.transform.position = new Vector3(interpPos.x, interpPos.y, camPos.z);
        }
        return null;
    }
}
