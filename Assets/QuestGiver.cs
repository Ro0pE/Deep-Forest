using UnityEngine;

public class QuestGiver : MonoBehaviour
{
    public QuestManager questManager;

    private void OnMouseDown()
    {
        Debug.Log("Time for a quest");
        questManager.ShowQuest();

        if (questManager.questCompleted == true)
        {
            Debug.Log("ota palkinto!");
            questManager.resetQuest();
        } else
        {
            Debug.Log("Time for a quest");
            questManager.ShowQuest();
        }
    }
}
