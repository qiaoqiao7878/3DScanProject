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
    
    private static string genderGM = "female";   //global Gender

    private int numRecord = 1; //5          change!

    private List<pose> poseList = new List<pose>();  //contains the target poses for the dancing

    private int _numJoints = 20;
    private int _numBones = 22;   
    //--------------------------------------------------------------------------------------

    //Class for a stored pose, with name, 14 angles and 20 joint positions
    public class pose
    {
        public string id;
        public Vector3[] jointPos;
        public Quaternion[] jointRot;
        public Vector3 offset;

        public Vector3[] bonesPos;
        public Quaternion[] bonesRot;

        public pose(string id, Vector3[] Pos, Quaternion[] Rot, Vector3 off, Vector3[] PosB, Quaternion[] RotB)
        {
            this.id = id;
            this.jointPos = Pos;
            this.jointRot = Rot;
            this.offset = off;
            this.bonesPos = PosB;
            this.bonesRot = RotB;
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
    public List<pose> getPoseList()
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
        initializePoseList(); //Fill the poseList with poses from txt Files      
        
    }

    public Vector3 StringToVector3(string sVector)
    {
        // Remove the parentheses
        if (sVector.StartsWith("(") && sVector.EndsWith(")"))
        {
            sVector = sVector.Substring(1, sVector.Length - 2);
        }
        // split the items
        string[] sArray = sVector.Split(',');
        // store as a Vector3
        Vector3 result = new Vector3(
            float.Parse(sArray[0]),
            float.Parse(sArray[1]),
            float.Parse(sArray[2])
            );
        return result;
    }

    public Quaternion StringToQuaternion(string sVector)
    {
        // Remove the parentheses
        if (sVector.StartsWith("(") && sVector.EndsWith(")"))
        {
            sVector = sVector.Substring(1, sVector.Length - 2);
        }
        // split the items
        string[] sArray = sVector.Split(',');
        // store as a Vector3
        Quaternion result = new Quaternion(
            float.Parse(sArray[0]),
            float.Parse(sArray[1]),
            float.Parse(sArray[2]),
            1.0f                            //float.Parse(sArray[3])   change!
            );
        return result;        
    }

    //Read the pose files and store it in the poseList 
    public void initializePoseList()
    {
        string newId;
        Vector3[] newPos = new Vector3[_numJoints];
        Quaternion[] newRot = new Quaternion[_numJoints];
        Vector3 newOff;
        Vector3[] newPosB = new Vector3[_numBones];
        Quaternion[] newRotB = new Quaternion[_numBones];

        for (int i = 1; i <= numRecord; i++)
        {            
            string pathout = "Assets/Files/record_" + i + ".txt";
            //set false so it will generate a new file every time.
            StreamReader sr = new StreamReader(pathout, false);

            newId = i.ToString();
            //Positions durchgehen               
            for (int p = 0; p < _numJoints; p++)
            {
                newPos[p] = StringToVector3(sr.ReadLine());                
            }
            //Rotations durchgehen
            for (int r = 0; r < _numJoints; r++)
            {
                newRot[r] = StringToQuaternion(sr.ReadLine());

            }
            //Offset
            newOff = StringToVector3(sr.ReadLine());
            //Bones durchgehen
            for (int bP = 0; bP < _numBones; bP++)
            {
                newPosB[bP] = StringToVector3(sr.ReadLine());
            }
            for (int bR = 0; bR < _numBones; bR++)
            {
                newRotB[bR] = StringToQuaternion(sr.ReadLine());
            }
            //add new pose to List
            pose newPose = new pose(newId, newPos, newRot, newOff, newPosB, newRotB);
            poseList.Add(newPose);            
            sr.Close();
            sr.Dispose();
        }       
    }

}
