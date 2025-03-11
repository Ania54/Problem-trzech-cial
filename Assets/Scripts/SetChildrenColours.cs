using UnityEngine;

public class SetChildrenColours : MonoBehaviour
{

    void Start()
    {
        ChangeColourOfChildren();
    }

    void ChangeColourOfChildren()
    {
        foreach (Transform child in transform)
        {
            if (child.TryGetComponent<Renderer>(out var renderer))
            {
				// hsv colour
                renderer.material.color = Color.HSVToRGB(child.GetSiblingIndex() / (float)transform.childCount, 1, 1);
				// also recolour children's children
				foreach (Transform grandchild in child)
				{
					if (grandchild.TryGetComponent<TrailRenderer>(out var grandchildTrailRenderer))
					{
						grandchildTrailRenderer.startColor = Color.HSVToRGB(child.GetSiblingIndex() / (float)transform.childCount, 1, 1);
						grandchildTrailRenderer.endColor = new Color(0, 0, 0, 0);
					}
				}
            }
        }
    }
}
