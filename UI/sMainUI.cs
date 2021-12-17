using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class sMainUI : MonoBehaviour
{

    static sMainUI _instance;
    public static sMainUI instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<sMainUI>();
            }
            return _instance;
        }
    }

    
    public EventSystem eventSystem;
    const string gamepadControlSchemeName = "GamePad";

    [Header("Main UI things")]
    public CanvasGroup mainUICanvas;
    public sMiniMap miniMap;
    public sDateDisplay dateDisplay;
    public sInteractHint interactHint;
    public sVillageBoardInterface villageBoard;
    public sResourceGatheredNotification resourceNotification;
    public sMusicPopUp musicPopUp;
    public sInfoPopUpMessage infoPopUpMessage;
    public sQuestTracker questTracker;

    [Header("Wilderness UI Things")]
    public sCritSpot critSpot;
    public sResourceHitPoints resourceHitPoints;


    [Header("Interfaces")]
    public sBuildInterface buildInterface;
    sInventory currentInventory; //this will need to be changed when moving to the wilderness and once their is more villages it will need to update depedning on which village you are at
    public sInventoryInterface inventoryInterface;
    public sCropFarmInterface cropFarmInterface;
    public sBlacksmithInterface blacksmithInterface;
    public sCookeryInterface cookeryInterface;
    public sQuestInterface questInterface;

    private bool inventoryActive;

    private GameObject currentWindow;

    bool showCursor;

    private sInventory activeInventory;

    [Header("UI objects")]
    public CanvasGroup sleepCanvas;
    public CanvasGroup sleepInfoCanvas;
    public TextMeshProUGUI sleepInfoText;
    const float fpsUpdateTime = 0.25f;
    float fpsT = fpsUpdateTime;
    [SerializeField] TextMeshProUGUI fpsIndicator;

    public bool canPause = true;


    [Header("SFX")]
    [SerializeField] SFX openBackPackSound;
    [SerializeField] SFX closeBackPackSound;


    public bool ShowCursor
    {
        get
        {
            return showCursor;
        }
        set
        {
            if (!PlayerIsUsingGamepad())
            {
                Cursor.visible = value;
                CursorLockMode temp = sSettings.instance.displayMode == 1 ? CursorLockMode.Confined : CursorLockMode.Locked;
                Cursor.lockState = value ? CursorLockMode.None : temp;
            }
            showCursor = value;
        }
    }

    private void Awake()
    {

    }

    // Start is called before the first frame update
    void Start()
    {
        currentInventory = VillageManager.instance.villageOneInventory;

        EnteredVillage();

        ShowHidePerformance(sSettings.instance.GetShowPerformance());
    }


    private void OnEnable()
    {
        sInputManager.instance.inventoryAction.performed += OpenInventoryInterface;

        sInputManager.instance.closeInventory.performed += CloseInventoryInterface;
        sInputManager.instance.closeWindowAction.performed += CloseCurrentWindow;

        sInputManager.instance.pauseAction.performed += PauseGame;
    }

    private void OnDisable()
    {
        sInputManager.instance.inventoryAction.performed -= OpenInventoryInterface;

        sInputManager.instance.closeInventory.performed -= CloseInventoryInterface;
        sInputManager.instance.closeWindowAction.performed -= CloseCurrentWindow;

        sInputManager.instance.pauseAction.performed -= PauseGame;
    }

    // Update is called once per frame
    void Update()
    {
        if (fpsT <= 0f)
        {
            fpsT += fpsUpdateTime;
            fpsIndicator.text = (1f / Time.unscaledDeltaTime).ToString("F0") + " FPS";
        }
        fpsT -= Time.unscaledDeltaTime;

    }

    public void ShowHidePerformance(bool active)
    {
        fpsIndicator.gameObject.SetActive(active);
    }

    public void CloseCurrentWindow(InputAction.CallbackContext context)
    {
        CloseCurrentWindow();
    }

    public void CloseCurrentWindow()
    {
        if (currentWindow && currentWindow.activeSelf)
        {
            if (currentWindow == inventoryInterface.interfaceObject)
            {
                SFXAudioManager.CreateSFX(closeBackPackSound);
            }
            currentWindow.SetActive(false);
            GameManager.instance.PlayerHasControl = true;
            ShowCursor = false;
            sInputManager.instance.MoveToGameMap();
            currentWindow = null;
        }
    }
    public void OpenBuildInterface()
    {
        GameManager.instance.PlayerHasControl = false;
        sInputManager.instance.MoveToUIMap();
        ShowCursor = true;
        currentWindow = buildInterface.buildInterfaceObject;
        buildInterface.buildInterfaceObject.SetActive(true);
    }

    public void OpenQuestInterface()
    {
        GameManager.instance.PlayerHasControl = false;
        sInputManager.instance.MoveToUIMap();
        ShowCursor = true;
        currentWindow = questInterface.interfaceObjects;
        questInterface.CreateQuestLogs();
        currentWindow.SetActive(true);
    }

    //public void ToggleInventory(InputAction.CallbackContext input)
    //{
    //    inventoryActive = !inventoryActive;
    //    if (inventoryActive) { OpenInventoryInterface(); }
    //    else { CloseInventoryInterface(); }
    //}
    public void OpenInventoryInterface(InputAction.CallbackContext input)
    {
        inventoryActive = true;
        inventoryInterface.interfaceObject.SetActive(true);
        GameManager.instance.PlayerHasControl = false;
        ShowCursor = true;
        inventoryInterface.InitiateInterface(activeInventory); //this will need to be updated to pass in the current active inventory
        currentWindow = inventoryInterface.interfaceObject;
        sInputManager.instance.MoveToUIMap();
        SFXAudioManager.CreateSFX(openBackPackSound);
    }

    public void CloseInventoryInterface(InputAction.CallbackContext input)
    {
        if (!inventoryActive) { return; }
        inventoryActive = false;
        inventoryInterface.interfaceObject.SetActive(false);
        GameManager.instance.PlayerHasControl = true;
        ShowCursor = false;
        currentWindow = null;
        sInputManager.instance.MoveToGameMap();
        SFXAudioManager.CreateSFX(closeBackPackSound);
    }

    public void OnInventoryUpdated(sInventory inventory, ResourceData newItem)
    {
        if (currentInventory == inventory)
        {
            inventoryInterface.UpdateInventory(newItem);
        }
    }

    public void OpenCropFarmUI(sCropFarm _currentCropFarm)
    {
        GameManager.instance.PlayerHasControl = false;
        currentWindow = cropFarmInterface.cropFarmInterfaceObject;
        ShowCursor = true;
        cropFarmInterface.PopulateSeedList();
        cropFarmInterface.InteractedWith();
        cropFarmInterface.cropFarmInterfaceObject.SetActive(true);
        cropFarmInterface.currentCropFarm = _currentCropFarm;
        sInputManager.instance.MoveToUIMap();
    }

    public void OpenBlacksmithUI(sBlacksmithBuilding blacksmithToOpen)
    {
        GameManager.instance.PlayerHasControl = false;
        ShowCursor = true;
        blacksmithInterface.Initialise(blacksmithToOpen);
        blacksmithInterface.thisContent.SetActive(true);
        currentWindow = blacksmithInterface.thisContent;
        blacksmithInterface.SetTab(0);
        sInputManager.instance.MoveToUIMap();
    }

    public void OpenCookeryUI(sCookery cookeryToOpen)
    {
        GameManager.instance.PlayerHasControl = false;
        ShowCursor = true;
        cookeryInterface.Initialise(cookeryToOpen);
        cookeryInterface.thisContent.SetActive(true);
        currentWindow = cookeryInterface.thisContent;
        cookeryInterface.SetTab(0);
        sInputManager.instance.MoveToUIMap();
    }

    public void EnteredVillage()
    {
        activeInventory = VillageManager.instance.villageOneInventory;
    }
    public void EnteredWilderness()
    {
        activeInventory = WildernessManager.instance.wildernessInventory;
    }

    public void OpenVIllageBoardUI()
    {
        GameManager.instance.PlayerHasControl = false;
        currentWindow = villageBoard.interfaceObject;
        ShowCursor = true;
        villageBoard.Populate();
        villageBoard.interfaceObject.SetActive(true);
    }

    public void PauseGame(InputAction.CallbackContext input)
    {
        if (Time.timeScale == 1f && canPause)
        {
            Time.timeScale = 0f;
            ShowCursor = true;
            sPausedUI.instance.ShowPauseCanvas();
            HideMainUI();
            sInputManager.instance.MoveToUIMap();
        }
    }

    public void ResumeGame()
    {
        sInputManager.instance.MoveToGameMap();
        ShowCursor = false;
        ShowMainUI();
    }

    public void LeavingDecorationEditor()
    {
        ShowCursor = false;
        ShowMainUI();
    }

    public void SetCurrentSelectedObjectForGamepad(GameObject newObject)
    {
        eventSystem.SetSelectedGameObject(newObject);
    }

    public bool PlayerIsUsingGamepad()
    {
        return sInputManager.instance.pInput.currentControlScheme == gamepadControlSchemeName;
    }

    public void HideMainUI()
    {
        mainUICanvas.alpha = 0f;
        mainUICanvas.blocksRaycasts = false;
        mainUICanvas.interactable = false;
    }

    public void ShowMainUI()
    {
        mainUICanvas.alpha = 1f;
        mainUICanvas.blocksRaycasts = true;
        mainUICanvas.interactable = true;
    }

}
