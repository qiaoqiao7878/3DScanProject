using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

//Script for Display Scene
public class DisplayMenu : MonoBehaviour
{
    private KinectManager manager = KinectManager.Instance;
    protected GlobalManager GM = GlobalManager.instanceGM;
    private int numRecordtotal = 5;
    private int numRecord = 1;
    //Kinect Skeleton related stuff
    private int _numJoints = 20; //22;
    private Vector3[] _joints;
    private uint UserId;
    

    //Modelobjects
    public GameObject femaleModel;
    public GameObject maleModel;
    protected GameObject currentModel;

    //PopUp Window variables
    public GameObject PopUpWindow;
    public TextMeshProUGUI popText;



    //Button-Functions--------------------------------------------------------

    //go back to MainMenu
    public void back()
    {
        SceneManager.LoadScene(1);
    }

    public void recordPose()
    {
        Debug.Log("Recording...");
        
        if (numRecord < numRecordtotal)
        {
            
            
            //write current point positions of Kinect Scan to a txt file
            string pathout = "Assets/Files/record_" + numRecord + ".txt";

            _joints = new Vector3[_numJoints];
            getJointPosition();

           
            //output jointposition to txt file

            if (true)
            //if (IsAllJointTracked() == true)
            {
                //set flase so it will generate a new file every time.
                StreamWriter sw = new StreamWriter(pathout, false);
                
                //sw.WriteLine("Recording...------------------------------------------");
                for (int i = 0; i < _joints.Length; i++)
                {
                    sw.WriteLine(_joints[i] + " ");
                }
                sw.Close();
                sw.Dispose();

               
                displayPopUp( numRecord + "Pose,"+ (numRecordtotal-numRecord) + "Poses left.");
                numRecord += 1;
            }
            else
            {
                displayPopUp("Some joints are missing!!!\nPlease find more suitable place!!!");
            } 
        }

        else {
            displayPopUp("Record ending! Go to the dancing part");
        }
        
        

    }
    
    //------------------------------------------------------------------------

    void Awake()
    {
        //numRecord = GM.numRecord;
        //choose current Model according to gender
        changeGenderModel();
        numRecord = 1;
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
    //get current joint position and store them in _joints
    void getJointPosition()
    {
        UserId = manager.GetPlayer1ID();
        for (int i = 0; i < _numJoints; i++)
        {
            if (manager.IsJointTracked(UserId, i))
            {
                _joints[i] = manager.GetJointPosition(UserId, i); //-manager.GetUserPosition(UserID);
            }
        }
    }
        
    

    bool IsAllJointTracked() {
        for (int i = 0; i < _numJoints; i++)
        {
            if (manager.IsJointTracked(UserId, i)==false)
            {
                return false;
            }
        }
        return true;
    }
    //Display the popUp with message
    public void displayPopUp(string msg)
    {
        popText.text = msg;
        PopUpWindow.SetActive(true);
    }

    //Close the popUp
    public void closePopUp()
    {
        PopUpWindow.SetActive(false);
    }
}
