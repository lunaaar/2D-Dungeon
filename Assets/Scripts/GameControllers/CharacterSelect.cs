using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class CharacterSelect : MonoBehaviour
{
    public List<GameObject> playerPrefabs = new List<GameObject>(8);

    public GameObject background;
    public GameObject gameManagerObject;

    private GameObject selectedCharacter;
    private Image image;
    private Animator anim;
    private int activeIndex;

    public float coolDownInSeconds;
    private float currentTime;

    // Start is called before the first frame update
    void Start()
    {
        selectedCharacter = playerPrefabs[0];
        activeIndex = 0;

        image = selectedCharacter.GetComponent<Image>();
        anim = selectedCharacter.GetComponent<Animator>();
        anim.Play("Hover Menu");

        image.color = new Color32(255, 255, 255, 255);
    }

    
    
    // Update is called once per frame
    void FixedUpdate()
    {

        //image.GetComponent<Image>().color = new Color32(255, 255, 225, 100);

        if (Time.time > currentTime)
        {
            image = selectedCharacter.GetComponent<Image>();
            anim = selectedCharacter.GetComponent<Animator>();

            if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
            {
                
                image.color = new Color32(90, 90, 90, 255);
                anim.Play("Non Hover Menu");

                activeIndex += 1;
                activeIndex = activeIndex % 8;

                selectedCharacter = playerPrefabs[activeIndex];

                image = selectedCharacter.GetComponent<Image>();
                image.color = new Color32(255, 255, 255, 255);

                anim = selectedCharacter.GetComponent<Animator>();
                anim.Play("Hover Menu");

                // Tried MoveTowards, didn't give desired result. Speed ended up getting to a point of too high, and still didn't feel right because
                // Position moved too fast for the movetowards. I need to figure out how to set a delay.
                //background.transform.position = Vector3.MoveTowards(background.transform.position, playerPrefabs[activeIndex].transform.position, Time.deltaTime * speed);

                background.transform.position = playerPrefabs[activeIndex].transform.position;
                currentTime = Time.time + coolDownInSeconds;
            }


            if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
            {
                image.color = new Color32(90, 90, 90, 255);
                anim.Play("Non Hover Menu");

                if (activeIndex - 1 < 0)
                    activeIndex = 7;
                else
                    activeIndex -= 1;

                selectedCharacter = playerPrefabs[activeIndex];

                image = selectedCharacter.GetComponent<Image>();
                image.color = new Color32(255, 255, 255, 255);

                anim = selectedCharacter.GetComponent<Animator>();
                anim.Play("Hover Menu");

                background.transform.position = playerPrefabs[activeIndex].transform.position;

                currentTime = Time.time + coolDownInSeconds;
            }


            if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
            {
                image.color = new Color32(90, 90, 90, 255);
                anim.Play("Non Hover Menu");

                activeIndex += 4;
                activeIndex = activeIndex % 8;

                selectedCharacter = playerPrefabs[activeIndex];

                image = selectedCharacter.GetComponent<Image>();
                image.color = new Color32(255, 255, 255, 255);

                anim = selectedCharacter.GetComponent<Animator>();
                anim.Play("Hover Menu");

                background.transform.position = playerPrefabs[activeIndex].transform.position;

                currentTime = Time.time + coolDownInSeconds;
            }

            if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
            {
                image.color = new Color32(90, 90, 90, 255);
                anim.Play("Non Hover Menu");

                if (activeIndex - 4 < 0)
                    activeIndex += 4;
                else
                    activeIndex -= 4;

                activeIndex = activeIndex % 8;

                selectedCharacter = playerPrefabs[activeIndex];

                image = selectedCharacter.GetComponent<Image>();
                image.color = new Color32(255, 255, 255, 255);

                anim = selectedCharacter.GetComponent<Animator>();
                anim.Play("Hover Menu");

                background.transform.position = playerPrefabs[activeIndex].transform.position;

                currentTime = Time.time + coolDownInSeconds;
            }
            
            if (Input.GetKey(KeyCode.Space))
            {
                var gameManager = gameManagerObject.GetComponent<GameManager>(); ;

                gameManager.characterSelected(activeIndex);
                gameManager.gameSwitch();
            }
        }
        
    }
}
