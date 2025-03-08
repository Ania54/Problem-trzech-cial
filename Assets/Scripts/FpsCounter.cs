using UnityEngine;

public class FpsCounter : MonoBehaviour
{
	private float deltaTime = 0.0f;

	void Update()
	{
		// Calculate the time between frames
		deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
	}

	void OnGUI()
	{
		// Calculate FPS
		float fps = 1.0f / deltaTime;
		string text = string.Format("{0:0.} FPS", fps);

		// Set the style for the label
		GUIStyle style = new()
		{
			fontSize = 24
		};
		style.normal.textColor = Color.white;

		// Draw the FPS label in the upper-left corner
		GUI.Label(new Rect(10, 10, 100, 25), text, style);
		// Draw debug label below
		// GUI.Label(new Rect(10, 40, 100, 25), 216.ToString(), style);
	}
}
