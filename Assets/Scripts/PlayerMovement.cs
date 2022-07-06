using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [HideInInspector]
    public Rigidbody2D rigidBody;
    private SpriteRenderer spriteRenderer;
    private AnimationScript animator;

    [Space]
    [Header("Movement Stats")]
    public float speed = 5;
    public float rollSpeed = 10;

    [Space]
    [Header("Booleans")]
    public bool canMove;
    public bool isRolling;
    public bool isBlocking;
    public bool isInvisible;

    [Space]
    public int side = 1;

    [Space]
    private bool hasRolled;


    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(DisableMovement(.01f));

        //rigidBody = GetComponent<Rigidbody2D>();
        //spriteRenderer = GetComponent<SpriteRenderer>();
        //animator = GetComponent<AnimationScript>();

        rigidBody = GetComponentInChildren<Rigidbody2D>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        animator = GetComponentInChildren<AnimationScript>();
    }

    // Update is called once per frame
    void Update()
    {
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");
        float xRaw = Input.GetAxisRaw("Horizontal");
        float yRaw = Input.GetAxisRaw("Vertical");

        Vector2 direction = new Vector2(xRaw, yRaw);

        Walk(direction);

        if (Input.GetButtonDown("Jump") && !hasRolled)
        {
            if (xRaw != 0 || yRaw != 0)
            {
                Roll(xRaw, yRaw);
            }
        }

        if (x > 0)
        {
            side = 1;
            animator.Flip(side);
        }
        if (x < 0)
        {
            side = -1;
            animator.Flip(side);
        }

    }

    private void Walk(Vector2 dir)
    {
        if (!canMove) return;
        if (isRolling) return;

        if (isBlocking)
        {
            rigidBody.velocity = dir * (speed / 2);
        }
        else
        {
            rigidBody.velocity = dir.normalized * speed;
        }

    }

    private void Roll(float x, float y)
    {
        hasRolled = true;

        spriteRenderer.color = new Color(255, 0, 0);

        rigidBody.velocity = Vector2.zero;
        Vector2 dash = new Vector2(x, y);

        rigidBody.velocity += dash.normalized * rollSpeed;

        StartCoroutine(RollWait(x,y));

    }

    IEnumerator RollWait(float x, float y)
    {
        StartCoroutine(GroundRoll());
        
        isRolling = true;
        //Duration of the roll
        yield return new WaitForSeconds(.3f);
        spriteRenderer.color = new Color(255, 255, 255);
        isRolling = false;
    }

    IEnumerator GroundRoll()
    {
        //Works as the cooldown for when you can dash next.
        yield return new WaitForSeconds(.15f);
        hasRolled = false;
    }

    IEnumerator DisableMovement(float time)
    {
        canMove = false;
        yield return new WaitForSeconds(time);
        canMove = true;
    }
}
