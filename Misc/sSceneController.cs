using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.AI;
using TMPro;

public enum SceneType
{
    Village, Wilderness
}


public class sSceneController : MonoBehaviour
{

    public static sSceneController instance;

    [SerializeField] public Transform villageParent;
    [SerializeField] public Transform wildernessParent;

    [Header("Village lists")]
    public List<Terrain> villageTerrains = new List<Terrain>();
    public List<Renderer> villageRenderers = new List<Renderer>();
    public List<Collider> villageColliders = new List<Collider>();
    public List<Light> villageLights = new List<Light>();
    //public List<NavMeshSurface> villageNavMeshSurface = new List<NavMeshSurface>();
    //public List<NavMeshLink> villageNavMeshLinks = new List<NavMeshLink>();
    //public List<NavMeshAgent> villageNavMeshAgents = new List<NavMeshAgent>();
    //public List<NavMeshModifierVolume> villageNavMeshVolume = new List<NavMeshModifierVolume>();
    //public List<NavMeshObstacle> villageNavMeshObstacles = new List<NavMeshObstacle>();
    public List<sPlot> villagePlots = new List<sPlot>();
    public List<AudioSource> villageAudio = new List<AudioSource>();

    [Header("Wilderness lists")]
    public List<Terrain> wildernessTerrains = new List<Terrain>();
    public List<Renderer> wildernessRenderers = new List<Renderer>();
    public List<Collider> wildernessColliders = new List<Collider>();
    public List<Light> wildernessLights = new List<Light>();
    //public List<NavMeshSurface> wildernessNavMeshSurface = new List<NavMeshSurface>();
    //public List<NavMeshLink> wildernessNavMeshLinks = new List<NavMeshLink>();
    //public List<NavMeshAgent> wildernessNavMeshAgents = new List<NavMeshAgent>();
    //public List<NavMeshModifierVolume> wildernessNavMeshVolume = new List<NavMeshModifierVolume>();
    //public List<NavMeshObstacle> wildernessNavMeshObstacles = new List<NavMeshObstacle>();
    public List<AudioSource> wildernessAudio = new List<AudioSource>();

    [Space(10)]

    SceneType nextSceneType;

    public SceneType currentSceneType;
    Transform currentSpawnPoint;
    int currentVillageNumber;

    bool finishedVillageObjects = false;
    bool finishedWildernessObjects = false;

    public Vector3 hiddenPos = new Vector3(0, -4200, 0);

    Coroutine changingScene;

    sPlayerOutfitController outfitController;

    [Header("Scene swap Canvas")]
    [SerializeField] CanvasGroup blackCanvas;
    [SerializeField] RectTransform loadingImageBlackScreen;
    [SerializeField] CanvasGroup sceneSwapCanvas;
    [SerializeField] RectTransform loadingImage;
    [SerializeField] TextMeshProUGUI travellingToText;

    [Header("Village")]
    public bool passOut;
    [SerializeField] string returnToVillagePopUp;
    [SerializeField] string passOutMessage = "When you pass out in the wilderness, you lose all your items from that wilderness session.<br>Make sure you get home safely!";
    [SerializeField] Transform playerPassOutRespawnLocation;
    [SerializeField] Transform[] villageSpawnPoints;

