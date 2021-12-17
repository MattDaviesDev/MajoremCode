using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class sQuestInterface : MonoBehaviour
{

    public GameObject interfaceObjects;

    GameObject currentQuestLog;

    [Header("Quest Logs")]
    [SerializeField] GameObject activeQuestLog;
    [SerializeField] GameObject completedQuestLog;

    [SerializeField] RectTransform activeQuestLogParent;
    [SerializeField] RectTransform completedQuestLogParent;
    [SerializeField] RectTransform questDescParent;
    [SerializeField] GameObject prefab;

    Quest trackedQuest;
    Quest selectedQuest;
    [SerializeField] TextMeshProUGUI selectedQuestName;
    [SerializeField] TextMeshProUGUI selectedQuestDesc;
    [SerializeField] GameObject mainQuestIcon;
    [SerializeField] TextMeshProUGUI questLogName;

    // Start is called before the first frame update
    void Start()
    {
        currentQuestLog = activeQuestLog;
        questLogName.text = "Active quests";
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ChangeQuestLog()        
    {
        currentQuestLog.SetActive(false);
        if (currentQuestLog == activeQuestLog)
        {
            currentQuestLog = completedQuestLog;
            questLogName.text = "Completed quests";
        }
        else
        {
            currentQuestLog = activeQuestLog;
            questLogName.text = "Active quests";
        }
        LayoutRebuilder.ForceRebuildLayoutImmediate(activeQuestLogParent);
        LayoutRebuilder.ForceRebuildLayoutImmediate(completedQuestLogParent);
        currentQuestLog.SetActive(true);

        
    }

    public void CreateQuestLogs()
    {
        CleanOutLogs();
        selectedQuestName.text = "No selected quest";
        selectedQuestDesc.text = "Click on a quest to select it.<br><br>Double click a quest to track it.";
        mainQuestIcon.SetActive(false);
        CreateActiveQuestLog(QuestManager.instance.activeQuests);
        CreateCompletedQuestLog(QuestManager.instance.allQuests);
    }

    public void CreateActiveQuestLog(List<Quest> activeQuests)
    {
        for (int i = 0; i < activeQuests.Count; i++)
        {
            GameObject temp = Instantiate(prefab, activeQuestLogParent);
            temp.GetComponent<sQuestLogItem>().InitMenuItem(activeQuests[i]);
            temp.SetActive(true);
        }
    }

    void CleanOutLogs()
    {
        for (int i = activeQuestLogParent.childCount - 1; i > 0; i--)
        {
            Destroy(activeQuestLogParent.GetChild(i).gameObject);
        }

        for (int i = completedQuestLogParent.childCount - 1; i >= 0; i--)
        {
            Destroy(completedQuestLogParent.GetChild(i).gameObject);
        }
    }

    public void CreateCompletedQuestLog(List<Quest> allQuests)
    {
        for (int i = 0; i < allQuests.Count; i++)
        {
            if (allQuests[i].turnedIn)
            {
                GameObject temp = Instantiate(prefab, completedQuestLogParent);
                temp.GetComponent<sQuestLogItem>().InitMenuItem(allQuests[i]);
                temp.SetActive(true);
            }
        }
    }

    public void ShowQuestInfo(Quest quest)
    {
        if (quest == selectedQuest && trackedQuest != selectedQuest)
        {
            TrackQuest();
        }
        mainQuestIcon.SetActive(quest.questType == Quest.QuestType.Main);
        quest.inspected = true;
        selectedQuest = quest;
        selectedQuestName.text = quest.QuestName;
        selectedQuestDesc.text = quest.QuestDescription;
        LayoutRebuilder.ForceRebuildLayoutImmediate(questDescParent);
    }

    public void TrackQuest()
    {
        if (trackedQuest != null)
        {
            trackedQuest.tracked = false;
        }
        selectedQuest.tracked = true;   
        trackedQuest = selectedQuest;
        sMainUI.instance.questTracker.TrackNewQuest(trackedQuest);
    }
}
