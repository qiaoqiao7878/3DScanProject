using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DanceMenu : MonoBehaviour
{
    
    public void start()
    {
        UnityEditor.AssetDatabase.Refresh();

    }

    public void back()
    {
        SceneManager.LoadScene(0);
    }
}
