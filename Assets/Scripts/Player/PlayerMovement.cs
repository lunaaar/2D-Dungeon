using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [HideInInspector]
    public AnimationScript animator;
    public SpriteRenderer spriteRenderer;
    public Rigidbody2D rigidBody;

    public ContactFilter2D movementFilter;
    //Sets stuff for  what layers to ignore
    private List<RaycastHit2D> castCollisions = new List<RaycastHit2D>();
    public float magicnumber = 8;

    [Space]
    [Header("Movement Stats")]
    public float speed = 5;
    public float rollSpeed = 20;

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
        spriteRenderer = transform.GetChild(0).GetComponent<SpriteRenderer>();
        animator = GetComponent<AnimationScript>();
        rigidBody = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");
        float xRaw = Input.GetAxisRaw("Horizontal");
        float yRaw = Input.GetAxisRaw("Vertical");

        Vector2 direction = new Vector2(xRaw, yRaw);

        bool moveable = MovePlayer(direction, speed * Time.deltaTime * magicnumber);
        if (moveable)
        {
            Walk(direction);

        } else if(MovePlayer(new Vector2(direction.x, 0), speed * Time.deltaTime * magicnumber))
        {
            Walk(new Vector2(direction.x, 0));
            
        } else if(MovePlayer(new Vector2(0, direction.y), speed * Time.deltaTime * magicnumber))
        {
            Walk(new Vector2(0, direction.y));
        
        } else
        {
            Walk(Vector2.zero);
        }

        if (Input.GetButtonDown("Jump") && !hasRolled)
        {
            if (xRaw != 0 || yRaw != 0)
            {

                if (MovePlayer(new Vector2(xRaw, yRaw), rollSpeed * Time.deltaTime * magicnumber))
                {
                    Roll(xRaw, yRaw);

                }
                else if (MovePlayer(new Vector2(xRaw, 0), speed * Time.deltaTime * magicnumber))
                {
                    Roll(xRaw, 0);

                }
                else if (MovePlayer(new Vector2(0, yRaw), speed * Time.deltaTime * magicnumber))
                {
                    Roll(0, yRaw);

                }

            }
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

        spriteRenderer.color = new Color(0, 255, 0);

        rigidBody.velocity = Vector2.zero;
        Vector2 dash = new Vector2(x, y);

        rigidBody.velocity += dash.normalized * rollSpeed;

        StartCoroutine(RollWait(dash));

    }

    //Sorta from https://www.youtube.com/watch?v=05eWA0TP3AA
    public bool MovePlayer(Vector2 direction, float playerSpeed)
    {
        var result = new RaycastHit2D[10];
        int count = rigidBody.Cast(direction, movementFilter, result, playerSpeed);
        if (count == 0)
        {
            return true;
        } else
        {
            return false;
        }
    }

    IEnumerator RollWait(Vector2 dash)
    {
        StartCoroutine(GroundRoll());

        isRolling = true;
        //Duration of the roll
        for (float i = 0f; i < .3f; i += Time.fixedDeltaTime)
        {
            if (!MovePlayer(dash, rollSpeed * Time.deltaTime))
                break;
            yield return new WaitForFixedUpdate();
        }
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