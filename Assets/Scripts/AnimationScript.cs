using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationScript : MonoBehaviour
{
    private Animator animator;
    private PlayerMovement playerMove;

    public SpriteRenderer spriteRenderer;
    
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        playerMove = GetComponent<PlayerMovement>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {

        animator.SetInteger("movement", (int)playerMove.rigidBody.velocity.magnitude);

        animator.SetBool("isRolling", playerMove.isRolling);
        animator.SetBool("isBlocking", playerMove.isBlocking);
    }

    public void Flip(int side)
    {
        bool state = (side == 1) ? false : true;
        spriteRenderer.flipX = state;
    }
}
