using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HoverBackgroundScript : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Image hoverBackground;
    private Coroutine fadeCoroutine;

    void Start()
    {
        // Start transparent
        Color newColor = hoverBackground.color;
        newColor.a = 0f;
        hoverBackground.color = newColor;
    }
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (fadeCoroutine != null) { StopCoroutine(fadeCoroutine); }
        fadeCoroutine = StartCoroutine(FadeTo(0.75f));
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (fadeCoroutine != null) { StopCoroutine(fadeCoroutine); }
        fadeCoroutine = StartCoroutine(FadeTo(0f));
    }
        
    private IEnumerator FadeTo(float targetAlpha)
    {
        float time = 0f;
        float fadeDuration = 0.5f;
        Color originalColor = hoverBackground.color;
           
        while (time < fadeDuration) 
        { 
            time += Time.deltaTime;
            float alpha = Mathf.Lerp(originalColor.a, targetAlpha, time / fadeDuration);
            Color newColor = originalColor;
            newColor.a = alpha;
            hoverBackground.color = newColor;
            yield return null;
        }

        hoverBackground.color = new Color(originalColor.r, originalColor.g, originalColor.b, targetAlpha);
    }
}