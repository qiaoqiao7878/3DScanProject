using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalManager : MonoBehaviour
{

    public static GlobalManager instanceGM = null;                //Static instance of GameManager which allows it to be accessed by any other script.
    public static string genderGM = "female";


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

    public ArrayList poseList;

    public string getGender()
    {
        return genderGM;
    }

    public void setGender(string newGen)
    {
        genderGM = newGen;
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
        initializePoseList();


    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //Read the file 
    void initializePoseList()
    {
        double[] newangle = new double[14];
        Vector3[] newPose = new Vector3[20];
        for (int i = 0; i < 14; i++)
        {
            newangle[i] = 0;
        }
        pose newpose = new pose("id1", newangle, newPose);
        poseList.Add(newpose);
    }
}
