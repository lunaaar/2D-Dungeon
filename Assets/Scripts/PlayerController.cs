using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Vector3 movement;
    //0 = elf, 1 = knight, 2 = lizard, 3 = wizard, 4 = doctor, 5 = pumkin
    public int playerClass;
    private bool playerSpecialActive;
    private Vector3 rollTowards = new Vector3(0,0,0);

    // Start is called before the first frame update
    void Start()
    {
        playerSpecialActive = false;
    }

    // Update is called once per frame
    void Update()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        movement = new Vector3(horizontal, vertical, 0f);

        if (movement.magnitude > 1.0f)
        {
            movement = movement.normalized;
        }

        if (!playerSpecialActive)
        {
            if (movement != Vector3.zero)
            {
                if (!GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Run"))
                    GetComponent<Animator>().Play("Run");
            }
            else
            {
                if (!GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Idle"))
                    GetComponent<Animator>().Play("Idle");
            }
        }

        //If rightclick (Actives)
        if (Input.GetMouseButtonDown(1))
        {
            switch (playerClass)
            {
                //Elf: Dodge roll
                case 0:
                    ////Do a dodge roll
                    if (!playerSpecialActive)
                    {
                        GetComponent<Animator>().Play("Roll");
                        playerSpecialActive = true;

                        //mafs to figure out what direction the roll goes
                        var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                        var xAmount = mousePos.x - transform.position.x;
                        var yAmount = mousePos.y - transform.position.y;
                        rollTowards = new Vector3(1 * Mathf.Sign(xAmount), Mathf.Abs(yAmount / xAmount) * Mathf.Sign(yAmount), 0).normalized;
                    }



                    break;

                //Knight: Block?
                case 1:
                    break;

                //Lizard: Invisibility?
                case 2:
                    break;

                //Wizard: ???
                case 3:
                    break;

                //Doctor: Potion switch?
                case 4:
                    break; 

                //Pumpkin: Raise dead?
                case 5:
                    break;

                default:
                    Debug.Log("Something went wrong");
                    break;
            }
        }
        
        
        




    }

    private void LateUpdate()
    {
        switch (playerClass)
        {
            //Elf
            case 0:
                ////Do a dodge roll
                if (GetComponent<Animator>().GetCurrentAnimatorClipInfo(0)[0].clip.name != "Roll")
                {
                    playerSpecialActive = false;
                }
                else
                {
                    movement = rollTowards;
                }


                break;

            //Knight
            case 1:
                break;

            //Lizard
            case 2:
                break;

            //Wizard
            case 3:
                break;

            //Doctor
            case 4:
                break;

            //Pumpkin
            case 5:
                break;

            default:
                Debug.Log("Something went wrong");
                break;
        }
        transform.position = transform.position + movement * Time.deltaTime * 7f;

    }
}
