using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInputManager : MonoBehaviour
{
    private PlayerController2D Controller;

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
        if (AimDir != Vector2.zero)
        {
            Controller.SetAimInputDirection(AimDir);
        }
		else
		{
            Vector2 MousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3 PlayerPos = Controller.gameObject.transform.position;
            Controller.SetAimInputDirection(MousePos - new Vector2(PlayerPos.x, PlayerPos.y));
		}

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
