using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using System.IO;
using System.Text;

//Script/Singelton for global variables over several scenes
public class GlobalManager : MonoBehaviour
{
    //Global Variables----------------------------------------------------------------------      
    public static GlobalManager instanceGM = null;     //Static instance of GameManager which allows it to be accessed by any other script.
    
    public static string genderGM = "female";   //global Gender

    public int numRecord = 5;

    //public ArrayList poseList;  //contains the target poses for the dancing
    public List<Vector3[]> poseList = new List<Vector3[]>();
    public TextAsset poseFile; //contains recorded poses in a txt file

    private Vector3[] _joints;
    private int _numJoints = 20; //22;

    //--------------------------------------------------------------------------------------

    //Class for a stored pose, with name, 14 angles and 20 joint positions
    public class pose
    {
        public string name;
        public double[] angles;
        public Vector3[] jointPos;

        public pose(string name, double[] angles, Vector3[] pos)
        {
            this.name = name;
            this.angles = angles;
            this.jointPos = pos;
        }
    }
     
    //get global gender
    public string getGender()
    {
        return genderGM;
    }
    //set global gender
    public void setGender(string newGen)
    {
        genderGM = newGen;
    }

    //get PoseList
    public List<Vector3[]> getPoseList()
    {
        return poseList;
    }

    //Awake is always called before any Start functions
    void Awake()
    {
        //Check if instance already exists
        if (instanceGM == null)
            //if not, set instance to this
            instanceGM = this;
        //If instance already exists and it's not this:
        else if (instanceGM != this)
            //Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of a GameManager.
            Destroy(gameObject);

        //Sets this to not be destroyed when reloading scene
        DontDestroyOnLoad(gameObject);        
    }

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("init_start");
        initializePoseList(); //Fill the poseList with poses in poseFile
    }

    // Update is called once per frame
    //void Update()
    //{

    //}

    



    public Vector3 StringToVector3(string sVector)
    {
        // Remove the parentheses
        if (sVector.StartsWith("(") && sVector.EndsWith(") "))
        {
            sVector = sVector.Substring(1, sVector.Length - 3);
        }
        Debug.Log(sVector+"sVec");
        // split the items
        string[] sArray = sVector.Split(',');
       

        // store as a Vector3
        Vector3 result = new Vector3(
            float.Parse(sArray[0]),
            float.Parse(sArray[1]),
            float.Parse(sArray[2]));

        return result;
    }

    //Read the pose file and store it in the poseList 
    public void initializePoseList()
    {
        //TODO
        //open poseFile

        //read poseFile
        
        for (int i = 1; i <= numRecord; i++)
        {
            
            string pathout = "Assets/Files/record_" + i + ".txt";
            //set flase so it will generate a new file every time.
            StreamReader sr = new StreamReader(pathout);
            //
            _joints = new Vector3[_numJoints];
            Debug.Log("init"+i);
            for (int j = 0; j < _numJoints; j++)
            {
                
                _joints[j] = StringToVector3(sr.ReadLine());
                Debug.Log(_joints[j]+"joint");
            }
            poseList.Add(_joints);
            sr.Close();
            sr.Dispose();
        }
        //calculate angles of joints

        //save poses in poseList

        /* Example for pose adding
        double[] newangle = new double[14];
        Vector3[] newPose = new Vector3[20];
        for (int i = 0; i < 14; i++)
        {
            newangle[i] = 0;
        }
        pose newpose = new pose("id1", newangle, newPose);
        poseList.Add(newpose);
        */
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
