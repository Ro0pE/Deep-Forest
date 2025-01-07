using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Quest
{
    public string questID; // Uniikki tunniste
    public string title; // Questin nimi
    public string description; // Kuvaus
    public bool isCompleted; // Onko quest suoritettu
    public List<QuestGoal> goals; // Tavoitteet
    public int experienceReward; // XP-palkinto
    public int goldReward;
    public List<string> itemRewards; // Esinepalkinnot (lista itemien nimistä)

    // Konstruktori QuestData:sta
    public Quest(QuestData data)
    {
        questID = data.questID;
        title = data.title;
        description = data.description;
        goals = new List<QuestGoal>(data.goals);
        experienceReward = data.experienceReward;
        goldReward = data.goldReward;
        itemRewards = new List<string>(data.itemRewardNames); // Korjattu nimi
    }

    // Oletuskonstruktori (valinnainen)
    public Quest() { }
}
