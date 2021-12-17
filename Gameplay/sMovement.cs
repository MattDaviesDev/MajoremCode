using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.VFX;

public class sMovement : MonoBehaviour
{
    Transform mainCamTransform;

    [SerializeField] Terrain currentTerrain;
    [SerializeField] LayerMask terrainMask;

    sFootStep footStepHandler;

    [Header("Input")]
    [SerializeField] Vector2 inputDir;
    [SerializeField] Transform playerCam;
    [SerializeField, Range(0f, 10f)] float rotationAmount;
    [HideInInspector] public CharacterController cc;

    [Header("Movement")]
    [SerializeField] float accelerationMultiplier;
    [SerializeField] float acceleration;
    public float currentMovementMagnitude;
    
    [Space(10)]
    [SerializeField] float gravity;

    [Space(10)]
    [SerializeField] public Vector3 velocity;
    [SerializeField] public float walkMoveSpeed;
    [SerializeField] float runMoveSpeed;
    [SerializeField] float swimmingMoveSpeed;
    [SerializeField] bool runIsToggle;
    [SerializeField, Range(0f, 1f)] float moveSpeedLerpSpeed;
    [SerializeField] float moveSpeed;
    [HideInInspector] public float desiredMoveSpeed;
    int moveSpeedMultiplier = 1;
    public int MoveSpeedMultiplier
    {
        get
        {
            return moveSpeedMultiplier;
        }
        set
        {
            if (value > 5)
            {
                value = 0;
            }
            moveSpeedMultiplier = value;
        }
    }
    
    [Space(10)]
    [SerializeField] float normalJumpSpeed;
    [SerializeField] float runningJumpSpeed;
    public int coyoteFrames = 60;
    public int currentCoyoteFrames = 0;

    [Space(10)]
    [SerializeField] float groundedCheckDistance = 1.5f;
    [SerializeField] LayerMask groundedCheckMask;
    [SerializeField] float maxSlopeAngle;
    [SerializeField] float slopeOverrideSpeed = 0.5f;
    Vector3 tempMoveDir;
    float tSlope = 0f;

    [Space(10)]
    [SerializeField] VisualEffect dustKickUp;
    [SerializeField] float dustKickUpEmissionRate;

    [SerializeField] private sCameraControl cameraControl;

    [SerializeField] Animator anim;

    public bool isRunning;

    public bool canMove = true;

    const float playRandomIdleAnimTimer = 45f;
    public float randomIdleT = playRandomIdleAnimTimer;

    const string _randomIdlePlay = "RandomIdlePlay";
    const string _randomIdleIndex = "RandomIdleIndex";
    const string _isHarvesting = "isHarvesting";
    const string _isWalking = "isWalking";
    const string _isRunning = "isRunning";
    const string _jump = "Jump";
    const string _randomJumpIndex = "RandomJumpIndex";
    const string _isGrounded = "isGrounded";
    const string _isSwimming = "isSwimming";
    const string _pickUp = "PickUp";
    const string _emissionRate = "EmissionRate";

    int swimmingLayer = 2;
    float swimmingLayerWeight = 0f;
    public bool swimming;
    public bool stoodInWater 
    {
        set
        {
            footStepHandler.inWater = value;
        }   
    }
    

    bool pickingUp;

    const float swapDirTimer = 0.1f;
    float swapDirT = swapDirTimer;

    SFX[] swimmingSFX;
    SFX[] jumpingSFX;

    const float harvestTimer = 0.3f;
    float tHarvesting = 0f; 

    // Start is called before the first frame update
    void Start()
    {
        
        cc = GetComponent<CharacterController>();
        if (!cc)
        {
            Debug.LogError("No character Controller attached to " + gameObject.name);
        }

        desiredMoveSpeed = walkMoveSpeed;

        anim = GetComponent<Animator>();

        GameManager.instance.playerMovement = this;

        mainCamTransform = Camera.main.transform;

        footStepHandler = GetComponent<sFootStep>();

        swimmingSFX = AssetFinder.instance.swimmingSFX;
        jumpingSFX = AssetFinder.instance.jumpingSFX;
        
        anim.SetBool(_isWalking, true);
    }

    private void OnEnable()
    {
        sInputManager.instance.movementAction.performed += MovementInput;
        sInputManager.instance.movementAction.canceled += MovementInput;

        sInputManager.instance.jumpAction.performed += Jump;

        sInputManager.instance.runAction.performed += Run;
        sInputManager.instance.runAction.canceled += Run;

        sInputManager.instance.testAction.performed += DevelopmentalMovementSpeedIncrease;
    }

