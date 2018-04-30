using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crosshairs : MonoBehaviour {

    public LayerMask targetMask;
    public SpriteRenderer center;
    public Color highlightColour;
    Color originalColour;

    void Start()
    {
        Cursor.visible = false;
        originalColour = center.color;
    }

    void Update () {
        transform.Rotate(Vector3.forward * -40 * Time.deltaTime);
	}

    public void onTarget(Ray ray)
    {
        if (Physics.Raycast(ray, 100, targetMask))
        {
            center.color = highlightColour;
        }
        else
        {
            center.color = originalColour;
        }
    }
}
