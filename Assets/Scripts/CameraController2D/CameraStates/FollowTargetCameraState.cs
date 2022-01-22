using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTargetCameraState : CameraState
{
    [SerializeField]
    private GameObject TargetObj;
    private Rigidbody2D TargetRbody;

    public float MinOffsetFraction = 0.0f;
    public float MaxOffsetFraction = 1.0f;
    public float MaxOffsetMoveSpeed = 1.0f;
    public float SpeedForMaxOffsetFraction = 10.0f;

    private Vector2 LastWorldOffset = Vector2.zero;

    public FollowTargetCameraState(GameObject Target)
    {
        TargetObj = Target;
    }

    override public void EnterState(CameraController2D Controller)
    {
        TargetRbody = TargetObj.GetComponent<Rigidbody2D>();
    }

    override public void ExitState(CameraController2D Controller)
    {
    }

    public override CameraState UpdateState(CameraController2D Controller)
    {
        Vector3 camPos = Controller.CameraObj.transform.position;
        Vector3 targetPos = TargetObj.transform.position;
        if (TargetRbody != null)
        {
            // Look in the direction of the player's velocity vector
            float speed = TargetRbody.velocity.magnitude;
            float offsetAlpha = (speed > SpeedForMaxOffsetFraction) ? 1.0f : (speed / SpeedForMaxOffsetFraction);
            float offsetFrac = Mathf.Lerp(MinOffsetFraction, MaxOffsetFraction, offsetAlpha);
            Vector2 screenOffset = TargetRbody.velocity.normalized * offsetFrac;
            screenOffset.x = Mathf.Clamp(screenOffset.x, -MaxOffsetFraction, MaxOffsetFraction);
            screenOffset.y = Mathf.Clamp(screenOffset.y, -MaxOffsetFraction, MaxOffsetFraction);
            // Camera.orthographicSize is actually only the half-width
            Vector2 worldOffset = new Vector2();
            worldOffset.x = (2.0f * Controller.CameraObj.orthographicSize * screenOffset.x);
            worldOffset.y = (2.0f * Controller.CameraObj.orthographicSize / Controller.CameraObj.aspect * screenOffset.y);

            // Smooth the transition when the player's velocity suddenly changes
            Vector2 changeInOffset = worldOffset - LastWorldOffset;
            float maxMoveDist = MaxOffsetMoveSpeed * Time.deltaTime;
            if (changeInOffset.magnitude > maxMoveDist)
			{
                worldOffset = LastWorldOffset + (changeInOffset.normalized * maxMoveDist);
			}
			LastWorldOffset = worldOffset;
			targetPos += new Vector3(worldOffset.x, worldOffset.y, 0.0f);
		}
        Controller.CameraObj.transform.position = new Vector3(targetPos.x, targetPos.y, camPos.z);
        return null;
    }
}
