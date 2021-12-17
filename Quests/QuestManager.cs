using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class QuestManager : MonoBehaviour
{
    public static QuestManager _instance;

    public static QuestManager instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<QuestManager>();
            }
            return _instance;
        }
    }

    [SerializeField] public List<Quest> allQuests = new List<Quest>();

    [SerializeField] public List<Quest> activeQuests = new List<Quest>();

    [SerializeField] List<Quest> mainQuests = new List<Quest>();
    [SerializeField] List<Quest> sideQuests = new List<Quest>();

    public int numberOfQuestsToTurnIn 
    {
        get
        {
            int returnVal = 0;
            for (int i = 0; i < activeQuests.Count; i++)
            {
                if (!activeQuests[i].turnedIn && activeQuests[i].completed)
                {
                    returnVal++;
                }
            }
            return returnVal;
        }   
    }
    

    // Start is called before the first frame update
    void Start()
    {
        AssignQuestsToCorrectList();
        sMainUI.instance.questTracker.AssignTrackedQuestFromAllQuests(allQuests);
    }

    private void OnEnable()
    {
        sInputManager.instance.showQuestLog.performed += ShowQuestLog;
    }

    private void OnDisable()
    {
        sInputManager.instance.showQuestLog.performed -= ShowQuestLog;
    }

    public void AssignQuestsToCorrectList()
    {
        activeQuests.Clear();
        for (int i = 0; i < allQuests.Count; i++)
        {
            if (allQuests[i].available && !allQuests[i].turnedIn)
            {
                activeQuests.Add(allQuests[i]);
            }
            switch (allQuests[i].questType)
            {
                case Quest.QuestType.Main:
                    mainQuests.Add(allQuests[i]);
                    break;
                case Quest.QuestType.Side:
                    sideQuests.Add(allQuests[i]);
                    break;
            }
        }
    }

    public void QuestObjectiveProgress(Quest.QuestObjective objectiveType, ResourceData data)
    {
        for (int i = 0; i < activeQuests.Count; i++)
        {
            bool isGather = activeQuests[i].questObjective[activeQuests[i].currentQuestObjective] == Quest.QuestObjective.Gather;
            bool isCraft = activeQuests[i].questObjective[activeQuests[i].currentQuestObjective] == Quest.QuestObjective.Craft;
            bool isCook = activeQuests[i].questObjective[activeQuests[i].currentQuestObjective] == Quest.QuestObjective.Cook;
            bool isGrow = activeQuests[i].questObjective[activeQuests[i].currentQuestObjective] == Quest.QuestObjective.Grow;
            if (isGather || isCraft || isCook || isGrow)
            {
                for (int j = 0; j < activeQuests[i].objectiveCompleted.Length; j++)
                {
                    if (!activeQuests[i].objectiveCompleted[j])
                    {
                        if (activeQuests[i].currentGatherObjectives[j].resource.ID == data.resource.ID)
                        {
                            int completionQuantity = activeQuests[i].gatherObjectives[j].quantity;
                            if (isCook)
                            {
                                activeQuests[i].currentGatherObjectives[j].quantity++;
                            }
                            else
                            {
                                activeQuests[i].currentGatherObjectives[j].quantity += data.quantity;
                            }
                            int clamp = Mathf.Clamp(activeQuests[i].currentGatherObjectives[j].quantity, 0, completionQuantity);
                            activeQuests[i].currentGatherObjectives[j].quantity = clamp;
                            if (activeQuests[i].currentGatherObjectives[j].quantity == completionQuantity)
                            {
                                activeQuests[i].objectiveCompleted[j] = true;
                            }
                        }
                    }
                }
            }
        }

        sMainUI.instance.questTracker.UpdateCurrentTrackerProgress();
    }

    public void QuestObjectiveProgress(Quest.QuestObjective objectiveType, BuildingType data)
    {
        print("Built item");
        for (int i = 0; i < activeQuests.Count; i++)
        {
            if (activeQuests[i].questObjective[activeQuests[i].currentQuestObjective] == Quest.QuestObjective.Build)
            {
                for (int j = 0; j < activeQuests[i].objectiveCompleted.Length; j++)
                {
                    if (!activeQuests[i].objectiveCompleted[j])
                    {
                        if (activeQuests[i].buildingType[j] == data)
                        {
                            print("Adding a building");
                            int completionQuantity = activeQuests[i].numberOfBuilding[j];
                            activeQuests[i].currentNumberOfBuilding[j]++;
                            int clamp = Mathf.Clamp(activeQuests[i].currentNumberOfBuilding[j], 0, completionQuantity);
                            activeQuests[i].currentNumberOfBuilding[j] = clamp;
                            if (activeQuests[i].currentNumberOfBuilding[j] == completionQuantity)
                            {
                                activeQuests[i].objectiveCompleted[j] = true;
                            }
                        }
                    }
                }
            }
        }
        sMainUI.instance.questTracker.UpdateCurrentTrackerProgress();
    }

    public void QuestObjectiveProgress(Quest.QuestObjective objectiveType, Transform data)
    {

        sMainUI.instance.questTracker.UpdateCurrentTrackerProgress();
    }

    public void QuestObjectiveProgress(Quest.QuestObjective objectiveType, IInteractableObject data)
    {

        sMainUI.instance.questTracker.UpdateCurrentTrackerProgress();
    }

    public void ShowQuestLog(InputAction.CallbackContext input)
    {
        sMainUI.instance.OpenQuestInterface();
    }

    public void TurnInAllQuests()
    {
        int turnedInQuests = 0;
        for (int i = 0; i < activeQuests.Count; i++)
        {
            if (!activeQuests[i].turnedIn && activeQuests[i].completed)
            {
                if (activeQuests[i] == sMainUI.instance.questTracker.trackedQuest)
                {
                    sMainUI.instance.questTracker.ResetQuestTracker();
                }
                activeQuests[i].turnedIn = true;
                turnedInQuests++;
            }
        }
        sMainUI.instance.infoPopUpMessage.PushNewInfoMessage("You turned in " + turnedInQuests + " Quests, Great Job!");
        AssignQuestsToCorrectList();

    }

    public bool AnyUninspectedQuests()
    {
        for (int i = 0; i < activeQuests.Count; i++)
        {
            if (!activeQuests[i].inspected)
            {
                return true;
            }
        }
        return false;
    }

    public void ResetQuestProgress()
    {
        for (int i = 0; i < allQuests.Count; i++)
        {
            allQuests[i].turnedIn = false;
            allQuests[i].inspected = false;
            allQuests[i].tracked = false;

            for (int j = 0;  j < allQuests[i].objectiveCompleted.Length;  j++)
            {
                allQuests[i].objectiveCompleted[j] = false;
            }

            for (int j = 0; j < allQuests[i].currentGatherObjectives.Length; j++)
            {
                allQuests[i].currentGatherObjectives[j].quantity = 0;
            }

            for (int j = 0; j < allQuests[i].currentNumberOfBuilding.Length; j++)
            {
                allQuests[i].currentNumberOfBuilding[j] = 0;
            }

        }
    }

}

#if UNITY_EDITOR
[CustomEditor(typeof(QuestManager))]
public class QuestManagerEditor : Editor
{

    QuestManager _target;

    public override void OnInspectorGUI()
    {
        _target = (QuestManager)target;

        DrawDefaultInspector();

        GUILayout.Space(20);
        if (GUILayout.Button("Reset All Quest Progrss"))
        {
            _target.ResetQuestProgress();
        }
    }

}
#endif