using UnityEngine;

public class EditMode : MonoBehaviour
{
    public GameObject bodyContainer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnGUI()
    {
        if (GUI.Button(new Rect(Screen.width - 100, Screen.height - 40, 90, 30), "Start"))
        {
            foreach (Transform child in bodyContainer.transform)
            {
                child.GetComponent<ApplyForce>().enabled = true;
            }
            enabled = false;
        }
    }
}
