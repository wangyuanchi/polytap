using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ToggleSwitchScript : MonoBehaviour, IPointerClickHandler
{
    private bool isEnabled;
    private Slider slider;

    [Header("Animation")]
    [SerializeField, Range(0, 1f)] private float animationDuration = 0.5f;
    [SerializeField] private AnimationCurve slideEase = AnimationCurve.EaseInOut(0, 0, 1, 1);
    private Coroutine animateSliderCoroutine;

    // Start is called before the first frame update
    void Start()
    {
        slider = GetComponent<Slider>();

        slider.interactable = false;
        var sliderColors = slider.colors;
        sliderColors.disabledColor = Color.white;
        slider.colors = sliderColors;
        slider.transition = Selectable.Transition.None;

        if (PlayerPrefs.GetString("Mode") == "N")
        {
            slider.value = 0;
            isEnabled = false;
        }
        else
        {
            slider.value = 1;
            isEnabled = true;
        }
    }
        
    public void OnPointerClick(PointerEventData eventData)
    {
        SetStateAndStartAnimation(!isEnabled);
    }

    private void SetStateAndStartAnimation(bool enabledState)
    {
        // Error where current state and target state is the same
        if (isEnabled == enabledState) return;

        isEnabled = enabledState;

        if (isEnabled) PlayerPrefs.SetString("Mode", "H");
        else PlayerPrefs.SetString("Mode", "N");

        // Prevents slider from moving to the end if the toggles are fast
        if (animateSliderCoroutine != null)
        { StopCoroutine(animateSliderCoroutine); }

        animateSliderCoroutine = StartCoroutine(AnimateSlider());
    }

    private IEnumerator AnimateSlider()
    {
        float startValue = slider.value;
        float endValue = isEnabled ? 1 : 0;

        float time = 0;
        if (animationDuration > 0)
        {
            while (time < animationDuration)
            {
                time += Time.deltaTime;

                float lerpFactor = slideEase.Evaluate(time / animationDuration);
                slider.value = Mathf.Lerp(startValue, endValue, lerpFactor);

                yield return null;
            }
        }

        // Set the slider value to the new exact value after toggle
        slider.value = endValue;
    }
}
