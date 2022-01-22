using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CameraState : MonoBehaviour
{
    abstract public void EnterState(CameraController2D Controller);
    abstract public void ExitState(CameraController2D Controller);
    abstract public CameraState UpdateState(CameraController2D Controller);
}
