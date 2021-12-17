using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sGatherNodeObject : MonoBehaviour, IInteractableObject
{

    public sGatherNode parentNode;

    public ResourceData resource;
    bool harvested = false;

    public float myTargetYPos;

    SFX grabSFX;

    private void Start()
    {
        grabSFX = AssetFinder.instance.grabSeedSFX;
    }

    private void Update()
    {
        transform.position = new Vector3(transform.position.x, myTargetYPos, transform.position.z);
    }

    public bool AlternateInteract()
    {
        return false;
    }

    public bool CanAlternateInteract()
    {
        return false;
    }

    public bool CanInteract()
    {
        if (!harvested)
        {
            return true;
        }
        return false;
    }

    public bool Interact()
    {
        if (!harvested) 
        {
            GameManager.instance.playerMovement.PlayPickUpAnim();
            WildernessManager.instance.wildernessInventory.AddItems(resource);
            sMainUI.instance.resourceNotification.SpawnNotification(resource);
            parentNode.OnNodeHarvested(this);
            harvested = true;

            SFXAudioManager.CreateSFX(grabSFX);
        }
        return true;
    }

    public string ReturnAlternateInteractActionWord()
    {
        return "";
    }

    public string ReturnInteractActionWord()
    {
        return "To gather " + resource.resource.resourceName;
    }

}
