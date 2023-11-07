using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HearXR;

public class PlayerController : MonoBehaviour
{
    void Start()
    {
        // This call initializes the native plugin.
        HeadphoneMotion.Init();

        // Check if headphone motion is available on this device.
        if (HeadphoneMotion.IsHeadphoneMotionAvailable())
        {
            // Subscribe to the rotation callback.
            // Alternatively, you can subscribe to OnHeadRotationRaw event to get the 
            // x, y, z, w values as they come from the API.
            HeadphoneMotion.OnHeadRotationQuaternion += HandleHeadRotationQuaternion;
            
            // Start tracking headphone motion.
            HeadphoneMotion.StartTracking();
        }
    }

    private void HandleHeadRotationQuaternion(Quaternion rotation)
    {
        transform.rotation = rotation;
    }
}
