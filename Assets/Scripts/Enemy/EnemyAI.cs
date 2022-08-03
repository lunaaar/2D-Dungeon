using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class EnemyAI : MonoBehaviour
{
    State currentState;
    //Idle is the enemy looking around and wandering randomly. Attack is moving towards the player until close enough and then attacking
    const int IDLE = 0, WANDER = 1, CHAT = 2, CHASE = 3, ATTACK = 4, FLEE = 5;
    /*Enemy starts at Idle 
     * If nothing happens the enemy will wander
     * If nothing happens while wandering the ai will go back to idle
     * 
     * If the enemy spots the character while idle/wander, it will then chase the player
     * If the enemy gets close enough to the player while chasing them, it will attack
     * If at any state it sees the player while on low health, while also not having any allies nearby, it will flee
     * 
     * If the enemy spots another enemy (that isnt already chatting) while idle, it will then chat with that enemy
    */

    private GameManager gameManager;
    private AIDestinationSetter aStar;
    private GameObject waypoint;
    private GameObject player;
    private Animator animator;
    private AIPath aiPath;


    private float idleTime = 0;
    private float attackTime = 0;
    private float rotateTimer = 0;
    private bool toLastKnown;

    public Vector3 targetPosition;
 
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    // Start is called before the first frame update
    void Start()
    {
        gameManager = transform.parent.GetComponent<GameManager>();
        aStar = GetComponent<AIDestinationSetter>();
        waypoint = transform.parent.GetChild(0).gameObject;
        player = GameObject.FindGameObjectWithTag("Player");
        aiPath = GetComponent<AIPath>();
        toLastKnown = false;

        State idle = new State(IDLE);
        State wander = new State(WANDER);
        State chat = new State(CHAT);
        State chase = new State(CHASE);
        State attack = new State(ATTACK);
        State flee = new State(FLEE);

        idle.transitions.Add(new Transition
        {
            condition = Idled,
            target = wander
        });

        idle.transitions.Add(new Transition
        {
            condition = TargetSpotted,
            target = chase
        });

        /*idle.transitions.Add(new Transition
        *{
        *    condition = FriendSpotted,
        *    target = chat
        *});
        */
        wander.transitions.Add(new Transition
        {
            condition = DestinationReached,
            target = idle
        });

        wander.transitions.Add(new Transition
        {
            condition = TargetSpotted,
            target = chase
        });

        /*chat.transitions.Add(new Transition
        *{
        *    condition = Chatted,
        *    target = idle
        *});
        */
        chat.transitions.Add(new Transition
        {
            condition = TargetSpotted,
            target = chase
        });
        
        chase.transitions.Add(new Transition
        {
            condition = TargetLost,
            target = idle
        });
        
        chase.transitions.Add(new Transition 
        {
            condition = TargetReached,
            target = attack
        });

        attack.transitions.Add(new Transition
        {
            condition = PlayerRan,
            target = chase
        });

        attack.transitions.Add(new Transition
        {
            condition = LowHealthAndAlone,
            target = flee
        });

        attack.transitions.Add(new Transition
        {
            condition = PlayerDead,
            target = idle
        });

        flee.transitions.Add(new Transition
        {
            condition = SafeDistance,
            target = idle
        });

        currentState = idle;

    }
















    // Update is called once per frame
    void Update()
    {
        
        // Debug.Log(GetComponentInChildren<SpriteRenderer>().flipX);

        //Debug.Log(currentState.id);
        switch (currentState.id)
        {
            case IDLE:
                //Looks around (-x or +x)
                if(rotateTimer <= 0)
                {
                    rotateTimer = Random.Range(.4f, 2.5f);
                    GetComponent<EnemyAnimationScript>().Flip();
                } else
                {
                    rotateTimer -= Time.deltaTime;
                }

                idleTime += Time.deltaTime;
                break;

            case WANDER:                
                break;

            //case CHAT:
                //Animation for speech bubbles
            //    break;

            case CHASE:
                if(player == null)
                {
                    break;
                }
                //Move towards the player's last seen position
                var hit = Physics2D.Linecast(transform.position, player.transform.position);
                Debug.DrawLine(transform.position, player.transform.position);
                var direction = 1;
                if (GetComponentInChildren<SpriteRenderer>().flipX)
                {
                    GetComponentInChildren<SpriteRenderer>().flipX = true;
                    direction = -1;
                    //Debug.Log("The sprite is facing to the left");
                }
                else
                {
                    GetComponentInChildren<SpriteRenderer>().flipX = false;
                    //Debug.Log("The sprite is facing to the right");
                }
                bool lookingAt = (direction >= 0 && player.transform.position.x - transform.position.x >= 0) || (direction <= 0 && player.transform.position.x - transform.position.x <= 0);

                if ((Vector3.Distance(transform.position, player.transform.position) > 15f || hit.collider.gameObject.tag != "Player") && !toLastKnown)
                {
                    toLastKnown = true;
                    waypoint.transform.position = player.transform.position;
                    aStar.target = waypoint.transform;
                    //Debug.Log("Target Lost!");
                }
                else if (hit.collider.gameObject.tag == "Player" && Vector3.Distance(transform.position, player.transform.position) < 10 && toLastKnown && lookingAt)
                {
                    aStar.target = player.transform;
                    toLastKnown = false;
                    //Debug.Log("Target Reaquired");
                }
                break;

            case ATTACK:
                //Plays attack animation
                if(attackTime >= .5f / GetComponent<EnemyStats>().attackSpeed)
                {
                    //Debug.Log("Attack Triggered     AttackTime = " + attackTime + "         Attack Time = " + (.5f / GetComponent<EnemyStats>().attackSpeed));
                    GetComponent<EnemyWeaponScript>().attack(player.transform.position);
                    attackTime = 0f;
                }
                attackTime += Time.deltaTime;
                break;

            case FLEE:
                //Keeps moving away from the player until it is x units away from them
                break;

            default:
                break;
        }
    }




















    private void LateUpdate()
    {
        //Debug.Log(currentState.id);
        if(animator == null)
        {
            animator = GetComponent<EnemyAnimationScript>().getAnimator();
            animator.SetFloat("MovementMulti", aiPath.maxSpeed / 4);
        }
        var newState = currentState.NextState();
        if (newState != currentState)
        {
            currentState = newState;
            switch (currentState.id) // Handle initializing the new state
            {
                case IDLE: Debug.Log("ENTERING: Idle!");
                    rotateTimer = 0;
                    aStar.target = transform;
                    animator.SetInteger("Movement", 0);
                    break;

                case WANDER: Debug.Log("ENTERING: Wander!");
                    /*Step 1: Determine facing direction
                 * Step 2: Get position using upper angle and lower angle (90, -90) as well as the lower radius and upper radius (Whatever you want)
                 * the position would then be (Vector2(Cos(lowerAngle),Sin(upperAngle)) * Random.range(lowerRadius, upperRadius))
                 * Step 3: See if that position is actually reachable using a linecast
                 * If not redo step 2
                */
                    var noTarget = true;
                    float theta = 0f;
                    float radius = 0f;
                    Vector3 unitVector = new Vector3();
                    var direction = 1;
                    if (GetComponentInChildren<SpriteRenderer>().flipX)
                    {
                        GetComponentInChildren<SpriteRenderer>().flipX = true;
                        direction = -1;
                        //Debug.Log("The sprite is facing to the left");
                    } else
                    {
                        GetComponentInChildren<SpriteRenderer>().flipX = false;
                        //Debug.Log("The sprite is facing to the right");
                    }
                    while (noTarget)
                    {
                        theta = Mathf.Deg2Rad * Random.Range(-90f, 90f);
                        radius = Random.Range(2f, 5f);
                        unitVector = new Vector3(Mathf.Cos(theta), Mathf.Sin(theta), 0) * direction;
                        targetPosition = unitVector * radius + transform.position;
                        waypoint.transform.position = unitVector * radius + transform.position;
                        if (Physics2D.Raycast(transform.position, unitVector, radius))
                        {
                            //Debug.Log("Hit something");
                        } 
                        else
                        {
                            aStar.target = waypoint.transform;
                            noTarget = false;
                            animator.SetInteger("Movement", 1);
                        }
                    }
                    break;

                //case CHAT:
                //    break;

                case CHASE: Debug.Log("ENTERING: Chase!");
                    aStar.target = player.transform;
                    animator.SetInteger("Movement", 1);
                    break;

                case ATTACK: Debug.Log("ENTERING: Attack!");
                    aStar.target = transform;
                    animator.SetInteger("Movement", 0);
                    break;

                case FLEE:
                    break;

            }
        }
        //Debug.DrawRay(transform.position, (targetPosition - transform.position), Color.white);
        //Debug.DrawLine(transform.position, targetPosition, Color.red);

    }













    bool Idled()
    {

        //Condition for idle -> walk
        if (idleTime > 8f)
        {
            idleTime = 0f;
            return true;
        }
        return false;
    }

    bool TargetSpotted()
    {
        if(player != null)
        {
            //Condition for x -> chase
            var direction = 1;
            if (GetComponentInChildren<SpriteRenderer>().flipX)
            {
                GetComponentInChildren<SpriteRenderer>().flipX = true;
                direction = -1;
                //Debug.Log("The sprite is facing to the left");
            }
            else
            {
                GetComponentInChildren<SpriteRenderer>().flipX = false;
                //Debug.Log("The sprite is facing to the right");
            }
            var hit = Physics2D.Linecast(transform.position, player.transform.position);
            bool canSee = hit.collider.gameObject.tag == "Player" && Vector3.Distance(transform.position, player.transform.position) < 10;
            bool lookingAt = (direction >= 0 && player.transform.position.x - transform.position.x >= 0) || (direction <= 0 && player.transform.position.x - transform.position.x <= 0);
            if (canSee && lookingAt)
            {
                return true;
            }
        }

        return false;
    }

    /*bool FriendSpotted()
    *{
    *    //Condition for idle -> chat
    *    return false;
    *}
    */
    bool DestinationReached()
    {
        //Condition for wander -> idle
        if(Vector3.Distance(waypoint.transform.position, transform.position) < .5 && !toLastKnown)
        {
            return true;
        }
        return false;
    }

    /*bool Chatted()
    *{a
    *    //Condition for chat -> idle
    *    return false;
    *}
    */

    bool TargetLost()
    {
        //Condition for chase -> idle
        //Debug.Log(toLastKnown);
        if (toLastKnown && Vector3.Distance(waypoint.transform.position, transform.position) < .5)
        {
            
            toLastKnown = false;
            Debug.Log("Went back to idle. toLastKnown is currently " + toLastKnown);
            return true;
        }
        return false;
    }

    bool TargetReached()
    {
        if(player != null)
        {
            //Condition for chase -> attack
            var hit = Physics2D.Linecast(transform.position, player.transform.position);
            bool canSee = hit.collider.gameObject.tag == "Player" && Vector3.Distance(transform.position, player.transform.position) < 10f;
            if (Vector3.Distance(transform.position, player.transform.position) < 1.5f * (GetComponent<EnemyStats>().size + 1) && canSee)
            {
                return true;
            }
        }
        return false;
    }

    bool PlayerRan()
    {
        if(player != null)
        {
            //Condition for attack -> chase
            var hit = Physics2D.Linecast(transform.position, player.transform.position);
            bool canSee = hit.collider.gameObject.tag == "Player" && Vector3.Distance(transform.position, player.transform.position) < 10f;
            if (Vector3.Distance(transform.position, player.transform.position) > 2.5f * (GetComponent<EnemyStats>().size + 1) || !canSee)
            {
                return true;
            }
        }
        return false;
    }

    bool LowHealthAndAlone()
    {
        //Condition for attack -> flee
        return false;
    }

    bool PlayerDead()
    {
        //Condition for attack -> idle
        if(player == null)
        {
            return true;
        }
        return false;
    }

    bool SafeDistance()
    {
        //Condition for flee -> idle
        return false;
    }
}
