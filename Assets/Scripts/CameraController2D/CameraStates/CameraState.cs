using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CameraState : MonoBehaviour
{
    abstract public void Enter(CameraController2D Controller);
    abstract public void Exit(CameraController2D Controller);
    abstract public CameraState Update(CameraController2D Controller);
}
