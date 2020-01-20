using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//updates the avatars of the Kinect user when Scene is loaded
public class SetAvatar : MonoBehaviour
{

    //global Instance of KinectManager
    KinectManager manager;

    //called on Start
    void Start()
    {
        manager = KinectManager.Instance;
        
        if (manager)
        {
            //manager.ClearKinectUsers();
            manager.Player1Avatars.Clear();

            AvatarController[] avatars = FindObjectsOfType(typeof(AvatarController)) as AvatarController[];

            foreach (AvatarController avatar in avatars)
            {
                manager.Player1Avatars.Add(avatar.gameObject);
            }

            manager.ResetAvatarControllers();                      

        }
    }

    //called when destroyed to delete the avatar in left scene
    void OnDestroy()
    {
        if (manager)
        {
            //manager.ClearKinectUsers();
            manager.Player1Avatars.Clear();

            manager.ResetAvatarControllers();
        }           
    }
          
}
