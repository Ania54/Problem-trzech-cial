using UnityEngine;
using UnityEngine.InputSystem;

public class EditMode : MonoBehaviour
{
	public GameObject bodyContainer;
	private GameObject hoveredObject;
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
		// Get the ray from the current mouse position
		Ray ray = Camera.main.ScreenPointToRay(mousePositionAction.ReadValue<Vector2>());
		if (Physics.Raycast(ray, out RaycastHit hit))
		{
			GameObject hitObject = hit.collider.gameObject;
			if (hitObject.TryGetComponent(out Renderer hitRenderer))
			{
				// If clicked, update the selection:
				if (mouseClickAction.WasPressedThisFrame())
				{
					// Only update selection if a new object is clicked.
					if (selectedObject != hitObject)
					{
						ClearSelection();
						selectedObject = hitObject;
						hitRenderer.material.EnableKeyword("_EMISSION");
					}
					// Optionally, you might want to deselect when clicking the already selected object:
					else
					{
						ClearSelection();
						ClearHover();
					}
				}

				// Handle hover: highlight objects that are not currently selected.
				if (hitObject != selectedObject && hoveredObject != hitObject)
				{
					ClearHover();
					hoveredObject = hitObject;
					hitRenderer.material.EnableKeyword("_EMISSION");
				}
			}
		}
		else
		{
			// No object is hit by the ray, so clear the hover effect.
			ClearHover();
		}
	}

	private void ClearHover()
	{
		if (hoveredObject != null && hoveredObject != selectedObject)
		{
			if (hoveredObject.TryGetComponent(out Renderer renderer))
			{
				renderer.material.DisableKeyword("_EMISSION");
			}
			hoveredObject = null;
		}
	}

	private void ClearSelection()
	{
		if (selectedObject != null)
		{
			if (selectedObject.TryGetComponent(out Renderer renderer))
			{
				renderer.material.DisableKeyword("_EMISSION");
			}
			selectedObject = null;
		}
	}

	private void OnGUI()
	{
		if (GUI.Button(new Rect(Screen.width - 110, Screen.height - 50, 100, 40), "Start"))
		{
			foreach (Transform child in bodyContainer.transform)
			{
				foreach (Transform grandchild in child.transform)
				{
					if (!grandchild.CompareTag("TheTrail"))
					{
						// disable the grandchild object
						grandchild.gameObject.SetActive(false);
					}
				}
				child.GetComponent<SphereCollider>().enabled = false;
				child.GetComponent<ApplyForce>().enabled = true;
			}
			ClearHover();
			ClearSelection();
			enabled = false;
		}
		if (selectedObject == null) { return; }
		GUI.Label(new Rect(Screen.width - 210, 10, 200, 30),
		"Zaznaczone ciało:\nnr " + selectedObject.name + "\n\nmasa: " + selectedObject.GetComponent<Rigidbody>().mass.ToString("0.000 kg") + "\nX: " + selectedObject.transform.position.x.ToString("0.000") + "\nY: " + selectedObject.transform.position.y.ToString("0.000") + "\nZ: " + selectedObject.transform.position.z.ToString("0.000"));
	}
}
