using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoverFadeScript : MonoBehaviour
{
    // Make sure that shapes with this script attached has a collider 2D and a animator with "HoverFade" controller component attached 
    void OnMouseEnter()
    {
        if (GetComponent<Animator>() == null || GetComponent<Animator>() == null)
        {
            Debug.Log("Missing Component(s)!");
            return;
        }
        GetComponent<Animator>().SetTrigger("HoverFadeOut");
    }

    void OnMouseExit()
    {
        if (GetComponent<Animator>() == null || GetComponent<Animator>() == null)
        {
            Debug.Log("Missing Component(s)!");
            return;
        }
        GetComponent<Animator>().SetTrigger("HoverFadeIn");
    }
}
