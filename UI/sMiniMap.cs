using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class sMiniMap : MonoBehaviour
{
    [SerializeField] bool fixedMiniMap;
    Transform cameraTarget;

    [SerializeField] GameObject miniMapObjectPrefab;
    [SerializeField] Transform miniMapParent;
    [SerializeField] Transform villageImage;
    [SerializeField] Transform wildernessImage;
    [SerializeField] float villageImageMoveSpeed;
    [SerializeField] float wildernessImageMoveSpeed;

    List<GameObject> miniMapGameObjects = new List<GameObject>();
    List<sMiniMapObject> miniMapObjects = new List<sMiniMapObject>();

    [SerializeField] float miniMapScale;
    [SerializeField] float maxMiniMapViewDistance;

    Vector2[] villageImageSpawnLocation = new Vector2[]
    {
        new Vector2(17.5f, 331.7f) 
    };
    Vector2[] wildernessImageSpawnLocation = new Vector2[]
    {
        new Vector2(-7.7f, -264f)
    };

    // Start is called before the first frame update
    void Start()
    {
        cameraTarget = GameManager.instance.cameraTransform;
    }

    // Update is called once per frame
    void Update()
    {
        if (fixedMiniMap)
        {
            miniMapParent.rotation = Quaternion.identity;
        }
        else
        {
            miniMapParent.rotation = Quaternion.Euler(0, 0, cameraTarget.rotation.eulerAngles.y);
        }

        bool isVillage = false;

        if (sSceneController.instance.currentSceneType == SceneType.Village)
        {
            isVillage = true;
        }

        Vector3 temp = GameManager.instance.playerMovement.cc.velocity;
        temp.y = temp.z;
        temp.z = 0f;

        if (sSceneController.instance.currentSceneType == SceneType.Village)
        {
            villageImage.localPosition -= temp * Time.deltaTime * villageImageMoveSpeed;
        }
        else
        {
            wildernessImage.localPosition -= temp * Time.deltaTime * wildernessImageMoveSpeed;
        }

        for (int i = 0; i < miniMapObjects.Count; i++)
        {
            Vector2 offset = miniMapObjects[i].GetCurrentOffset();


            if (offset.magnitude < maxMiniMapViewDistance)
            {
                if (miniMapObjects[i].inVillage && isVillage || !miniMapObjects[i].inVillage && !isVillage)
                {
                    miniMapGameObjects[i].SetActive(true);
                    miniMapGameObjects[i].transform.localPosition = offset * miniMapScale;
                }
                else
                {
                    miniMapGameObjects[i].SetActive(false);
                }
            }
            else 
            {
                miniMapGameObjects[i].SetActive(false);
            }
        }

    }

    public RectTransform AddMiniMapObject(sMiniMapObject newMiniMapObject)
    {
        GameObject temp = Instantiate(miniMapObjectPrefab, miniMapParent);
        miniMapGameObjects.Add(temp);
        miniMapObjects.Add(newMiniMapObject);

        return temp.GetComponent<RectTransform>();
    }

    public void ChangeMiniMapStyle(bool isFixed)
    {
        fixedMiniMap = isFixed;
    }

    public void SetMapImageToSpawnLocation(int villageNumber, bool showVillageImage)
    {
        villageImage.gameObject.SetActive(showVillageImage);
        wildernessImage.gameObject.SetActive(!showVillageImage);
        villageImage.localPosition = villageImageSpawnLocation[villageNumber];
        wildernessImage.localPosition = wildernessImageSpawnLocation[villageNumber];
    }
}
