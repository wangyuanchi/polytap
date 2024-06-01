using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ScrollControllerScript : MonoBehaviour, IEndDragHandler
{
    [Header("General")]
    [SerializeField] private Vector3 pageStep;
    [SerializeField] private RectTransform levelsRectTransform;
    private static int currentPage = 1;
    private int maxPage;
    private Vector3 targetPos;

    [Header("Navigation")]
    [SerializeField] private Button nextButton;
    [SerializeField] private Button prevButton;

    [Header("Animation")]
    [SerializeField] private float tweenTime;
    [SerializeField] private LeanTweenType tweenType;
    private float dragThreshold;

    void Start()
    {
        // Shift to the previously accessed level
        levelsRectTransform.localPosition += (currentPage - 1) * pageStep;

        maxPage = levelsRectTransform.childCount;
        targetPos = levelsRectTransform.localPosition;
        dragThreshold = Screen.width / 15;

        // Move to current page, for setting of interactability of the nav buttons 
        MovePage();
    }

    public void Next()
    {
        if (currentPage < maxPage)
        {
            currentPage++;
            targetPos += pageStep;
            MovePage();
        }
    }

    public void Previous()
    {
        if (currentPage > 1)
        {
            currentPage--;
            targetPos -= pageStep;
            MovePage(); 
        }
    }
    
    private void MovePage()
    {
        levelsRectTransform.LeanMoveLocal(targetPos, tweenTime).setEase(tweenType);
        nextButton.interactable = true;
        prevButton.interactable = true;
        if (currentPage == maxPage) nextButton.interactable = false;
        if (currentPage == 1) prevButton.interactable = false;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (Mathf.Abs(eventData.position.x - eventData.pressPosition.x) > dragThreshold)
        {
            if (eventData.position.x > eventData.pressPosition.x)
            {
                Previous();
            }
            else
            {
                Next();
            }
        }
        else
        {
            MovePage();
        }
    }
}
