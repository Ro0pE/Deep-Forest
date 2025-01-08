using UnityEngine;
using TMPro;

public class QuestUI : MonoBehaviour
{
    public GameObject questPanel;  // Quest listan tausta (panel)
    public TextMeshProUGUI questText; // Questin tiedot (esim. nimi, progress)
    private QuestManager questManager; // Viittaus QuestManageriin

    void Start()
    {
        questManager = FindObjectOfType<QuestManager>(); // Hae QuestManager pelistä
        questPanel.SetActive(false); // Piilota quest-lista aluksi
    }

    void Update()
    {
        if (questManager.activeQuests.Count > 0)
        {
            // Näytetään questin tiedot ja progress
            questPanel.SetActive(true);

            // Päivitä quest-lista
            UpdateQuestList();
        }
        else
        {
            // Piilota quest-lista, jos ei ole aktiivisia questteja
            questPanel.SetActive(false);
        }
    }

    private void UpdateQuestList()
    {
        // Päivitetään UI:n tekstikenttä questin nimellä ja edistymisellä
        string questInfo = "";
        
        foreach (var quest in questManager.activeQuests)
        {
            if (quest.isReadyForCompletion)
            {
                questInfo += $"{quest.title}:  COMPLETED!";
            }
            else
            {
            questInfo += $"{quest.title}:  ";
            questInfo += $"Progress: {GetQuestProgress(quest)}";
            }

        }
        
        questText.text = questInfo;
    }

    // Palautetaan questin edistyminen tekstinä (tavoitteet / kokonaismäärä)
    private string GetQuestProgress(Quest quest)
    {
        string progress = "";
        foreach (var goal in quest.goals)
        {
            progress += $"{goal.currentAmount}/{goal.requiredAmount} "; // Esim. "10/20"
        }
        return progress; // Palautetaan progress
    }
}
