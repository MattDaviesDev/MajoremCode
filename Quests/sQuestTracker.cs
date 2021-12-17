using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class sQuestTracker : MonoBehaviour
{
    [SerializeField] Transform objectiveParent;
    GameObject objectivePrefab;
    [SerializeField] GameObject turnInMessagePrefab;

    [SerializeField] TextMeshProUGUI questName;

    public Quest trackedQuest;
    // Start is called before the first frame update
    void Start()
    {
        objectivePrefab = objectiveParent.GetChild(0).gameObject;
        StartCoroutine(LerpScaleOfTurnInMessage());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TrackNewQuest(Quest _trackedQuest)
    {
        CleanOutChildren();

        trackedQuest = _trackedQuest;

        questName.text = _trackedQuest.QuestName;

        switch (trackedQuest.questObjective[trackedQuest.currentQuestObjective])
        {
            case Quest.QuestObjective.Gather:
                for (int i = 0; i < trackedQuest.gatherObjectives.Length; i++)
                {
                    GameObject temp = Instantiate(objectivePrefab, objectiveParent);
                    sQuestTrackerObjective tempTracker = temp.GetComponent<sQuestTrackerObjective>();
                    tempTracker.objectiveTitle.text = "Gather " + trackedQuest.gatherObjectives[i].quantity 
                        + " " + trackedQuest.gatherObjectives[i].resource.resourceName + ".";
                    tempTracker.objectiveProgress.text = trackedQuest.currentGatherObjectives[i].quantity 
                        + "/" + trackedQuest.gatherObjectives[i].quantity;
                    tempTracker.objectiveCompleted.isOn = trackedQuest.objectiveCompleted[i];
                    temp.SetActive(true);
                }
                break;
            case Quest.QuestObjective.Craft:
                for (int i = 0; i < trackedQuest.gatherObjectives.Length; i++)
                {
                    GameObject temp = Instantiate(objectivePrefab, objectiveParent);
                    sQuestTrackerObjective tempTracker = temp.GetComponent<sQuestTrackerObjective>();
                    string plural = trackedQuest.gatherObjectives[i].quantity > 1 ? "'s" : "";
                    tempTracker.objectiveTitle.text = "Craft " + trackedQuest.gatherObjectives[i].quantity
                        + " " + trackedQuest.gatherObjectives[i].resource.resourceName + plural + ".";
                    tempTracker.objectiveProgress.text = trackedQuest.currentGatherObjectives[i].quantity
                        + "/" + trackedQuest.gatherObjectives[i].quantity;
                    tempTracker.objectiveCompleted.isOn = trackedQuest.objectiveCompleted[i];
                    temp.SetActive(true);
                }
                break;
            case Quest.QuestObjective.Cook:
                for (int i = 0; i < trackedQuest.gatherObjectives.Length; i++)
                {
                    GameObject temp = Instantiate(objectivePrefab, objectiveParent);
                    sQuestTrackerObjective tempTracker = temp.GetComponent<sQuestTrackerObjective>();
                    string plural = trackedQuest.gatherObjectives[i].quantity > 1 ? "'s" : "";
                    tempTracker.objectiveTitle.text = "Cook " + trackedQuest.gatherObjectives[i].quantity
                        + " " + trackedQuest.gatherObjectives[i].resource.resourceName + plural + ".";
                    tempTracker.objectiveProgress.text = trackedQuest.currentGatherObjectives[i].quantity
                        + "/" + trackedQuest.gatherObjectives[i].quantity;
                    tempTracker.objectiveCompleted.isOn = trackedQuest.objectiveCompleted[i];
                    temp.SetActive(true);
                }
                break;
            case Quest.QuestObjective.Grow:
                for (int i = 0; i < trackedQuest.gatherObjectives.Length; i++)
                {
                    GameObject temp = Instantiate(objectivePrefab, objectiveParent);
                    sQuestTrackerObjective tempTracker = temp.GetComponent<sQuestTrackerObjective>();
                    tempTracker.objectiveTitle.text = "Grow " + trackedQuest.gatherObjectives[i].quantity
                        + " " + trackedQuest.gatherObjectives[i].resource.resourceName + ".";
                    tempTracker.objectiveProgress.text = trackedQuest.currentGatherObjectives[i].quantity
                        + "/" + trackedQuest.gatherObjectives[i].quantity;
                    tempTracker.objectiveCompleted.isOn = trackedQuest.objectiveCompleted[i];
                    temp.SetActive(true);
                }
                break;
            case Quest.QuestObjective.Build:
                for (int i = 0; i < trackedQuest.buildingType.Length; i++)
                {
                    GameObject temp = Instantiate(objectivePrefab, objectiveParent);
                    sQuestTrackerObjective tempTracker = temp.GetComponent<sQuestTrackerObjective>();
                    string plural = trackedQuest.numberOfBuilding[i] > 1 ? "'s" : "";
                    tempTracker.objectiveTitle.text = "Build " + trackedQuest.numberOfBuilding[i]
                        + " " + trackedQuest.buildingType[i].ToString() + plural + ".";
                    tempTracker.objectiveProgress.text = trackedQuest.currentNumberOfBuilding[i]
                        + "/" + trackedQuest.numberOfBuilding[i];
                    tempTracker.objectiveCompleted.isOn = trackedQuest.objectiveCompleted[i];
                    temp.SetActive(true);
                }
                break;
            case Quest.QuestObjective.GoTo:
                break;
            case Quest.QuestObjective.Interact:
                break;
        }

        if (trackedQuest.completed && !trackedQuest.turnedIn)
        {
            turnInMessagePrefab.transform.parent = objectiveParent;
            turnInMessagePrefab.SetActive(true);
        }
        else
        {
            turnInMessagePrefab.transform.parent = null;
            turnInMessagePrefab.SetActive(false);
        }
    }

    public void UpdateCurrentTrackerProgress()
    {
        if (trackedQuest == null)
        {
            return;
        }
        switch (trackedQuest.questObjective[trackedQuest.currentQuestObjective])
        {
            case Quest.QuestObjective.Gather:
                for (int i = 1; i < objectiveParent.childCount; i++)
                {
                    sQuestTrackerObjective tempTracker = objectiveParent.GetChild(i).GetComponent<sQuestTrackerObjective>();
                    if (tempTracker != null)
                    {
                        tempTracker.objectiveTitle.text = "Gather " + trackedQuest.gatherObjectives[i - 1].quantity
                            + " " + trackedQuest.gatherObjectives[i - 1].resource.resourceName + ".";
                        tempTracker.objectiveProgress.text = trackedQuest.currentGatherObjectives[i - 1].quantity
                            + "/" + trackedQuest.gatherObjectives[i - 1].quantity;
                        tempTracker.objectiveCompleted.isOn = trackedQuest.objectiveCompleted[i - 1];
                    }
                }
                break;
            case Quest.QuestObjective.Craft:
                for (int i = 1; i < objectiveParent.childCount; i++)
                {
                    sQuestTrackerObjective tempTracker = objectiveParent.GetChild(i).GetComponent<sQuestTrackerObjective>();
                    if (tempTracker != null)
                    {
                        tempTracker.objectiveTitle.text = "Craft " + trackedQuest.gatherObjectives[i - 1].quantity
                            + " " + trackedQuest.gatherObjectives[i - 1].resource.resourceName + ".";
                        tempTracker.objectiveProgress.text = trackedQuest.currentGatherObjectives[i - 1].quantity
                            + "/" + trackedQuest.gatherObjectives[i - 1].quantity;
                        tempTracker.objectiveCompleted.isOn = trackedQuest.objectiveCompleted[i - 1];
                    }
                }
                break;
            case Quest.QuestObjective.Cook:
                for (int i = 1; i < objectiveParent.childCount; i++)
                {
                    sQuestTrackerObjective tempTracker = objectiveParent.GetChild(i).GetComponent<sQuestTrackerObjective>();
                    if (tempTracker != null)
                    {
                        tempTracker.objectiveTitle.text = "Cook " + trackedQuest.gatherObjectives[i - 1].quantity
                            + " " + trackedQuest.gatherObjectives[i - 1].resource.resourceName + ".";
                        tempTracker.objectiveProgress.text = trackedQuest.currentGatherObjectives[i - 1].quantity
                            + "/" + trackedQuest.gatherObjectives[i - 1].quantity;
                        tempTracker.objectiveCompleted.isOn = trackedQuest.objectiveCompleted[i - 1];
                    }
                }
                break;
            case Quest.QuestObjective.Grow:
                for (int i = 1; i < objectiveParent.childCount; i++)
                {
                    sQuestTrackerObjective tempTracker = objectiveParent.GetChild(i).GetComponent<sQuestTrackerObjective>();
                    if (tempTracker != null)
                    {
                        tempTracker.objectiveTitle.text = "Grow " + trackedQuest.gatherObjectives[i - 1].quantity
                            + " " + trackedQuest.gatherObjectives[i - 1].resource.resourceName + ".";
                        tempTracker.objectiveProgress.text = trackedQuest.currentGatherObjectives[i - 1].quantity
                            + "/" + trackedQuest.gatherObjectives[i - 1].quantity;
                        tempTracker.objectiveCompleted.isOn = trackedQuest.objectiveCompleted[i - 1];
                    }
                }
                break;
            case Quest.QuestObjective.Build:
                for (int i = 1; i < objectiveParent.childCount; i++)
                {
                    sQuestTrackerObjective tempTracker = objectiveParent.GetChild(i).GetComponent<sQuestTrackerObjective>();
                    if (tempTracker != null)
                    {
                        string plural = trackedQuest.numberOfBuilding[i -1] > 1 ? "'s" : "";
                        tempTracker.objectiveTitle.text = "Build " + trackedQuest.numberOfBuilding[i - 1]
                            + " " + trackedQuest.buildingType[i - 1].ToString() + plural + ".";
                        tempTracker.objectiveProgress.text = trackedQuest.currentNumberOfBuilding[i - 1]
                            + "/" + trackedQuest.numberOfBuilding[i - 1];
                        tempTracker.objectiveCompleted.isOn = trackedQuest.objectiveCompleted[i - 1];
                    }
                }
                break;
            case Quest.QuestObjective.GoTo:
                break;
            case Quest.QuestObjective.Interact:
                break;
            default:
                break;
        }
        
        if (trackedQuest.completed && !trackedQuest.turnedIn)
        {
            turnInMessagePrefab.transform.parent = objectiveParent;
            turnInMessagePrefab.SetActive(true);
        }
        else
        {
            turnInMessagePrefab.transform.parent = null;
            turnInMessagePrefab.SetActive(false);
        }

    }

    void CleanOutChildren()
    {
        turnInMessagePrefab.transform.parent = null;
        turnInMessagePrefab.SetActive(false);
        for (int i = objectiveParent.childCount - 1; i > 0; i--)
        {
            if (objectiveParent.GetChild(i).gameObject != turnInMessagePrefab)
            {
                Destroy(objectiveParent.GetChild(i).gameObject);
            }
        }
    }

    public void AssignTrackedQuestFromAllQuests(List<Quest> allQuests)
    {
        for (int i = 0; i < allQuests.Count; i++)
        {
            if (allQuests[i].tracked)
            {
                TrackNewQuest(allQuests[i]);
                return;
            }
        }
    }

    public void ResetQuestTracker()
    {
        trackedQuest.tracked = false;
        questName.text = "No tracked quest.";
        CleanOutChildren();
    }

    IEnumerator LerpScaleOfTurnInMessage()
    {
        while (true)
        {
            float t = 0f;
            Vector3 startScale = Vector3.one * 0.95f;
            Vector3 endScale = Vector3.one * 1.05f;
            while (t <= 1f)
            {
                t += Time.deltaTime;
                turnInMessagePrefab.transform.localScale = Vector3.Lerp(startScale, endScale, Lerp.SmootherStep(t));
                yield return null;
            }
            t = 0f;
            while (t <= 1f)
            {
                t += Time.deltaTime;
                turnInMessagePrefab.transform.localScale = Vector3.Lerp(endScale, startScale, Lerp.SmootherStep(t));
                yield return null;
            }

        }
    }
}
