using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController2D : MonoBehaviour
{
    CameraState State = null;

    public CameraState DefaultState = null;
    public Camera CameraObj;

    // Start is called before the first frame update
    void Start()
    {
        if (CameraObj == null)
		{
            SetControlledCamera(Camera.main);
		}
        if (DefaultState == null)
		{
            DefaultState = new NullCameraState();
		}
        if (State == null)
        {
            State = DefaultState;
        }
        State.EnterState(this);
    }

    // LateUpdate is called once per frame, after all of the Update calls
    void LateUpdate()
    {
        CameraState newState = State.UpdateState(this);
        if (newState != null)
		{
            TransitionTo(newState);
		}
    }

    public void SetControlledCamera(Camera NewCamera)
	{
        CameraObj = NewCamera;
	}

    public void TransitionTo(CameraState NewState)
	{
        State.ExitState(this);
        NewState.EnterState(this);
        State = NewState;
	}
}