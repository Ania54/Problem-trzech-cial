using System;
using UnityEngine;

public class ApplyForce : MonoBehaviour
{
	readonly float G = 10e8f * 6.6743e-11f;
	public float accel = 50;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
		// apply a force in the direction of looking
		GetComponent<Rigidbody>().AddForce(transform.forward * accel, ForceMode.Acceleration);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
		// for every sibling object
		foreach (Transform sibling in transform.parent)
		{
			float distance = (float)Math.Pow(Vector3.Distance(transform.position, sibling.transform.position), 2);
			if (distance == 0) continue;
			Vector3 target = G * GetComponent<Rigidbody>().mass * sibling.GetComponent<Rigidbody>().mass / distance * - (transform.position - sibling.transform.position).normalized;
			GetComponent<Rigidbody>().AddForce(target, ForceMode.Force);
		}
    }
}
