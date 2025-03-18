using UnityEngine;
using UnityEngine.InputSystem;

public class CamControl : MonoBehaviour
{
	public float moveSpeed = 6f;
	public float mouseSensitivity = 72f;
	public GameObject bodyContainer;
	private float xRotation = 0f;
	private int currentMode;
	private InputAction jump2DAction;
	private InputAction moveAction;
	private InputAction lookAction;
	private InputAction jumpAction;
	private InputAction d2Action;
	private InputAction d3Action;

	// Start is called before the first frame update
	public void Start()
	{
		jump2DAction = InputSystem.actions.FindAction("Jump2D");
		moveAction = InputSystem.actions.FindAction("Move");
		lookAction = InputSystem.actions.FindAction("Look");
		jumpAction = InputSystem.actions.FindAction("Jump");
		d2Action = InputSystem.actions.FindAction("2D");
		d3Action = InputSystem.actions.FindAction("3D");

		float? childY = null;
		currentMode = 2;
		// check if y coordinates of all children are equal
		foreach (Transform child in bodyContainer.transform)
		{
			childY ??= child.transform.position.y;
			if (childY != child.transform.position.y)
			{
				currentMode = 3;
				Cursor.lockState = CursorLockMode.Locked;
				// make camera perspective
				Camera.main.orthographic = false;
				// cancel the loop
				break;
			}
		}
	}

	// Update is called once per frame
	private void Update()
	{
		float leftRight = moveAction.ReadValue<Vector2>().x;
		float forBack = moveAction.ReadValue<Vector2>().y;

		Vector3 flatMove = moveSpeed * Time.deltaTime * new Vector3(leftRight, 0, forBack);

		if (d2Action.ReadValue<float>() > 0)
		{
			currentMode = 2;
			Cursor.lockState = CursorLockMode.None;
			// make camera orthographic
			Camera.main.orthographic = true;
			// revert to default rotation
			transform.localRotation = Quaternion.Euler(90, 0, 0);
			// revert to default position
			transform.position = new Vector3(transform.position.x, 9, transform.position.z);
		}
		else if (d3Action.ReadValue<float>() > 0)
		{
			currentMode = 3;
			Cursor.lockState = CursorLockMode.Locked;
			// make camera perspective
			Camera.main.orthographic = false;
		}

		switch (currentMode)
		{
			case 2:
				float upDown2D = jump2DAction.ReadValue<float>();

				Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize + upDown2D, 5, 15);

				//theCamera.orthographicSize = Mathf.Clamp(theCamera.orthographicSize + upDown2D, 5, 15);

				transform.Translate(flatMove, Space.World);

				break;

			case 3:
				// Get mouse movement input
				float mouseX = lookAction.ReadValue<Vector2>().x * mouseSensitivity * Time.deltaTime;
				float mouseY = lookAction.ReadValue<Vector2>().y * mouseSensitivity * Time.deltaTime;

				// Apply vertical rotation and clamp it to avoid flipping the camera
				xRotation -= mouseY;
				xRotation = Mathf.Clamp(xRotation, -90f, 90f);

				// Rotate the camera around the X-axis
				transform.localRotation = Quaternion.Euler(xRotation, transform.localEulerAngles.y, 0f);

				// Rotate the camera around the Y-axis
				transform.Rotate(Vector3.up * mouseX, Space.World);

				float upDown = jumpAction.ReadValue<float>();

				Vector3 upMove = moveSpeed * Time.deltaTime * new Vector3(0, upDown, 0);

				transform.Translate(flatMove);
				transform.Translate(upMove, Space.World);

				break;
			default:
				break;
		}
	}

	private void OnGUI()
	{
		string currentModeText = currentMode == 2 ? "Obecny tryb: 2D (naciśnij [3] dla trybu 3D)" : "Obecny tryb: 3D (naciśnij [2] dla trybu 2D)";

		// Draw the label in the lower-left corner
		GUI.Label(new Rect(10, Screen.height - 40, 100, 30), currentModeText);
	}
}
