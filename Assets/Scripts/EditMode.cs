using UnityEngine;
using UnityEngine.InputSystem;

public class EditMode : MonoBehaviour
{
	public GameObject bodyContainer;
	private GameObject lastHoveredObject;
	private GameObject selectedObject;
	private InputAction mousePositionAction;
	private InputAction mouseClickAction;

	private void Start()
	{
		mousePositionAction = InputSystem.actions.FindAction("MousePosition");
		mouseClickAction = InputSystem.actions.FindAction("MouseClick");
	}
	private void Update()
	{
		Ray ray = Camera.main.ScreenPointToRay(mousePositionAction.ReadValue<Vector2>());
		if (Physics.Raycast(ray, out RaycastHit hit))
		{
			if (hit.collider.TryGetComponent(out Renderer renderer))
			{
				// select on mouse click
				if (mouseClickAction.ReadValue<float>() > 0)
				{
					ResetLastObject();
					selectedObject = hit.collider.gameObject;
				}

				if (lastHoveredObject != hit.collider.gameObject)
				{
					ResetLastObject();
					lastHoveredObject = hit.collider.gameObject;
					// enable emission in the shader
					renderer.material.EnableKeyword("_EMISSION");
				}
			}
		}
		else { ResetLastObject(); }
	}

	private void ResetLastObject()
	{
		if (lastHoveredObject != null && lastHoveredObject != selectedObject)
		{
			lastHoveredObject.GetComponent<Renderer>().material.DisableKeyword("_EMISSION");
			lastHoveredObject = null;
		}
	}
	private void OnGUI()
	{
		if (GUI.Button(new Rect(Screen.width - 110, Screen.height - 50, 100, 40), "Start"))
		{
			foreach (Transform child in bodyContainer.transform) { child.GetComponent<ApplyForce>().enabled = true; }
			ResetLastObject();
			enabled = false;
		}
		// show a label on the right side of the screen
		if (selectedObject == null) { return; }
		GUI.Label(new Rect(Screen.width - 210, 10, 200, 30), "Zaznaczone ciało:\n" + selectedObject.name);
	}
}
