using UnityEngine;
using UnityEngine.InputSystem;

public class DebugCamMove : MonoBehaviour
{
    public float speed = 6f;

    // Update is called once per frame
    void Update()
    {
		float leftRight = InputSystem.actions.FindAction("Move").ReadValue<Vector2>().x;
		float   forBack = InputSystem.actions.FindAction("Move").ReadValue<Vector2>().y;

		Vector3 flatMove = speed * Time.deltaTime * new Vector3(leftRight, 0, forBack);
        transform.Translate(flatMove);
    }
}
