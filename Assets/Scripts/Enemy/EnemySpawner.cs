using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    //Which enemy prefab to spawn (Random for now but should be defined lated on)
    //0-4 are zombies, 0 being small 4 being large. 5-6 are swampy/muddy. 7-11 are orcs. 7 being a goblin, 11 being ogre. 12-16 are demons, 12 being imp, 16 being demon
    public int enemyNumb = 0;
    public GameManager gamemanager;
    public GameObject enemy;

    // Start is called before the first frame update
    void Awake()
    {
        gamemanager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        //Right now its just some random enemy that gets spawned
        if(enemyNumb == -1)
        {
            enemyNumb = Random.Range(0, 16);
        }
        GameObject spawnedEnemy = Instantiate(enemy);
        spawnedEnemy.transform.parent = this.transform;
        spawnedEnemy.transform.position = this.transform.position;
    }

    public int getEnemyNumb()
    {
        return enemyNumb;
    }
}
