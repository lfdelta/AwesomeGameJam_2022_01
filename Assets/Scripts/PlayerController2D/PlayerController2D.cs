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

    private class PlayerSwingInfo
	{
        public bool AttachPointValid = false;
        public Vector2 WorldAttachPoint = Vector2.zero;
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
    private bool IsGrounded = false;
    private bool IsJumpingUp = false;

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
        if (NumJumpsRemaining == 0)
		{
            return;
		}
        Debug.Log("Jumping");
        State = EPlayerState.Freefall;
        IsJumpingUp = true;
        // Reset vertical velocity
        Rbody.velocity = new Vector2(Rbody.velocity.x, JumpVelocity);
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
            State = EPlayerState.Swinging;
            SwingJoint.enabled = true;
            SwingJoint.connectedAnchor = SwingInfo.WorldAttachPoint;
            SwingJoint.distance = (SwingInfo.WorldAttachPoint - new Vector2(transform.position.x, transform.position.y)).magnitude;
        }
	}

    public void EndSwing()
	{
        if (State == EPlayerState.Swinging)
		{
            SwingJoint.enabled = false;
            State = GetIsGrounded() ? EPlayerState.Grounded : EPlayerState.Freefall;
		}
	}

    private bool GetIsGrounded()
	{
        Vector2 colliderBottom = ColliderObj.bounds.center;
        colliderBottom.y -= ColliderObj.bounds.extents.y + 0.01f; // Epsilon to get out of the collider itself
        RaycastHit2D hit = Physics2D.Raycast(colliderBottom, Vector2.down, GroundRaycastDist);
        return hit.collider != null;
    }

    // FixedUpdate is called once per physics tick
	void FixedUpdate()
	{
        if (State == EPlayerState.Swinging)
		{
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
        bool newIsGrounded = GetIsGrounded();
        if (newIsGrounded != IsGrounded)
        {
            if (newIsGrounded)
            {
                State = EPlayerState.Grounded;
                NumJumpsRemaining = NumJumps;
                Rbody.gravityScale = 1.0f;
            }
            else if (NumJumps > 0 && NumJumpsRemaining == NumJumps)
            {
                State = EPlayerState.Freefall;
                // Remove the "first jump" if the player runs off a ledge
                --NumJumpsRemaining;
            }
            IsGrounded = newIsGrounded;
        }

        // Apply horizontal physics
        float horizVelocity = Rbody.velocity.x;
        float horizSpeed = Mathf.Abs(horizVelocity);
        float maxSpeed = IsGrounded ? MaxGroundSpeed : MaxAirSpeed;
        float dragSpeedChange;
        // Only apply drag when the user isn't trying to accelerate
        if (horizSpeed == 0.0f || (Mathf.Abs(MoveInputDir.x) > 0.001f && (MoveInputDir.x > 0.0f) == (horizVelocity > 0.0f)))
        {
            dragSpeedChange = 0.0f;
        }
        else
        {
            dragSpeedChange = (IsGrounded ? GroundDragAcceleration : AirDragAcceleration) * Time.fixedDeltaTime;
        }
        // Apply input and drag, clamping final horizontal speed to [0, maxSpeed]
        horizVelocity += MoveInputDir.x * MoveAcceleration * Time.fixedDeltaTime;
        horizSpeed = Mathf.Abs(horizVelocity);
        if (horizSpeed < dragSpeedChange)
		{
            horizVelocity = 0.0f;
		}
        else if (horizSpeed > maxSpeed)
		{
            horizVelocity = Mathf.Sign(horizVelocity) * maxSpeed;
		}
		else
		{
            horizVelocity -= Mathf.Sign(horizVelocity) * dragSpeedChange;
		}
        Rbody.velocity = new Vector2(horizVelocity, Rbody.velocity.y);
        LastMotionDir = Rbody.velocity;
    }

    // Update is called once per frame
    // Do visual/gameplay stuff
    void Update()
    {
        if (State == EPlayerState.Swinging)
        {
            // Render current swing joint
            Color lineColor = Color.white;
            LineRenderComp.startColor = lineColor;
            LineRenderComp.endColor = lineColor;
            LineRenderComp.SetPositions(new Vector3[] { gameObject.transform.position, SwingInfo.WorldAttachPoint });
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
            Vector3 aimOffset = new Vector3(AimDir.x, AimDir.y, 0.0f) * MaxSwingRadius;
            Color lineColor = (SwingInfo.AttachPointValid) ? Color.green : Color.red;
            LineRenderComp.startColor = lineColor;
            LineRenderComp.endColor = lineColor;
            LineRenderComp.SetPositions(new Vector3[] { gameObject.transform.position, gameObject.transform.position + aimOffset });
        }
        // Render swing attach point, if applicable
        if (SwingInfo.AttachPointValid)
        {
            SwingPointMarker.enabled = true;
            SwingPointMarker.transform.position = new Vector3(SwingInfo.WorldAttachPoint.x, SwingInfo.WorldAttachPoint.y, 0.0f);
        }
        else
        {
            SwingPointMarker.enabled = false;
        }
    }
}
