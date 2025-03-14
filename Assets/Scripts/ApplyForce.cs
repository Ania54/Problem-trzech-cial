using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ApplyForce : MonoBehaviour
{
	private readonly float G = 10e8f * 6.6743e-11f;
	public float accel = 50;
	// Start is called once before the first execution of Update after the MonoBehaviour is created
	private void Start()
	{
		// apply a force in the direction of looking
		GetComponent<Rigidbody>().AddForce(transform.forward * accel, ForceMode.Acceleration);
	}

	// Update is called once per frame
	private void FixedUpdate()
	{
		// for every sibling object
		foreach (Transform sibling in transform.parent)
		{
			float distance = (float)Math.Pow(Vector3.Distance(transform.position, sibling.transform.position), 2);
			if (distance == 0) { continue; }

			Vector3 target = G * GetComponent<Rigidbody>().mass * sibling.GetComponent<Rigidbody>().mass / distance * -(transform.position - sibling.transform.position).normalized;
			GetComponent<Rigidbody>().AddForce(target, ForceMode.Force);
		}
	}

	private void OnGUI()
	{
		if (GUI.Button(new Rect(Screen.width - 110, Screen.height - 50, 100, 40), "Stop"))
		{
			SceneManager.LoadScene(SceneManager.GetActiveScene().name);
		}
	}
}
