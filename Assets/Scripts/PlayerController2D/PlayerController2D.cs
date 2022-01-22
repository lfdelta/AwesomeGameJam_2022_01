using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController2D : MonoBehaviour
{
    private Rigidbody2D Rbody;
    private Collider2D ColliderObj;
    private Vector2 InputDir = Vector2.zero;
    private int NumJumpsRemaining = 0;
    private bool IsGrounded = false;
    private bool IsJumpingUp = false;

    public float MoveAcceleration;
    public float MaxGroundSpeed;
    public float MaxAirSpeed;
    public float GroundDragAcceleration;
    public float AirDragAcceleration;
    public float JumpVelocity;
    public float JumpHoldGravityScale = 1.0f;
    public float GroundRaycastDist = 0.05f;
    public int NumJumps = 1;

    // Start is called before the first frame update
    void Start()
    {
        Rbody = GetComponent<Rigidbody2D>();
        ColliderObj = GetComponent<Collider2D>();
    }

    public void SetInputDirection(Vector2 Dir)
	{
        InputDir = Dir.normalized;
	}

    public void StartJump()
	{
        if (NumJumpsRemaining == 0)
		{
            return;
		}
        Debug.Log("Jumping");
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

    // FixedUpdate is called once per physics tick
	void FixedUpdate()
	{
        // Detect transition between jumping up and falling down
        if (IsJumpingUp && Rbody.velocity.y < 0.0f)
        {
            IsJumpingUp = false;
            Rbody.gravityScale = 1.0f;
        }

        // Detect grounded state and transition appropriately
        Vector2 colliderBottom = ColliderObj.bounds.center;
        colliderBottom.y -= ColliderObj.bounds.extents.y + 0.01f; // Epsilon to get out of the collider itself
        RaycastHit2D hit = Physics2D.Raycast(colliderBottom, Vector2.down, GroundRaycastDist);
        bool newIsGrounded = hit.collider != null;
        Debug.Log(newIsGrounded ? "Grounded" : "Not grounded");
        if (newIsGrounded != IsGrounded)
        {
            if (newIsGrounded)
            {
                NumJumpsRemaining = NumJumps;
                IsJumpingUp = false;
                Rbody.gravityScale = 1.0f;
                Debug.Log("Becoming grounded");
            }
            else if (NumJumps > 0 && NumJumpsRemaining == NumJumps)
            {
                // Remove the "first jump" if the player runs off a ledge
                --NumJumpsRemaining;
                Debug.Log("Running off ledge");
            }
            IsGrounded = newIsGrounded;
        }

        // Apply horizontal physics
        float horizVelocity = Rbody.velocity.x;
        float horizSpeed = Mathf.Abs(horizVelocity);
        float maxSpeed = IsGrounded ? MaxGroundSpeed : MaxAirSpeed;
        float dragSpeedChange;
        // Only apply drag when the user isn't trying to accelerate
        if (horizSpeed == 0.0f || (Mathf.Abs(InputDir.x) > 0.001f && (InputDir.x > 0.0f) == (horizVelocity > 0.0f)))
        {
            dragSpeedChange = 0.0f;
        }
        else
        {
            dragSpeedChange = (IsGrounded ? GroundDragAcceleration : AirDragAcceleration) * Time.fixedDeltaTime;
        }
        // Apply input and drag, clamping final horizontal speed to [0, maxSpeed]
        horizVelocity += InputDir.x * MoveAcceleration * Time.fixedDeltaTime;
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
    }

    // Update is called once per frame
    void Update()
    {
        // Do visual/gameplay stuff
    }
}
