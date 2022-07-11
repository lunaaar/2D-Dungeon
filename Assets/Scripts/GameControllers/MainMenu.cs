using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    //This is a (modified) script from my previous game. The summary of it is that its basically an on off switch that allows you to switch between different scenes by just adding one of the switch atributes to a button
    public GameObject mainMenu;
    public GameObject characterSelection;
    public GameObject credits;
    public GameObject settings;
    bool inCredits = false;
    bool inSettings = false;
    bool inCharacterSelect = false;

    private void Start()
    {
        mainMenu.SetActive(true);
        credits.SetActive(false);
        settings.SetActive(false);
        characterSelection.SetActive(false);

    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && inSettings)
        {
            inSettings = false;
            settings.SetActive(false);
            mainMenu.SetActive(true);
        }
        else if (Input.GetKeyDown(KeyCode.Escape) && inCredits)
        {
            inCredits = false;
            credits.SetActive(false);
            mainMenu.SetActive(true);
        } else if (Input.GetKeyDown(KeyCode.Escape) && inCharacterSelect)
        {
            inCharacterSelect = false;
            characterSelection.SetActive(false);
            mainMenu.SetActive(true);

        }
    }

    public void creditMenuSwitch()
    {
        if (inCredits)
        {
            inCredits = false;
            credits.SetActive(false);
            mainMenu.SetActive(true);
        }
        else
        {
            inCredits = true;
            credits.SetActive(true);
            mainMenu.SetActive(false);
        }
    }
    public void settingsMenuSwitch()
    {
        if (inSettings)
        {
            inSettings = false;
            settings.SetActive(false);
            mainMenu.SetActive(true);
        }
        else
        {
            inSettings = true;
            settings.SetActive(true);
            mainMenu.SetActive(false);
        }
    }
    public void selectCharacterSwitch()
    {
        if (inCharacterSelect)
        {
            inCharacterSelect = false;
            characterSelection.SetActive(false);
            mainMenu.SetActive(true);
        }
        else
        {
            inCharacterSelect = true;
            characterSelection.SetActive(true);
            mainMenu.SetActive(false);
        }
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


    public void Close_Game()
    {
        Application.Quit();
    }



}

