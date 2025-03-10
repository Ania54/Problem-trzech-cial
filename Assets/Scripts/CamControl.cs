using UnityEngine;
using UnityEngine.InputSystem;

public class CamControl : MonoBehaviour
{
	public float moveSpeed = 6f;
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

		float leftRight = InputSystem.actions.FindAction("Move").ReadValue<Vector2>().x;
		float   forBack = InputSystem.actions.FindAction("Move").ReadValue<Vector2>().y;
		float    upDown = InputSystem.actions.FindAction("Jump").ReadValue<float>();

		Vector3 flatMove = moveSpeed * Time.deltaTime * new Vector3(leftRight, upDown, forBack);
        transform.Translate(flatMove);
    }

	void OnGUI()
	{
		// Set the style for the label
		GUIStyle style = new()
		{
			fontSize = 24
		};
		style.normal.textColor = Color.white;

		// Draw the FPS label in the lower-left corner
		GUI.Label(new Rect(10, 10, 100, 25), "text", style);
		// Draw debug label below
		// GUI.Label(new Rect(10, 40, 100, 25), 216.ToString(), style);
	}
}
