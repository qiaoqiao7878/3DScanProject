    using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using UnityEngine.UI;
using TMPro;
using System.IO;
using System.Text;
using System.Diagnostics;

//Script for Dancing Scene
public class DanceMenu : MonoBehaviour
{

    protected GlobalManager GM = GlobalManager.instanceGM;
    private KinectManager manager = KinectManager.Instance;

    //ModelObjects of player
    public GameObject femaleModel;
    public GameObject maleModel;

    //Modelobject for Targetpose
    public GameObject targetModel;
    private int _numBones = 22; //Bones = Avatar Skeleton
    protected Transform[] bonesTarget = new Transform[22];
    protected Vector3[] initialPos = new Vector3[22];
    protected Quaternion[] initialRot = new Quaternion[22];

    //Kinect & Player stuff
    private uint UserId;
    private int _numJoints = 20;
    private Vector3[] playerJoints = new Vector3[20];

    private int _numAngles = 8;

    //Points Variables
    private int totalPoints = 0;
    public TextMeshProUGUI pointText;

    public TextMeshProUGUI msgText;

    //Variables for the target Poses
    //private ArrayList poseList;
    public List<GlobalManager.pose> poseList;
    private int numPose;
    private int currentPose = -1;

    private double[] anglesPlayer = new double[8];//14
    private double[] anglesTarget = new double[8];

    private bool started = false;
    public Button startButton;
    private float roundTime = 100f; //seconds
    private float startTime;


    //Button-Functions--------------------------------------------------------

    public void startDance()
    {
        showText("");
        totalPoints = 0;
        currentPose = -1;
        setPointText();

        bool next = makeNextPose();
        if (next)
        {
            started = true;
            startButton.interactable = false;
        }
        else
        {
            UnityEngine.Debug.Log("PoseList is empty!!");
        }

    }

    //Go back to MainMenu
    public void back()
    {
        SceneManager.LoadScene(1);
    }
    //------------------------------------------------------------------------

    void Awake()
    {
        //choose current Model according to gender
        changeGenderModel();
        //get the poseList
        poseList = GM.getPoseList();
        numPose = poseList.Count;
        //bind the transformed of the Bones of the target Model
        var animatorComponent = targetModel.GetComponentInChildren<Animator>();
        for (int boneIndex = 0; boneIndex <_numBones; boneIndex++)
        {
            bonesTarget[boneIndex] = animatorComponent.GetBoneTransform(boneIndex2MecanimMap[boneIndex]);
        }
        //Initialize initial Position and Rotation of TargetModel
        for(int i = 0; i < _numBones; i++)
        {
            initialPos[i] = bonesTarget[i].localPosition;
            initialRot[i] = bonesTarget[i].localRotation;
        }
    }

