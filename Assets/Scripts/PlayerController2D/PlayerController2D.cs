using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(LineRenderer))]
public class PlayerController2D : MonoBehaviour
{
	private enum EPlayerState
	{
		Grounded,
		Freefall,
		Swinging
	}
	private EPlayerState State;

	private class PrevSwingPoint
	{
		public Vector2 WorldPosition;
		public bool WrappedCW;
		public float Angle;
	}

	private class PlayerSwingInfo
	{
		public bool AttachPointValid = false;
		public bool SwingingCW = false;
		public Vector2 WorldAttachPoint = Vector2.zero;
		public List<PrevSwingPoint> PreviousAttachPoints = new List<PrevSwingPoint>();
	}
	private PlayerSwingInfo SwingInfo = new PlayerSwingInfo();

	private Rigidbody2D Rbody;
	private Collider2D ColliderObj;
	private LineRenderer LineRenderComp;
	private DistanceJoint2D SwingJoint;
	private Vector2 MoveInputDir = Vector2.zero;
	private Vector2 AimDir = Vector2.right;
	private int NumJumpsRemaining = 0;
	private Vector2 LastMotionDir = Vector2.zero;
	private bool IsJumpingUp = false;

	private Vector2 LastGroundPosition = new Vector2(0.0f, float.NegativeInfinity);
	private Vector2 GroundNormal = Vector2.up;

	public SpriteRenderer SwingPointMarker;

	public float MoveAcceleration;
	public float MaxGroundSpeed;
	public float MaxAirSpeed;
	public float GroundDragAcceleration;
	public float AirDragAcceleration;
	public float JumpVelocity;
	public float JumpHoldGravityScale = 1.0f;
	public float GroundRaycastDist = 0.05f;
	public float MaxSwingRadius = 10.0f;
	public int NumJumps = 1;
	public float FallKillSpeed = 1000.0f;
	public float WorldKillPlane = float.NegativeInfinity;

	// Start is called before the first frame update
	void Start()
	{
		Rbody = GetComponent<Rigidbody2D>();
		ColliderObj = GetComponent<Collider2D>();

		LineRenderComp = GetComponent<LineRenderer>();
		LineRenderComp.colorGradient.mode = GradientMode.Fixed;

		SwingJoint = gameObject.AddComponent<DistanceJoint2D>();
		SwingJoint.enabled = false;
		SwingJoint.enableCollision = true;
		SwingJoint.autoConfigureDistance = false;
		SwingJoint.maxDistanceOnly = false;

		State = GetIsGrounded() ? EPlayerState.Grounded : EPlayerState.Freefall;
	}

	public void Teleport(Vector2 TargetPos)
	{
		EndSwing();
		transform.position = TargetPos;
		Rbody.velocity = Vector2.zero;
	}

	public void SetMoveInputDirection(Vector2 Dir)
	{
		MoveInputDir = Dir.normalized;
	}

	public void SetAimInputDirection(Vector2 Dir)
	{
		if (Dir == Vector2.zero)
		{
			// Up and forward at a 45-degree angle
			float xSign = (LastMotionDir.x == 0.0f) ? 1.0f : Mathf.Sign(LastMotionDir.x);
			AimDir = new Vector2(xSign, 1.0f).normalized;
		}
		else
		{
			AimDir = Dir.normalized;
		}
	}

	public void StartJump()
	{
		// No jumping while swinging
		if (NumJumpsRemaining == 0 || State == EPlayerState.Swinging)
		{
			return;
		}
		Vector2 rightVelNormal = (State == EPlayerState.Grounded) ? -Vector2.Perpendicular(GroundNormal) : Vector2.right;
		State = EPlayerState.Freefall;
		IsJumpingUp = true;
		// Reset vertical velocity
		float signedHorizSpeed = Vector2.Dot(Rbody.velocity, rightVelNormal);
		Rbody.velocity = (signedHorizSpeed * rightVelNormal) + new Vector2(0.0f, JumpVelocity);
		Rbody.gravityScale = JumpHoldGravityScale;
		--NumJumpsRemaining;
	}

