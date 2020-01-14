using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalManager : MonoBehaviour
{

    public static GlobalManager instanceGM = null;                //Static instance of GameManager which allows it to be accessed by any other script.
    public static string genderGM = "female";


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
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
