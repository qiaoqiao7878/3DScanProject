﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void configAvatar()
    {
        SceneManager.LoadScene(2);
    }

    public void startGame()
    {
        SceneManager.LoadScene(3);
    }

    public void display()
    {
        SceneManager.LoadScene(4);
    }

    public void closeGame()
    {
        Application.Quit();
    }
}