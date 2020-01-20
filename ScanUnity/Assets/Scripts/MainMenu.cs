using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//Script for MainMenuScene
public class MainMenu : MonoBehaviour
{



    //Button-Functions--------------------------------------------------------

    //Load Config Scene
    public void configAvatar()
    {
        SceneManager.LoadScene(2);
    }

    //Load Dancing Scene
    public void startGame()
    {
        SceneManager.LoadScene(3);
    }

    //Load Display Scene
    public void display()
    {
        SceneManager.LoadScene(4);
    }

    //Exit the Game
    public void closeGame()
    {
        Application.Quit();
    }

    //------------------------------------------------------------------------
}
