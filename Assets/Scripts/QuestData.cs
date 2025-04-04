using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ItemReward
{
    public string itemName;
    public int quantity;

    public ItemReward(string itemName, int quantity)
    {
        this.itemName = itemName;
        this.quantity = quantity;
    }
}

[CreateAssetMenu(fileName = "NewQuestData", menuName = "Quests/Quest Data")]
public class QuestData : ScriptableObject
{
    public string npcID;
    public string questID;
    public string title;
    public string description;
    public string completeText;
    public bool isCompleted;
    public int experienceReward;
    public int goldReward;
    public List<ItemReward> itemRewards; // Lista itemeistä, jotka sisältävät nimen ja määrän
    public List<QuestGoal> goals;
    public bool isReadyForCompletion;
    public string preQuestID;

    public Quest ToQuest()
    {
        return new Quest
        {
            npcID = this.npcID,
            questID = this.questID,
            title = this.title,
            description = this.description,
            completeText = this.completeText,
            isCompleted = this.isCompleted,
            experienceReward = this.experienceReward,
            goldReward = this.goldReward,
            itemRewards = new List<ItemReward>(this.itemRewards), // Siirretään myös itemRewards lista
            goals = new List<QuestGoal>(this.goals),
            isReadyForCompletion = this.isReadyForCompletion,
            preQuestID = this.preQuestID
        };
    }
}

