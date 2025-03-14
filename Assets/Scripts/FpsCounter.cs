using UnityEngine;

public class FpsCounter : MonoBehaviour
{
	private float deltaTime = 0.0f;

	private void Update()
	{
		// Calculate the time between frames
		deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
	}

	private void OnGUI()
	{
		// Calculate FPS
		float fps = 1.0f / deltaTime;
		string text = string.Format("{0:0.} FPS", fps);

		GUI.skin.label.fontSize = 24;
		GUI.skin.label.clipping = TextClipping.Overflow;
		GUI.skin.label.wordWrap = false;
		GUI.Label(new Rect(10, 10, 100, 25), text);
	}
}
