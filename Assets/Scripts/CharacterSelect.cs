using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSelect : MonoBehaviour
{
    public List<GameObject> playerPrefabs = new List<GameObject>(8);

    public GameObject background;

    private GameObject selectedCharacter;
    private int activeIndex;

    public float coolDownInSeconds;
    private float currentTime;

    // Start is called before the first frame update
    void Start()
    {
        selectedCharacter = playerPrefabs[0];
        activeIndex = 0;
    }

    
    
    // Update is called once per frame
    void FixedUpdate()
    {
        if (Time.time > currentTime)
        {
            if (Input.GetKey(KeyCode.RightArrow))
            {
                activeIndex += 1;
                activeIndex = activeIndex % 8;

                selectedCharacter = playerPrefabs[activeIndex];

                // Tried MoveTowards, didn't give desired result. Speed ended up getting to a point of too high, and still didn't feel right because
                // Position moved too fast for the movetowards. I need to figure out how to set a delay.
                //background.transform.position = Vector3.MoveTowards(background.transform.position, playerPrefabs[activeIndex].transform.position, Time.deltaTime * speed);

                background.transform.position = playerPrefabs[activeIndex].transform.position;
                currentTime = Time.time + coolDownInSeconds;
            }


            if (Input.GetKey(KeyCode.LeftArrow))
            {
                if (activeIndex - 1 < 0)
                    activeIndex = 7;
                else
                    activeIndex -= 1;

                selectedCharacter = playerPrefabs[activeIndex];
                background.transform.position = playerPrefabs[activeIndex].transform.position;

                currentTime = Time.time + coolDownInSeconds;
            }


            if (Input.GetKey(KeyCode.DownArrow))
            {
                activeIndex += 4;
                activeIndex = activeIndex % 8;

                selectedCharacter = playerPrefabs[activeIndex];
                background.transform.position = playerPrefabs[activeIndex].transform.position;

                currentTime = Time.time + coolDownInSeconds;
            }

            if (Input.GetKey(KeyCode.UpArrow))
            {
                if (activeIndex - 4 < 0)
                    activeIndex += 4;
                else
                    activeIndex -= 4;

                activeIndex = activeIndex % 8;

                selectedCharacter = playerPrefabs[activeIndex];
                background.transform.position = playerPrefabs[activeIndex].transform.position;

                currentTime = Time.time + coolDownInSeconds;
            }
        }

    }
}