    [Header("Wilderness")]
    [SerializeField] Transform[] wildernessSpawnPoints;


    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }
    }

    private void Start()
    {
        outfitController = GameManager.instance.player.GetComponent<sPlayerOutfitController>();
    }

    public void SceneTransition(SceneType sceneType, int villageNumber)
    {
        if (changingScene != null)
        {
            return;
        }

        currentVillageNumber = villageNumber;
        switch (sceneType)
        {
            case SceneType.Village:
                travellingToText.text = "Travelling to - Village " + villageNumber;
                currentSpawnPoint = villageSpawnPoints[villageNumber - 1];
                break;
            case SceneType.Wilderness:
                currentSpawnPoint = wildernessSpawnPoints[villageNumber - 1];
                travellingToText.text = "Travelling to - The Wilderness";
                break;
        }
        changingScene = StartCoroutine(SceneTransitionAsync(sceneType, sceneSwapCanvas, villageNumber - 1));
    }

    public void PlayerPassOut()
    {
        if (changingScene != null)
        {
            return;
        }
        passOut = true; 
        currentSpawnPoint = villageSpawnPoints[currentVillageNumber - 1];
        changingScene = StartCoroutine(SceneTransitionAsync(SceneType.Village, blackCanvas, currentVillageNumber - 1));
    }

    IEnumerator SceneTransitionAsync(SceneType sceneType, CanvasGroup changingCanvas, int villageNum)
    {

        sMainUI.instance.canPause = false;
        Coroutine fakeLoading;

        nextSceneType = sceneType;

        if (sceneType == SceneType.Village)
        {
            sMainUI.instance.EnteredVillage();
        }
        else
        {
            sMainUI.instance.EnteredWilderness();
        }

        finishedVillageObjects = false;
        finishedWildernessObjects = false;

        sMainUI.instance.HideMainUI();

        fakeLoading = StartCoroutine(ShowLoading());
        float t = 0f;
        do
        {
            t += Time.unscaledDeltaTime / 0.5f;
            changingCanvas.alpha = Mathf.Lerp(0f, 1f, t);
            yield return null;
        } while (t <= 1f);

        // change outfit
        outfitController.ChangeOutfit(sceneType);

        sMainUI.instance.miniMap.SetMapImageToSpawnLocation(villageNum, sceneType == SceneType.Village);

        PostProcessEffects.instance.SetVignetteValue(0f);

        GameManager.instance.PlayerHasControl = false;
        GameManager.instance.playerMovement.cc.enabled = false;
        //GameManager.instance.playerMovement.enabled = false;

        StartCoroutine(ChangeStateOfVillageObjects());
        StartCoroutine(ChangeStateOfWildernessObjects());


        currentSceneType = sceneType;

        villageParent.position = sceneType == SceneType.Village ? Vector3.zero : hiddenPos;
        wildernessParent.position = sceneType == SceneType.Wilderness ? Vector3.zero : hiddenPos;

        if (changingCanvas == blackCanvas)
        {
            GameManager.instance.player.position = playerPassOutRespawnLocation.position;
            GameManager.instance.player.rotation = playerPassOutRespawnLocation.rotation * Quaternion.Euler(0, 180, 0);
        }
        else
        {
            GameManager.instance.player.position = currentSpawnPoint.position;
            GameManager.instance.player.rotation = currentSpawnPoint.rotation * Quaternion.Euler(0, 180, 0);
        }

        do
        {
            yield return null;
        } while (!finishedVillageObjects && !finishedWildernessObjects);

        yield return new WaitForSecondsRealtime(1f);

        // if (sceneType == SceneType.Wilderness) //temp while camera transitions are implemented
        // {
        //     GameManager.instance.player.position = currentSpawnPoint.position;
        //     GameManager.instance.player.rotation = currentSpawnPoint.rotation;
        //     
        //     //GameManager.instance.playerMovement.enabled = true;
        //     GameManager.instance.playerMovement.cc.enabled = true;
        //     GameManager.instance.PlayerHasControl = true;
        // }

        // player passed out
        if (changingCanvas == blackCanvas)
        {
            Date newDate = new Date();
            if (newDate.Hours > 21)
            {
                newDate.Days++;
            }
            newDate.Seconds = 0;
            newDate.Minutes = 0;
            newDate.Hours = 6;

            Date _timeSkipped = new Date(newDate - GameManager.instance.dayCycle.currentDate);

            GameManager.instance.dayCycle.SkipTime(_timeSkipped);
            GameManager.instance.dayCycle.currentDate.Seconds = 0;
            GameManager.instance.dayCycle.currentDate.Minutes = 0;
            GameManager.instance.dayCycle.currentDate.Hours = 6;
        }
        else
        {
            if (sceneType == SceneType.Village)
            {
                GameManager.instance.cinemachineController.TriggerIntoVillageTransition();
            }
            else
            {
                GameManager.instance.cinemachineController.TriggerIntoWildernessTransition();
            }
        }

        t = 0f;
        do
        {
            t += Time.unscaledDeltaTime / 0.5f;
            changingCanvas.alpha = Mathf.Lerp(1f, 0f, t);
            yield return null;
        } while (t <= 1f);

        sMainUI.instance.ShowMainUI();
        sMainUI.instance.canPause = true;

        StopCoroutine(fakeLoading);
        fakeLoading = null;

        if (sceneType == SceneType.Village && !passOut)
        {
            sMainUI.instance.infoPopUpMessage.PushNewInfoMessage(returnToVillagePopUp);
        }

        changingScene = null;
        passOut = false;

        if (changingCanvas == blackCanvas)
        {
            GameManager.instance.PlayerHasControl = true;
            GameManager.instance.playerMovement.cc.enabled = true;
            sMainUI.instance.infoPopUpMessage.PushNewInfoMessage(passOutMessage);
        }
    }

    IEnumerator ShowLoading()
    {
        WaitForSecondsRealtime waitTime = new WaitForSecondsRealtime(0.1f);

        do
        {
            loadingImage.rotation *= Quaternion.Euler(0, 0, -360 / 10);
            loadingImageBlackScreen.rotation = loadingImage.rotation;
            yield return waitTime;
        } while (true);
    }

    IEnumerator ChangeStateOfVillageObjects()
    {
        bool active = nextSceneType == SceneType.Village;
        for (int i = 0; i < villageTerrains.Count; i++)
        {
            if (villageTerrains[i] == null)
            {
                villageTerrains.RemoveAt(i);
                i--;
            }
            else
            {
                villageTerrains[i].enabled = active;
            }
        }

        for (int i = 0; i < villageRenderers.Count; i++)
        {
            if (villageRenderers[i] == null)
            {
                villageRenderers.RemoveAt(i);
                i--;
            }
            else
            {
                villageRenderers[i].enabled = active;
            }
        }

        for (int i = 0; i < villageColliders.Count; i++)
        {
            if (villageColliders[i] == null)
            {
                villageColliders.RemoveAt(i);
                i--;
            }
            else
            {
                villageColliders[i].enabled = active;
            }
        }

        //for (int i = 0; i < villageNavMeshSurface.Count; i++)
        //{
        //    if (villageNavMeshSurface[i] == null)
        //    {
        //        villageNavMeshSurface.RemoveAt(i);
        //        i--;
        //    }
        //    else
        //    {
        //        villageNavMeshSurface[i].enabled = active;
        //    }
        //}

        //for (int i = 0; i < villageNavMeshLinks.Count; i++)
        //{
        //    if (villageNavMeshLinks[i] == null)
        //    {
        //        villageNavMeshLinks.RemoveAt(i);
        //        i--;
        //    }
        //    else
        //    {
        //        villageNavMeshLinks[i].enabled = active;
        //    }
        //}

        //for (int i = 0; i < villageNavMeshAgents.Count; i++)
        //{
        //    if (villageNavMeshAgents[i] == null)
        //    {
        //        villageNavMeshAgents.RemoveAt(i);
        //        i--;
        //    }
        //    else
        //    {
        //        villageNavMeshAgents[i].enabled = active;
        //    }
        //}

        //for (int i = 0; i < villageNavMeshVolume.Count; i++)
        //{
        //    if (villageNavMeshVolume[i] == null)
        //    {
        //        villageNavMeshVolume.RemoveAt(i);
        //        i--;
        //    }
        //    else
        //    {
        //        villageNavMeshVolume[i].enabled = active;
        //    }
        //}
        
        //for (int i = 0; i < villageNavMeshObstacles.Count; i++)
        //{
        //    if (villageNavMeshObstacles[i] == null)
        //    {
        //        villageNavMeshObstacles.RemoveAt(i);
        //        i--;
        //    }
        //    else
        //    {
        //        villageNavMeshObstacles[i].enabled = active;
        //    }
        //}

        for (int i = 0; i < villagePlots.Count; i++)
        {
            if (villagePlots[i] == null)
            {
                villagePlots.RemoveAt(i);
                i--;
            }
            else
            {
                villagePlots[i].enabled = active;
            }
        }
        finishedVillageObjects = true;

        yield return null;
    }

    IEnumerator ChangeStateOfWildernessObjects()
    {
        bool active = nextSceneType == SceneType.Wilderness;
        for (int i = 0; i < wildernessTerrains.Count; i++)
        {
            if (wildernessTerrains[i] == null)
            {
                wildernessTerrains.RemoveAt(i);
                i--;
            }
            else
            {
                wildernessTerrains[i].enabled = active;
            }
        }

        for (int i = 0; i < wildernessRenderers.Count; i++)
        {
            if (wildernessRenderers[i] == null)
            {
                wildernessRenderers.RemoveAt(i);
                i--;
            }
            else
            {
                wildernessRenderers[i].enabled = active;
            }
        }

        for (int i = 0; i < wildernessColliders.Count; i++)
        {
            if (wildernessColliders[i] == null)
            {
                wildernessColliders.RemoveAt(i);
                i--;
            }
            else
            {
                wildernessColliders[i].enabled = active;
            }
        }

        //for (int i = 0; i < wildernessNavMeshSurface.Count; i++)
        //{
        //    if (wildernessNavMeshSurface[i] == null)
        //    {
        //        wildernessNavMeshSurface.RemoveAt(i);
        //        i--;
        //    }
        //    else
        //    {
        //        wildernessNavMeshSurface[i].enabled = active;
        //    }
        //}

        //for (int i = 0; i < wildernessNavMeshLinks.Count; i++)
        //{
        //    if (wildernessNavMeshLinks[i] == null)
        //    {
        //        wildernessNavMeshLinks.RemoveAt(i);
        //        i--;
        //    }
        //    else
        //    {
        //        wildernessNavMeshLinks[i].enabled = active;
        //    }
        //}

        //for (int i = 0; i < wildernessNavMeshAgents.Count; i++)
        //{
        //    if (wildernessNavMeshAgents[i] == null)
        //    {
        //        wildernessNavMeshAgents.RemoveAt(i);
        //        i--;
        //    }
        //    else
        //    {
        //        wildernessNavMeshAgents[i].enabled = active;
        //    }
        //}

        //for (int i = 0; i < wildernessNavMeshVolume.Count; i++)
        //{
        //    if (wildernessNavMeshVolume[i] == null)
        //    {
        //        wildernessNavMeshVolume.RemoveAt(i);
        //        i--;
        //    }
        //    else
        //    {
        //        wildernessNavMeshVolume[i].enabled = active;
        //    }
        //}
        
        //for (int i = 0; i < wildernessNavMeshObstacles.Count; i++)
        //{
        //    if (wildernessNavMeshObstacles[i] == null)
        //    {
        //        wildernessNavMeshObstacles.RemoveAt(i);
        //        i--;
        //    }
        //    else
        //    {
        //        wildernessNavMeshObstacles[i].enabled = active;
        //    }
        //}
        finishedWildernessObjects = true;

        yield return null;
    }

    public void UpdateLists()
    {
        UpdateVillageList();
        UpdateWildernessList();
    }

    public void UpdateVillageList()
    {

        if (villageParent == null)
        {
            Debug.LogError("No Village Parent selected");
            return;
        }

        villageTerrains.Clear();
        villageRenderers.Clear();
        villageColliders.Clear();
        villageLights.Clear();
        //villageNavMeshSurface.Clear();
        //villageNavMeshLinks.Clear();
        //villageNavMeshAgents.Clear();
        //villageNavMeshVolume.Clear();
        //villageNavMeshObstacles.Clear();
        villagePlots.Clear();
        villageAudio.Clear();


        foreach (Terrain terrain in Resources.FindObjectsOfTypeAll(typeof(Terrain)) as Terrain[])
        {
            if (terrain.transform.root == villageParent)
            {
                villageTerrains.Add(terrain);
            }
        }

        foreach (Renderer rend in Resources.FindObjectsOfTypeAll(typeof(Renderer)) as Renderer[])
        {
            if (rend.transform.root == villageParent)
            {
                villageRenderers.Add(rend);
            }
        }

        foreach (Collider collider in Resources.FindObjectsOfTypeAll(typeof(Collider)) as Collider[])
        {
            if (collider.transform.root == villageParent)
            {
                villageColliders.Add(collider);
            }
        }

        foreach (Light light in Resources.FindObjectsOfTypeAll(typeof(Light)) as Light[])
        {
            if (light.transform.root == villageParent)
            {
                villageLights.Add(light);
            }
        }

        //foreach (NavMeshSurface navMeshSurface in Resources.FindObjectsOfTypeAll(typeof(NavMeshSurface)) as NavMeshSurface[])
        //{
        //    if (navMeshSurface.transform.root == villageParent)
        //    {
        //        villageNavMeshSurface.Add(navMeshSurface);
        //    }
        //}
        //
        //foreach (NavMeshLink navMeshLink in Resources.FindObjectsOfTypeAll(typeof(NavMeshLink)) as NavMeshLink[])
        //{
        //    if (navMeshLink.transform.root == villageParent)
        //    {
        //        villageNavMeshLinks.Add(navMeshLink);
        //    }
        //}
        //
        //foreach (NavMeshAgent navMeshAgent in Resources.FindObjectsOfTypeAll(typeof(NavMeshAgent)) as NavMeshAgent[])
        //{
        //    if (navMeshAgent.transform.root == villageParent)
        //    {
        //        villageNavMeshAgents.Add(navMeshAgent);
        //    }
        //}
        //
        //foreach (NavMeshModifierVolume navMeshVolume in Resources.FindObjectsOfTypeAll(typeof(NavMeshModifierVolume)) as NavMeshModifierVolume[])
        //{
        //    if (navMeshVolume.transform.root == villageParent)
        //    {
        //        villageNavMeshVolume.Add(navMeshVolume);
        //    }
        //}
        //
        //foreach (NavMeshObstacle navMeshObstacle in Resources.FindObjectsOfTypeAll(typeof(NavMeshObstacle)) as NavMeshObstacle[])
        //{
        //    if (navMeshObstacle.transform.root == villageParent)
        //    {
        //        villageNavMeshObstacles.Add(navMeshObstacle);
        //    }
        //}

        foreach (sPlot plot in Resources.FindObjectsOfTypeAll(typeof(sPlot)) as sPlot[])
        {
            if (plot.transform.root == villageParent)
            {
                villagePlots.Add(plot);
            }
        }

        foreach (AudioSource audio in Resources.FindObjectsOfTypeAll(typeof(AudioSource)) as AudioSource[])
        {
            if (audio.transform.root == villageParent)
            {
                villageAudio.Add(audio);
            }
        }
    }

    public void UpdateWildernessList()
    {
        if (wildernessParent == null)
        {
            Debug.LogError("No Wilderness Parent selected");
            return;
        }

        wildernessTerrains.Clear();
        wildernessRenderers.Clear();
        wildernessColliders.Clear();
        wildernessLights.Clear();
        wildernessAudio.Clear();
        //wildernessNavMeshSurface.Clear();
        //wildernessNavMeshLinks.Clear();
        //wildernessNavMeshAgents.Clear();
        //wildernessNavMeshVolume.Clear();
        //wildernessNavMeshObstacles.Clear();

        foreach (Terrain terrain in Resources.FindObjectsOfTypeAll(typeof(Terrain)) as Terrain[])
        {
            if (terrain.transform.root == wildernessParent)
            {
                wildernessTerrains.Add(terrain);
            }
        }

        foreach (Renderer rend in Resources.FindObjectsOfTypeAll(typeof(Renderer)) as Renderer[])
        {
            if (rend.transform.root == wildernessParent)
            {
                wildernessRenderers.Add(rend);
            }
        }

        foreach (Collider collider in Resources.FindObjectsOfTypeAll(typeof(Collider)) as Collider[])
        {
            if (collider.transform.root == wildernessParent)
            {
                wildernessColliders.Add(collider);
            }
        }

        foreach (Light light in Resources.FindObjectsOfTypeAll(typeof(Light)) as Light[])
        {
            if (light.transform.root == wildernessParent)
            {
                wildernessLights.Add(light);
            }
        }

        //foreach (NavMeshSurface navMeshSurface in Resources.FindObjectsOfTypeAll(typeof(NavMeshSurface)) as NavMeshSurface[])
        //{
        //    if (navMeshSurface.transform.root == wildernessParent)
        //    {
        //        wildernessNavMeshSurface.Add(navMeshSurface);
        //    }
        //}
        //
        //foreach (NavMeshLink navMeshLink in Resources.FindObjectsOfTypeAll(typeof(NavMeshLink)) as NavMeshLink[])
        //{
        //    if (navMeshLink.transform.root == wildernessParent)
        //    {
        //        wildernessNavMeshLinks.Add(navMeshLink);
        //    }
        //}
        //
        //foreach (NavMeshAgent navMeshAgent in Resources.FindObjectsOfTypeAll(typeof(NavMeshAgent)) as NavMeshAgent[])
        //{
        //    if (navMeshAgent.transform.root == wildernessParent)
        //    {
        //        wildernessNavMeshAgents.Add(navMeshAgent);
        //    }
        //}
        //
        //foreach (NavMeshModifierVolume navMeshVolume in Resources.FindObjectsOfTypeAll(typeof(NavMeshModifierVolume)) as NavMeshModifierVolume[])
        //{
        //    if (navMeshVolume.transform.root == wildernessParent)
        //    {
        //        wildernessNavMeshVolume.Add(navMeshVolume);
        //    }
        //}
        //
        //foreach (NavMeshObstacle navMeshObstacle in Resources.FindObjectsOfTypeAll(typeof(NavMeshObstacle)) as NavMeshObstacle[])
        //{
        //    if (navMeshObstacle.transform.root == wildernessParent)
        //    {
        //        wildernessNavMeshObstacles.Add(navMeshObstacle);
        //    }
        //}

        foreach (AudioSource audio in Resources.FindObjectsOfTypeAll(typeof(AudioSource)) as AudioSource[])
        {
            if (audio.transform.root == wildernessParent)
            {
                wildernessAudio.Add(audio);
            }
        }
    }

    //public void ChangeStateOfVillageObjectsEditor()
    //{
    //    for (int i = 0; i < villageTerrains.Count; i++)
    //    {
    //        if (villageTerrains[i] == null)
    //        {
    //            villageTerrains.RemoveAt(i);
    //            i--;
    //        }
    //        else
    //        {
    //            villageTerrains[i].enabled = !villageTerrains[i].enabled;
    //        }
    //    }

    //    for (int i = 0; i < villageRenderers.Count; i++)
    //    {
    //        if (villageRenderers[i] == null)
    //        {
    //            villageRenderers.RemoveAt(i);
    //            i--;
    //        }
    //        else
    //        {
    //            villageRenderers[i].enabled = !villageRenderers[i].enabled;
    //        }
    //    }

    //    for (int i = 0; i < villageColliders.Count; i++)
    //    {
    //        if (villageColliders[i] == null)
    //        {
    //            villageColliders.RemoveAt(i);
    //            i--;
    //        }
    //        else
    //        {
    //            villageColliders[i].enabled = !villageColliders[i].enabled;
    //        }
    //    }

    //    for (int i = 0; i < villageLights.Count; i++)
    //    {
    //        if (villageLights[i] == null)
    //        {
    //            villageLights.RemoveAt(i);
    //            i--;
    //        }
    //        else
    //        {
    //            villageLights[i].enabled = !villageLights[i].enabled;
    //        }
    //    }

    //    for (int i = 0; i < villageNavMeshSurface.Count; i++)
    //    {
    //        if (villageNavMeshSurface[i] == null)
    //        {
    //            villageNavMeshSurface.RemoveAt(i);
    //            i--;
    //        }
    //        else
    //        {
    //            villageNavMeshSurface[i].enabled = !villageNavMeshSurface[i].enabled;
    //        }
    //    }

    //    for (int i = 0; i < villageNavMeshLinks.Count; i++)
    //    {
    //        if (villageNavMeshLinks[i] == null)
    //        {
    //            villageNavMeshLinks.RemoveAt(i);
    //            i--;
    //        }
    //        else
    //        {
    //            villageNavMeshLinks[i].enabled = !villageNavMeshLinks[i].enabled;
    //        }
    //    }

    //    for (int i = 0; i < villageNavMeshAgents.Count; i++)
    //    {
    //        if (villageNavMeshAgents[i] == null)
    //        {
    //            villageNavMeshAgents.RemoveAt(i);
    //            i--;
    //        }
    //        else
    //        {
    //            villageNavMeshAgents[i].enabled = !villageNavMeshAgents[i].enabled;
    //        }
    //    }

    //    for (int i = 0; i < villageNavMeshVolume.Count; i++)
    //    {
    //        if (villageNavMeshVolume[i] == null)
    //        {
    //            villageNavMeshVolume.RemoveAt(i);
    //            i--;
    //        }
    //        else
    //        {
    //            villageNavMeshVolume[i].enabled = !villageNavMeshVolume[i].enabled;
    //        }
    //    }

    //    for (int i = 0; i < villageNavMeshObstacles.Count; i++)
    //    {
    //        if (villageNavMeshObstacles[i] == null)
    //        {
    //            villageNavMeshObstacles.RemoveAt(i);
    //            i--;
    //        }
    //        else
    //        {
    //            villageNavMeshObstacles[i].enabled = !villageNavMeshObstacles[i].enabled;
    //        }
    //    }

    //    for (int i = 0; i < villagePlots.Count; i++)
    //    {
    //        if (villagePlots[i] == null)
    //        {
    //            villagePlots.RemoveAt(i);
    //            i--;
    //        }
    //        else
    //        {
    //            villagePlots[i].enabled = !villagePlots[i].enabled;
    //        }
    //    }
    //    finishedVillageObjects = true;
    //}

    //public void ChangeStateOfWildernessObjectsEditor()
    //{
    //    for (int i = 0; i < wildernessTerrains.Count; i++)
    //    {
    //        if (wildernessTerrains[i] == null)
    //        {
    //            wildernessTerrains.RemoveAt(i);
    //            i--;
    //        }
    //        else
    //        {
    //            wildernessTerrains[i].enabled = !wildernessTerrains[i].enabled;
    //        }
    //    }

    //    for (int i = 0; i < wildernessRenderers.Count; i++)
    //    {
    //        if (wildernessRenderers[i] == null)
    //        {
    //            wildernessRenderers.RemoveAt(i);
    //            i--;
    //        }
    //        else
    //        {
    //            wildernessRenderers[i].enabled = !wildernessRenderers[i].enabled;
    //        }
    //    }

    //    for (int i = 0; i < wildernessColliders.Count; i++)
    //    {
    //        if (wildernessColliders[i] == null)
    //        {
    //            wildernessColliders.RemoveAt(i);
    //            i--;
    //        }
    //        else
    //        {
    //            wildernessColliders[i].enabled = !wildernessColliders[i].enabled;
    //        }
    //    }

    //    for (int i = 0; i < wildernessLights.Count; i++)
    //    {
    //        if (wildernessLights[i] == null)
    //        {
    //            wildernessLights.RemoveAt(i);
    //            i--;
    //        }
    //        else
    //        {
    //            wildernessLights[i].enabled = !wildernessLights[i].enabled;
    //        }
    //    }

    //    for (int i = 0; i < wildernessNavMeshSurface.Count; i++)
    //    {
    //        if (wildernessNavMeshSurface[i] == null)
    //        {
    //            wildernessNavMeshSurface.RemoveAt(i);
    //            i--;
    //        }
    //        else
    //        {
    //            wildernessNavMeshSurface[i].enabled = !wildernessNavMeshSurface[i].enabled;
    //        }
    //    }

    //    for (int i = 0; i < wildernessNavMeshLinks.Count; i++)
    //    {
    //        if (wildernessNavMeshLinks[i] == null)
    //        {
    //            wildernessNavMeshLinks.RemoveAt(i);
    //            i--;
    //        }
    //        else
    //        {
    //            wildernessNavMeshLinks[i].enabled = !wildernessNavMeshLinks[i].enabled;
    //        }
    //    }

    //    for (int i = 0; i < wildernessNavMeshAgents.Count; i++)
    //    {
    //        if (wildernessNavMeshAgents[i] == null)
    //        {
    //            wildernessNavMeshAgents.RemoveAt(i);
    //            i--;
    //        }
    //        else
    //        {
    //            wildernessNavMeshAgents[i].enabled = !wildernessNavMeshAgents[i].enabled;
    //        }
    //    }

    //    for (int i = 0; i < wildernessNavMeshVolume.Count; i++)
    //    {
    //        if (wildernessNavMeshVolume[i] == null)
    //        {
    //            wildernessNavMeshVolume.RemoveAt(i);
    //            i--;
    //        }
    //        else
    //        {
    //            wildernessNavMeshVolume[i].enabled = !wildernessNavMeshVolume[i].enabled;
    //        }
    //    }

    //    for (int i = 0; i < wildernessNavMeshObstacles.Count; i++)
    //    {
    //        if (wildernessNavMeshObstacles[i] == null)
    //        {
    //            wildernessNavMeshObstacles.RemoveAt(i);
    //            i--;
    //        }
    //        else
    //        {
    //            wildernessNavMeshObstacles[i].enabled = !wildernessNavMeshObstacles[i].enabled;
    //        }
    //    }
    //}

    public void SetStateOfVillageObjectsEditor(bool active)
    {
        for (int i = 0; i < villageTerrains.Count; i++)
        {
            if (villageTerrains[i] == null)
            {
                villageTerrains.RemoveAt(i);
                i--;
            }
            else
            {
                villageTerrains[i].enabled = active;
            }
        }

        for (int i = 0; i < villageRenderers.Count; i++)
        {
            if (villageRenderers[i] == null)
            {
                villageRenderers.RemoveAt(i);
                i--;
            }
            else
            {
                villageRenderers[i].enabled = active;
            }
        }

        for (int i = 0; i < villageColliders.Count; i++)
        {
            if (villageColliders[i] == null)
            {
                villageColliders.RemoveAt(i);
                i--;
            }
            else
            {
                villageColliders[i].enabled = active;
            }
        }

        for (int i = 0; i < villageLights.Count; i++)
        {
            if (villageLights[i] == null)
            {
                villageLights.RemoveAt(i);
                i--;
            }
            else
            {
                villageLights[i].enabled = active;
            }
        }

        //for (int i = 0; i < villageNavMeshSurface.Count; i++)
        //{
        //    if (villageNavMeshSurface[i] == null)
        //    {
        //        villageNavMeshSurface.RemoveAt(i);
        //        i--;
        //    }
        //    else
        //    {
        //        villageNavMeshSurface[i].enabled = active;
        //    }
        //}

        //for (int i = 0; i < villageNavMeshLinks.Count; i++)
        //{
        //    if (villageNavMeshLinks[i] == null)
        //    {
        //        villageNavMeshLinks.RemoveAt(i);
        //        i--;
        //    }
        //    else
        //    {
        //        villageNavMeshLinks[i].enabled = active;
        //    }
        //}

        //for (int i = 0; i < villageNavMeshAgents.Count; i++)
        //{
        //    if (villageNavMeshAgents[i] == null)
        //    {
        //        villageNavMeshAgents.RemoveAt(i);
        //        i--;
        //    }
        //    else
        //    {
        //        villageNavMeshAgents[i].enabled = active;
        //    }
        //}
        //
        //for (int i = 0; i < villageNavMeshVolume.Count; i++)
        //{
        //    if (villageNavMeshVolume[i] == null)
        //    {
        //        villageNavMeshVolume.RemoveAt(i);
        //        i--;
        //    }
        //    else
        //    {
        //        villageNavMeshVolume[i].enabled = active;
        //    }
        //}
        //
        //for (int i = 0; i < villageNavMeshObstacles.Count; i++)
        //{
        //    if (villageNavMeshObstacles[i] == null)
        //    {
        //        villageNavMeshObstacles.RemoveAt(i);
        //        i--;
        //    }
        //    else
        //    {
        //        villageNavMeshObstacles[i].enabled = active;
        //    }
        //}

        for (int i = 0; i < villagePlots.Count; i++)
        {
            if (villagePlots[i] == null)
            {
                villagePlots.RemoveAt(i);
                i--;
            }
            else
            {
                villagePlots[i].enabled = active;
            }
        }

        for (int i = 0; i < villageAudio.Count; i++)
        {
            if (villageAudio[i] == null)
            {
                villageAudio.RemoveAt(i);
                i--;
            }
            else
            {
                villageAudio[i].enabled = active;
            }
        }
        finishedVillageObjects = true;
    }

    public void SetStateOfWildernessObjectsEditor(bool active)
    {
        for (int i = 0; i < wildernessTerrains.Count; i++)
        {
            if (wildernessTerrains[i] == null)
            {
                wildernessTerrains.RemoveAt(i);
                i--;
            }
            else
            {
                wildernessTerrains[i].enabled = active;
            }
        }

        for (int i = 0; i < wildernessRenderers.Count; i++)
        {
            if (wildernessRenderers[i] == null)
            {
                wildernessRenderers.RemoveAt(i);
                i--;
            }
            else
            {
                wildernessRenderers[i].enabled = active;
            }
        }

        for (int i = 0; i < wildernessColliders.Count; i++)
        {
            if (wildernessColliders[i] == null)
            {
                wildernessColliders.RemoveAt(i);
                i--;
            }
            else
            {
                wildernessColliders[i].enabled = active;
            }
        }

        for (int i = 0; i < wildernessLights.Count; i++)
        {
            if (wildernessLights[i] == null)
            {
                wildernessLights.RemoveAt(i);
                i--;
            }
            else
            {
                wildernessLights[i].enabled = active;
            }
        }

        //for (int i = 0; i < wildernessNavMeshSurface.Count; i++)
        //{
        //    if (wildernessNavMeshSurface[i] == null)
        //    {
        //        wildernessNavMeshSurface.RemoveAt(i);
        //        i--;
        //    }
        //    else
        //    {
        //        wildernessNavMeshSurface[i].enabled = active;
        //    }
        //}

        //for (int i = 0; i < wildernessNavMeshLinks.Count; i++)
        //{
        //    if (wildernessNavMeshLinks[i] == null)
        //    {
        //        wildernessNavMeshLinks.RemoveAt(i);
        //        i--;
        //    }
        //    else
        //    {
        //        wildernessNavMeshLinks[i].enabled = active;
        //    }
        //}

        //for (int i = 0; i < wildernessNavMeshAgents.Count; i++)
        //{
        //    if (wildernessNavMeshAgents[i] == null)
        //    {
        //        wildernessNavMeshAgents.RemoveAt(i);
        //        i--;
        //    }
        //    else
        //    {
        //        wildernessNavMeshAgents[i].enabled = active;
        //    }
        //}
        //
        //for (int i = 0; i < wildernessNavMeshVolume.Count; i++)
        //{
        //    if (wildernessNavMeshVolume[i] == null)
        //    {
        //        wildernessNavMeshVolume.RemoveAt(i);
        //        i--;
        //    }
        //    else
        //    {
        //        wildernessNavMeshVolume[i].enabled = active;
        //    }
        //}
        //
        //for (int i = 0; i < wildernessNavMeshObstacles.Count; i++)
        //{
        //    if (wildernessNavMeshObstacles[i] == null)
        //    {
        //        wildernessNavMeshObstacles.RemoveAt(i);
        //        i--;
        //    }
        //    else
        //    {
        //        wildernessNavMeshObstacles[i].enabled = active;
        //    }
        //}

        for (int i = 0; i < wildernessAudio.Count; i++)
        {
            if (wildernessAudio[i] == null)
            {
                wildernessAudio.RemoveAt(i);
                i--;
            }
            else
            {
                wildernessAudio[i].enabled = active;
            }
        }
        finishedWildernessObjects = true;
    }

    public Transform GetCurrentSceneParent()
    {
        switch (currentSceneType)
        {
            case SceneType.Village:
                return villageParent;
            case SceneType.Wilderness:
                return wildernessParent;
        }
        return null;
    }

}

