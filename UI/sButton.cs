using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class sButton : MonoBehaviour, IPointerEnterHandler, IPointerDownHandler, IPointerExitHandler
{
    enum ButtonStates
    {
        Normal, Highlighted, Pressed
    }

    Button myButton;

    TextMeshProUGUI buttonText;

    CanvasGroup parentCanvas;

    ButtonStates currentState = ButtonStates.Normal;

    Color normalCol;
    Vector3 normalSize;
    Color highlightedCol;
    Vector3 highlightedSize;
    Color selectedCol;
    Vector3 selectedSize;

    const float lerpTime = 0.2f;

    private void Awake()
    {
        myButton = GetComponent<Button>();
        buttonText = GetComponentInChildren<TextMeshProUGUI>();
        parentCanvas = GetComponentInParent<CanvasGroup>();

        normalCol = buttonText.color;
        normalCol.a = 0.5f;
        normalSize = Vector3.one * 0.8f;

        highlightedCol = normalCol;
        highlightedCol.a = 1f;
        highlightedSize = Vector3.one;

        selectedCol = normalCol;
        selectedCol.a = 0.9f;
        selectedSize = Vector3.one * 0.9f;
    }

    private void OnDisable()
    {
        currentState = ButtonStates.Normal;
    }


    // Update is called once per frame
    void Update()
    {
        if (parentCanvas != null)
        {
            if (parentCanvas.blocksRaycasts == false || parentCanvas.interactable == false)
            {
                currentState = ButtonStates.Normal;
                buttonText.color = normalCol;
                buttonText.transform.localScale = normalSize;
            }
        }


        switch (currentState)
        {
            case ButtonStates.Normal:
                buttonText.color = Color.Lerp(buttonText.color, normalCol, lerpTime);
                buttonText.transform.localScale = Vector3.Lerp(buttonText.transform.localScale, normalSize, lerpTime);
                break;
            case ButtonStates.Highlighted:
                buttonText.color = Color.Lerp(buttonText.color, highlightedCol, lerpTime);
                buttonText.transform.localScale = Vector3.Lerp(buttonText.transform.localScale, highlightedSize, lerpTime);
                break;
            case ButtonStates.Pressed:
                buttonText.color = Color.Lerp(buttonText.color, selectedCol, lerpTime);
                buttonText.transform.localScale = Vector3.Lerp(buttonText.transform.localScale, selectedSize, lerpTime);
                break;
        }


    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        currentState = ButtonStates.Highlighted;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        currentState = ButtonStates.Pressed;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        currentState = ButtonStates.Normal;
    }
}
