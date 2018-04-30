using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerMotor : NetworkBehaviour
{
    public struct Position
    {
        public double time;
        public Vector2 position;
    }

    [SerializeField] private float speed = 0.1f;
    [SerializeField] private float lerpRate = 0.15f;

    private List<Position> previousPositions = new List<Position>();
    private Rigidbody2D rb;
    private Transform t;

    PlayerAnimation playerAnim;

    private void Awake()
    {
        playerAnim = GetComponent<PlayerAnimation>();
        rb = GetComponent<Rigidbody2D>();
        t = transform;
    }

    [Command]
    public void Cmd_ServerMovePlayer(float _x, float _y)
    {
        playerAnim.x = (int)_x;
        playerAnim.y = (int)_y;

        MovePlayer(_x, _y);
        Rpc_ClientsMovePlayer(rb.position);
    }

    [ClientRpc]
    private void Rpc_ClientsMovePlayer(Vector2 _position)
    {
        if (isLocalPlayer == false)
            MovePlayer(_position);
        else if (isLocalPlayer)
            ReconcileToServer(_position);
    }

    private void MovePlayer(float _x, float _y)
    {
        rb.MovePosition(new Vector2(rb.position.x + (_x * speed), rb.position.y + (_y * speed)));
    }

    private void MovePlayer(Vector2 _position)
    {
        rb.MovePosition(Vector2.Lerp(rb.position, _position, lerpRate));
    }

    public void MovementPrediction(float _x, float _y)
    {
        Position pos = new Position();
        pos.time = Network.time;

        Vector2 position = new Vector2(rb.position.x + (_x * speed), rb.position.y + (_y * speed));

        rb.MovePosition(Vector2.Lerp(rb.position, position, lerpRate));

        pos.position = t.position;
        previousPositions.Add(pos);
    }

    private void ReconcileToServer(Vector2 _position)
    {
        for (int i = 0; i < previousPositions.Count; i++)
        {
            if (_position == previousPositions[i].position)
            {
                previousPositions.RemoveRange(0, i);
                return;
            }
            else
            {
                previousPositions.Clear();
                MovePlayer(_position);
            }
        }
    }
}