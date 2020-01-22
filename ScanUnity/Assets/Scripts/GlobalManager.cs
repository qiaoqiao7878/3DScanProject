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
    //public TextAsset poseFile; //contains recorded poses in a txt file

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
    }

}
