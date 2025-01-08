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

    public void MarkQuestAsReadyForCompletion(Quest quest)
    {
        if (activeQuests.Contains(quest) && !quest.isCompleted)
        {
            quest.isReadyForCompletion = true;
            Debug.Log($"Quest {quest.title} is ready for completion!");
        }
    }

    public void CompleteQuest(Quest quest)
    {
        if (activeQuests.Contains(quest) && quest.isReadyForCompletion)
        {
            activeQuests.Remove(quest);
            completedQuests.Add(quest);
            quest.isCompleted = true;
            Debug.Log($"Quest {quest.title} completed and moved to completed quests!");
        }
        else
        {
            Debug.Log($"Quest {quest.title} is not ready for completion yet.");
        }
    }


    public void UpdateQuestProgress(string questID, GoalType goalType, int amount)
    {
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
                    quest.isReadyForCompletion = true; // Merkitään valmiiksi kerättäväksi
                    Debug.Log($"Quest {quest.title} is ready for completion!");
                }
            }
        }
    }


    public void OnEnemyKill(string enemyName)
    {
        foreach (var quest in activeQuests)
        {
            foreach (var goal in quest.goals)
            {
                if (goal.goalType == GoalType.Kill && enemyName == "Bear")
                {
                    goal.currentAmount += 1;
                    Debug.Log($"Karhut tapettu: {goal.currentAmount}/{goal.requiredAmount}");

                    if (goal.IsGoalCompleted())
                    {
                        Debug.Log($"Tavoite saavutettu questille: {quest.title}");
                    }

                    UpdateQuestProgress(quest.questID, GoalType.Kill, 1);
                }
            }
        }
    }
}