	public void EndJump()
	{
		if (IsJumpingUp)
		{
			IsJumpingUp = false;
			Rbody.gravityScale = 1.0f;
		}
	}

	public void StartSwing()
	{
		if (State != EPlayerState.Swinging && SwingInfo.AttachPointValid)
		{
			SwingInfo.PreviousAttachPoints.Clear();
			State = EPlayerState.Swinging;
			SwingJoint.enabled = true;
			PushSwingJointPivot();
		}
	}

	public void EndSwing()
	{
		if (State == EPlayerState.Swinging)
		{
			SwingInfo.PreviousAttachPoints.Clear();
			SwingJoint.enabled = false;
			State = EPlayerState.Freefall; // This will get fixed up on the next FixedUpdate if the player is actually grounded
		}
	}

	private void PushSwingJointPivot()
	{
		SwingJoint.connectedAnchor = SwingInfo.WorldAttachPoint;
		SwingJoint.distance = (SwingInfo.WorldAttachPoint - new Vector2(transform.position.x, transform.position.y)).magnitude;
	}

	private bool GetIsGrounded()
	{
		// By default, raycast from the bottom of the collider
		Vector2 castOrigin = ColliderObj.bounds.center;
		castOrigin.y -= ColliderObj.bounds.extents.y;
		float castDist = GroundRaycastDist;
		Vector2[] castDirs;

		CapsuleCollider2D capsule = (CapsuleCollider2D)ColliderObj;
		if (capsule == null)
		{
			// Only check straight down
			castDirs = new[] { Vector2.down };
		}
		else
		{
			// Cast in various directions, from the center of the lower hemisphere, to detect collisions with slopes
			float capsuleRadius = capsule.size.x;
			castOrigin.y += capsuleRadius;
			castDist += capsuleRadius;
			castDirs = new[] { Vector2.down, (Vector2.down + Vector2.left).normalized, (Vector2.down + Vector2.right).normalized };
		}

		RaycastHit2D[] rayHits = new RaycastHit2D[10];
		ContactFilter2D filter = new ContactFilter2D();
		int numHits;
		foreach (Vector2 castDir in castDirs)
		{
			numHits = Physics2D.Raycast(castOrigin, castDir, filter, rayHits, castDist);
			for (int i = 0; i < numHits; ++i)
			{
				if (rayHits[i].collider.gameObject != gameObject && !rayHits[i].collider.isTrigger)
				{
					// Allow slopes just steeper than 45 degrees
					if (rayHits[i].normal.y > 0.7f)
					{
						GroundNormal = rayHits[i].normal;
						return true;
					}
					return false;
				}
			}
		}
		return false;
	}

	private void OnCollisionEnter2D(Collision2D collision)
	{
		// TODO: remove once FallKillSpeed is tuned
		Debug.Log($"{collision.relativeVelocity}");
	}

