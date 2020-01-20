﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.IO;
using TMPro;

public class ConfigMenu : MonoBehaviour

{
    private double bodyheight = 1.7;
    private double bodyweight = 0.5;
    private string _gender = "female";
    private string json;
    public SMPLBlendshapes SMPLBlendshapes;
    protected GlobalManager GM = GlobalManager.instanceGM;

    public GameObject femaleModel;
    public GameObject maleModel;

    protected GameObject currentModel;
    private KinectManager manager = KinectManager.Instance;

    public GameObject PopUpWindow;
    public TextMeshProUGUI popText;

    private int _numJoints = 22;
    private Vector3[] _joints;
    private uint UserId;

    public void scanning()
    {
        _joints = new Vector3[_numJoints];
        updateBody();

        //just use the head joint
        //bodyheight = _joints[3].y;


        // output jointposition to txt file 

        string pathout = "Assets/smpl/Samples/Betas/joint.txt";

        StreamWriter sw = new StreamWriter(pathout, true);

        for (int i = 0; i < _joints.Length; i++)
        {
            sw.WriteLine(_joints[i] + " ");
        }
        sw.Close();
        sw.Dispose();


        //Debug.Log(bodyheight.ToString());
        Bodyshape_female bodyshape_female = new Bodyshape_female();
        //bodyshape_female.betas[0] = (bodyheight - 1.7) * 0.7;
        Bodyshape_male bodyshape_male = new Bodyshape_male();
        //bodyshape_male.betas[0] = (bodyheight - 1.7) * 0.7;

        //just first directly set the bodyheight as betas[0]
        bodyshape_female.betas[0] = bodyheight;
        bodyshape_male.betas[0] = bodyheight;
        bodyshape_female.betas[1] = -(bodyweight - 0.5) * 10;
        bodyshape_male.betas[1] = -(bodyweight - 0.5) * 10;
        Debug.Log(bodyweight.ToString());

        if (_gender == "male")
        {
            json = JsonUtility.ToJson(bodyshape_male);
            File.WriteAllText("Assets/Files/shape_male.json", json);
        }
        else
        {
            json = JsonUtility.ToJson(bodyshape_female);
            File.WriteAllText("Assets/Files/shape_female.json", json);
        }

        //File.WriteAllText("Assets/smpl/Samples/Betas/user.json", json);

        UnityEditor.AssetDatabase.Refresh();
        SceneManager.LoadScene(1);
    }

    private class Bodyshape_female
    {
        public double[] betas = new double[10] { -1.50338909, 0.41133214, -0.31445075, -0.90729174, 0.89161303, -1.1674648, -0.36843207, -0.42175958, 1.00391208, 1.2608627 };
    }
    private class Bodyshape_male
    {
        public double[] betas = new double[10] { 0.55717977, -1.81291238, -0.54321285, 0.23705893, -0.50107065, 1.24639222, 0.43375487, 0.15281353, -0.23500944, 0.10896058 };
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
        displayPopUp("I just cannot find you!!!\nPlease find more suitable place!!!");
    }

    public void back()
    {
        SceneManager.LoadScene(1);
    }

    public void closePopUp(){

        PopUpWindow.SetActive(false);

    }

    public void displayPopUp(string msg)
    {
        popText.text = msg;
        PopUpWindow.SetActive(true);
    }

    void Awake()
    {
        _gender = GM.getGender();
        changeGenderModel();
    }

