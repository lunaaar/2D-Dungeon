using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWeaponScript : MonoBehaviour
{
    public List<GameObject> enemyWeapon;

    //set these to private after testing
    //public SpriteRenderer spriteRenderer;
    public Transform weaponObj;
    public Transform helperObj;
    public Animator animator;
    public SpriteRenderer spriteRenderer;

    // Start is called before the first frame update
    void Start()
    {
        GameObject selectedWeapon = Instantiate(enemyWeapon[GetComponent<EnemyStats>().size]);
        selectedWeapon.transform.position = this.transform.position;
        selectedWeapon.transform.parent = transform.GetChild(0).transform;
        weaponObj = transform.GetChild(0).GetChild(0).GetChild(0);
        animator = weaponObj.GetComponent<Animator>();
        spriteRenderer = weaponObj.GetComponentInChildren<SpriteRenderer>();
        helperObj = transform.GetChild(0).GetChild(0);
    }

    public void attack(Vector3 target)
    {
        if (this.transform.position.x - target.x < 0)
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
        var targetAngle = Mathf.Atan2(target.y - transform.position.y, target.x - transform.position.x) * (180 / Mathf.PI);
        if (targetAngle > 90 || targetAngle < -90)
        {
            if (targetAngle > 90)
            {
                transform.GetChild(0).GetChild(0).eulerAngles = new Vector3(transform.GetChild(0).GetChild(0).rotation.x, transform.GetChild(0).GetChild(0).rotation.y, targetAngle - 180);
            }
            else
            {
                transform.GetChild(0).GetChild(0).eulerAngles = new Vector3(transform.GetChild(0).GetChild(0).rotation.x, transform.GetChild(0).GetChild(0).rotation.y, targetAngle + 180);
            }
            
        }
        else
        {
            transform.GetChild(0).GetChild(0).eulerAngles = new Vector3(transform.GetChild(0).GetChild(0).rotation.x, transform.GetChild(0).GetChild(0).rotation.y, targetAngle);
            
        }
        animator.SetTrigger("Attack");

    }
}
