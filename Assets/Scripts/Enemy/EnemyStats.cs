using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class EnemyStats : MonoBehaviour
{
    public int health = 0;
    public int damage = 0;
    public float attackSpeed = 0;
    public float speed = 0;
    public int size = 0;
    private CapsuleCollider2D hitbox;
    public AIPath aiSpeed;
    public void Start()
    {
        hitbox = GetComponent<CapsuleCollider2D>();
        setStats(transform.parent.GetComponent<EnemySpawner>().getEnemyNumb());
    }

    public void Update()
    {
        if(health <= 0)
        {
            Destroy(gameObject);
        }   
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Debug.Log(collision.gameObject.tag + "Trigger");
        if (collision.gameObject.tag == "PlayerAttack")
        {
            health--;
        }
    }


    public void setStats(int monsterNumb)
    {
        switch (monsterNumb)
        {
            //Tiny Zombie
            case 0:
                health = 1;
                damage = 1;
                attackSpeed = 2;
                aiSpeed.maxSpeed = 1.6f * 3;
                size = 0;
                hitbox.size = new Vector2(.5f, .5f);
                hitbox.offset = new Vector2(0f, -0.25f);
                break;
            //Zombie
            case 1:
                health = 3;
                damage = 1;
                attackSpeed = 1;
                aiSpeed.maxSpeed = 1 * 3;
                size = 1;
                hitbox.size = new Vector2(.75f, 1f);
                hitbox.offset = new Vector2(0f, -0.25f);
                break;
            //Skeleton
            case 2:
                health = 2;
                damage = 1;
                attackSpeed = 1.2f;
                aiSpeed.maxSpeed = 1.2f * 3;
                size = 1;
                hitbox.size = new Vector2(.75f, 1f);
                hitbox.offset = new Vector2(0f, -0.25f);
                break;
            //Ice Zombie
            case 3:
                health = 3;
                damage = 2;
                attackSpeed = 1;
                aiSpeed.maxSpeed = 0.8f * 3;
                size = 1;
                hitbox.size = new Vector2(.75f, 1f);
                hitbox.offset = new Vector2(0f, -0.25f);
                break;
            //Big Zombie
            case 4:
                health = 12;
                damage = 2;
                attackSpeed = .2f;
                aiSpeed.maxSpeed = .5f * 3;
                size = 2;
                hitbox.size = new Vector2(1, 1.5f);
                hitbox.offset = new Vector2(0f, -0.5f);
                break;
            //Swampy
            case 5:
                health = 3;
                damage = 2;
                attackSpeed = 1;
                aiSpeed.maxSpeed = .8f * 3;
                size = 1;
                hitbox.size = new Vector2(.75f, 1f);
                hitbox.offset = new Vector2(0f, -0.25f);
                break;
            //Muddy Isle
            case 6:
                health = 3;
                damage = 1;
                attackSpeed = 2;
                aiSpeed.maxSpeed = 0.8f * 3;
                size = 1;
                hitbox.size = new Vector2(.75f, 1f);
                hitbox.offset = new Vector2(0f, -0.25f);
                break;
            //Goblin
            case 7:
                health = 3;
                damage = 1;
                attackSpeed = 1;
                aiSpeed.maxSpeed = 1.6f * 3;
                size = 0;
                hitbox.size = new Vector2(.5f, .5f);
                hitbox.offset = new Vector2(0f, -0.25f);
                break;
            //Orc Warrior
            case 8:
                health = 3;
                damage = 1;
                attackSpeed = 1;
                aiSpeed.maxSpeed = 1 * 3;
                size = 1;
                hitbox.size = new Vector2(.75f, 1f);
                hitbox.offset = new Vector2(0f, -0.25f);
                break;
            //Masked Orc
            case 9:
                health = 3;
                damage = 1;
                attackSpeed = 1;
                aiSpeed.maxSpeed = 1.1f * 3;
                size = 1;
                hitbox.size = new Vector2(.75f, 1f);
                hitbox.offset = new Vector2(0f, -0.25f);
                break;
            //Orc Shaman
            case 10:
                health = 2;
                damage = 1;
                attackSpeed = 2;
                aiSpeed.maxSpeed = 1.3f * 3;
                size = 1;
                hitbox.size = new Vector2(.75f, 1f);
                hitbox.offset = new Vector2(0f, -0.25f);
                break;
            //Ogre
            case 11:
                health = 15;
                damage = 2;
                attackSpeed = .4f;
                aiSpeed.maxSpeed = .6f * 3;
                size = 2;
                hitbox.size = new Vector2(1, 1.5f);
                hitbox.offset = new Vector2(0f, -0.5f);
                break;
            //Imp
            case 12:
                health = 2;
                damage = 1;
                attackSpeed = 2;
                aiSpeed.maxSpeed = 2f * 3;
                size = 0;
                hitbox.size = new Vector2(.5f, .5f);
                hitbox.offset = new Vector2(0f, -0.25f);
                break;
            //Chort
            case 13:
                health = 3;
                damage = 1;
                attackSpeed = 2;
                aiSpeed.maxSpeed = .8f * 3;
                size = 1;
                hitbox.size = new Vector2(.75f, 1f);
                hitbox.offset = new Vector2(0f, -0.25f);
                break;
            //Wogol
            case 14:
                health = 1;
                damage = 1;
                attackSpeed = 2;
                aiSpeed.maxSpeed = 1.1f * 3;
                size = 1;
                hitbox.size = new Vector2(.75f, 1f);
                hitbox.offset = new Vector2(0f, -0.25f);
                break;
            //Necromancer
            case 15:
                health = 1;
                damage = 1;
                attackSpeed = 2;
                aiSpeed.maxSpeed = 1.2f * 3;
                size = 1;
                hitbox.size = new Vector2(.75f, 1f);
                hitbox.offset = new Vector2(0f, -0.25f);
                break;
            //Big Demon
            case 16:
                health = 20;
                damage = 3;
                attackSpeed = .2f;
                aiSpeed.maxSpeed = 1f * 3;
                size = 2;
                hitbox.size = new Vector2(1, 1.5f);
                hitbox.offset = new Vector2(0f, -0.5f);
                break;

            default:
                Debug.Log("Something went wrong");
                break;
        }
    }
}
