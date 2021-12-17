using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sBush : MonoBehaviour
{
    Renderer rend;

    bool shake;
    float t = 0f;

    const string _windDensity = "Vector1_CE098D07";
    float startWindDensity = 0.01f;
    float midWindDensity = 0.015f;
    float endWindDensity = 0.4f;

    float targetDensity;

    SFX[] shakeSFX;

    // Start is called before the first frame update
    void Start()
    {
        rend = GetComponent<Renderer>();
        shakeSFX = AssetFinder.instance.bushShakeSFX;
    }

    // Update is called once per frame
    void Update()
    {
        if (shake)
        {
            t += Time.deltaTime / 0.3f;
            if (t >= 1f)
            {
                shake = false;  
            }
        }
        else
        {
            t -= Time.deltaTime / 0.5f;
        }
        t = Mathf.Clamp01(t);
        float windVal = Mathf.Lerp(startWindDensity, targetDensity, t);
        rend.material.SetFloat(_windDensity, windVal);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            shake = true;  
            if (GameManager.instance.playerMovement.desiredMoveSpeed > GameManager.instance.playerMovement.walkMoveSpeed + 1f)
            {
                targetDensity = endWindDensity;
            }
            else
            {
                targetDensity = midWindDensity;
            }
            float volumeMultiplier = targetDensity == midWindDensity ? 0.3f : 1f;
            if (shakeSFX != null)
            {
                SFXAudioManager.CreateSFX(shakeSFX[Random.Range(0, shakeSFX.Length)], volumeMultiplier);
            }
        }
    }
}
