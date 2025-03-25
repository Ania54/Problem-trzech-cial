using UnityEngine;
using UnityEngine.InputSystem;

public class EditMode : MonoBehaviour
{
	public GameObject bodyContainer;
	public GameObject bodyPrefab;
	public GameObject point;
	private GameObject hoveredObject;
	private GameObject selBody;
	private InputAction mousePositionAction;
	private InputAction mouseClickAction;
	private InputAction rightClickAction;
	private InputAction startAction;
	private string[] strings;
	private string[] newStrings;
	private float tempFloat;
	private GameObject draggingObject = null;
	private Vector3 offset;
	private float zDepth;
	private bool collisions = false;
	private CamControl camControl;

	private void Start()
	{
		mousePositionAction = InputSystem.actions.FindAction("MousePosition");
		mouseClickAction = InputSystem.actions.FindAction("MouseClick");
		rightClickAction = InputSystem.actions.FindAction("RightMouseClick");
		startAction = InputSystem.actions.FindAction("Attack");

		camControl = Camera.main.GetComponent<CamControl>();
		ChangeColourOfChildren();
	}

	// selecting objects
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
					if (hitObject != selBody)
					{
						ClearSelection();
						selBody = hitObject;
						hitRenderer.material.EnableKeyword("_EMISSION");

						UpdateStats();

						// Prepare for dragging
						draggingObject = selBody;
						zDepth = Camera.main.WorldToScreenPoint(draggingObject.transform.position).z;
						offset = draggingObject.transform.position - Camera.main.ScreenToWorldPoint(new Vector3(mousePositionAction.ReadValue<Vector2>().x, mousePositionAction.ReadValue<Vector2>().y, zDepth));
					}
					// Optionally, deselect when clicking the already selected object:
					else
					{
						ClearSelection();
						ClearHover();
					}
				}

				// Handle hover: highlight objects that are not currently selected.
				if (hitObject != selBody && hitObject != hoveredObject && !draggingObject)
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

		// Handle dragging
		if (draggingObject != null && mouseClickAction.IsPressed())
		{
			Vector3 newMousePosition = new(mousePositionAction.ReadValue<Vector2>().x, mousePositionAction.ReadValue<Vector2>().y, zDepth);
			draggingObject.transform.position = Camera.main.ScreenToWorldPoint(newMousePosition) + offset;
			UpdateStats();
		}
		else if (mouseClickAction.WasReleasedThisFrame())
		{
			draggingObject = null;
		}
		if (rightClickAction.ReadValue<float>() > 0 && selBody != null)
		{
			// Look at the mouse
			point.transform.position = Camera.main.ScreenToWorldPoint(new Vector3(mousePositionAction.ReadValue<Vector2>().x, mousePositionAction.ReadValue<Vector2>().y, zDepth));
			if (camControl.currentMode == 2) { point.transform.position = new(point.transform.position.x, 0, point.transform.position.z); }
			point.SetActive(true);
			selBody.transform.LookAt(point.transform);
			UpdateStats();
		}
		else
		{
			point.SetActive(false);
		}

		ChangeColourOfChildren();
	}

	private void UpdateStats()
	{
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

	private void ClearHover()
	{
		if (hoveredObject != null && hoveredObject != selBody)
		{
			if (hoveredObject.TryGetComponent(out Renderer renderer)) { renderer.material.DisableKeyword("_EMISSION"); }
			hoveredObject = null;
		}
	}

	private void ClearSelection()
	{
		if (selBody != null)
		{
			if (selBody.TryGetComponent(out Renderer renderer)) { renderer.material.DisableKeyword("_EMISSION"); }
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

	private void ClearBodies()
	{
		ClearHover();
		ClearSelection();
		foreach (Transform child in bodyContainer.transform) { Destroy(child.gameObject); }
	}
	private void NewBody(float x, float y, float z, float rx, float ry, float rz, float m, float v)
	{
		int i = 1;
		// find the next available name
		while (GameObject.Find(i.ToString()) != null) { i++; }

		// create an instance of the prefab and name it
		GameObject newBody = Instantiate(bodyPrefab, new(x, y, z), Quaternion.Euler(new(rx, ry, rz)), bodyContainer.transform);
		newBody.GetComponent<Rigidbody>().mass = m;
		newBody.GetComponent<ApplyForce>().accel = v;
		newBody.name = i.ToString();
		ClearSelection();
		selBody = newBody;
		UpdateStats();
		newBody.GetComponent<Renderer>().material.EnableKeyword("_EMISSION");
	}

	private void OnGUI()
	{
		// checkbox
		collisions = GUI.Toggle(new Rect(Screen.width - 90, Screen.height - 90, 100, 40), collisions, "Kolizje");

		if (bodyContainer.transform.childCount > 0)
		{
			if (GUI.Button(new Rect(Screen.width - 110, Screen.height - 50, 100, 40), "Start [enter]") || startAction.WasPressedThisFrame())
			{
				foreach (Transform child in bodyContainer.transform)
				{
					foreach (Transform grandchild in child.transform)
					{
						if (grandchild.CompareTag("TheTrail"))
						{
							// enable the grandchild object
							grandchild.gameObject.SetActive(true);
						}
						else
						{
							// disable the grandchild object
							grandchild.gameObject.SetActive(false);
						}
					}
					child.GetComponent<SphereCollider>().enabled = collisions;
					child.GetComponent<SphereCollider>().isTrigger = !collisions;
					child.GetComponent<ApplyForce>().enabled = true;
				}
				ClearHover();
				ClearSelection();
				camControl.Start();
				enabled = false;
			}
		}
		// add a new object button
		if (GUI.Button(new Rect(Screen.width - 220, Screen.height - 50, 100, 40), "Dodaj ciało"))
		{
			NewBody(0, 0, 0, 0, 0, 0, 100, 50);
		}

		// presets
		if (GUI.Button(new Rect(10, Screen.height - 90, 100, 40), "Puste"))
		{
			ClearBodies();
		}
		if (GUI.Button(new Rect(120, Screen.height - 90, 100, 40), "2 ciała"))
		{
			ClearBodies();
			NewBody(03, 0, 0, 0, 000, 0, 100, 30);
			NewBody(-3, 0, 0, 0, 180, 0, 100, 30);
		}
		if (GUI.Button(new Rect(230, Screen.height - 90, 100, 40), "3-kąt"))
		{
			ClearBodies();
			NewBody(00003, 0, 00000000000000000000000000, 0, 000, 0, 100, 50);
			NewBody(-1.5f, 0, 03 * Mathf.Pow(3, .5f) / 2, 0, 240, 0, 100, 50);
			NewBody(-1.5f, 0, -3 * Mathf.Pow(3, .5f) / 2, 0, 120, 0, 100, 50);
		}
		if (GUI.Button(new Rect(340, Screen.height - 90, 100, 40), "4-kąt"))
		{
			ClearBodies();
			NewBody(03, 0, -3, 0, 000, 0, 100, 50);
			NewBody(03, 0, 03, 0, 090, 0, 100, 50);
			NewBody(-3, 0, 03, 0, 180, 0, 100, 50);
			NewBody(-3, 0, -3, 0, 270, 0, 100, 50);
		}
		if (GUI.Button(new Rect(10, Screen.height - 140, 100, 40), "4-ścian"))
		{
			ClearBodies();
		}
		if (GUI.Button(new Rect(120, Screen.height - 140, 100, 40), "6-ścian"))
		{
			ClearBodies();
			NewBody(03, 03, -3, 0, 000, 0, 100, 50);
			NewBody(03, 03, 03, 0, 090, 0, 100, 50);
			NewBody(-3, 03, 03, 0, 180, 0, 100, 50);
			NewBody(-3, 03, -3, 0, 270, 0, 100, 50);
			NewBody(03, -3, -3, 0, 180, 0, 100, 50);
			NewBody(03, -3, 03, 0, 270, 0, 100, 50);
			NewBody(-3, -3, 03, 0, 000, 0, 100, 50);
			NewBody(-3, -3, -3, 0, 090, 0, 100, 50);
		}

		if (selBody == null) { return; }

		GUI.Label(new Rect(Screen.width - 210, 10, 200, 30),
		"Zaznaczone ciało:\nnr " + selBody.name + "\n\nm[kg] =\n\nPozycja [m]\nX =\nY =\nZ =\n\nObrót [°]\nX =\nY =\nZ =\n\nv₀[m/s] =");

		newStrings = new string[8]
		{
			GUI.TextField(new Rect(Screen.width - 120, 092, 110, 25), strings[0]),
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
				if (newStrings[i] is "" or "-") { tempFloat = 0; }
				else
				{
					if (newStrings[i].Contains(",")) { newStrings[i] = newStrings[i].Replace(",", "."); }
					if (!float.TryParse(newStrings[i], out tempFloat)) { continue; }
				}
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

		// deselect the body button
		if (GUI.Button(new Rect(Screen.width - 110, 450, 100, 40), "Ok"))
		{
			ClearHover();
			ClearSelection();
		}
	}
}
