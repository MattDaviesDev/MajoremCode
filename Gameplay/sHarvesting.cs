using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class sHarvesting : MonoBehaviour
{

    public enum ToolType
    {
        Pickaxe = 0, Axe = 1
    }

    bool unsheath = false;
    public bool  allowUnsheath = true;
    const float unsheathTimer = 0.2f;
    float tUnsheath = unsheathTimer;

    [SerializeField] float animSpeedIncrease = 0.1f;

    [SerializeField] GameObject currentToolObject;
    [SerializeField] GameObject axe;
    [SerializeField] Transform axeSheathPos;
    Renderer axeRend;
    [SerializeField] int bestAxeLevel;
    [SerializeField] GameObject pickAxe;
    [SerializeField] Transform pickAxeSheathPos;
    Renderer pickaxeRend;
    [SerializeField] int bestPickAxeLevel;

    [SerializeField] ToolType currentTool = ToolType.Axe;

    [SerializeField] List<Resource> axes = new List<Resource>();
    [SerializeField] List<Texture2D> axeTextures = new List<Texture2D>();
    [SerializeField] List<Resource> pickAxes = new List<Resource>();
    [SerializeField] List<Texture2D> pickAxeTextures = new List<Texture2D>();

    [SerializeField] LayerMask harvestMask;


    [SerializeField, Range(1f, 20f)] float harvestRange;

    [SerializeField, Range(0f, 1f)] float onHitScreenShakeIntensity;
    [SerializeField, Range(0f, 2f)] float onCritScreenShakeIntensity;

    [SerializeField] private float dotThreshold;

    IHarvestable currentNode;
    GameObject currentObject;

    Animator anim;

    string _axeSpeed = "AxeSwingMultiplier";
    string _pickAxeSpeed = "PickAxeSwingMultiplier";
    string _isHarvesting = "isHarvesting";
    string _toolType = "ToolType";
    string _sheath = "Sheath";
    string _unsheath = "Unsheath";
    string _noMotion = "NoMotion";
    bool harvesting;

    Coroutine changingWeight;

    bool showHarvestBox = true;


    [Header("OnHit")]
    [SerializeField] Vector3 minVelocityDirection;
    [SerializeField] Vector3 maxVelocityDirection;
    [SerializeField] float minLifetime;
    [SerializeField] float maxLifetime;
    [SerializeField] int minBurstSize;
    [SerializeField] int maxBurstSize;
    [SerializeField] float critMultiplier;
    GameObject onHitPrefab;
    private CinemachineImpulseSource impulseSource;

    List<sOnHitDebris> onHitVfxPool = new List<sOnHitDebris>();

    public Transform testCube;

    const float animHarvestWaitTime = 0.3f;
    float animHarvestT = animHarvestWaitTime;

    SFX swooshSFX;

    private void Awake()
    {
        axeRend = axe.GetComponent<Renderer>();
        pickaxeRend = pickAxe.GetComponent<Renderer>();
        anim = GetComponent<Animator>();
        currentTool = ToolType.Axe;
        currentToolObject = axe;

        SetToolType();
    }

    // Start is called before the first frame update
    void Start()
    {
        if (onCritScreenShakeIntensity < onHitScreenShakeIntensity)
        {
            onCritScreenShakeIntensity = onHitScreenShakeIntensity;
        }

        swooshSFX = AssetFinder.instance.toolSwooshSFX;

        onHitPrefab = AssetFinder.instance.onHitDebrisVFX;
        impulseSource = GetComponent<CinemachineImpulseSource>();

        
        
    }

    private void OnEnable()
    {
        sInputManager.instance.harvestAction.performed += Harvest;
        sInputManager.instance.harvestAction.canceled += Harvest;
    }

    private void OnDisable()
    {
        sInputManager.instance.harvestAction.performed -= Harvest;
        sInputManager.instance.harvestAction.canceled -= Harvest;
    }

    // Update is called once per frame
    void Update()
    {
        harvesting = !anim.GetCurrentAnimatorStateInfo(1).IsName(_noMotion);

        if (harvesting && !GameManager.instance.playerMovement.swimming)
        {
            anim.SetLayerWeight(1, Mathf.Lerp(anim.GetLayerWeight(1), 1f, 0.1f));
        }
        else
        {
            if (animHarvestT <= 0f)
            {
                anim.SetLayerWeight(1, Mathf.Lerp(anim.GetLayerWeight(1), 0f, 0.1f));
            }
            else
            {
                animHarvestT -= Time.deltaTime;
            }
        }

        Collider[] col = Physics.OverlapSphere(transform.position, harvestRange, harvestMask);

        if (col.Length > 0)
        {
            bool interactableObjectInArray = false;
            int closestObject = 0;
            float closestDotToOne = 100f;
            float closestDist = 10000f;
            Vector3 playerPos = GameManager.instance.player.position;
            for (int i = 0; i < col.Length; i++)
            {
                if (col[i].transform.parent.TryGetComponent(out IHarvestable interactableObject))
                {
                    //check if infront of camera forward axis
                    bool inFront = false;
                    Vector3 objectVector = col[i].transform.position - Camera.main.transform.position;
                    objectVector.Normalize();

                    float dot = Vector3.Dot(objectVector, Camera.main.transform.forward);
                    if (dot > dotThreshold)
                    {
                        inFront = true;
                    }

                    if (1 - dot < closestDotToOne && 1 - dot > 0)
                    {
                        float dist = Vector3.Distance(playerPos, col[i].transform.position);
                        if (dist < closestDist)
                        {
                            closestDotToOne = 1 - dot;
                            closestDist = dist;
                            closestObject = i;
                        }
                    }

                    if (inFront)
                    {
                        interactableObjectInArray = true;
                    }
                }
            }
            if (interactableObjectInArray)
            {
                currentNode = col[closestObject].transform.parent.GetComponent<IHarvestable>();
                currentObject = col[closestObject].gameObject;
                if (anim.GetBool(_isHarvesting) && !currentNode.GetDepleated())
                {
                    SetToolType();
                }
            }
            else
            {
                currentNode = null;
                currentObject = null;
            }
        }
        else
        {
            currentNode = null;
            currentObject = null;
        }

        if (currentNode != null)
        {
            if (sMainUI.instance.resourceHitPoints.selectedNode != currentNode && !sMainUI.instance.resourceHitPoints.shown)
            {
                sMainUI.instance.resourceHitPoints.ShowHealthBar(currentNode);
            }
            else if (sMainUI.instance.resourceHitPoints.selectedNode != currentNode)
            {
                sMainUI.instance.resourceHitPoints.HideHealthBar();
            }
        }
        else if (sMainUI.instance.resourceHitPoints.shown)
        {
            sMainUI.instance.resourceHitPoints.HideHealthBar();
            if (sMainUI.instance.critSpot.shown)
            {
                sMainUI.instance.critSpot.HideCrit();
            }
        }

        // Unsheath timer
        if (allowUnsheath)
        {
            bool notUnsheath = !anim.GetCurrentAnimatorStateInfo(1).IsName(_unsheath);
            bool notSheath = !anim.GetCurrentAnimatorStateInfo(1).IsName(_sheath);
            bool notNoMotion = !anim.GetCurrentAnimatorStateInfo(1).IsName(_noMotion);

            if (unsheath && notNoMotion && notSheath && notUnsheath)
            {
                if (tUnsheath > 0f)
                {
                    tUnsheath -= Time.deltaTime;
                }
                else
                {
                    unsheath = false;
                    allowUnsheath = false;
                    anim.SetTrigger(_unsheath);
                    print("UnSheath");
                }
            }
        }
        else
        {
            if (anim.GetCurrentAnimatorStateInfo(1).IsName(_noMotion)) 
            {
                allowUnsheath = true;   
            }
        }
    }

    void Harvest(InputAction.CallbackContext input)
    {
        if (input.performed)
        {
            animHarvestT = animHarvestWaitTime;

            //harvesting = true;

            anim.SetBool(_isHarvesting, true);
            anim.SetBool(_sheath, true);
            tUnsheath = unsheathTimer;
            unsheath = false;
        }
        else
        {
            //harvesting = false; 
            anim.SetBool(_sheath, false);
            anim.SetBool(_isHarvesting, false);
            unsheath = true;
        }
    }

    public void SetToolFalse()
    {
        axe.SetActive(false);
        pickAxe.SetActive(false);
    }

    public void SetToolTrue()
    {
        switch (currentTool)
        {
            case ToolType.Pickaxe:
                pickAxe.SetActive(true);
                break;
            case ToolType.Axe:
                axe.SetActive(true);
                break;
            default:
                break;
        }
    }

    public void PlaySwooshSFX()
    {
        SFXAudioManager.CreateSFX(swooshSFX);
    }

    public void SpawnCrit()
    {
        if (currentNode != null)
        {
            if (!currentNode.GetDepleated())
            {
                currentNode.SpawnCritSpot();
            }
        }
    }

    public void SetToolType()
    {
        if (currentNode != null)
        {
            if (currentNode.GetTool() != currentTool )
            {
                anim.SetTrigger(_unsheath);
            }
            SetAvailableTool(currentNode.GetTool());
        }
        else
        {
            SetAvailableTool(ToolType.Axe);
        }
    }

    public void SetBestTool()
    {
        if (axeRend == null)
        {
            axeRend = axe.GetComponent<Renderer>();
        }
        if (pickaxeRend == null)
        {
            pickaxeRend = pickAxe.GetComponent<Renderer>();
        }
        if (anim == null)
        {
            anim = GetComponent<Animator>();
        }
        int best = 0;
        for (int i = 0; i < axes.Count; i++)
        {
            // if they have more than 0
            if (VillageManager.instance.villageOneInventory.ReturnQuantity(axes[i].ID) > 0)
            {
                // assign as current tool
                best = i;
            }
        }
        if (axeRend != null)
        {
            axeRend.sharedMaterial.SetTexture("_Diffuse", axeTextures[best]);
        }
        bestAxeLevel = best;
        if (anim != null)
        {
            anim.SetFloat(_axeSpeed, 1 + (animSpeedIncrease * best));
        }

        best = 0;

        for (int i = 0; i < pickAxes.Count; i++)
        {
            // if they have more than 0
            if (VillageManager.instance.villageOneInventory.ReturnQuantity(pickAxes[i].ID) > 0)
            {
                // assign as current tool
                best = i;
            }
        }
        bestPickAxeLevel = best;
        if (pickaxeRend != null)
        {
            pickaxeRend.sharedMaterial.SetTexture("_Diffuse", pickAxeTextures[best]);
        }
        if (anim != null)
        {
            anim.SetFloat(_pickAxeSpeed, 1 + (animSpeedIncrease * best));
        }
    }

    public void SetAvailableTool(ToolType toolType)
    {
        currentTool = toolType;
        switch (toolType)
        {
            case ToolType.Pickaxe:
                currentToolObject = pickAxe;
                break;
            case ToolType.Axe:
                currentToolObject = axe;
                break;
        }
        anim.SetInteger(_toolType, (int)toolType);
    }

    public void HitNode()
    {
        if (currentNode != null)
        {
            if (!currentNode.GetDepleated())
            {
                Vector3 centre = GameManager.instance.player.position - (GameManager.instance.player.forward * (harvestRange * 0.5f));
                Vector3 halfExtents = new Vector3(0.5f, 5f, harvestRange * 0.5f);
                Vector3 forwardDir = GameManager.instance.player.forward;
                Collider[] col = Physics.OverlapBox(centre, halfExtents, Quaternion.LookRotation(forwardDir), harvestMask);
                if (col.Length > 0)
                {
                    for (int i = 0; i < col.Length; i++)
                    {
                        if (col[i].GetComponentInParent<IHarvestable>() == currentNode)
                        {
                            bool isCrit = sMainUI.instance.critSpot.IsCrit();
                            currentNode.OnHit(isCrit);

                            SpawnVFX(isCrit);

                            float traumaVal = isCrit ? onCritScreenShakeIntensity : onHitScreenShakeIntensity;
                            // GameManager.instance.playerCameraControl.cameraShaker.AddTrauma(traumaVal); //TODO: Rework camera shake to use cinemachine
                            impulseSource.GenerateImpulse(traumaVal);
                            break;
                        }
                    }

                }
            }
        }



    }

    void SpawnVFX(bool isCrit)
    {
        Ray ray = new Ray(GameManager.instance.cameraTransform.position, GameManager.instance.cameraTransform.forward);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, harvestMask))
        {
            IHarvestable node = hit.collider.gameObject.GetComponentInParent<IHarvestable>();
            if (node != null && node == currentNode)
            {
                sOnHitDebris temp = GetNewVFX();
                float tempMultiplier = isCrit ? critMultiplier : 1f;
                temp.maxLifetime = maxLifetime * tempMultiplier;
                temp.minLifetime = minLifetime * tempMultiplier;
                temp.minVelDir = minVelocityDirection * tempMultiplier;
                temp.maxVelDir = maxVelocityDirection * tempMultiplier;
                temp.burstSize = Random.Range(minBurstSize, maxBurstSize + 1);
                temp.mesh = node.GetMesh();
                temp.size = node.GetVFXSize();
                temp.texture = node.GetTexture();
                temp.Init(hit.point, hit.normal);
            }
        }
    }

    sOnHitDebris GetNewVFX()
    {
        for (int i = 0; i < onHitVfxPool.Count; i++)
        {
            if (!onHitVfxPool[i].active)
            {
                return onHitVfxPool[i];
            }
        }
        GameObject temp = Instantiate(onHitPrefab, sSceneController.instance.GetCurrentSceneParent());
        onHitVfxPool.Add(temp.GetComponent<sOnHitDebris>());
        return onHitVfxPool[onHitVfxPool.Count - 1];

    }

}
