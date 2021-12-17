using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum SurfaceTypes
{
    Dirt, Sand, Rock, Grass, Wood
}

public class sFootStep : MonoBehaviour
{
    public bool checkForFootSteps;

    public float speedMultipler = 1f;

    [SerializeField] Transform leftFoot;
    [SerializeField] Transform rightFoot;

    LayerMask footStepMask;

    SFX[] dirtFootsteps;
    SFX[] grassFootsteps;
    SFX[] sandFootsteps;
    SFX[] rockFootsteps;
    SFX[] woodFootsteps;

    bool leftFootDown = true;
    bool rightFootDown = true;

    const float leftFootMaxTimer = 0.2f;
    const float rightFootMaxTimer = 0.2f;
    float leftFootTimer = leftFootMaxTimer;
    float rightFootTimer = rightFootMaxTimer;

    bool leftFootCanStep = true;
    bool rightFootCanStep = true;

    public bool inWater;

    SFX[] waterFootSteps;

    public bool isSprinting;

    private void Start()
    {
        footStepMask = AssetFinder.instance.footStepMask;
        dirtFootsteps = AssetFinder.instance.dirtFootsteps;
        grassFootsteps = AssetFinder.instance.grassFootsteps;
        woodFootsteps = AssetFinder.instance.woodFootsteps;
        rockFootsteps = AssetFinder.instance.rockFootsteps;
        waterFootSteps = AssetFinder.instance.waterFootSteps;
    }

    // Update is called once per frame
    void Update()
    {
        if (checkForFootSteps)
        {
            // run all checks for the left foot, and play a sound if the foot touches something
            Ray ray = new Ray(leftFoot.position, -leftFoot.up);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 0.2f, footStepMask)) // Raycast hit something
            {
                if (!leftFootDown) // the foot is not already down
                {
                    leftFootDown = true; // the foot is now down, dont play a sound until it has been lifted
                    SFX footStep = GetFootStepSound(hit);
                    if (footStep != null && leftFootCanStep)
                    {
                        float volumeMultiplier = isSprinting ? 1f : 0.3f;
                        TriggerStep(footStep, volumeMultiplier);
                    }
                    leftFootCanStep = false;
                }
                else // the foot is down, dont run any code
                {
                }
            }
            else // raycast did not hit something
            {
                leftFootDown = false; // the foot is not touching anything
            }

            // do the same code, but for the rightfoot.
            ray = new Ray(rightFoot.position, -rightFoot.up);
            if (Physics.Raycast(ray, out hit, 0.2f, footStepMask)) // Raycast hit something
            {
                if (!rightFootDown) // the foot is not already down
                {
                    rightFootDown = true; // the foot is now down, dont play a sound until it has been lifted
                    SFX footStep = GetFootStepSound(hit);
                    if (footStep != null && rightFootCanStep)
                    {
                        float volumeMultiplier = isSprinting ? 1f : 0.3f;
                        TriggerStep(footStep, volumeMultiplier);
                    }
                    rightFootCanStep = false;
                }
                else // the foot is down, dont run any code
                {

                }
            }
            else // raycast did not hit something
            {
                rightFootDown = false; // the foot is not touching anything
            }
        }

        if (!leftFootCanStep)
        {
            leftFootTimer -= Time.deltaTime * speedMultipler;
            if (leftFootTimer <= 0f)
            {
                leftFootCanStep = true;
                leftFootTimer = leftFootMaxTimer;
            }
        }
        if (!rightFootCanStep)
        {
            rightFootTimer -= Time.deltaTime * speedMultipler;
            if (rightFootTimer <= 0f)
            {
                rightFootCanStep = true;
                rightFootTimer = rightFootMaxTimer;
            }
        }
    }

    public virtual void TriggerStep(SFX _footStep, float volMultiplier)
    {
        SFXAudioManager.CreateSFX(_footStep, volMultiplier);
    }

    SFX GetFootStepSound(RaycastHit hit)
    {
        if (inWater)
        {
            return waterFootSteps[Random.Range(0, waterFootSteps.Length)];
        }

        SFX[] currentFootsteps = GetFootStepArray(hit);

        if (currentFootsteps == null)
        {
            currentFootsteps = dirtFootsteps;
        }
        else if (currentFootsteps.Length == 0)
        {
            currentFootsteps = dirtFootsteps;
        }

        return currentFootsteps[Random.Range(0, currentFootsteps.Length)];
    }

    SFX[] GetFootStepArray(RaycastHit hit)
    {
        SurfaceTypes temp = GetSurfaceType(hit);
        switch (temp)
        {
            case SurfaceTypes.Dirt:
                return dirtFootsteps;
            case SurfaceTypes.Sand:
                return sandFootsteps;
            case SurfaceTypes.Rock:
                return rockFootsteps;
            case SurfaceTypes.Grass:
                return grassFootsteps;
            case SurfaceTypes.Wood:
                return woodFootsteps;
            default:
                return dirtFootsteps;
        }
    }

    SurfaceTypes GetSurfaceType(RaycastHit hit)
    {
        if (hit.collider.gameObject.TryGetComponent(out sFootStepSurface scr))
        {
            return scr.mySurfaceType;
        }
        else
        {
            return SurfaceTypes.Dirt;
        }
    }


    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(leftFoot.position, leftFoot.position - (leftFoot.up * 0.2f));
        Gizmos.DrawLine(rightFoot.position, rightFoot.position - (rightFoot.up * 0.2f));
    }
}
