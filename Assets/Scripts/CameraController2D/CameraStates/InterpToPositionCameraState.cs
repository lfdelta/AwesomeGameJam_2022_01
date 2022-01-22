using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InterpToPositionCameraState : CameraState
{
    private Vector2 StartPos;
    private Vector2 EndPos;
    private float StartTime;
    private float EndTime;

    public InterpToPositionCameraState(Vector2 Target, float InterpTime)
    {
        EndPos = Target;
        StartTime = Time.time;
        EndTime = StartTime + InterpTime;
    }

    override public void Enter(CameraController2D Controller)
    {
        Vector3 camPos = Controller.CameraObj.transform.position;
        StartPos = new Vector2(camPos.x, camPos.y);
    }

    override public void Exit(CameraController2D Controller)
    {
    }

    public override CameraState Update(CameraController2D Controller)
    {
        float alpha = (Time.time - StartTime) / (EndTime - StartTime);
        Vector3 camPos = Controller.CameraObj.transform.position;
        if (alpha >= 1.0f)
        {
            Controller.CameraObj.transform.position = new Vector3(EndPos.x, EndPos.y, camPos.z);
            return Controller.DefaultState;
        }
        else
        {
            Vector2 interpPos = (alpha * StartPos) + ((1.0f - alpha) * EndPos);
            Controller.CameraObj.transform.position = new Vector3(interpPos.x, interpPos.y, camPos.z);
        }
        return null;
    }
}
