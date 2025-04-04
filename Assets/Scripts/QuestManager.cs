using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class QuestManager : MonoBehaviour
{
    public List<Quest> activeQuests; // Pelaajan aktiiviset questit
    public List<Quest> completedQuests; // Suoritetut questit
    public Inventory inventory; // Viittaus pelaajan inventaariin

public void AddQuest(Quest newQuest)
{
    if (newQuest == null)
    {
        Debug.LogError("Quest is null!");
        return;
    }

    if (!activeQuests.Contains(newQuest))
    {
        activeQuests.Add(newQuest);
        Debug.Log($"Quest {newQuest.title} lisätty aktiivisiin questteihin!");

        // Tarkista inventaario uusille keräysquesteille
        foreach (var goal in newQuest.goals)
        {
            if (goal.goalType == GoalType.Collect)
            {
                if (string.IsNullOrEmpty(goal.itemToCollect))
                {
                    Debug.LogWarning($"Item to collect is missing for goal in quest {newQuest.title}");
                    continue;
                }

                if (inventory == null)
                {
                    Debug.LogError("Inventory is null!");
                    return;
                }

                Item itemInstance = ScriptableObject.CreateInstance<Item>();
                itemInstance.itemName = goal.itemToCollect;
                int itemCount = inventory.GetItemCount(itemInstance);

                Debug.Log($"Item count for {goal.itemToCollect}: {itemCount}");
                goal.currentAmount += itemCount;
                Debug.Log($"Inventaarion tarkistus: {goal.itemToCollect} määrä päivitetty questille {newQuest.title}: {goal.currentAmount}/{goal.requiredAmount}");
            }
        }
    }
}
        public bool IsQuestCompleted(string questID)
        {
            return completedQuests.Any(q => q.questID == questID);
        }


    public void MarkQuestAsReadyForCompletion(Quest quest)
    {
        Debug.Log("Marking quest rdy for completion" + quest.isCompleted);
        if (activeQuests.Contains(quest) && !quest.isCompleted)
        {
            quest.isReadyForCompletion = true;
            Debug.Log($"Quest {quest.title} is now ready for completion!");
        }
        else
        {
            Debug.Log("Markquest ei muuta isreadya oikein");
        }
    }

    public void CompleteQuest(Quest quest)
    {
        Debug.Log("Quest name  " + quest.title);
        Debug.Log(quest.isReadyForCompletion);
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

    public void UpdateKillQuestProgress(string questID, GoalType goalType, int amount)
    {
        
        foreach (var quest in activeQuests)
        {
            if (quest.questID == questID)
            {
                foreach (var goal in quest.goals)
                {
                    if (goal.goalType == goalType)
                    {
                        Debug.Log("kill update run");
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
                    quest.isReadyForCompletion = true; // Merkitään valmiiksi suoritettavaksi
                    Debug.Log($"Quest {quest.title} is ready for completion!");
                }
            }
        }
    }
    public void UpdateCollectQuestProgress(Item item)
    {
        Debug.Log("Quest manager metodi!");
    foreach (var quest in activeQuests)
    {
        foreach (var goal in quest.goals)
        {
            // Tarkista, onko tavoite sama kuin kerätty esine
            if (goal.goalType == GoalType.Collect && goal.itemToCollect == item.itemName)
            {
                int currentCount = inventory.GetItemCount(item); // Hae pelaajan inventaarion esinemäärä
                goal.currentAmount = currentCount; // Päivitä tavoitteen nykyinen määrä
                Debug.Log("Item COUNT: " + goal.currentAmount);

                Debug.Log($"Quest {quest.title}, goal updated: {goal.itemToCollect} {goal.currentAmount}/{goal.requiredAmount}");

                // Tarkista, onko tavoite suoritettu
                if (goal.IsGoalCompleted())
                {
                    Debug.Log($"Goal completed for quest: {quest.title}, item: {item.itemName}");
                }
                else
                {
                    quest.isReadyForCompletion = false;
                }
            }
        }

        // Tarkista, onko kaikki tavoitteet suoritettu
        if (quest.goals.TrueForAll(goal => goal.IsGoalCompleted()))
        {
            quest.isReadyForCompletion = true; // Merkitään valmiiksi suoritettavaksi
            Debug.Log($"Quest {quest.title} is ready for completion!");
        }
    }
}


  /*  public void OnEnemyKill(string enemyName)
    {
        foreach (var quest in activeQuests)
        {
            foreach (var goal in quest.goals)
            {
                if (goal.goalType == GoalType.Kill && enemyName == goal.enemyToKill)
                {
                    goal.currentAmount += 1;
                    Debug.Log($"{enemyName} tapettu: {goal.currentAmount}/{goal.requiredAmount}");

                    if (goal.IsGoalCompleted())
                    {
                        Debug.Log($"Tavoite saavutettu questille: {quest.title}");
                    }

                    UpdateKillQuestProgress(quest.questID, GoalType.Kill, 1);
                }
            }
        }
    }

    public void OnItemCollected(Item item, int currentCount)
    {
        foreach (var quest in activeQuests)
        {
            foreach (var goal in quest.goals)
            {
                if (goal.goalType == GoalType.Collect && goal.itemToCollect == item.itemName)
                {
                    goal.currentAmount = currentCount;
                    if (goal.currentAmount >= goal.requiredAmount)
                    {
                        quest.isCompleted = true; // Merkkaa questin tavoite suoritetuksi
                    }
                }
            }
        }
    }*/

}
