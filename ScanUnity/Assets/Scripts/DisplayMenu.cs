using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DisplayMenu : MonoBehaviour
{
    protected GlobalManager GM = GlobalManager.instanceGM;

    public GameObject femaleModel;
    public GameObject maleModel;

    public void back()
    {
        SceneManager.LoadScene(0);
    }

    public void recordStart()
    {
        
    }

    public void recordStop()
    {

    }

    void Awake()
    {
        if(GM.getGender() == "female")
        {
            femaleModel.SetActive(true);
            maleModel.SetActive(false);
        }
        else
        {
            femaleModel.SetActive(false);
            maleModel.SetActive(true);
        }
    }
}
