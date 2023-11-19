using UnityEngine;

public class MouseLook : MonoBehaviour
{
    public float mouseSensitivity = 100f;

    private float xRotation = 0f;
    private float yRotation = 0f; // Added for horizontal rotation

    void Start()
    {
        // Locks the cursor to the center of the screen
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        Vector2 mouseInput = InputManager.Instance.GetMouseInput();

        xRotation -= mouseInput.y;
        yRotation += mouseInput.x;

        transform.localRotation = Quaternion.Euler(xRotation, yRotation, 0f);
    }
}
