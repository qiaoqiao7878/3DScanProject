using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using UnityEngine.UI;
using TMPro;
using System.IO;
using System.Text;

//Script for Dancing Scene
public class DanceMenu : MonoBehaviour
{

    protected GlobalManager GM = GlobalManager.instanceGM;

    //ModelObjects of player
    public GameObject femaleModel;
    public GameObject maleModel;

    //Modelobject for Targetpose
    public GameObject targetModel;

    //Points Variables
    private int totalPoints = 0;
    public TextMeshProUGUI pointText;

    //Variables for the target Poses
    //private ArrayList poseList;
    public List<Vector3[]> poseList = new List<Vector3[]>();
    private int numPose;
    private int currentPose = -1;

    private double[] anglesPlayer = new double[14];
    private double[] anglesTarget = new double[14];

    private bool started = false;
    public Button startButton;
    private float roundTime = 100f; //seconds
    private float startTime;


    //Button-Functions--------------------------------------------------------

    public void startDance()
    {
        //GM.initializePoseList();
        totalPoints = 0;
        currentPose = -1;
        Debug.Log(poseList[0]);
        setPointText();
        bool next = makeNextPose();
        if (next)
        {
            started = true;
            startButton.interactable = false;
            
        }
        else
        {
            Debug.Log("PoseList is empty!!");
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
        
    }

    void Update()
    {
        //when a dance is playing
        if (started)
        {
            //calculate 14 angles
            calculatePlayerAngles();

            //compare with poseList[currentPose]
            bool match = compareAngles();

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
                    Debug.Log("All Poses matched!");
                    started = false;
                    startButton.interactable = true;
                    //Reload TargetModel Restpose
                    //TODO

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

    //Set the new Skeleton joint position of the avatar of the target model
    void changeTargetModel(Vector3[] jointPos)
    {
        //TODO
        //apply the new joint positions to avatar of the targetmodel
    }

    //choose next Pose in List, return false if List reached the end
    bool makeNextPose()
    {
        currentPose++;
        if(currentPose < numPose)
        {
            Vector3[] newPose = poseList[currentPose];
            changeTargetModel(newPose);
            calculateTargetAngles(newPose);
            startTime = Time.time;

            return true;
        }
        else
        {
            return false;
        }
    }

    //calculate 14 angles of the Player from Kinect Data
    void calculatePlayerAngles()
    {
        //TODO
        //calculate angles and store them in anglesPlayer
    }

    //calculate 14 angles of the Target of Vector3 array
    void calculateTargetAngles(Vector3[] jointPos)
    {
        //TODO
        //calculate angles and store them in anglesTarget
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
        return Math.Acos(angle);
    }

    // pick up points to calculate the final point
    public static double pointGiven(ref Vector3[] jointPos)
    {
        double[] points = new double[14];
        points[0] = calAngle(ref jointPos, NuiSkeletonPositionIndex.Head, NuiSkeletonPositionIndex.ShoulderCenter, NuiSkeletonPositionIndex.ShoulderLeft);
        points[1] = calAngle(ref jointPos, NuiSkeletonPositionIndex.ShoulderCenter, NuiSkeletonPositionIndex.ShoulderLeft, NuiSkeletonPositionIndex.ElbowLeft);
        points[2] = calAngle(ref jointPos, NuiSkeletonPositionIndex.ShoulderLeft, NuiSkeletonPositionIndex.ElbowLeft, NuiSkeletonPositionIndex.WristLeft);
        points[3] = calAngle(ref jointPos, NuiSkeletonPositionIndex.ElbowLeft, NuiSkeletonPositionIndex.WristLeft, NuiSkeletonPositionIndex.HandLeft);
        points[4] = calAngle(ref jointPos, NuiSkeletonPositionIndex.HipCenter, NuiSkeletonPositionIndex.HipLeft, NuiSkeletonPositionIndex.KneeLeft);
        points[5] = calAngle(ref jointPos, NuiSkeletonPositionIndex.HipLeft, NuiSkeletonPositionIndex.KneeLeft, NuiSkeletonPositionIndex.AnkleLeft);
        points[6] = calAngle(ref jointPos, NuiSkeletonPositionIndex.KneeLeft, NuiSkeletonPositionIndex.AnkleLeft, NuiSkeletonPositionIndex.FootLeft);
        points[7] = calAngle(ref jointPos, NuiSkeletonPositionIndex.Head, NuiSkeletonPositionIndex.ShoulderCenter, NuiSkeletonPositionIndex.ShoulderRight);
        points[8] = calAngle(ref jointPos, NuiSkeletonPositionIndex.ShoulderCenter, NuiSkeletonPositionIndex.ShoulderRight, NuiSkeletonPositionIndex.ElbowRight);
        points[9] = calAngle(ref jointPos, NuiSkeletonPositionIndex.ShoulderRight, NuiSkeletonPositionIndex.ElbowRight, NuiSkeletonPositionIndex.WristRight);
        points[10] = calAngle(ref jointPos, NuiSkeletonPositionIndex.ElbowRight, NuiSkeletonPositionIndex.WristRight, NuiSkeletonPositionIndex.HandRight);
        points[11] = calAngle(ref jointPos, NuiSkeletonPositionIndex.HipCenter, NuiSkeletonPositionIndex.HipRight, NuiSkeletonPositionIndex.KneeRight);
        points[12] = calAngle(ref jointPos, NuiSkeletonPositionIndex.HipRight, NuiSkeletonPositionIndex.KneeRight, NuiSkeletonPositionIndex.AnkleRight);
        points[13] = calAngle(ref jointPos, NuiSkeletonPositionIndex.KneeRight, NuiSkeletonPositionIndex.AnkleRight, NuiSkeletonPositionIndex.FootRight);
        double angle = 0;
        for (int i = 0; i < 14; i++)
        {
            angle += points[i];
        }
        Debug.Log("the final point is: " + angle);

        return angle;
    }

    //compare anglesPlayer with anglesTarget, return true if the difference is under a threshold
    bool compareAngles()
    {
        //TODO
        return true;
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
}
