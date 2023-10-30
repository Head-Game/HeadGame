using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 5.0f;
    public float turnSpeed = 0.5f; // Turning speed for the camera

    private CharacterController characterController;
    private float xRotation = 0f; // For up and down look of the camera
    private Transform cameraTransform; // Store the camera's transform

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        cameraTransform = Camera.main.transform; // Grab the main camera's transform
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        MovePlayer();
        LookAroundWithKeypad();
    }

    void MovePlayer()
    {
        float horizontal = -Input.GetAxis("Horizontal"); // Inverted
        float vertical = -Input.GetAxis("Vertical");     // Inverted

        Vector3 direction = new Vector3(horizontal, 0, vertical).normalized;

        if (direction.magnitude >= 0.1f)
        {
            Vector3 moveDirection = direction * speed * Time.deltaTime;
            characterController.Move(moveDirection);
        }
    }


    void LookAroundWithKeypad()
    {
        float turnHorizontal = Input.GetAxis("KeypadHorizontal");
        float turnVertical = Input.GetAxis("KeypadVertical");

        xRotation -= turnVertical * turnSpeed; // Adjust the '-' sign as needed
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f); // Up and down rotation on the camera
        transform.Rotate(Vector3.up * turnHorizontal * turnSpeed); // Left and right rotation on the player
    }
}
