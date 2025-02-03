using UnityEngine;
using TMPro;

public class QuestUI : MonoBehaviour
{
    public GameObject questPrefab; // Prefab, joka sisältää nimen ja progressin
    public Transform questListContainer; // Kontaineri, johon quest-prefabit lisätään
    public GameObject activeQuestText;
    private QuestManager questManager; // Viittaus QuestManageriin

    void Start()
    {
        questManager = FindObjectOfType<QuestManager>(); // Hae QuestManager pelistä
        UpdateQuestList(); // Päivitä quest-lista heti alussa
    }

    void Update()
    {
        if (questManager.activeQuests.Count > 0)
        {
            activeQuestText.SetActive(true);
            // Päivitä quest-lista, jos on aktiivisia questeja
            UpdateQuestList();
        }
        else
        {
            activeQuestText.SetActive(false);
        }
    }

    private void UpdateQuestList()
    {
        // Tyhjennetään vanhat prefab-instanssit kontainerista
        foreach (Transform child in questListContainer)
        {
            Destroy(child.gameObject);
        }

        // Luodaan uudet prefab-instanssit aktiivisille questeille
        foreach (var quest in questManager.activeQuests)
        {
            GameObject questInstance = Instantiate(questPrefab, questListContainer);

            // Hae prefabista tekstikentät
            TextMeshProUGUI titleText = questInstance.transform.Find("QuestNameText").GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI progressText = questInstance.transform.Find("QuestText").GetComponent<TextMeshProUGUI>();

            // Päivitä questin nimi
            titleText.text = quest.title;

            // Päivitä questin progress
            string progressInfo = "";
            if (quest.isReadyForCompletion)
            {
                progressInfo = "Complete!";
            }
            else
            {
                foreach (var goal in quest.goals)
                {
                    if (goal.goalType == GoalType.Collect)
                    {
                        progressInfo += $"Collect {goal.itemToCollect}: {goal.currentAmount}/{goal.requiredAmount}\n";
                    }
                    else if (goal.goalType == GoalType.Kill)
                    {
                        progressInfo += $"Kill {goal.enemyToKill}: {goal.currentAmount}/{goal.requiredAmount}\n";
                    }
                    else
                    {
                        progressInfo += $"Progress: {goal.currentAmount}/{goal.requiredAmount}\n";
                    }
                }
            }
            progressText.text = progressInfo;
        }
    }
}
