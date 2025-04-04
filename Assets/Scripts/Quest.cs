using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Quest
{
    public string questID;
    public string npcID; // NPC:n tunniste
    public string title;
    public string description;
    public string completeText;
    public bool isCompleted;
    public List<QuestGoal> goals;
    public int experienceReward;
    public int goldReward;
    public List<ItemReward> itemRewards; // Muutetaan tästä lista ItemReward-olioista
    public bool isReadyForCompletion;
    public string preQuestID;

    public Quest(QuestData data)
    {
        npcID = data.npcID; // Siirretään NPC ID
        isCompleted = data.isCompleted;
        questID = data.questID;
        title = data.title;
        description = data.description;
        completeText = data.completeText;
        goals = new List<QuestGoal>(data.goals);
        experienceReward = data.experienceReward;
        goldReward = data.goldReward;

        // Muutetaan itemRewardNames -> itemRewards
        itemRewards = new List<ItemReward>();
        foreach (var itemReward in data.itemRewards)
        {
            itemRewards.Add(new ItemReward(itemReward.itemName, itemReward.quantity)); // Siirretään ItemReward objektit
        }

        isReadyForCompletion = data.isReadyForCompletion;
        preQuestID = data.preQuestID;
    }

    public Quest() { }
}
