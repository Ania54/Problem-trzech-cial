using UnityEngine;
using UnityEngine.InputSystem;

public class EditMode : MonoBehaviour
{
	public GameObject bodyContainer;
	public GameObject bodyPrefab;
	private GameObject hoveredObject;
	private GameObject selectedObject;
	private InputAction mousePositionAction;
	private InputAction mouseClickAction;

	private void Start()
	{
		mousePositionAction = InputSystem.actions.FindAction("MousePosition");
		mouseClickAction = InputSystem.actions.FindAction("MouseClick");
		ChangeColourOfChildren();
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
		ChangeColourOfChildren();
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

	private void ChangeColourOfChildren()
	{
		foreach (Transform child in bodyContainer.transform)
		{
			if (child.TryGetComponent(out Renderer renderer))
			{
				// hsv colour
				renderer.material.color = Color.HSVToRGB(child.GetSiblingIndex() / (float)bodyContainer.transform.childCount, 1, 1);
				// also recolour children's children
				foreach (Transform grandchild in child)
				{
					if (grandchild.TryGetComponent(out TrailRenderer grandchildTrailRenderer))
					{
						grandchildTrailRenderer.startColor = Color.HSVToRGB(child.GetSiblingIndex() / (float)bodyContainer.transform.childCount, 1, 1);
						grandchildTrailRenderer.endColor = new Color(0, 0, 0, 0);
					}
				}
			}
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
		// add a new object button
		if (GUI.Button(new Rect(Screen.width - 220, Screen.height - 50, 100, 40), "Dodaj ciało"))
		{
			int i = 1;
			// find the next available name
			while (GameObject.Find(i.ToString()) != null) { i++; }

			// create an instance of the prefab and name it
			GameObject newBody = Instantiate(bodyPrefab, Vector3.zero, Quaternion.identity, bodyContainer.transform);
			newBody.name = i.ToString();
			ClearSelection();
			selectedObject = newBody;
			newBody.GetComponent<Renderer>().material.EnableKeyword("_EMISSION");
		}

		// presets
		if (GUI.Button(new Rect(10, Screen.height - 90, 100, 40), "3-kąt"))
		{

		}

		if (selectedObject == null) { return; }

		GUI.Label(new Rect(Screen.width - 210, 10, 200, 30),
		"Zaznaczone ciało:\nnr " + selectedObject.name + "\n\nm[kg] =\n\nPozycja [m]\nX =\nY =\nZ =\n\nObrót [°]\nX =\nY =\nZ =\n\nPrędkość [m/s] =");

		_ = GUI.TextField(new Rect(10, 30, 200, 30), "inputText", 25);

		// remove the selected object button
		if (GUI.Button(new Rect(Screen.width - 110, 45, 100, 40), "Usuń ciało"))
		{
			Destroy(selectedObject);
			ClearHover();
			ClearSelection();
		}
	}
}
