using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

//Script for Display Scene
public class DisplayMenu : MonoBehaviour
{
    protected GlobalManager GM = GlobalManager.instanceGM;
    private int numRecord;

    //Modelobjects
    public GameObject femaleModel;
    public GameObject maleModel;



    //Button-Functions--------------------------------------------------------

    //go back to MainMenu
    public void back()
    {
        SceneManager.LoadScene(1);
    }

    public void recordPose()
    {
        //TODO
        //write current point positions of Kinect Scan to a txt file

        string pathout = "Assets/Files/record_" + numRecord + ".txt";
    }
    
    //------------------------------------------------------------------------

    void Awake()
    {
        numRecord = GM.numRecord;
        //choose current Model according to gender
        changeGenderModel();
    }

    //Activate the model according to gender
    public void changeGenderModel()
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
