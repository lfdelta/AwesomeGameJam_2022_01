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
        Controller.SetInputDirection(MoveDir);

        if (Input.GetButtonDown("Jump"))
		{
            Controller.StartJump();
		}
        else if (Input.GetButtonUp("Jump"))
		{
            Controller.EndJump();
		}
    }
}
