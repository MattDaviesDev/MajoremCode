using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class sSpawnedObject : MonoBehaviour
{

    Collider myCol;
    Renderer myRend;
    Light myLight;
    //NavMeshLink myLink;
    //NavMeshAgent myAgent;
    //NavMeshObstacle myObstacle;

    Collider[] allCols;
    Renderer[] allRends;
    Light[] allLights;
    //NavMeshLink[] allLinks;
    //NavMeshAgent[] allAgents;
    //NavMeshObstacle[] allObstacles;

    // Start is called before the first frame update
    void Start()
    {

        myRend = GetComponent<Renderer>();
        myCol = GetComponent<Collider>();
        myLight = GetComponent<Light>();
        //myLink = GetComponent<NavMeshLink>();
        //myAgent = GetComponent<NavMeshAgent>();
        //myObstacle = GetComponent<NavMeshObstacle>();

        allRends = GetComponentsInChildren<Renderer>();
        allCols = GetComponentsInChildren<Collider>();
        allLights = GetComponentsInChildren<Light>();
        //allLinks = GetComponentsInChildren<NavMeshLink>();
        //allAgents = GetComponentsInChildren<NavMeshAgent>();
        //allObstacles = GetComponentsInChildren<NavMeshObstacle>();

        bool active = false;

        if (transform.root == sSceneController.instance.villageParent)
        {
            if (sSceneController.instance.currentSceneType == SceneType.Village)
            {
                active = true;
            }
            
            sSceneController.instance.villageRenderers.Add(myRend);
            sSceneController.instance.villageColliders.Add(myCol);
            //sSceneController.instance.villageNavMeshAgents.Add(myAgent);
            //sSceneController.instance.villageNavMeshLinks.Add(myLink);
            //sSceneController.instance.villageNavMeshObstacles.Add(myObstacle);
            if (myRend != null)
            {
                myRend.enabled = active;
            }
            if (myCol != null)
            {
                myCol.enabled = active;
            }
            if (myLight != null)
            {
                myLight.enabled = active;
            }
            //if (myAgent != null)
            //{
            //    myAgent.enabled = active;
            //}
            //if (myLink != null)
            //{
            //    myLink.enabled = active;
            //}
            //if (myObstacle != null)
            //{
            //    myObstacle.enabled = active;
            //}

            for (int i = 0; i < allRends.Length; i++)
            {
                sSceneController.instance.villageRenderers.Add(allRends[i]);
                allRends[i].enabled = active;
            }

            for (int i = 0; i < allCols.Length; i++)
            {
                sSceneController.instance.villageColliders.Add(allCols[i]);
                allCols[i].enabled = active;
            }

            for (int i = 0; i < allLights.Length; i++)
            {
                sSceneController.instance.villageLights.Add(allLights[i]);
                allLights[i].enabled = active;
            }

            //for (int i = 0; i < allLinks.Length; i++)
            //{
            //    sSceneController.instance.villageNavMeshLinks.Add(allLinks[i]);
            //    allLinks[i].enabled = active;
            //}
            //
            //for (int i = 0; i < allAgents.Length; i++)
            //{
            //    sSceneController.instance.villageNavMeshAgents.Add(allAgents[i]);
            //    allAgents[i].enabled = active;
            //}
            //
            //for (int i = 0; i < allObstacles.Length; i++)
            //{
            //    sSceneController.instance.villageNavMeshObstacles.Add(allObstacles[i]);
            //    allObstacles[i].enabled = active;
            //}
        }
        else
        {
            if (sSceneController.instance.currentSceneType == SceneType.Wilderness)
            {
                active = true;
            }

            sSceneController.instance.wildernessRenderers.Add(myRend);
            sSceneController.instance.wildernessColliders.Add(myCol);
            //sSceneController.instance.wildernessNavMeshAgents.Add(myAgent);
            //sSceneController.instance.wildernessNavMeshLinks.Add(myLink);
            //sSceneController.instance.wildernessNavMeshObstacles.Add(myObstacle);
            if (myRend != null)
            {
                myRend.enabled = active;
            }
            if (myCol != null)
            {
                myCol.enabled = active;
            }
            //if (myAgent != null)
            //{
            //    myAgent.enabled = active;
            //}
            //if (myLink != null)
            //{
            //    myLink.enabled = active;
            //}
            //if (myObstacle != null)
            //{
            //    myObstacle.enabled = active;
            //}

            for (int i = 0; i < allRends.Length; i++)
            {
                sSceneController.instance.wildernessRenderers.Add(allRends[i]);
                allRends[i].enabled = active;
            }

            for (int i = 0; i < allCols.Length; i++)
            {
                sSceneController.instance.wildernessColliders.Add(allCols[i]);
                allCols[i].enabled = active;
            }

            //for (int i = 0; i < allLinks.Length; i++)
            //{
            //    sSceneController.instance.wildernessNavMeshLinks.Add(allLinks[i]);
            //    allLinks[i].enabled = active;
            //}
            //
            //for (int i = 0; i < allAgents.Length; i++)
            //{
            //    sSceneController.instance.wildernessNavMeshAgents.Add(allAgents[i]);
            //    allAgents[i].enabled = active;
            //}
            //
            //for (int i = 0; i < allObstacles.Length; i++)
            //{
            //    sSceneController.instance.wildernessNavMeshObstacles.Add(allObstacles[i]);
            //    allObstacles[i].enabled = active;
            //}
        }
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
