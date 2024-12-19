using UnityEngine;

public class Bear : MonoBehaviour
{
    public QuestManager questManager;


void Start()
{
    questManager = FindObjectOfType<QuestManager>();
}
public void KillBear()
{

    if (questManager != null)
    {
        questManager.OnBearKilled();
        Debug.Log("Bear killed, quest updated"); // Vahvistus viestistä
    }
    else
    {
        Debug.LogWarning("QuestManager is not assigned!"); // Varoitus, jos questManager on tyhjä
    }
    
}
}
