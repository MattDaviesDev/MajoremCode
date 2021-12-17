using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class sQuestLogItem : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI questName;
    [SerializeField] TextMeshProUGUI questDesc;

    Quest myQuest;

    [SerializeField] GameObject notInspected;

    [SerializeField] GameObject mainQuestIcon;
    [SerializeField] GameObject completedIcon;

    public void ButtonClicked()
    {
        sMainUI.instance.questInterface.ShowQuestInfo(myQuest);
        notInspected.SetActive(false);
    }

    public void InitMenuItem(Quest questData)
    {
        myQuest = questData;
        notInspected.SetActive(!questData.inspected);
        mainQuestIcon.SetActive(myQuest.questType == Quest.QuestType.Main);
        completedIcon.SetActive(myQuest.turnedIn);
        GetComponentInChildren<Button>().onClick.AddListener(ButtonClicked);
        questName.text = myQuest.QuestName;
        questDesc.text = myQuest.QuestDescription;
    }

    public void UpdateTrackers()
    {

    }

}
