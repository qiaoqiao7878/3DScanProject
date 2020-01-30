using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.IO;
using TMPro;

//Script for Config Scene 
public class ConfigMenu : MonoBehaviour

{ 
    private KinectManager manager = KinectManager.Instance;
    protected GlobalManager GM = GlobalManager.instanceGM;

    //ModelObjects
    public GameObject femaleModel;
    public GameObject maleModel;
    protected GameObject currentModel;    

    //PopUp Window variables
    public GameObject PopUpWindow;
    public TextMeshProUGUI popText;

    private string _gender = "female";

    //default average values for height and weight
    private double bodyheight = 1.7;
    private double bodyweight = 0.5;
    
    //Variable for content for json File
    private string json;

    //Kinect Skeleton related stuff
    private int _numJoints = 20; //22;
    private Vector3[] _joints;
    private uint UserId;


    private class Bodyshape_female
    {
        //Default values of shape blendshapes
        public double[] betas = new double[10] { -1.50338909, 0.41133214, -0.31445075, -0.90729174, 0.89161303, -1.1674648, -0.36843207, -0.42175958, 1.00391208, 1.2608627 };
    }
    private class Bodyshape_male
    {
        //Default values of shape blendshapes
        public double[] betas = new double[10] { 0.55717977, -1.81291238, -0.54321285, 0.23705893, -0.50107065, 1.24639222, 0.43375487, 0.15281353, -0.23500944, 0.10896058 };
    }


    //Button-Functions--------------------------------------------------------
    //Scanning the Skeleton for height and weight values for shape file
    public void scanning()
    {
        Debug.Log("Scanning...");
        _joints = new Vector3[_numJoints];
        bool correctCal = updateBody();

        //just use the head joint
        //bodyheight = _joints[3].y;
        
        //output jointposition to txt file
        string pathout = "Assets/Files/joint.txt";
        StreamWriter sw = new StreamWriter(pathout, true);
        sw.WriteLine("Scan...------------------------------------------");
        for (int i = 0; i < _joints.Length; i++)
        {
            sw.WriteLine(_joints[i] + " ");
        }
        sw.Close();
        sw.Dispose();

        if (correctCal)
        {        
            Bodyshape_female bodyshape_female = new Bodyshape_female();            
            Bodyshape_male bodyshape_male = new Bodyshape_male();

            //just first directly set the bodyheight as betas[0]
            //bodyshape_female.betas[0] = bodyheight;
            bodyshape_female.betas[0] = (bodyheight - 1.7) * 10.0;
            //bodyshape_male.betas[0] = bodyheight;
            bodyshape_male.betas[0] = -(bodyheight - 1.7) * 10.0;
            //Set bodyweight as beta[1]
            bodyshape_female.betas[1] = -bodyweight * 10.0 + 5.0;
            bodyshape_male.betas[1] = -bodyweight * 10.0 + 5.0;
            Debug.Log("Height: " + bodyweight.ToString());

            //Write new shape parameters in json File according to gender
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

            //Reload Scene to apply the new ShapeFile
            UnityEditor.AssetDatabase.Refresh();
            SceneManager.LoadScene(2);
        }
        else
        {
            displayPopUp("I just cannot find you!!!\nPlease find more suitable place!!!");
        }
    }
     
    //change the gender
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
        SceneManager.LoadScene(2);
    }

    //turn the model clockwise by 20°
    public void turnClock()
    {
        currentModel.transform.RotateAroundLocal(Vector3.up, 20.0f);

    }
    //turn the model counter clockwise by 20°
    public void turnCounterClock()
    {
        currentModel.transform.RotateAroundLocal(Vector3.down, 20.0f);
    }

    //Go back to MainMenu
    public void back()
    {
        SceneManager.LoadScene(1);
    }

    //Close the popUp
    public void closePopUp()
    {
        PopUpWindow.SetActive(false);
    }

    //------------------------------------------------------------------------   

    void Awake()
    {
        _gender = GM.getGender();
        changeGenderModel();
    }

    //void Update()
    //{
    //}

    //Display the popUp with message
    public void displayPopUp(string msg)
    {
        popText.text = msg;
        PopUpWindow.SetActive(true);
    }

    //change model according to gender
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
    bool updateBody() {
        UserId = manager.GetPlayer1ID();
        for (int i = 0; i < _numJoints; i++) { 
            if (manager.IsJointTracked(UserId,i)){
                _joints[i] = manager.GetJointPosition(UserId, i);
             }
        }
        bodyheight = calBodyheight(ref _joints);
        bodyweight = calBodyweight(ref _joints);

        //if some joints cant be tracked
        if(bodyheight == 0.0 || bodyweight == 0.0)
        {
            return false;
        }

        return true;
    }

    //Helperfunctions for Height & Weight Calculation
    //Get Length between two joints
    public static double Length(ref Vector3[] jointsPos, NuiSkeletonPositionIndex p1, NuiSkeletonPositionIndex p2)
    {
        Vector3 pVec1 = jointsPos[(int)p1];
        Vector3 pVec2 = jointsPos[(int)p2];
        double length = Math.Sqrt(Math.Pow(pVec1.x - pVec2.x, 2) + Math.Pow(pVec1.y - pVec2.y, 2) + Math.Pow(pVec1.z - pVec2.z, 2));
        return length;
    }
    //Get Length between one joint and a vector
    public static double Lengthwithvector(ref Vector3[] jointsPos, NuiSkeletonPositionIndex p1, Vector3 p2)
    {
        Vector3 pVec1 = jointsPos[(int)p1];
        double length = Math.Sqrt(Math.Pow(pVec1.x - p2.x, 2) + Math.Pow(pVec1.y - p2.y, 2) + Math.Pow(pVec1.z - p2.z, 2));
        return length;
    }
    //Get Middle of two joints
    public static Vector3 Average(ref Vector3[] jointsPos, NuiSkeletonPositionIndex p1, NuiSkeletonPositionIndex p2)
    {
        Vector3 pVec1 = jointsPos[(int)p1];
        Vector3 pVec2 = jointsPos[(int)p2];
        Vector3 avg_joint;
        avg_joint = (pVec1 + pVec2) / 2;
        return avg_joint;
    }

    
    //Calculate the bodyheight from the tracked joint positions
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
            torso_height += 0.15;

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
            Debug.Log("I just cannot find you!");
            return 0.0;
        }
        return tot_height;
    }

    //Calculate the bodyweight from the tracked joint positions of the hands/elbows
    public static double calBodyweight(ref Vector3[] jointsPos)
    {
        double weightValue = 0;
        if (manager.IsJointTracked(UserId, (int)NuiSkeletonPositionIndex.ElbowLeft) && manager.IsJointTracked(UserId,
            (int)NuiSkeletonPositionIndex.ElbowRight)){
            weightValue = Length(ref jointsPos, NuiSkeletonPositionIndex.ElbowLeft, NuiSkeletonPositionIndex.ElbowRight);
        }
        else
        {
            Debug.Log("I just cannot find you!");
            return 0.0;
        }
        return weightValue;
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