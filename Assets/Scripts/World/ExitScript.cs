using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitScript : MonoBehaviour
{
    public GameObject mapGenerator;
    public MapGenerator mapGeneratorScript;
    public GameObject player;
    // Start is called before the first frame update
    void Start()
    {
        mapGenerator = GameObject.FindGameObjectWithTag("MapGenerator");
        mapGeneratorScript = mapGenerator.GetComponent<MapGenerator>();
        player = GameObject.FindGameObjectWithTag("Player");
    }

    // if the player presses the e key while near an exit they go to a new level
    void Update()
    {
        if (Input.GetKeyDown("e"))
        {
            Debug.Log(Vector3.Distance(transform.position, player.transform.position));
            if (Vector3.Distance(transform.position, player.transform.position) < 2)
            {
                
                mapGeneratorScript.generateDungeon();
            }
        }
    }
}
