using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    public List<Quest> activeQuests; // Pelaajan aktiiviset questit
    public List<Quest> completedQuests; // Suoritetut questit

    public void AddQuest(Quest newQuest)
    {
        if (!activeQuests.Contains(newQuest))
        {
            activeQuests.Add(newQuest);
        }
    }

    public void CompleteQuest(Quest quest)
    {
        if (activeQuests.Contains(quest))
        {
            activeQuests.Remove(quest);
            completedQuests.Add(quest);
            Debug.Log($"Quest {quest.title} completed!");
        }
    }

    public void UpdateQuestProgress(string questID, GoalType goalType, int amount)
    {
        // Muutetaan foreach -> for-silmukaksi
        for (int i = 0; i < activeQuests.Count; i++)
        {
            Quest quest = activeQuests[i];
            if (quest.questID == questID)
            {
                for (int j = 0; j < quest.goals.Count; j++)
                {
                    QuestGoal goal = quest.goals[j];
                    if (goal.goalType == goalType)
                    {
                        goal.currentAmount += amount;
                        if (goal.IsGoalCompleted())
                        {
                            Debug.Log($"Goal completed for quest: {quest.title}");
                        }
                    }
                }

                // Tarkista, onko kaikki tavoitteet suoritettu
                if (quest.goals.TrueForAll(goal => goal.IsGoalCompleted()))
                {
                    CompleteQuest(quest);
                }
            }
        }
    }
    public void OnEnemyKill(string enemyName)
    {
        // Käydään läpi kaikki aktiiviset questit
        foreach (var quest in activeQuests)
        {
            // Käydään läpi kaikki questin tavoitteet
            foreach (var goal in quest.goals)
            {
                // Tarkistetaan, että tavoite on "Kill" ja että vihollinen on karhu
                if (goal.goalType == GoalType.Kill && enemyName == "Bear") // Voit vaihtaa tähän tarkemman nimen
                {
                    goal.currentAmount += 1; // Lisätään tappoja
                    Debug.Log($"Karhut tapettu: {goal.currentAmount}/{goal.requiredAmount}");

                    // Tarkista, onko tavoite saavutettu
                    if (goal.IsGoalCompleted())
                    {
                        Debug.Log($"Tavoite saavutettu questille: {quest.title}");
                    }

                    // Päivitetään questin edistyminen
                    UpdateQuestProgress(quest.questID, GoalType.Kill, 1); // Päivitetään progress
                }
            }
        }
    }


}
