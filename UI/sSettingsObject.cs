using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


public class sSettingsObject : MonoBehaviour, IPointerEnterHandler
{
    [SerializeField] string settingName;
    [SerializeField, Multiline] string settingDescription;

    public void OnPointerEnter(PointerEventData eventData)
    {
        sSettings.instance.UpdateSettingData(settingName, settingDescription);
    }

}
