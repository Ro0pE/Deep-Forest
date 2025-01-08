using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewQuestData", menuName = "Quests/Quest Data")]
public class QuestData : ScriptableObject
{
    public string questID;
    public string title;
    public string description;
    public string completeText;
    public int experienceReward;
    public int goldReward;
    public List<string> itemRewardNames; // Lista itemien nimistä
    public List<QuestGoal> goals;
    public bool isReadyForCompletion; // Uusi tila

    // Luo Quest-objekti QuestData:sta
    public Quest ToQuest()
    {
        return new Quest
        {
            questID = this.questID,
            title = this.title,
            description = this.description,
            completeText = this.completeText,
            experienceReward = this.experienceReward,
            goldReward = this.goldReward,
            itemRewards = new List<string>(this.itemRewardNames), // Tässä myös oikea nimi
            goals = new List<QuestGoal>(this.goals),
            isReadyForCompletion = this.isReadyForCompletion
        };
    }
}
