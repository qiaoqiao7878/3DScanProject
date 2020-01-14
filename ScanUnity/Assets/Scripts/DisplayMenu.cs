using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DisplayMenu : MonoBehaviour
{
    
    public void back()
    {
        UnityEditor.AssetDatabase.Refresh();
        SceneManager.LoadScene(0);
    }

    public void recordStart()
    {
        
    }

    public void recordStop()
    {

    }
}
