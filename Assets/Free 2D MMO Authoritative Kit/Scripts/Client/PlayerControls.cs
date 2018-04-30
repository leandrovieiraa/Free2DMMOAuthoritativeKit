using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerControls : NetworkBehaviour
{

    private PlayerMotor movement;
    private PlayerAnimation playerAnim;

    private void Awake()
    {
        movement = GetComponent<PlayerMotor>();
        playerAnim = GetComponent<PlayerAnimation>();
    }

    private void Update()
    {
        if (isLocalPlayer)
        {
            float x = Input.GetAxisRaw("Horizontal");
            float y = Input.GetAxisRaw("Vertical");

            if (x == 0 && y == 0)
            {
                if (playerAnim.x != 0 || playerAnim.y != 0)
                    playerAnim.AnimateMovement(0, 0);
            }
            else
                playerAnim.AnimateMovement((int)x, (int)y);

            movement.MovementPrediction(x, y);
            movement.Cmd_ServerMovePlayer(x, y);

        }
    }
}