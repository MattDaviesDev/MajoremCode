using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.VFX;

public class sGatherNode : MonoBehaviour
{

    [SerializeField, Range(1f, 5f)] float maxRadius;

    [SerializeField, Range(1, 5)] int maxNodesSpawned;
    [SerializeField] GameObject nodePrefab;
    public int maxFromOneNode = 3;

    [SerializeField] Terrain myTerrain;

    public Date currentTimeToReplenish;

    [SerializeField] ResourceNodeData resourceData;

    public List<sGatherNodeObject> activeNodes = new List<sGatherNodeObject>();

    bool depleated = true;

    List<VisualEffect> pool = new List<VisualEffect>();


    // Start is called before the first frame update
    void Start()
    {
        //sInputManager.instance.testAction.performed += SpawnNode;
    }

    // Update is called once per frame
    void Update()
    {
        if (depleated)
        {
            currentTimeToReplenish.Seconds -= Time.deltaTime * GameManager.instance.dayCycle.currentTimeConversion;
            if (currentTimeToReplenish.Equals(Date.Zero()))
            {
                OnReplenish();
            }
        }

    }

    private void OnEnable()
    {
        sDayCycle.onTimeSkipped += TimeSkipped;
    }

    private void OnDisable()
    {
        sDayCycle.onTimeSkipped -= TimeSkipped;
    }

    public void OnNodeHarvested(sGatherNodeObject node)
    {
        StartCoroutine(HarvestedNodeObjectAnim(node));
    }

    IEnumerator HarvestedNodeObjectAnim(sGatherNodeObject node)
    {
        float t = 0f;
        Transform nodeTransform = node.transform;
        Vector3 startScale = nodeTransform.localScale;
        Vector3 endScale = startScale * 1.2f;
        while (t <= 1f)
        {
            t += Time.deltaTime / 0.2f;
            nodeTransform.localScale = Vector3.Lerp(startScale, endScale, Lerp.SmootherStep(t));
            yield return null;
        }
        t = 0f;
        startScale = endScale;
        endScale = Vector3.zero;
        VisualEffect temp = GetFreeObject();
        temp.transform.position = node.transform.position + (Vector3.up * 0.1f);
        temp.transform.rotation = Quaternion.identity;
        temp.Play();
        while (t <= 1f)
        {
            t += Time.deltaTime / 0.3f;
            nodeTransform.localScale = Vector3.Lerp(startScale, endScale, Lerp.SmootherStep(t));
            yield return null;
        }
        activeNodes.Remove(node);
        Destroy(node.gameObject);
        if (activeNodes.Count == 0)
        {
            OnDepleated();
        }
    }

    void OnDepleated()
    {
        currentTimeToReplenish = new Date(resourceData.replenishTime);
        depleated = true;
    }

    void OnReplenish()
    {
        if (myTerrain != null)
        {
            depleated = false;
            int count = Random.Range(1, maxNodesSpawned + 1);
            for (int i = 0; i < count; i++)
            {
                SpawnNode();
            }
        }
        else
        {
            print("No terrain");
        }
    }

    void SpawnNode()
    {
        Vector3 localPos = new Vector3(Random.Range(-1f, 1f), 0f, Random.Range(-1f, 1f));
        localPos = localPos.normalized * Random.Range(1f, maxRadius);
        localPos = transform.position + localPos;
        float tempY = myTerrain.SampleHeight(localPos);
        GameObject temp = Instantiate(nodePrefab, localPos, Quaternion.identity);
        temp.transform.parent = transform;
        sGatherNodeObject tempScr = temp.GetComponent<sGatherNodeObject>();
        tempScr.myTargetYPos = tempY;
        tempScr.parentNode = this;
        tempScr.resource = new ResourceData(resourceData.defaultResources[0].resource, Random.Range(1, resourceData.defaultResources[0].quantity + 1));
        activeNodes.Add(tempScr);
    }

    void TimeSkipped()
    {
        if (currentTimeToReplenish > Date.Zero())
        {
            sDayCycle.ReduceTime(currentTimeToReplenish);
        }
    }

    VisualEffect GetFreeObject()
    {
        for (int i = 0; i < pool.Count; i++)
        {
            if (!pool[i].gameObject.activeInHierarchy)
            {
                return pool[i];
            }
        }
        GameObject temp = Instantiate(AssetFinder.instance.gatherPoofVFX, sSceneController.instance.wildernessParent);
        pool.Add(temp.GetComponent<VisualEffect>());
        return pool[pool.Count - 1];
    }

}
