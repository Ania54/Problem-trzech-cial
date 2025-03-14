using UnityEngine;
using UnityEngine.InputSystem;

public class EditMode : MonoBehaviour
{
	public GameObject bodyContainer;
	public Color hoverColor = Color.white;
	private GameObject lastHoveredObject;
	private Color originalColor;
	private InputAction mousePositionAction;

	private void Start()
	{
		mousePositionAction = InputSystem.actions.FindAction("MousePosition");
	}
	private void Update()
	{
		Ray ray = Camera.main.ScreenPointToRay(mousePositionAction.ReadValue<Vector2>());
		if (Physics.Raycast(ray, out RaycastHit hit))
		{
			if (hit.collider.TryGetComponent(out Renderer renderer))
			{
				if (lastHoveredObject != hit.collider.gameObject)
				{
					ResetLastObject();
					lastHoveredObject = hit.collider.gameObject;
					originalColor = renderer.material.color;
					renderer.material.color = hoverColor;
				}
			}
		}
		else { ResetLastObject(); }
	}

	private void ResetLastObject()
	{
		if (lastHoveredObject != null)
		{
			lastHoveredObject.GetComponent<Renderer>().material.color = originalColor;
			lastHoveredObject = null;
		}
	}
	private void OnGUI()
	{
		if (GUI.Button(new Rect(Screen.width - 110, Screen.height - 50, 100, 40), "Start"))
		{
			foreach (Transform child in bodyContainer.transform) { child.GetComponent<ApplyForce>().enabled = true; }
			enabled = false;
		}
		// show a label on the right side of the screen
		GUI.Label(new Rect(Screen.width - 210, 10, 200, 30), "Zaznaczone ciało");
	}
}