    private void OnDisable()
    {
        sInputManager.instance.movementAction.performed -= MovementInput;
        sInputManager.instance.movementAction.canceled -= MovementInput;

        sInputManager.instance.jumpAction.performed -= Jump;

        sInputManager.instance.runAction.performed -= Run;
        sInputManager.instance.runAction.canceled -= Run;
#if UNITY_EDITOR
        sInputManager.instance.testAction.performed -= DevelopmentalMovementSpeedIncrease;
#endif
    }

    // Update is called once per frame
    void Update()
    {

        #region Anim Picking Up
        if (pickingUp)
        {
            anim.SetLayerWeight(4, Mathf.Lerp(anim.GetLayerWeight(4), 1f, 0.1f));
        }
        else
        {
            anim.SetLayerWeight(4, Mathf.Lerp(anim.GetLayerWeight(4), 0f, 0.1f));
        }
        #endregion

        #region Anim Swimming Code

        swimmingLayerWeight = swimming ? Mathf.Lerp(swimmingLayerWeight, 1f, 0.1f) : Mathf.Lerp(swimmingLayerWeight, 0f, 0.1f);

        anim.SetLayerWeight(swimmingLayer, swimmingLayerWeight);

        

        #endregion

        isRunning = desiredMoveSpeed == runMoveSpeed;

        footStepHandler.isSprinting = isRunning;

        if (moveSpeed != desiredMoveSpeed)
        {
            moveSpeed = Mathf.Lerp(moveSpeed, desiredMoveSpeed, moveSpeedLerpSpeed);
        }

        if (swimming)
        {
            moveSpeed = swimmingMoveSpeed;
        }

        Acceleration();
        if (canMove)
        {
            velocity = new Vector3(0f, velocity.y, 0f);

            velocity += new Vector3(playerCam.forward.x * inputDir.y, 0f, playerCam.forward.z * inputDir.y); // adding forward camera direction 
            velocity += new Vector3(playerCam.right.x * inputDir.x, 0f, playerCam.right.z * inputDir.x); // adding right camera direction 
        }

        Vector3 moveDir;

        currentTerrain = GetCurrentTerrain();

        Vector3 tangent;
        Vector3 down = Vector3.zero;
        float currentSlope = 0f;

        if (IsGrounded())
        {
            Ray ray = new Ray(transform.position, Vector3.down);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, groundedCheckMask))
            {
                currentSlope = Vector3.Angle(hit.normal, Vector3.up);
                if (currentSlope > maxSlopeAngle)
                {
                    tangent = Vector3.Cross(hit.normal, Vector3.up);
                    down = Vector3.Cross(hit.normal, tangent);
                }
            }
        }
        
        if (IsGrounded() && currentSlope > maxSlopeAngle)
        {
            tSlope += Time.deltaTime / slopeOverrideSpeed;
            tSlope = Mathf.Clamp01(tSlope);
            moveDir = Vector3.Lerp(tempMoveDir, down * 10, tSlope);
        }
        else
        {   
            float temp = velocity.y;
            moveDir = new Vector3(velocity.x, 0, velocity.z).normalized; // assigning moveDir
            moveDir *= moveSpeed * acceleration;
            moveDir.y = temp;
            tempMoveDir = moveDir;
            tSlope = 0f;
        }

        if (!IsGrounded() || velocity.y > 0f)
        {
            velocity.y -= gravity * Time.deltaTime;
        }

        cc.Move(moveDir * MoveSpeedMultiplier * Time.deltaTime);

        currentMovementMagnitude = new Vector2(moveDir.x, moveDir.z).magnitude;

        if (!swimming)
        {
            Vector3 mainCamForward = sBillboard.GetForwardNoYAxis(mainCamTransform, true);
            if (Time.timeScale != 0f)
            {
                if (anim.GetBool(_isHarvesting))
                {
                    tHarvesting = harvestTimer;
                }
                if (tHarvesting > 0f)
                {
                    tHarvesting -= Time.deltaTime;
                    transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(-mainCamForward), rotationAmount * Time.deltaTime);
                }
            }
        }
        else
        {
        }

        if (currentMovementMagnitude > 0.2f)
        {
            swapDirT = swapDirTimer;

            randomIdleT = playRandomIdleAnimTimer;

            anim.SetBool(_isWalking, desiredMoveSpeed == walkMoveSpeed);

            anim.SetBool(_isRunning, isRunning);

            anim.SetBool(_isSwimming, true);

            if (tHarvesting <= 0f)
            {
                if (Time.timeScale != 0f)
                {
                    transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(new Vector3(-moveDir.x, 0f, -moveDir.z).normalized), rotationAmount * Time.deltaTime);
                }
            }

            if (IsGrounded())
            {
                if (!swimming)
                {
                    footStepHandler.checkForFootSteps = true;
                }
                else
                {
                    footStepHandler.checkForFootSteps = false;
                }

                if (dustKickUp != null)
                {
                    if (isRunning && !swimming)
                    {
                        dustKickUp.SetFloat(_emissionRate, dustKickUpEmissionRate);
                    }
                    else
                    {
                        dustKickUp.SetFloat(_emissionRate, 0);

                    }
                }
            }
            else
            {
                footStepHandler.checkForFootSteps = false;
                if (dustKickUp != null)
                {
                    dustKickUp.SetFloat(_emissionRate, 0);
                }
            }

        }
        else
        {
            footStepHandler.checkForFootSteps = false;
            if (dustKickUp != null)
            {
                dustKickUp.SetFloat(_emissionRate, 0);
            }

            anim.SetBool(_isSwimming, false);

            if (randomIdleT <= 0f)
            {
                anim.SetInteger(_randomIdleIndex, Random.Range(0, 4));
                anim.SetTrigger(_randomIdlePlay);
                randomIdleT += playRandomIdleAnimTimer;
            }
            else
            {
                randomIdleT -= Time.deltaTime;
            }

            if (swapDirT <= 0f)
            {
                anim.SetBool(_isWalking, false);
                anim.SetBool(_isRunning, false);
                desiredMoveSpeed = walkMoveSpeed;
            }
            else
            {
                swapDirT -= Time.deltaTime;
            }
        }

        if (currentCoyoteFrames <= coyoteFrames && !IsGrounded())
        {
            currentCoyoteFrames++;
        }

        if (IsGrounded() && currentCoyoteFrames != 0)
        {
            currentCoyoteFrames = 0;
        }

        anim.SetBool(_isGrounded, IsGrounded());
    }

    Terrain GetCurrentTerrain()
    {
        Ray ray = new Ray(transform.position, Vector3.down);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, terrainMask))
        {
            if (hit.collider.TryGetComponent(out Terrain terrain))
            {
                return terrain;
            }
        }
        return null;
    }

    void MovementInput(InputAction.CallbackContext input)
    {
        inputDir = input.ReadValue<Vector2>();
    }

    void Jump(InputAction.CallbackContext input)
    {
        if (swimmingLayerWeight > 0.5f)
        {
            return;
        }

        if (IsGrounded() || currentCoyoteFrames <= coyoteFrames)
        {
            anim.SetInteger(_randomJumpIndex, Random.Range(1, 2));
            anim.SetTrigger(_jump);
            float tempJumpSpeed = isRunning ? runningJumpSpeed : normalJumpSpeed;
            velocity.y = tempJumpSpeed;
            // cameraControl.OnPlayerJump();
            currentCoyoteFrames = coyoteFrames + 1;

            SFXAudioManager.CreateSFX(jumpingSFX[Random.Range(0, jumpingSFX.Length)]);
        }
    }

    void Run(InputAction.CallbackContext input)
    {
        if (input.performed)
        {
            if (sSettings.instance.sprintType == 0)
            {
                desiredMoveSpeed = desiredMoveSpeed == walkMoveSpeed ? runMoveSpeed : walkMoveSpeed;
            }
            else
            {
                desiredMoveSpeed = runMoveSpeed;
            }
        }

        if (input.canceled)
        {
            if (sSettings.instance.sprintType == 1)
            {
                desiredMoveSpeed = walkMoveSpeed;
            }
        }
    }

    void Acceleration()
    {
        if (inputDir != Vector2.zero)
        {
            if (acceleration < 1)
            {
                acceleration += Time.deltaTime * accelerationMultiplier;
            }
            else
            {
                acceleration = 1;
            }
        }
        else
        {
            if (acceleration > 0)
            {
                acceleration -= Time.deltaTime * accelerationMultiplier;
            }
            else
            {
                acceleration = 0;
            }
        }
    }

    bool IsGrounded()
    {
        Collider[] col = Physics.OverlapBox(transform.position + (Vector3.up * 0.12f), Vector3.one * 0.2f, Quaternion.identity, groundedCheckMask);
        if (col.Length > 0)
        {
            if (velocity.y < -2.2f)
            {
                velocity.y = -2f;
            }
            // cameraControl.OnPlayerGrounded();
            return true;
        }
        // cameraControl.OnPlayerNotGrounded();
        return false;
    }

    void DevelopmentalMovementSpeedIncrease(InputAction.CallbackContext input)
    {
        if (sSettings.instance.cheatsEnabled)
        {
            Debug.LogError("REMOVE THIS FUNCTION BEFORE SHIPPING.");
            MoveSpeedMultiplier++;
        }
    }

    public void PlayPickUpAnim()
    {
        pickingUp = true;
        anim.SetTrigger(_pickUp);
    }

    public void StopPickUpAnim()
    {
        pickingUp = false;
    }

    public void PlaySwimSound()
    {
        if (swimming)
        {
            SFXAudioManager.CreateSFX(swimmingSFX[Random.Range(0, swimmingSFX.Length)]);
        }
    }
}
