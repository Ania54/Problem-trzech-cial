using UnityEngine;
using UnityEngine.InputSystem;

public class EditMode : MonoBehaviour
{
	public GameObject bodyContainer;
	public GameObject bodyPrefab;
	private GameObject hoveredObject;
	private GameObject selBody;
	private InputAction mousePositionAction;
	private InputAction mouseClickAction;
	private string[] strings;
	private string[] newStrings;
	private float tempFloat;

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
					if (selBody != hitObject)
					{
						ClearSelection();
						selBody = hitObject;
						hitRenderer.material.EnableKeyword("_EMISSION");

						strings = new string[8]
						{
							selBody.GetComponent<Rigidbody>().mass.ToString(),
							selBody.transform.position.x.ToString(),
							selBody.transform.position.y.ToString(),
							selBody.transform.position.z.ToString(),
							selBody.transform.rotation.eulerAngles.x.ToString(),
							selBody.transform.rotation.eulerAngles.y.ToString(),
							selBody.transform.rotation.eulerAngles.z.ToString(),
							selBody.GetComponent<ApplyForce>().accel.ToString()
						};
					}
					// Optionally, you might want to deselect when clicking the already selected object:
					else
					{
						ClearSelection();
						ClearHover();
					}
				}

				// Handle hover: highlight objects that are not currently selected.
				if (hitObject != selBody && hoveredObject != hitObject)
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
		if (hoveredObject != null && hoveredObject != selBody)
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
		if (selBody != null)
		{
			if (selBody.TryGetComponent(out Renderer renderer))
			{
				renderer.material.DisableKeyword("_EMISSION");
			}
			selBody = null;
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
			Camera.main.GetComponent<CamControl>().Start();
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
			selBody = newBody;
			newBody.GetComponent<Renderer>().material.EnableKeyword("_EMISSION");
		}

		// presets
		if (GUI.Button(new Rect(10, Screen.height - 90, 100, 40), "3-kąt"))
		{

		}
		if (GUI.Button(new Rect(120, Screen.height - 90, 100, 40), "4-kąt"))
		{

		}
		if (GUI.Button(new Rect(230, Screen.height - 90, 100, 40), "4-ścian"))
		{

		}
		if (GUI.Button(new Rect(340, Screen.height - 90, 100, 40), "6-ścian"))
		{

		}

		if (selBody == null) { return; }

		GUI.Label(new Rect(Screen.width - 210, 10, 200, 30),
		"Zaznaczone ciało:\nnr " + selBody.name + "\n\nm[kg] =\n\nPozycja [m]\nX =\nY =\nZ =\n\nObrót [°]\nX =\nY =\nZ =\n\nv₀[m/s] =");

		newStrings = new string[8]
		{
			GUI.TextField(new Rect(Screen.width - 120, 92, 110, 25), strings[0]),
			GUI.TextField(new Rect(Screen.width - 160, 171, 150, 25), strings[1]),
			GUI.TextField(new Rect(Screen.width - 160, 197, 150, 25), strings[2]),
			GUI.TextField(new Rect(Screen.width - 160, 223, 150, 25), strings[3]),
			GUI.TextField(new Rect(Screen.width - 160, 303, 150, 25), strings[4]),
			GUI.TextField(new Rect(Screen.width - 160, 329, 150, 25), strings[5]),
			GUI.TextField(new Rect(Screen.width - 160, 355, 150, 25), strings[6]),
			GUI.TextField(new Rect(Screen.width - 110, 410, 100, 25), strings[7])
		};
		for (int i = 0; i < 8; i++)
		{
			if (newStrings[i] != strings[i])
			{
				if (newStrings[i] == "") { tempFloat = 0; }
				else { if (!float.TryParse(newStrings[i], out tempFloat)) { continue; } }
				switch (i)
				{
					case 0:
						selBody.GetComponent<Rigidbody>().mass = tempFloat;
						break;
					case 1:
						selBody.transform.position = new Vector3(tempFloat, selBody.transform.position.y, selBody.transform.position.z);
						break;
					case 2:
						selBody.transform.position = new Vector3(selBody.transform.position.x, tempFloat, selBody.transform.position.z);
						break;
					case 3:
						selBody.transform.position = new Vector3(selBody.transform.position.x, selBody.transform.position.y, tempFloat);
						break;
					case 4:
						selBody.transform.rotation = Quaternion.Euler(tempFloat, selBody.transform.rotation.eulerAngles.y, selBody.transform.rotation.eulerAngles.z);
						break;
					case 5:
						selBody.transform.rotation = Quaternion.Euler(selBody.transform.rotation.eulerAngles.x, tempFloat, selBody.transform.rotation.eulerAngles.z);
						break;
					case 6:
						selBody.transform.rotation = Quaternion.Euler(selBody.transform.rotation.eulerAngles.x, selBody.transform.rotation.eulerAngles.y, tempFloat);
						break;
					case 7:
						selBody.GetComponent<ApplyForce>().accel = tempFloat;
						break;
					default:
						break;
				}
				strings[i] = newStrings[i];
			}
		}

		// remove the selected object button
		if (GUI.Button(new Rect(Screen.width - 110, 43, 100, 40), "Usuń ciało"))
		{
			Destroy(selBody);
			ClearHover();
			ClearSelection();
		}
	}
}
