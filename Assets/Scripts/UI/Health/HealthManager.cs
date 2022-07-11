using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthManager : MonoBehaviour
{
    public GameObject heartPrefab;

    public int health, maxHealth;

    List<HealthHeart> hearts = new List<HealthHeart>();

    private void Start()
    {
        DrawHearts();
    }

    public void DrawHearts()
    {
        ClearHearts();

        float maxHealthRemainder = maxHealth % 2;
        int heartsToCreate = (int)(maxHealth / 2 + maxHealthRemainder);

        for (int i = 0; i < heartsToCreate; i++)
        {
            CreateEmptyHeart();
        }

        for (int i = 0; i < hearts.Count; i++)
        {
            int heartStatus = (int)Mathf.Clamp(health - (i * 2), 0, 2);
            hearts[i].SetHeartImage((HeartStatus)heartStatus);
        }
    }

    public void CreateEmptyHeart()
    {
        GameObject newHeart = Instantiate(heartPrefab);

        newHeart.transform.SetParent(transform);

        HealthHeart heartComponent = newHeart.GetComponent<HealthHeart>();
        heartComponent.SetHeartImage(HeartStatus.Empty);
        hearts.Add(heartComponent);
    }
    
    public void ClearHearts()
    {
        foreach(Transform t in transform)
        {
            Destroy(t.gameObject);
        }
        hearts = new List<HealthHeart>();
    }

    //This could me migrated to a player manager script
    public void TakeDamage(int amount)
    {
        health -= amount;

        if (health > maxHealth) health = maxHealth;

        if (health <= 0) health = 0;

        DrawHearts();
    }

    //This is here for testing

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.N))
        {
            TakeDamage(1);
        }
        if (Input.GetKeyDown(KeyCode.M))
        {
            TakeDamage(-1);
        }
    }
}
