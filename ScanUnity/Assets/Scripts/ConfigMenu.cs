using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ConfigMenu : MonoBehaviour
{

    protected GlobalManager GM = GlobalManager.instanceGM;

    public GameObject femaleModel;
    public GameObject maleModel;

    protected GameObject currentModel;

    public void scanning()
    {
        
    }

    public void gender()
    {
        if (GM.getGender() == "female")
        {
            GM.setGender("male");
        }
        else
        {
            GM.setGender("female");
        }

        changeGenderModel();
    }

    public void turnClock()
    {
        Debug.Log("Turn");
        currentModel.transform.RotateAroundLocal(Vector3.up, 20.0f);

    }

    public void turnCounterClock()
    {
        currentModel.transform.RotateAroundLocal(Vector3.down, 20.0f);
    }

    public void save()
    {

    }

    public void back()
    {
        SceneManager.LoadScene(0);
    }

    void Awake()
    {
        changeGenderModel();
    }


    public void changeGenderModel()
    {
        if (GM.getGender() == "female")
        {
            femaleModel.SetActive(true);
            maleModel.SetActive(false);
            currentModel = femaleModel;
        }
        else
        {
            femaleModel.SetActive(false);
            maleModel.SetActive(true);
            currentModel = maleModel;
        }
    }
}
