using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class sWaterBody : MonoBehaviour
{

    Collider _col;

    Collider Col
    {
        get
        {
            if (_col == null)
            {
                _col = GetComponent<Collider>();
                _col.isTrigger = true;
            }
            return _col;
        }
    }

    bool playerInWaterBody;

    List<VisualEffect> waterSplash = new List<VisualEffect>();
    GameObject prefab;

    SFX waterSplashSFX;

    // Start is called before the first frame update
    void Start()
    {
        //prefab = Instantiate(AssetFinder.instance.waterSplashVFX, transform.position, AssetFinder.instance.waterSplashVFX.transform.rotation).GetComponent<VisualEffect>();
        prefab = AssetFinder.instance.waterSplashVFX;
        waterSplashSFX = AssetFinder.instance.waterSplashSFX;
    }

    // Update is called once per frame
    void Update()
    {
        if (playerInWaterBody)
        {
            float playerYPos = GameManager.instance.player.position.y;

            if (playerYPos < transform.position.y - 0.9f)
            {
                GameManager.instance.playerMovement.swimming = true;
            }
            else
            {
                GameManager.instance.playerMovement.swimming = false;
            }
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.transform == GameManager.instance.player)
        {
            playerInWaterBody = true;
            GameManager.instance.playerMovement.stoodInWater = true;
            if (GameManager.instance.playerMovement.velocity.y < -4f)
            {
                VisualEffect tempWaterSplash = GetFreeVFX();
                Vector3 playerPos = GameManager.instance.player.position;
                tempWaterSplash.transform.position = new Vector3(playerPos.x, transform.position.y, playerPos.z);
                Vector3 temp = GameManager.instance.playerMovement.velocity.normalized;
                temp.y = 0f;
                tempWaterSplash.SetVector3("PlayerVelocity", temp.normalized * 1f);
                tempWaterSplash.gameObject.SetActive(true);
                tempWaterSplash.Play();
                SFXAudioManager.CreateSFX(waterSplashSFX);
            }
        }
    }

    VisualEffect GetFreeVFX()
    {
        for (int i = 0; i < waterSplash.Count; i++)
        {
            if (!waterSplash[i].gameObject.activeInHierarchy)
            {
                return waterSplash[i];
            }
        }

        GameObject temp = Instantiate(prefab, transform.position, AssetFinder.instance.waterSplashVFX.transform.rotation);
        waterSplash.Add(temp.GetComponent<VisualEffect>());

        return waterSplash[waterSplash.Count - 1];
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.transform == GameManager.instance.player)
        {
            playerInWaterBody = false;
            GameManager.instance.playerMovement.stoodInWater = false;
        }
    }
}
