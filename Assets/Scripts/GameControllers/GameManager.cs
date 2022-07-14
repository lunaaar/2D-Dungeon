using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    static int selectedCharacter = 0;
    static int selectedCursor = 3;
    public GameObject player;


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void characterSelected(int charNumber)
    {
        selectedCharacter = charNumber;
    }

    public int getSelectedCharacter()
    {
        return selectedCharacter;
    }

    public void cursorSelected(int cursNumber)
    {
        selectedCursor = cursNumber;
    }

    public int getSelectedCursor()
    {
        return selectedCursor;
    }

    public GameObject getPlayer()
    {
        return player;
    }

    public void gameSwitch()
    {
        if (SceneManager.GetActiveScene().name == "GameTest")
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(sceneName: "MainMenu");
        }
        else
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(sceneName: "GameTest");
        }
    }
}
