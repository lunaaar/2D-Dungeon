using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class EnemyAnimationScript : MonoBehaviour
{
    private Animator animator;
    private Rigidbody2D rigidbody2Dvar;
    private SpriteRenderer spriteRenderer;
    private Vector3 lastPos;

    public GameObject[] enemyPrefab;
    

    private void Start()
    {
        GameObject selectedEnemy = Instantiate(enemyPrefab[transform.parent.GetComponent<EnemySpawner>().getEnemyNumb()]);

        selectedEnemy.transform.parent = this.transform;
        selectedEnemy.transform.position = this.transform.position;

        animator = GetComponentInChildren<Animator>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        rigidbody2Dvar = spriteRenderer.gameObject.GetComponent<Rigidbody2D>();
        lastPos = transform.position;

    }



    // Update is called once per frame
    void Update()
    {
        //Flips the sprite based on the direction it is walking
        //Debug.Log(transform.position.x - lastPos.x);
        if(transform.position.x - lastPos.x > 0)
        {
            spriteRenderer.flipX = false;
        }
        else if (transform.position.x - lastPos.x < 0)
        {
            spriteRenderer.flipX = true;
        }
        lastPos = transform.position;
    }

    public Animator getAnimator()
    {
        return animator;
    }

    public void Flip()
    {
        spriteRenderer.flipX = !spriteRenderer.flipX;
    }
}