	// FixedUpdate is called once per physics tick
	void FixedUpdate()
	{
		// Detect player hitting the kill plane
		if (transform.position.y < WorldKillPlane)
		{
			Teleport(LastGroundPosition);
			return;
		}

		if (State == EPlayerState.Swinging)
		{
			// If needed, simulate wrapping the rope around corners as the player swings
			RaycastHit2D[] rayHits = new RaycastHit2D[10];
			ContactFilter2D filter = new ContactFilter2D();
			Vector2 toSwingPoint = SwingInfo.WorldAttachPoint - new Vector2(transform.position.x, transform.position.y);
			SwingInfo.SwingingCW = Vector3.Cross(toSwingPoint, Rbody.velocity).z < 0.0f;
			Debug.Log($"{SwingInfo.SwingingCW}");
			int numHits = Physics2D.Raycast(transform.position, toSwingPoint.normalized, filter, rayHits, toSwingPoint.magnitude + 0.01f);
			bool wrapped = false;
			for (int i = 0; i < numHits; ++i)
			{
				if (rayHits[i].collider.gameObject != gameObject && !rayHits[i].collider.isTrigger)
				{
					if ((rayHits[i].point - SwingInfo.WorldAttachPoint).magnitude > 0.01f)
					{
						// The player no longer has direct line-of-sight to the swing point, so we need to wrap
						PrevSwingPoint wrapInfo = new PrevSwingPoint();
						wrapInfo.WorldPosition = SwingInfo.WorldAttachPoint;
						wrapInfo.WrappedCW = SwingInfo.SwingingCW;
						wrapInfo.Angle = Mathf.Atan2(toSwingPoint.y, toSwingPoint.x);
						SwingInfo.PreviousAttachPoints.Add(wrapInfo);
						// TODO: This isn't actually the exact wrapping point; that should be set to the collider vertex
						// (however it should be pretty close to the exact point in general)
						SwingInfo.WorldAttachPoint = rayHits[i].point;
						PushSwingJointPivot();
						wrapped = true;
					}
					break;
				}
			}
			// Simulate unwrapping
			if (!wrapped && SwingInfo.PreviousAttachPoints.Count > 0)
			{
				PrevSwingPoint prevPoint = SwingInfo.PreviousAttachPoints[SwingInfo.PreviousAttachPoints.Count - 1];
				if (SwingInfo.SwingingCW != prevPoint.WrappedCW)
				{
					toSwingPoint = prevPoint.WorldPosition - new Vector2(transform.position.x, transform.position.y);
					//float angle = Vector2.Angle(Vector2.up, toSwingPoint);
					float angle = Mathf.Atan2(toSwingPoint.y, toSwingPoint.x);
					if (prevPoint.WrappedCW ? (angle < prevPoint.Angle) : (angle > prevPoint.Angle))
					{
						// The player has come back into alignment with the previous swinging point, so we unwrap
						SwingInfo.WorldAttachPoint = prevPoint.WorldPosition;
						SwingInfo.PreviousAttachPoints.RemoveAt(SwingInfo.PreviousAttachPoints.Count - 1);
					}
				}
			}
			// Player is FULLY controlled by gravity, with no motion input
			return;
		}

		// Detect transition between jumping up and falling down
		if (State == EPlayerState.Freefall && IsJumpingUp && Rbody.velocity.y < 0.0f)
		{
			IsJumpingUp = false;
			Rbody.gravityScale = 1.0f;
		}

		// Detect grounded state and transition appropriately
		bool isGrounded = GetIsGrounded() && !IsJumpingUp;
		if (isGrounded != (State == EPlayerState.Grounded))
		{
			if (isGrounded)
			{
				// Detect when the player hits the ground too quickly
				// Doing this here because OnCollisionEnter2D can be called after the player is set
				//  to the grounded state, which causes them to "teleport" the exact spot they just hit
				if (Rbody.velocity.y <= -FallKillSpeed)
				{
					Teleport(LastGroundPosition);
					return;
				}
				State = EPlayerState.Grounded;
				// Track some safe position to reset to on death
				LastGroundPosition = transform.position;
				NumJumpsRemaining = NumJumps;
				//Rbody.gravityScale = 1.0f;
				Rbody.gravityScale = 0.0f;
			}
			else
			{
				State = EPlayerState.Freefall;
				if (NumJumps > 0 && NumJumpsRemaining == NumJumps)
				{
					// Remove the "first jump" if the player runs off a ledge
					--NumJumpsRemaining;
				}
				Rbody.gravityScale = 1.0f;
			}
		}

		// Apply horizontal physics
		Vector2 upVelNormal = isGrounded ? GroundNormal : Vector2.up;
		Vector2 rightVelNormal = isGrounded ? -Vector2.Perpendicular(GroundNormal) : Vector2.right;
		float signedHorizSpeed = Vector2.Dot(Rbody.velocity, rightVelNormal);
		float signedVertSpeed = Vector2.Dot(Rbody.velocity, upVelNormal);
		float horizSpeed = Mathf.Abs(signedHorizSpeed);
		float maxSpeed = isGrounded ? MaxGroundSpeed : MaxAirSpeed;
		float dragSpeedChange;
		// Only apply drag when the user isn't trying to accelerate
		if (horizSpeed == 0.0f || (Mathf.Abs(MoveInputDir.x) > 0.001f && Mathf.Sign(MoveInputDir.x) == Mathf.Sign(signedHorizSpeed)))
		{
			dragSpeedChange = 0.0f;
		}
		else
		{
			dragSpeedChange = (isGrounded ? GroundDragAcceleration : AirDragAcceleration) * Time.fixedDeltaTime;
		}
		// Apply input and drag, clamping final horizontal speed to [0, maxSpeed]
		signedHorizSpeed += MoveInputDir.x * MoveAcceleration * Time.fixedDeltaTime;
		horizSpeed = Mathf.Abs(signedHorizSpeed);
		// Try not to prevent the player from starting to move if the drag exceeds the acceleration
		if (horizSpeed < dragSpeedChange && (dragSpeedChange < MoveAcceleration * Time.fixedDeltaTime))
		{
			signedHorizSpeed = 0.0f;
		}
		else if (horizSpeed > maxSpeed + dragSpeedChange)
		{
			signedHorizSpeed = Mathf.Sign(signedHorizSpeed) * maxSpeed;
		}
		else
		{
			signedHorizSpeed -= Mathf.Sign(signedHorizSpeed) * dragSpeedChange;
		}
		Rbody.velocity = (signedHorizSpeed * rightVelNormal) + (signedVertSpeed * upVelNormal);
		LastMotionDir = Rbody.velocity;
	}

