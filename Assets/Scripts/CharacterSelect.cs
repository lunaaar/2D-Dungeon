using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSelect : MonoBehaviour
{
    public List<GameObject> playerPrefabs = new List<GameObject>(8);

    public GameObject background;

    private GameObject selectedCharacter;
    private int activeIndex;

    // Start is called before the first frame update
    void Start()
    {
        selectedCharacter = playerPrefabs[0];
        activeIndex = 0;
    }

    
    
    // Update is called once per frame
    void FixedUpdate()
    {
        if (Input.GetKey(KeyCode.RightArrow))
        {
            activeIndex += 1;
            if (activeIndex == 8)
            {
                activeIndex = 0;
            }
            
            selectedCharacter = playerPrefabs[activeIndex];
            background.transform.position = playerPrefabs[activeIndex].transform.position; 
        }

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            activeIndex -= 1;
            if (activeIndex == -1)
            {
                activeIndex = 7;
            }

            selectedCharacter = playerPrefabs[activeIndex];
            background.transform.position = playerPrefabs[activeIndex].transform.position;
        }
    }
}
