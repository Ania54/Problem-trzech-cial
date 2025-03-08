using UnityEngine;
using UnityEngine.InputSystem;

public class DebugCamRot : MonoBehaviour
{
    public float mouseSensitivity = 72f;

    private float xRotation = 0f;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        // Get mouse movement input
        float mouseX = InputSystem.actions.FindAction("Look").ReadValue<Vector2>().x * mouseSensitivity * Time.deltaTime;
        float mouseY = InputSystem.actions.FindAction("Look").ReadValue<Vector2>().y * mouseSensitivity * Time.deltaTime;

        // Apply vertical rotation and clamp it to avoid flipping the camera
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        // Rotate the camera around the X-axis
        transform.localRotation = Quaternion.Euler(xRotation, transform.localEulerAngles.y, 0f);

        // Rotate the camera around the Y-axis
        transform.Rotate(Vector3.up * mouseX, Space.World);
    }
}