	// Update is called once per frame
	// Do visual/gameplay stuff
	void Update()
	{
		if (State == EPlayerState.Swinging)
		{
			// Render current swing joint, and all previous wrapped segments if present
			Color lineColor = Color.white;
			LineRenderComp.startColor = lineColor;
			LineRenderComp.endColor = lineColor;
			LineRenderComp.positionCount = SwingInfo.PreviousAttachPoints.Count + 2;
			for (int i = 0; i < SwingInfo.PreviousAttachPoints.Count; ++i)
			{
				LineRenderComp.SetPosition(i, SwingInfo.PreviousAttachPoints[i].WorldPosition);
			}
			LineRenderComp.SetPosition(SwingInfo.PreviousAttachPoints.Count, SwingInfo.WorldAttachPoint);
			LineRenderComp.SetPosition(SwingInfo.PreviousAttachPoints.Count + 1, gameObject.transform.position);
		}
		else
		{
			// Update swing info
			SwingInfo.AttachPointValid = false;
			RaycastHit2D[] rayHits = new RaycastHit2D[10];
			ContactFilter2D filter = new ContactFilter2D();
			int numHits = Physics2D.Raycast(transform.position, AimDir, filter, rayHits, MaxSwingRadius);
			for (int i = 0; i < numHits; ++i)
			{
				// We can swing if we hit any non-trigger collider object
				if (rayHits[i].collider.gameObject != gameObject && !rayHits[i].collider.isTrigger)
				{
					SwingInfo.AttachPointValid = true;
					SwingInfo.WorldAttachPoint = rayHits[i].point;
					break;
				}
			}
			// Render the sight-line for the swing
			Color lineColor;
			Vector3 endPoint;
			if (SwingInfo.AttachPointValid)
			{
				lineColor = Color.green;
				endPoint = SwingInfo.WorldAttachPoint;
			}
			else
			{
				lineColor = Color.red;
				endPoint = gameObject.transform.position + new Vector3(AimDir.x, AimDir.y, 0.0f) * MaxSwingRadius;
			}
			LineRenderComp.startColor = lineColor;
			LineRenderComp.endColor = lineColor;
			LineRenderComp.positionCount = 2;
			LineRenderComp.SetPositions(new Vector3[] { gameObject.transform.position, endPoint });
		}
		// Render swing attach point, if applicable
		if (SwingInfo.AttachPointValid)
		{
			SwingPointMarker.enabled = true;
			Vector2 renderPoint = (SwingInfo.PreviousAttachPoints.Count > 0) ? SwingInfo.PreviousAttachPoints[0].WorldPosition : SwingInfo.WorldAttachPoint;
			SwingPointMarker.transform.position = new Vector3(renderPoint.x, renderPoint.y, 0.0f);
		}
		else
		{
			SwingPointMarker.enabled = false;
		}
	}
}
