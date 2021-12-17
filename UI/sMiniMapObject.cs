using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sMiniMapObject : MonoBehaviour
{
    public bool inVillage = false;

    [SerializeField] bool alwaysOnMiniMap;

    [SerializeField] Sprite miniMapSprite;

    sMiniMap miniMap;
    [SerializeField] float sizeOfSprite = 10f;
    public RectTransform myMiniMapObject;

    Transform player;

    // Start is called before the first frame update
    void Start()
    {
        player = GameManager.instance.player;

        miniMap = sMainUI.instance.miniMap;

        myMiniMapObject = miniMap.AddMiniMapObject(this);

        myMiniMapObject.sizeDelta = new Vector2(sizeOfSprite, sizeOfSprite);
        myMiniMapObject.GetComponent<UnityEngine.UI.Image>().sprite = miniMapSprite;
        myMiniMapObject.GetComponent<UnityEngine.UI.Image>().color = Color.white;

        if (transform.root == sSceneController.instance.villageParent)
        {
            inVillage = true;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public Vector2 GetCurrentOffset()
    {
        Vector3 offset = transform.position - player.position;

        return new Vector2(offset.x, offset.z);
    }
}