    void Update()
    {
        //when a dance is playing
        if (started)
        {
            //calculate 14 angles
            getJointPosition();
            calculatePlayerAngles(playerJoints);

            //compare with poseList[currentPose]
            bool match = compareAngles();
            //bool match = false;
            //TODO maybe compare jointRotations too
            //UnityEngine.Debug.Log(Time.time - startTime);
            if (match || Time.time - startTime >= roundTime) //Timer if you took too long to find the target pose
            {
                if (match)
                {
                    //give point for a correct pose match
                    addPoints(1);
                }

                //show next pose
                bool next = makeNextPose();
                if (!next)
                {
                    UnityEngine.Debug.Log("All Poses matched!");
                    started = false;
                    startButton.interactable = true;
                    //Reload TargetModel Restpose
                    changeTargetModel(initialPos, initialRot);
                    showText("Game Over!");
                }
            }

        }
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

    //display the points
    void setPointText()
    {
        pointText.text = totalPoints.ToString();
    }

    //add to point the value plus
    void addPoints(int plus)
    {
        totalPoints += plus;
        setPointText();
    }

    void showText(string msg)
    {
        msgText.text = msg;
    }

    //Set the new Skeleton joint position of the avatar of the target model
    void changeTargetModel(Vector3[] pos, Quaternion[] rot)
    {        

        //apply the new transform to avatar of the targetmodel
        for(int i = 0; i < _numBones; i++)
        {
            
            //TODO check if this is working
            if (bonesTarget[i]!= null)
            {
                //bonesTarget[i].SetPositionAndRotation(pos[i], rot[i]);
                
                bonesTarget[i].localPosition = pos[i];
                bonesTarget[i].localRotation = rot[i];

            }

            //or
            //bonesTarget[i].position = pos[i];
            //bonesTarget[i].rotation = rot[i];
            //or
            //bonesTarget[i].localPosition = pos[i];
            //bonesTarget[i].localRotation = rot[i];

        }
    }    

    //choose next Pose in List, return false if List reached the end
    bool makeNextPose()
    {
        currentPose++;
        
        if (currentPose < numPose)
        {
            //UnityEngine.Debug.Log("current Pose" + currentPose);

            GlobalManager.pose newPose = poseList[currentPose];

            UnityEngine.Debug.Log("PoseName: " + newPose.id);
            changeTargetModel(newPose.bonesPos, newPose.bonesRot);
            UnityEngine.Debug.Log("bonesPos: " + newPose.bonesRot[0] );
            calculateTargetAngles(newPose.jointPos);
            startTime = Time.time;

            return true;
        }
        else
        {
            return false;
        }
    }

    //calculate 8 angles of the Player from Kinect Data
    void calculatePlayerAngles(Vector3[] jointPos)
    {        
        //calculate angles and store them in anglesPlayer
        anglesPlayer[0] = calAngle(ref jointPos, NuiSkeletonPositionIndex.WristLeft, NuiSkeletonPositionIndex.ElbowLeft, NuiSkeletonPositionIndex.ShoulderLeft);
        anglesPlayer[1] = calAngle(ref jointPos, NuiSkeletonPositionIndex.ElbowLeft, NuiSkeletonPositionIndex.ShoulderLeft, NuiSkeletonPositionIndex.ShoulderCenter);
        anglesPlayer[2] = calAngle(ref jointPos, NuiSkeletonPositionIndex.HipCenter, NuiSkeletonPositionIndex.HipLeft, NuiSkeletonPositionIndex.KneeLeft);
        anglesPlayer[3] = calAngle(ref jointPos, NuiSkeletonPositionIndex.AnkleLeft, NuiSkeletonPositionIndex.KneeLeft, NuiSkeletonPositionIndex.HipLeft);
        anglesPlayer[4] = calAngle(ref jointPos, NuiSkeletonPositionIndex.WristRight, NuiSkeletonPositionIndex.ElbowRight, NuiSkeletonPositionIndex.ShoulderRight);
        anglesPlayer[5] = calAngle(ref jointPos, NuiSkeletonPositionIndex.ElbowRight, NuiSkeletonPositionIndex.ShoulderRight, NuiSkeletonPositionIndex.ShoulderCenter);
        anglesPlayer[6] = calAngle(ref jointPos, NuiSkeletonPositionIndex.HipCenter, NuiSkeletonPositionIndex.HipRight, NuiSkeletonPositionIndex.KneeRight);
        anglesPlayer[7] = calAngle(ref jointPos, NuiSkeletonPositionIndex.AnkleRight, NuiSkeletonPositionIndex.KneeRight, NuiSkeletonPositionIndex.HipRight);       
    }

    //calculate 8 angles of the Target of Vector3 array
    void calculateTargetAngles(Vector3[] jointPos)
    {
        //calculate angles and store them in anglesTarget
        anglesPlayer[0] = calAngle(ref jointPos, NuiSkeletonPositionIndex.WristLeft, NuiSkeletonPositionIndex.ElbowLeft, NuiSkeletonPositionIndex.ShoulderLeft);
        anglesPlayer[1] = calAngle(ref jointPos, NuiSkeletonPositionIndex.ElbowLeft, NuiSkeletonPositionIndex.ShoulderLeft, NuiSkeletonPositionIndex.ShoulderCenter);
        anglesPlayer[2] = calAngle(ref jointPos, NuiSkeletonPositionIndex.HipCenter, NuiSkeletonPositionIndex.HipLeft, NuiSkeletonPositionIndex.KneeLeft);
        anglesPlayer[3] = calAngle(ref jointPos, NuiSkeletonPositionIndex.AnkleLeft, NuiSkeletonPositionIndex.KneeLeft, NuiSkeletonPositionIndex.HipLeft);
        anglesPlayer[4] = calAngle(ref jointPos, NuiSkeletonPositionIndex.WristRight, NuiSkeletonPositionIndex.ElbowRight, NuiSkeletonPositionIndex.ShoulderRight);
        anglesPlayer[5] = calAngle(ref jointPos, NuiSkeletonPositionIndex.ElbowRight, NuiSkeletonPositionIndex.ShoulderRight, NuiSkeletonPositionIndex.ShoulderCenter);
        anglesPlayer[6] = calAngle(ref jointPos, NuiSkeletonPositionIndex.HipCenter, NuiSkeletonPositionIndex.HipRight, NuiSkeletonPositionIndex.KneeRight);
        anglesPlayer[7] = calAngle(ref jointPos, NuiSkeletonPositionIndex.AnkleRight, NuiSkeletonPositionIndex.KneeRight, NuiSkeletonPositionIndex.HipRight);
    }

    // j2 is the joint that links the other two joints
    public static double calAngle(ref Vector3[] jointsPos, NuiSkeletonPositionIndex n1, NuiSkeletonPositionIndex n2, NuiSkeletonPositionIndex n3)
    {
        Vector3 j1 = jointsPos[(int)n1];
        Vector3 j2 = jointsPos[(int)n2];
        Vector3 j3 = jointsPos[(int)n3];
        double link1 = Math.Sqrt(Math.Pow(j1.x - j2.x, 2) + Math.Pow(j1.y - j2.y, 2) + Math.Pow(j1.z - j2.z, 2));
        double link2 = Math.Sqrt(Math.Pow(j3.x - j2.x, 2) + Math.Pow(j3.y - j2.y, 2) + Math.Pow(j3.z - j2.z, 2));
        double dot = Vector3.Dot((j1 - j2), (j3 - j2));
        double angle = dot / (link1 * link2);
        return Math.Acos(angle) * 180 / Math.PI; //in degree
    }

    // j2 is the joint that links the other two joints
    public static double calAngle2(ref Vector3[] jointsPos, NuiSkeletonPositionIndex n1, NuiSkeletonPositionIndex n2, NuiSkeletonPositionIndex n3)
    {
        Vector3 j1 = jointsPos[(int)n1];
        Vector3 j2 = jointsPos[(int)n2];
        Vector3 j3 = jointsPos[(int)n3];
        double link1 = Math.Sqrt(Math.Pow(j1.x - j2.x, 2) + Math.Pow(j1.y - j2.y, 2) + Math.Pow(j1.z - j2.z, 2));
        double link2 = Math.Sqrt(Math.Pow(j3.x - j2.x, 2) + Math.Pow(j3.y - j2.y, 2) + Math.Pow(j3.z - j2.z, 2));
        double dot = Vector3.Dot((j1 - j2), (j3 - j2));
        double angle = dot / (link1 * link2);
        return Math.Acos(angle);
    }

    //// time counter
    //void timeCounter()
    //{
    //    Stopwatch stopwatch = new Stopwatch();
    //    stopwatch.Start();

    //    for (int i = 0; i < 500; i++)
    //    {
    //        System.Threading.Thread.Sleep(10);
    //    }

    //    stopwatch.Stop();

    //}
    


    //compare anglesPlayer with anglesTarget, return true if the difference is under a threshold

    bool compareAngles()
    {
        //TODO
        double threshold = 10; // in degree
        double angleDiff = 0.0;

        for (int i = 0; i < _numAngles; i++)
        {
            angleDiff = Math.Abs(anglesPlayer[i] - anglesTarget[i]);
            UnityEngine.Debug.Log(angleDiff);
            if (angleDiff > threshold)
            {
                return false;
            }
        }
        return true;
    }

    //get current joint position and store them in _joints
    void getJointPosition()
    {
        UserId = manager.GetPlayer1ID();
        for (int i = 0; i < _numJoints; i++)
        {
            if (manager.IsJointTracked(UserId, i))
            {
                playerJoints[i] = manager.GetJointPosition(UserId, i);
            }
            else
            {
                //TODO if a joint isnt tracked
                playerJoints[i] = new Vector3(0, 0, 0);
            }
        }
    }


    //Enum for Joint Indices
    public enum NuiSkeletonPositionIndex : int
    {
        HipCenter = 0,
        Spine = 1,
        ShoulderCenter = 2,
        Head = 3,
        ShoulderLeft = 4,
        ElbowLeft = 5,
        WristLeft = 6,
        HandLeft = 7,
        ShoulderRight = 8,
        ElbowRight = 9,
        WristRight = 10,
        HandRight = 11,
        HipLeft = 12,
        KneeLeft = 13,
        AnkleLeft = 14,
        FootLeft = 15,
        HipRight = 16,
        KneeRight = 17,
        AnkleRight = 18,
        FootRight = 19,
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
