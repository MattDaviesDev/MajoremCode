using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class sVillageBoardToolTip : MonoBehaviour
{
    
    [SerializeField] TextMeshProUGUI nameText;
    [SerializeField] TextMeshProUGUI descriptionText;
    [SerializeField] Image effectorImage;

    [SerializeField] public Transform tooltipHolder;

    public void UpdatePosition()
    {
        tooltipHolder.position = sInputManager.instance.mousePositionAction.ReadValue<Vector2>();
        print(sInputManager.instance.mousePositionAction.ReadValue<Vector2>());
    } 

    public void PopulateTooltip(string effectorName, string effectorDescritpion, Sprite effectorSprite)
    {
        nameText.text = effectorName;
        descriptionText.text = effectorDescritpion;
        effectorImage.sprite = effectorSprite;
    }
}
