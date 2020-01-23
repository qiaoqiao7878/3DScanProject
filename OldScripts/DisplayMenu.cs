using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System;

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

            //if (true)
            if (IsAllJointTracked() == true)
            {
                //set flase so it will generate a new file every time.
                StreamWriter sw = new StreamWriter(pathout, false);
                
                //sw.WriteLine("Recording...------------------------------------------");
                for (int i = 0; i < _joints.Length; i++)
                {
                    sw.WriteLine(_joints[i]);
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

            //// TODO: where to store the angles' information
            //// calculate angles for joints
            //double[] angles = posAngle(ref _joints);
        }

        else {
            displayPopUp("Record ending! Go to the dancing part");
        }
        
        

    }

    //// TODO: we have to ensure the sequence of NuiSkeletonPositionIndex is the same as sequence in _joint
    //public static double calAngle(ref Vector3[] jointsPos, NuiSkeletonPositionIndex n1, NuiSkeletonPositionIndex n2, NuiSkeletonPositionIndex n3)
    //{
    //    Vector3 j1 = jointsPos[(int)n1];
    //    Vector3 j2 = jointsPos[(int)n2];
    //    Vector3 j3 = jointsPos[(int)n3];
        
    //    double link1 = Math.Sqrt(Math.Pow(j1.x - j2.x, 2) + Math.Pow(j1.y - j2.y, 2) + Math.Pow(j1.z - j2.z, 2));
    //    double link2 = Math.Sqrt(Math.Pow(j3.x - j2.x, 2) + Math.Pow(j3.y - j2.y, 2) + Math.Pow(j3.z - j2.z, 2));
    //    double dot = Vector3.Dot((j1 - j2), (j3 - j2));
    //    double angle = dot / (link1 * link2);
    //    return Math.Acos(angle);
    //}

    //// calculate model's angle
    //public static double[] posAngle(ref Vector3[] jointPos)
    //{
    //    double[] points = new double[14];
    //    points[0] = calAngle(ref jointPos, NuiSkeletonPositionIndex.Head, NuiSkeletonPositionIndex.ShoulderCenter, NuiSkeletonPositionIndex.ShoulderLeft);
    //    points[1] = calAngle(ref jointPos, NuiSkeletonPositionIndex.ShoulderCenter, NuiSkeletonPositionIndex.ShoulderLeft, NuiSkeletonPositionIndex.ElbowLeft);
    //    points[2] = calAngle(ref jointPos, NuiSkeletonPositionIndex.ShoulderLeft, NuiSkeletonPositionIndex.ElbowLeft, NuiSkeletonPositionIndex.WristLeft);
    //    points[3] = calAngle(ref jointPos, NuiSkeletonPositionIndex.ElbowLeft, NuiSkeletonPositionIndex.WristLeft, NuiSkeletonPositionIndex.HandLeft);
    //    points[4] = calAngle(ref jointPos, NuiSkeletonPositionIndex.HipCenter, NuiSkeletonPositionIndex.HipLeft, NuiSkeletonPositionIndex.KneeLeft);
    //    points[5] = calAngle(ref jointPos, NuiSkeletonPositionIndex.HipLeft, NuiSkeletonPositionIndex.KneeLeft, NuiSkeletonPositionIndex.AnkleLeft);
    //    points[6] = calAngle(ref jointPos, NuiSkeletonPositionIndex.KneeLeft, NuiSkeletonPositionIndex.AnkleLeft, NuiSkeletonPositionIndex.FootLeft);
    //    points[7] = calAngle(ref jointPos, NuiSkeletonPositionIndex.Head, NuiSkeletonPositionIndex.ShoulderCenter, NuiSkeletonPositionIndex.ShoulderRight);
    //    points[8] = calAngle(ref jointPos, NuiSkeletonPositionIndex.ShoulderCenter, NuiSkeletonPositionIndex.ShoulderRight, NuiSkeletonPositionIndex.ElbowRight);
    //    points[9] = calAngle(ref jointPos, NuiSkeletonPositionIndex.ShoulderRight, NuiSkeletonPositionIndex.ElbowRight, NuiSkeletonPositionIndex.WristRight);
    //    points[10] = calAngle(ref jointPos, NuiSkeletonPositionIndex.ElbowRight, NuiSkeletonPositionIndex.WristRight, NuiSkeletonPositionIndex.HandRight);
    //    points[11] = calAngle(ref jointPos, NuiSkeletonPositionIndex.HipCenter, NuiSkeletonPositionIndex.HipRight, NuiSkeletonPositionIndex.KneeRight);
    //    points[12] = calAngle(ref jointPos, NuiSkeletonPositionIndex.HipRight, NuiSkeletonPositionIndex.KneeRight, NuiSkeletonPositionIndex.AnkleRight);
    //    points[13] = calAngle(ref jointPos, NuiSkeletonPositionIndex.KneeRight, NuiSkeletonPositionIndex.AnkleRight, NuiSkeletonPositionIndex.FootRight);

    //    return points;
    //}

    //public enum NuiSkeletonPositionIndex : int
    //{
    //    HipCenter = 0,
    //    Spine = 1,
    //    ShoulderCenter = 2,
    //    Head = 3,
    //    ShoulderLeft = 4,
    //    ElbowLeft = 5,
    //    WristLeft = 6,
    //    HandLeft = 7,
    //    ShoulderRight = 8,
    //    ElbowRight = 9,
    //    WristRight = 10,
    //    HandRight = 11,
    //    HipLeft = 12,
    //    KneeLeft = 13,
    //    AnkleLeft = 14,
    //    FootLeft = 15,
    //    HipRight = 16,
    //    KneeRight = 17,
    //    AnkleRight = 18,
    //    FootRight = 19,
    //}

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