#if UNITY_EDITOR
[CustomEditor(typeof(sSceneController))]
public class SceneControllerEditor : Editor
{
    sSceneController _target;

    public override void OnInspectorGUI()
    {

        _target = (sSceneController)target;

        DrawDefaultInspector();

        GUILayout.Space(20);
        if (GUILayout.Button("Update Village List?"))
        {
            _target.UpdateVillageList();
        }

        GUILayout.Space(20);

        if (GUILayout.Button("Update Wilderness List?"))
        {
            _target.UpdateWildernessList();
        }

        GUILayout.Space(20);

        if (GUILayout.Button("Update Lists?"))
        {
            _target.UpdateLists();
        }

        //GUILayout.Space(20);

        //if (GUILayout.Button("Turn On/Off Village objects"))
        //{
        //    _target.ChangeStateOfVillageObjectsEditor();
        //}

        //GUILayout.Space(20);

        //if (GUILayout.Button("Turn On/Off Wilderness objects"))
        //{
        //    _target.ChangeStateOfWildernessObjectsEditor();
        //}

        GUILayout.Space(20);

        if (GUILayout.Button("Show only Village"))
        {
            _target.SetStateOfVillageObjectsEditor(true);
            _target.SetStateOfWildernessObjectsEditor(false);
            _target.wildernessParent.position = _target.hiddenPos;
            _target.villageParent.position = Vector3.zero;
        }

        GUILayout.Space(20);

        if (GUILayout.Button("Show only Wilderness"))
        {
            _target.SetStateOfVillageObjectsEditor(false);
            _target.SetStateOfWildernessObjectsEditor(true);
            _target.villageParent.position = _target.hiddenPos;
            _target.wildernessParent.position = Vector3.zero;
        }
    }
}

#endif

