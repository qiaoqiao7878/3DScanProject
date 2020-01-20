using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//Script for StartUp Scene
public class StartUp : MonoBehaviour
{



    //Button-Functions--------------------------------------------------------

    //Load the MainMenu Scene by pressing Startbutton
    public void StartGame()
    {
        SceneManager.LoadScene(1);
    }

    //------------------------------------------------------------------------
}
