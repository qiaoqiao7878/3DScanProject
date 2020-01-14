using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DanceMenu : MonoBehaviour
{

    protected GlobalManager GM = GlobalManager.instanceGM;

    public GameObject femaleModel;
    public GameObject maleModel;

    public void start()
    {

    }

    public void back()
    {
        SceneManager.LoadScene(0);
    }


    void Awake()
    {
        if (GM.getGender() == "female")
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
