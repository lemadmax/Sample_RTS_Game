using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBarManager : MonoBehaviour
{
    public GameObject bar;
    public GameObject theBar;

    public void SetColor(Color color)
    {
        theBar.GetComponent<SpriteRenderer>().color = color;
    }
    public void SetBar(Vector3 newBar)
    {
        transform.right = -Camera.main.transform.right;
        transform.forward = -Camera.main.transform.forward;
        bar.transform.localScale = newBar;
    }
}
