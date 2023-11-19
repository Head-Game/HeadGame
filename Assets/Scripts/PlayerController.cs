using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HearXR;

public class PlayerController : MonoBehaviour
{
    public float mouseSensitivity = 100f;

    private Camera mainCamera;
    private float xRotation = 0f;
    private float yRotation = 0f;

    private bool useAirPodsInput = false; // Flag to toggle input method

    void Start()
    {
        mainCamera = Camera.main;

        // Initialize AirPods if available and on a mobile platform
#if UNITY_IOS || UNITY_ANDROID
        HeadphoneMotion.Init();

        if (HeadphoneMotion.IsHeadphoneMotionAvailable())
        {
            HeadphoneMotion.OnHeadRotationQuaternion += HandleHeadRotationQuaternion;
            HeadphoneMotion.StartTracking();
            useAirPodsInput = true;
        }
#endif

        // Locks the cursor to the center of the screen
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        if (useAirPodsInput)
        {
            // AirPods input will be handled in HandleHeadRotationQuaternion
        }
        else
        {
            // Handle mouse input
            Vector2 mouseInput = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")) * mouseSensitivity * Time.deltaTime;

            xRotation -= mouseInput.y;
            yRotation += mouseInput.x;

            xRotation = Mathf.Clamp(xRotation, -90f, 90f);

            mainCamera.transform.localRotation = Quaternion.Euler(xRotation, yRotation, 0f);
        }
    }

    private void HandleHeadRotationQuaternion(Quaternion rotation)
    {
        // Apply the AirPods rotation to the camera
        mainCamera.transform.rotation = rotation;
    }
}
