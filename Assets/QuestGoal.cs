using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class QuestGoal
{
    public string goalDescription; // Tavoitteen kuvaus
    public int requiredAmount; // Kuinka monta tavoitetta pitää saavuttaa
    public int currentAmount; // Nykyinen määrä
    public string itemToCollect;
    public string enemyToKill;
    public GoalType goalType; // Tavoitetyyppi (esim. tappaminen, kerääminen)

    public bool IsGoalCompleted()
    {
     
        return currentAmount >= requiredAmount;
    }
}

public enum GoalType
{
    Kill, // Tapa vihollisia
    Collect, // Kerää esineitä
    TalkToNPC // Puhu NPC:n kanssa
}
