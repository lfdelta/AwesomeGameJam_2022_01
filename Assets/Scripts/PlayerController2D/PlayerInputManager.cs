using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInputManager : MonoBehaviour
{
    private PlayerController2D Controller;
    private bool AimWithMouse = true;
    private Vector3 LastMousePosition = Vector3.zero;

    // Start is called before the first frame update
    void Start()
    {
        if (Controller == null)
		{
            Controller = FindObjectOfType<PlayerController2D>();
		}
    }

    public void BindToPlayer(PlayerController2D InPlayer)
	{
        Controller = InPlayer;
	}

    // Update is called once per frame
    void Update()
    {
        if (Controller == null)
		{
            return;
		}
        Vector2 MoveDir = new Vector2(Input.GetAxis("MoveHorizontal"), Input.GetAxis("MoveVertical"));
        Controller.SetMoveInputDirection(MoveDir);

        Vector2 AimDir = new Vector2(Input.GetAxis("AimHorizontal"), Input.GetAxis("AimVertical"));
        if (Input.mousePosition != LastMousePosition)
        {
            AimWithMouse = true;
            LastMousePosition = Input.mousePosition;
        }
        else if (AimWithMouse && AimDir != Vector2.zero)
		{
            AimWithMouse = false;
		}
        if (AimWithMouse)
        {
            // Override (zero-val) AimDir
            Vector2 MousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3 PlayerPos = Controller.gameObject.transform.position;
            AimDir = MousePos - new Vector2(PlayerPos.x, PlayerPos.y);
        }
        Controller.SetAimInputDirection(AimDir);

        if (Input.GetButtonDown("Jump"))
		{
            Controller.StartJump();
		}
        else if (Input.GetButtonUp("Jump"))
		{
            Controller.EndJump();
		}

        if (Input.GetButtonDown("Swing"))
		{
            Controller.StartSwing();
		}
        else if (Input.GetButtonUp("Swing"))
		{
            Controller.EndSwing();
		}
    }
}
