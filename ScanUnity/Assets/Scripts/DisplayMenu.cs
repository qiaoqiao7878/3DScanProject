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
    protected Transform[] newT;

    private KinectManager manager = KinectManager.Instance;
    protected GlobalManager GM = GlobalManager.instanceGM;

    private int numRecordtotal = 5;
    private int numRecord = 1;
    //Kinect Skeleton related stuff
    private uint UserId;
    private int _numJoints = 20;
    private int _numBones = 22;
    private Vector3[] newPos;
    private Quaternion[] newRot;
    private Vector3 newOff;
    private Vector3[] newPosB;
    private Quaternion[] newRotB;

    //Modelobjects
    public GameObject femaleModel;
    public GameObject maleModel;
    protected GameObject currentModel;

    //PopUp Window variables
    public GameObject PopUpWindow;
    public TextMeshProUGUI popText;

    //Button-Functions--------------------------------------------------------
    private Transform _transformCache;
    public new Transform transform
    {
        get
        {
            if (!_transformCache)
                _transformCache = base.transform;

            return _transformCache;
        }
    }

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

            
            newPosB = new Vector3[_numBones];
            newRotB = new Quaternion[_numBones];

            getJointPosition();
            getJointRotation();
            getOffset();
            getBonesTransform();                       

            if (IsAllJointTracked() == true)
            //if (true)
            {
                //set false so it will generate a new file every time.
                StreamWriter sw = new StreamWriter(pathout, false);
                
                for (int i = 0; i < _numJoints; i++)
                {
                    sw.WriteLine(newPos[i]);
                }
                for (int i = 0; i < _numJoints; i++)
                {
                    sw.WriteLine(newRot[i]); //txt Format: (0,0, 0,0, 0,0, 1,0)
                }
                sw.WriteLine(newOff);
                for (int i = 0; i < _numBones; i++)
                {
                    sw.WriteLine(newPosB[i]);
                }
                for (int i = 0; i < _numBones; i++)
                {
                    sw.WriteLine(newRotB[i]);
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
        else
        {
            displayPopUp("Record ending! Go to the dancing part");
        }     

    }     

    //------------------------------------------------------------------------

    void Awake()
    {
        UserId = manager.GetPlayer1ID();
        //choose current Model according to gender
        changeGenderModel();
        numRecord = 1;
        newPos = new Vector3[_numJoints];
        newRot = new Quaternion[_numJoints];
    }

    //Activate the model according to gender
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

    //get current joint position and store them in _joints
    void getJointPosition()
    {
        
        for (int i = 0; i < _numJoints; i++)
        {
            if (manager.IsJointTracked(UserId, i))
            {
                newPos[i] = manager.GetJointPosition(UserId, i);
            }
        }
    }

    void getJointRotation()
    {
        for (int i = 0; i < _numJoints; i++)
        {
            if (manager.IsJointTracked(UserId, i))
            {
                newRot[i] = manager.GetJointOrientation(UserId, i, false);
            }
        }
    }

    void getOffset()
    {
        newOff = manager.GetUserPosition(UserId);    
    }

    void getBonesTransform()
    {
        newT = new Transform[1];
        //var animatorComponent = currentModel.GetComponent<Animator>();
        var animatorComponent = currentModel.GetComponentsInChildren<Animator>();
        Debug.Log(animatorComponent[0]);
        for (int i = 0; i < _numBones; i++)
        {
            if (!boneIndex2MecanimMap.ContainsKey(i))
                continue;

            newT[0] = animatorComponent[0].GetBoneTransform(boneIndex2MecanimMap[i]);
            Debug.Log(newT[0]);
            if (newT[0] != null)
            {
                //TODO
                //newPosB[i] = newT[0].position; //position or localPosition ??
                //newRotB[i] = newT[0].rotation; //rotation or localRotation ??
                //or
                newPosB[i] = newT[0].localPosition; 
                newRotB[i] = newT[0].localRotation;
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

    private readonly Dictionary<int, HumanBodyBones> boneIndex2MecanimMap = new Dictionary<int, HumanBodyBones>
    {
        {0, HumanBodyBones.Hips},
        {1, HumanBodyBones.Spine},
        {2, HumanBodyBones.Neck},
        {3, HumanBodyBones.Head},

        {4, HumanBodyBones.LeftShoulder},
        {5, HumanBodyBones.LeftUpperArm},
        {6, HumanBodyBones.LeftLowerArm},
        {7, HumanBodyBones.LeftHand},
        {8, HumanBodyBones.LeftIndexProximal},

        {9, HumanBodyBones.RightShoulder},
        {10, HumanBodyBones.RightUpperArm},
        {11, HumanBodyBones.RightLowerArm},
        {12, HumanBodyBones.RightHand},
        {13, HumanBodyBones.RightIndexProximal},

        {14, HumanBodyBones.LeftUpperLeg},
        {15, HumanBodyBones.LeftLowerLeg},
        {16, HumanBodyBones.LeftFoot},
        {17, HumanBodyBones.LeftToes},

        {18, HumanBodyBones.RightUpperLeg},
        {19, HumanBodyBones.RightLowerLeg},
        {20, HumanBodyBones.RightFoot},
        {21, HumanBodyBones.RightToes},
    };
}
