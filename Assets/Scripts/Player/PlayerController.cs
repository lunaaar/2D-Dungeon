using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    public GameObject health;
    public GameObject gameManager;
    private HealthManager healthManager;
    private float iframes;
    private bool hit = false;

    // Start is called before the first frame update
    void Start()
    {
        health = GameObject.FindGameObjectWithTag("UI").gameObject.transform.GetChild(0).gameObject;
        healthManager = health.GetComponent<HealthManager>();
        gameManager = GameObject.FindGameObjectWithTag("GameController");
    }

    // Update is called once per frame
    void Update()
    {
        if(hit)
        {
            iframes += Time.deltaTime;
        }
        if(iframes > 1)
        {
            iframes = 0;
            hit = false;
        }
        if (healthManager.health < 1)
        {
            Destroy(gameObject);
            //TriggerGameOverScrene
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        
        if (collision.gameObject.tag == "EnemyAttack" && !hit)
        {
            healthManager.TakeDamage(collision.gameObject.transform.parent.parent.parent.GetComponent<EnemyStats>().damage);
            hit = true;
        }
    }
}