    void Update()
    {
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

    void updateBody() {
        UserId = manager.GetPlayer1ID();
        //public Vector3 GetJointPosition(uint UserId, int joint)     
        for (int i = 0; i < _numJoints; i++) { 
            if (manager.IsJointTracked(UserId,i)){
                _joints[i] = manager.GetJointPosition(UserId, i);
             }
        }
        bodyheight = calBodyheight(ref _joints);
        bodyweight = calBodyweight(ref _joints);

    }

    public static double Length(ref Vector3[] jointsPos, NuiSkeletonPositionIndex p1, NuiSkeletonPositionIndex p2)
    {
        Vector3 pVec1 = jointsPos[(int)p1];
        Vector3 pVec2 = jointsPos[(int)p2];
        double length = Math.Sqrt(Math.Pow(pVec1.x - pVec2.x, 2) + Math.Pow(pVec1.y - pVec2.y, 2) + Math.Pow(pVec1.z - pVec2.z, 2));
        return length;
    }

    public static double Lengthwithvector(ref Vector3[] jointsPos, NuiSkeletonPositionIndex p1, Vector3 p2)
    {
        Vector3 pVec1 = jointsPos[(int)p1];
        double length = Math.Sqrt(Math.Pow(pVec1.x - p2.x, 2) + Math.Pow(pVec1.y - p2.y, 2) + Math.Pow(pVec1.z - p2.z, 2));
        return length;
    }

    public static Vector3 Average(ref Vector3[] jointsPos, NuiSkeletonPositionIndex p1, NuiSkeletonPositionIndex p2)
    {
        Vector3 pVec1 = jointsPos[(int)p1];
        Vector3 pVec2 = jointsPos[(int)p2];
        Vector3 avg_joint;
        avg_joint = (pVec1 + pVec2) / 2;
        return avg_joint;
    }



    public double calBodyheight(ref Vector3[] jointsPos)
    {
        double torso_height = 0;
        double tot_height = 0;

        if (manager.IsJointTracked(UserId, (int)NuiSkeletonPositionIndex.FootLeft) && manager.IsJointTracked(UserId,
            (int)NuiSkeletonPositionIndex.FootRight) && manager.IsJointTracked(UserId, (int)NuiSkeletonPositionIndex.Head))
        {
            torso_height = Length(ref jointsPos, NuiSkeletonPositionIndex.Head, NuiSkeletonPositionIndex.ShoulderCenter);
            torso_height += Length(ref jointsPos, NuiSkeletonPositionIndex.ShoulderCenter, NuiSkeletonPositionIndex.Spine);
            torso_height += Length(ref jointsPos, NuiSkeletonPositionIndex.Spine, NuiSkeletonPositionIndex.HipCenter);
            torso_height += Lengthwithvector(ref jointsPos, NuiSkeletonPositionIndex.HipCenter, Average(ref jointsPos, NuiSkeletonPositionIndex.HipRight, NuiSkeletonPositionIndex.HipLeft));

            double left_leg_height = 0;
            left_leg_height = Length(ref jointsPos, NuiSkeletonPositionIndex.HipLeft, NuiSkeletonPositionIndex.KneeLeft);
            left_leg_height += Length(ref jointsPos, NuiSkeletonPositionIndex.KneeLeft, NuiSkeletonPositionIndex.AnkleLeft);
            left_leg_height += Length(ref jointsPos, NuiSkeletonPositionIndex.AnkleLeft, NuiSkeletonPositionIndex.FootLeft);

            double right_leg_height = 0;
            right_leg_height = Length(ref jointsPos, NuiSkeletonPositionIndex.HipRight, NuiSkeletonPositionIndex.KneeRight);
            right_leg_height += Length(ref jointsPos, NuiSkeletonPositionIndex.KneeRight, NuiSkeletonPositionIndex.AnkleRight);
            right_leg_height += Length(ref jointsPos, NuiSkeletonPositionIndex.AnkleRight, NuiSkeletonPositionIndex.FootRight);

            tot_height = torso_height + (left_leg_height + right_leg_height) / 2.0;
        }
        else
        {
            Debug.Log("I just cannot find you!!! Please find more suitable place!!! :(");
            displayPopUp("I just cannot find you!!!\nPlease find more suitable place!!!");
            return 0.0;
        }
        return tot_height;

    }
    public static double calBodyweight(ref Vector3[] jointsPos)
    {
        double weightValue = 0;
        weightValue = Length(ref jointsPos, NuiSkeletonPositionIndex.ElbowLeft, NuiSkeletonPositionIndex.ElbowRight);
        return weightValue;
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
        for(int i=0; i<14; i++)
        {
            angle += points[i];
        }
        Debug.Log("the final point is: " + angle);

        return angle;
    }




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