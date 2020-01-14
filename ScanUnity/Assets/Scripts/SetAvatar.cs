using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetAvatar : MonoBehaviour
{
    void Start()
    {
        KinectManager manager = KinectManager.Instance;

        if (manager)
        {
            manager.ClearKinectUsers();
            manager.Player1Avatars.Clear();

            AvatarController[] avatars = FindObjectsOfType(typeof(AvatarController)) as AvatarController[];

            foreach (AvatarController avatar in avatars)
            {
                manager.Player1Avatars.Add(avatar.gameObject);
            }

            manager.ResetAvatarControllers();

            // add available gesture listeners
            manager.gestureListeners.Clear();
                        

        }
    }

    void OnDestroy()
    {
        KinectManager manager = KinectManager.Instance;

        if (manager)
        {
             manager.ClearKinectUsers();
             manager.Player1Avatars.Clear();
        }
           
    }
}
