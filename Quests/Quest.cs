using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "New Quest", order = 9)]
public class Quest : ScriptableObject
{
    public enum QuestType
    {
        Main, Side
    }

    public enum QuestObjective
    {
        Gather, Craft, Cook, Grow, Build, GoTo, Interact
    }

    public QuestType questType;
    public QuestObjective[] questObjective;
    public int currentQuestObjective;

    public string QuestName;
    
    [Multiline] public string QuestDescription;

    public bool turnedIn;

    public bool completed
    {
        get
        {
            for (int i = 0; i < objectiveCompleted.Length; i++)
            {
                if (!objectiveCompleted[i])
                {
                    return false;
                }
            }
            return true;
        }
    }

    public Quest[] questRequirements;

    public bool available
    {
        get
        {
            for (int i = 0; i < questRequirements.Length; i++)
            {
                if (!questRequirements[i].turnedIn)
                {
                    return false;   
                }
            }
            return true;
        }
    }

    public bool[] objectiveCompleted;

    [Header("Gather")]
    public ResourceData[] gatherObjectives;
    public ResourceData[] currentGatherObjectives;

    [Header("Build")]
    public BuildingType[] buildingType;
    public int[] numberOfBuilding;
    public int[] currentNumberOfBuilding;

    [Header("GoTo")]
    public Transform[] goToObjectives;

    [Header("Interact")]
    public IInteractableObject[] interactableObjectives;

    public bool tracked;

    public bool inspected;
}
