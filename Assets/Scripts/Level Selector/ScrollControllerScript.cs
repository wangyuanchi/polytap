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
    private Coroutine scrollCoroutine;

    [Header("Animation")]
    [SerializeField] private float scrollDuration;
    [SerializeField] private AnimationCurve slideEase;
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
        // Accomodates for multiple calls if previous animation has not completed
        if (scrollCoroutine != null)
        {
            StopCoroutine(scrollCoroutine);
        }
        scrollCoroutine = StartCoroutine(ScrollToPosition(targetPos.x));

        nextButton.interactable = true;
        prevButton.interactable = true;
        if (currentPage == maxPage) nextButton.interactable = false;
        if (currentPage == 1) prevButton.interactable = false;
    }

    private IEnumerator ScrollToPosition(float targetX)
    {
        float currentX = levelsRectTransform.localPosition.x;
        float timeElapsed = 0f;

        while (timeElapsed < scrollDuration)
        {
            timeElapsed += Time.deltaTime;
            float lerpFactor = slideEase.Evaluate(timeElapsed / scrollDuration);
            levelsRectTransform.localPosition = new Vector3(Mathf.Lerp(currentX, targetX, lerpFactor),
                levelsRectTransform.localPosition.y, levelsRectTransform.localPosition.z);
            yield return null;
        }

        // Makes sure the final position is exact
        levelsRectTransform.localPosition = new Vector3(targetX, levelsRectTransform.localPosition.y, levelsRectTransform.localPosition.z);
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
