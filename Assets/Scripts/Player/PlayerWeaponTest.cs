using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeaponTest : MonoBehaviour
{
    public GameObject weapon;
    private SpriteRenderer spriteRenderer;
    private Transform weaponObj;
    private Transform helperObj;
    private Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        GameObject addedWeapon = Instantiate(weapon);

        addedWeapon.transform.position = this.transform.position;
        addedWeapon.transform.parent = transform.GetChild(0).transform;
        weaponObj = transform.GetChild(0).GetChild(0);
        animator = weaponObj.GetComponent<Animator>();
        helperObj = weaponObj.Find("helper");
        spriteRenderer = helperObj.GetComponentInChildren<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        float x = Input.GetAxis("Horizontal");
        if(x != 0f)
        {
            if (x > 0)
            {
                spriteRenderer.flipX = false;
                weaponObj.localScale = new Vector3(1, 1, 1);
                helperObj.localScale = new Vector3(1, 1, 1);
            }
            else
            {
                spriteRenderer.flipX = true;
                weaponObj.localScale = new Vector3(-1, 1, 1);
                helperObj.localScale = new Vector3(-1, 1, 1);
            }
        }
        //Gets where the mouse is. If its >90 or <-90 than the mouse is on the right side
        var mouseAngle = Mathf.Atan2(Camera.main.ScreenToWorldPoint(Input.mousePosition).y - transform.position.y, Camera.main.ScreenToWorldPoint(Input.mousePosition).x - transform.position.x) * (180/Mathf.PI);
        if(mouseAngle > 90 || mouseAngle < -90)
        {
            if(mouseAngle > 90)
            {
                transform.GetChild(0).GetChild(0).eulerAngles = new Vector3(transform.GetChild(0).GetChild(0).rotation.x, transform.GetChild(0).GetChild(0).rotation.y, mouseAngle - 180);
            } else
            {
                transform.GetChild(0).GetChild(0).eulerAngles = new Vector3(transform.GetChild(0).GetChild(0).rotation.x, transform.GetChild(0).GetChild(0).rotation.y, mouseAngle + 180);
            }
            spriteRenderer.flipX = true;
            weaponObj.localScale = new Vector3(-1, 1, 1);
            helperObj.localScale = new Vector3(-1, 1, 1);
            GetComponent<AnimationScript>().Flip(-1);
        } else
        {
            transform.GetChild(0).GetChild(0).eulerAngles = new Vector3(transform.GetChild(0).GetChild(0).rotation.x, transform.GetChild(0).GetChild(0).rotation.y, mouseAngle);
            spriteRenderer.flipX = false;
            weaponObj.localScale = new Vector3(1, 1, 1);
            helperObj.localScale = new Vector3(1, 1, 1);
            GetComponent<AnimationScript>().Flip(1);
        }
        
        if (Input.GetMouseButtonDown(0))
        {
            animator.SetTrigger("Attack");
        }
    }
}
