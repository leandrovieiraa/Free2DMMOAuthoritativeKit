using UnityEngine;
using UnityEngine.Networking;

public class PlayerAnimation : NetworkBehaviour
{
    [SyncVar] public int x;
    [SyncVar] public int y;

    private Animator anim;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        AnimateMovement(x, y);
    }

    public void AnimateMovement(int _x, int _y)
    {
        anim.SetFloat("x", _x);
        anim.SetFloat("y", _y);
    }

    public void SlashUpAnimation()
    {
        anim.SetFloat("x", 0);
        anim.SetFloat("y", 1);
    }
}