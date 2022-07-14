using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationScript : MonoBehaviour
{
    private Animator animator;
    private PlayerMovement playerMove;
    private Rigidbody2D rigidbody2Dvar;
    private SpriteRenderer spriteRenderer;

    public GameObject[] playerPrefab;
    public GameObject gameManager;

    // Start is called before the first frame update
    void Awake()
    {
        
        
        //Gets which player to select off of a given number (0 is M lizard, 1 is F lizard, 2 is F elf, 3 is M elf, 4 is F knight, 5 is M knight, 6 is F wizard, 7 is M wizard, 8 is Doctor, 9 is pumkin)
        GameObject selectedPrefab = Instantiate(playerPrefab[gameManager.GetComponent<GameManager>().getSelectedCharacter()]);
        
        //Sets starting position of the player
        selectedPrefab.transform.position = Vector2.zero;
        selectedPrefab.transform.parent = this.transform;


        animator = GetComponentInChildren<Animator>();
        playerMove = GetComponent<PlayerMovement>();        
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        rigidbody2Dvar = spriteRenderer.gameObject.GetComponent<Rigidbody2D>();


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