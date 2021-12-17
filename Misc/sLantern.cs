using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sLantern : MonoBehaviour, IInteractableObject
{
    Rigidbody rb;

    [SerializeField] Light myLight;
    [SerializeField] Renderer myLanternRend;

    string emissionParam = "_Emission";

    const float startEmissionValue = 0f;
    const float endEmissionValue = 28000f;

    const float startIntensityValue = 0f;
    const float endIntensityValue = 40000f;

    Coroutine activating;

    bool on;

    public Vector3 forceDir;

    Color emissionColor;

    sDecoration myDecoration;
    BoxCollider myCol;

    [Header("lantern sway")]
    [SerializeField] private float hitMultiplier = 10f;
    [SerializeField] private Transform impactLocation;

    private Vector3 contactPoint;
    private Vector3 reflectionVelocity;

    SFX turnOnSFX;
    SFX turnOffSFX;
    SFX[] hitEffects;
    SFX[] squeakEffects;
    sAudioObject ambientSqueak;

    const float timeTillCanPlay = 0.2f;
    float tLastPlayed = 0f;


    private void Awake()
    {
        emissionColor = Color.white;
    }

    private void Start()
    {
        if (GetComponentInParent<sDecoration>())
        {
            myDecoration = GetComponentInParent<sDecoration>();
            myCol = GetComponent<BoxCollider>();
        }
        rb = GetComponent<Rigidbody>();

        turnOnSFX = AssetFinder.instance.lanternTurnOnSFX;
        turnOffSFX = AssetFinder.instance.lanternTurnOffSFX;
        hitEffects = AssetFinder.instance.lanternHits;
        squeakEffects = AssetFinder.instance.lanternSqueaks;
        ambientSqueak = GetComponentInChildren<sAudioObject>();

        ambientSqueak.source.clip = squeakEffects[Random.Range(0, squeakEffects.Length)].sfx;
    }

    private void Update()
    {
        if (myDecoration != null)
        {
            if (myDecoration.isPlaced)
            {
                myCol.enabled = true;
            }
            else
            {
                myCol.enabled = false;
            }
        }
        float temp = rb.angularVelocity.magnitude;

        if (temp < 0.1f)
        {
            temp = 0f;
        }

        temp /= 1f;

        temp = Mathf.Clamp(temp, 0f, 1f);
        if (temp <= 0.1f)
        {
            temp = 0f;
        }

        if (ambientSqueak != null)
        {
            ambientSqueak.source.volume = Mathf.Lerp(0f, 1f, temp);
        }

        if (tLastPlayed > 0f)
        {
            tLastPlayed -= Time.deltaTime;
        }
    }

    public void ActivateLantern()
    {
        if (activating == null && !on)
        {
            activating = StartCoroutine(LerpEmission(startIntensityValue, endIntensityValue, startEmissionValue, endEmissionValue));
            on = true;
            SFXAudioManager.Create3DSFX(turnOnSFX, transform.position);
        }
    }

    public void DeactivateLantern()
    {
        if (activating == null && on)
        {
            activating = StartCoroutine(LerpEmission(endIntensityValue, startIntensityValue, endEmissionValue, startEmissionValue));
            on = false;
            if (turnOffSFX != null)
            {
                SFXAudioManager.Create3DSFX(turnOffSFX, transform.position);
            }

        }
    }

    IEnumerator LerpEmission(float a, float b, float c, float d)
    {
        float t = 0f;

        while (t <= 1f)
        {
            t += Time.deltaTime / 1f;
            myLight.intensity = Mathf.Lerp(a, b, Lerp.EaseIn(t));
            myLanternRend.material.SetColor(emissionParam, emissionColor * Mathf.Lerp(c, d, Lerp.EaseIn(t)));
            yield return null;
        }
        activating = null;
    }

    public string ReturnInteractActionWord()
    {
        if (on)
        {
            return "Turn off Lantern";
        }
        return "Turn on Lantern";
    }

    public string ReturnAlternateInteractActionWord()
    {
        return "";
    }

    void ChangeState()
    {
        if (on)
        {
            DeactivateLantern();
        }
        else
        {
            ActivateLantern();
        }
    }

    public bool Interact()
    {
        //if (!GameManager.instance.dayCycle.isDay)
        //{
        //    ChangeState();
        //    return true;
        //}

        ChangeState();
        //if (on)
        //{
        //}
        return true;    
    }

    public bool AlternateInteract()
    {
        return false;
    }

    public bool CanInteract()
    {
        //if (!GameManager.instance.dayCycle.isDay)
        //{
        //    return true;
        //}
        //else if (on)
        //{
        //    return true;
        //}
        //return false;
        return true;
    }

    public bool CanAlternateInteract()
    {
        return false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (tLastPlayed <= 0f)
        {
            if (other.CompareTag("Player"))
            {
                SFXAudioManager.Create3DSFX(hitEffects[Random.Range(0, hitEffects.Length)], transform.position);
                if (ambientSqueak.source.volume <= 0.05f)
                {
                    ambientSqueak.source.clip = squeakEffects[Random.Range(0, squeakEffects.Length)].sfx;
                }
                ambientSqueak.Play();
                tLastPlayed = timeTillCanPlay;
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            contactPoint = other.ClosestPoint(impactLocation.position);
            reflectionVelocity = impactLocation.position - contactPoint;
            rb.AddForceAtPosition(reflectionVelocity * hitMultiplier, contactPoint, ForceMode.Impulse);
        }
    }

}
