using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.IO;

public class ConfigMenu : MonoBehaviour

{
    private double bodyheight = 1.7;
    private double bodyweight = 0.0;
    private string _gender = "male";
    private string json;
    public SMPLBlendshapes SMPLBlendshapes;
    protected GlobalManager GM = GlobalManager.instanceGM;

    public GameObject femaleModel;
    public GameObject maleModel;

    protected GameObject currentModel;
    private KinectManager manager = KinectManager.Instance;
    
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
        /*
        string pathout = "Assets/smpl/Samples/Betas/joint.txt";

        StreamWriter sw = new StreamWriter(pathout, true);
        
        for (int i = 0; i < _joints.Length; i++)
        {
            sw.WriteLine(_joints[i] + " ");
        }
        sw.Close();
        sw.Dispose();
        */

        Debug.Log(bodyheight.ToString());
        Bodyshape_female bodyshape_female = new Bodyshape_female();
        //bodyshape_female.betas[0] = (bodyheight - 1.7) * 0.7;
        Bodyshape_male bodyshape_male = new Bodyshape_male();
        //bodyshape_male.betas[0] = (bodyheight - 1.7) * 0.7;

        //just first directly set the bodyheight as betas[0]
        bodyshape_female.betas[0] = bodyheight;
        bodyshape_male.betas[0] = bodyheight;
        bodyshape_female.betas[1] = bodyweight;
        bodyshape_male.betas[1] = bodyweight;
        

        if (_gender == "male")
        {
            json = JsonUtility.ToJson(bodyshape_male);
        }
        else
        {
            json = JsonUtility.ToJson(bodyshape_female);
        }

        File.WriteAllText("Assets/smpl/Samples/Betas/user.json", json);
      
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

    }

    public void back()
    {
        SceneManager.LoadScene(0);
    }

    void Awake()
    {
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
   

    
    public static double calBodyheight(ref Vector3[] jointsPos)
    {
        double torso_height = 0;
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

        double tot_height = torso_height + (left_leg_height + right_leg_height) / 2.0;
        
        return tot_height;

    }

    public static double calBodyweight(ref Vector3[] jointsPos)
    {
        double weightValue = 0;
        weightValue = Length(ref jointsPos, NuiSkeletonPositionIndex.ElbowLeft, NuiSkeletonPositionIndex.ElbowRight);
        
        return weightValue;
    }

    
    
    public enum NuiSkeletonPositionIndex : int
    {
        HipCenter = 0,
        Spine = 1,
        ShoulderCenter = 2,
        Head = 3,
        ShoulderLeft = 5,
        ElbowLeft = 6,
        WristLeft = 7,
        HandLeft = 8,
        ShoulderRight = 10,
        ElbowRight = 11,
        WristRight = 12,
        HandRight = 13,
        HipLeft = 14,
        KneeLeft = 15,
        AnkleLeft = 16,
        FootLeft = 17,
        HipRight = 18,
        KneeRight = 19,
        AnkleRight = 20,
        FootRight = 21
    }

}
