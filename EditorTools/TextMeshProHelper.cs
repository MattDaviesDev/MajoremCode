using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEditor;

namespace TextMeshProHelper
{
    public enum TextMeshType
    {
        Header, Body
    }

    public class TextMeshProHelper : MonoBehaviour
    {

        [Header("Defaults")]
        [SerializeField] TextMeshType defaultType = TextMeshType.Body;
        [SerializeField] Color defaultColor;

        [Header("Header")]
        [SerializeField] bool setHeaderFontAsset = false;
        [SerializeField] TMP_FontAsset headerFontAsset;
        [SerializeField] Material headerFontMaterial;
        [Space(5)]
        [SerializeField] bool setHeaderColor = false;
        [SerializeField] Color headerColor;
        [Space(5)]
        [SerializeField] bool setHeaderSize = false;
        [SerializeField] float headerSize;

        [Header("Body")]
        [SerializeField] bool setBodyFontAsset = false;
        [SerializeField] TMP_FontAsset bodyFontAsset;
        [SerializeField] Material bodyFontMaterial;
        [Space(5)]
        [SerializeField] bool setBodyColor = false;
        [SerializeField] Color bodyColor;
        [Space(5)]
        [SerializeField] bool setBodySize = false;
        [SerializeField] float bodySize;

        public void ChangeAllFontAssets()
        {
            int changeCounter = 0;
            List<TextMeshProUGUI> textsInScene = GetAllTextMeshProUGUI();
            for (int i = 0; i < textsInScene.Count; i++)
            {
                // setting default type
                TextMeshType currentType = defaultType;
                if (textsInScene[i].TryGetComponent(out sTextMeshObject scr))
                {
                    // overriding default type if type is available
                    currentType = scr.textType;
                    // skip to next text object if this one is locked
                    if (scr.locked)
                    {
                        continue;
                    }
                }
                bool setFont = true;
                TMP_FontAsset newFontAsset = null;
                Material newMaterial = null;
                bool setColor = true;
                Color newColor = Color.white;
                bool setSize = true;
                float newSize = 10f;

                // assigning the values based on whether its a header of body font object
                switch (currentType)
                {
                    case TextMeshType.Header:
                        setFont = setHeaderFontAsset;
                        newFontAsset = headerFontAsset;
                        newMaterial = headerFontMaterial;

                        setColor = setHeaderColor;
                        newColor = headerColor;

                        setSize = setHeaderSize;
                        newSize = headerSize;
                        break;
                    case TextMeshType.Body:
                        setFont = setBodyFontAsset;
                        newFontAsset = bodyFontAsset;
                        newMaterial = bodyFontMaterial;

                        setColor = setBodyColor;
                        newColor = bodyColor;

                        setSize = setBodySize;
                        newSize = bodySize;
                        break;
                }
                // setting the font
                if (setFont)
                {
                    textsInScene[i].font = newFontAsset;
                    textsInScene[i].fontSharedMaterial = newMaterial;
                }
                // setting the color
                if (setColor)
                {
                    textsInScene[i].color = newColor;
                }
                // setting the size
                if (setSize)
                {
                    textsInScene[i].fontSize = newSize;
                }
                if (setSize || setFont || setColor)
                {
                    changeCounter++;
                }
            }
            Debug.Log(changeCounter + " TextMeshProUGUI objects changed.");
        }

        List<TextMeshProUGUI> GetAllTextMeshProUGUI()
        {
            List<TextMeshProUGUI> returnList = new List<TextMeshProUGUI>();

            foreach (GameObject go in Resources.FindObjectsOfTypeAll(typeof(GameObject)) as GameObject[])
            {
                if (go.GetInstanceID() > 0)
                {
                    if (go.TryGetComponent(out TextMeshProUGUI txt))
                    {
                        returnList.Add(txt);
                    }
                }
            }
            return returnList;
        }
    }


#if UNITY_EDITOR
    [CustomEditor(typeof(TextMeshProHelper))]
    public class TextMeshHelperEditor : Editor
    {
        TextMeshProHelper textMeshHelper;

        public override void OnInspectorGUI()
        {
            textMeshHelper = (TextMeshProHelper)target;

            DrawDefaultInspector();

            GUILayout.Space(10);

            if (GUILayout.Button("Update all text objects"))
            {
                textMeshHelper.ChangeAllFontAssets();
            }

        }
    }
#endif

}



