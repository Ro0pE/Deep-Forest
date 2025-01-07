using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QuestGiver : MonoBehaviour
{
    public GameObject questInProgress;
    public GameObject questCompleted;
    public GameObject questHereMark;
    public Transform markPanel;
    public Camera playerCamera; // Pelaajan kamera
    public Vector3 nameOffset = new Vector3(0, 2f, 0); // Offset NPC:n yläpuolelle

    public QuestData questToGive; // Quest-tiedot (asetetaan Inspectorissa)
    private QuestManager questManager; // Viittaus QuestManageriin
    public PlayerStats playerStats;
    public Inventory playerInventory;
    private bool questAccepted = false;

    // Uudet UI-elementit
    public GameObject questWindow; // Quest-ikkuna
    public TextMeshProUGUI questTitleText; // Questin otsikko
    public TextMeshProUGUI questDescriptionText; // Questin kuvaus
    public Image questRewardItem1; // Palkintoesine 1
    public Image questRewardItem2; // Palkintoesine 2
    public TextMeshProUGUI questRewardExp; // Palkintokokemus
    public TextMeshProUGUI questRewardGold; // Palkintoraha
    public Button acceptButton; // Hyväksy-nappi
    public Button rejectButton; // Hylkää-nappi

    void Start()
    {
        questManager = FindObjectOfType<QuestManager>(); // Etsi QuestManager
        playerStats = FindObjectOfType<PlayerStats>();
        playerInventory = FindObjectOfType<Inventory>();
        questInProgress.SetActive(false);
        questCompleted.SetActive(false);
        questHereMark.SetActive(true);

        // Piilotetaan quest-ikkuna aluksi
        questWindow.SetActive(false);
    }

    void Update()
    {
        // Päivitetään quest-merkintöjen sijainti ja suunta pelaajaa kohti
        if (questHereMark != null && playerCamera != null)
        {
            Vector3 worldPosition = transform.position + nameOffset;
            markPanel.position = worldPosition;
            markPanel.rotation = Quaternion.LookRotation(markPanel.position - playerCamera.transform.position);
            markPanel.Rotate(0, 180, 0);
        }

        // Päivitä merkit questin tilan mukaan
        if (questAccepted && IsQuestCompleted())
        {
            questInProgress.SetActive(false);
            questCompleted.SetActive(true);
        }
    }

    void OnMouseDown()
    {
        if (!questAccepted && questToGive != null)
        {
            ShowQuestWindow(); // Avaa quest-ikkuna
        }
        else if (questAccepted && IsQuestCompleted())
        {
            CompleteQuest(); // Suorita quest, jos se on tehty
        }
    }

    private void ShowQuestWindow()
    {
        questWindow.SetActive(true); // Näytetään quest-ikkuna

        // Näytetään questin tiedot
        questTitleText.text = questToGive.title;
        questDescriptionText.text = questToGive.description;

        Item item1 = FindObjectOfType<ItemDatabase>().GetItemByName(questToGive.itemRewardNames.Count > 0 ? questToGive.itemRewardNames[0] : null);
        Item item2 = FindObjectOfType<ItemDatabase>().GetItemByName(questToGive.itemRewardNames.Count > 1 ? questToGive.itemRewardNames[1] : null);

        // Näytetään esineiden kuvat (sprite) UI:ssa
        questRewardItem1.sprite = item1 != null ? item1.icon : null;
        questRewardItem2.sprite = item2 != null ? item2.icon : null;

        questRewardExp.text = questToGive.experienceReward > 0 ? questToGive.experienceReward.ToString() : "None";
        questRewardGold.text = questToGive.goldReward > 0 ? questToGive.goldReward.ToString() : "None";

        // Asetetaan napit toimimaan
        acceptButton.onClick.AddListener(AcceptQuest);
        rejectButton.onClick.AddListener(RejectQuest);
    }

    private void AcceptQuest()
    {
        if (questManager != null && questToGive != null)
        {
            // Muunna QuestData Quest-objektiksi
            Quest questInstance = questToGive.ToQuest();
            questManager.AddQuest(questInstance);
            questAccepted = true;
            questWindow.SetActive(false); // Piilotetaan quest-ikkuna
            questInProgress.SetActive(true);
            Debug.Log($"Quest '{questToGive.title}' accepted!");
        }
    }

    private void RejectQuest()
    {
        questWindow.SetActive(false); // Piilotetaan quest-ikkuna
        Debug.Log("Quest rejected.");
    }

    private void CompleteQuest()
    {
        if (questManager != null)
        {
            // Muunna QuestData Quest-objektiksi
            Quest questInstance = new Quest(questToGive); // Käytämme aiemmin lisättyä konstruktoria
            
            // Merkitse quest suoritetuksi
            questManager.CompleteQuest(questInstance);
            questCompleted.SetActive(true);
            questAccepted = false;
            GiveRewardsToPlayer(questInstance);
            Debug.Log($"Quest '{questToGive.title}' completed!");
        }
    }

    private bool IsQuestCompleted()
    {
        if (questManager != null)
        {
            bool isQuestInCompleted = questManager.completedQuests.Exists(q => q.questID == questToGive.questID && q.goals.TrueForAll(goal => goal.IsGoalCompleted()));
            return isQuestInCompleted; // Quest löytyy suoritetuista questista ja kaikki tavoitteet on suoritettu
        }
        return false;
    }

    // Metodi, joka antaa pelaajalle kaikki palkinnot
    private void GiveRewardsToPlayer(Quest quest)
    {
        // Palkintokokemus
        if (quest.experienceReward > 0)
        {
            playerStats.AddExperience(quest.experienceReward);
            Debug.Log($"Gave {quest.experienceReward} XP to player.");
        }

        // Jos questilla on varusteita tai esineitä palkintoina
        foreach (string itemName in quest.itemRewards)
        {
            // Hae item ItemDatabase:sta sen nimellä
            Item itemToGive = FindObjectOfType<ItemDatabase>().GetItemByName(itemName);
            
            // Lisää item pelaajan inventaarioon
            if (itemToGive != null)
            {
                playerInventory.AddItem(itemToGive); // Esimerkiksi annetaan 1 kappale
                Debug.Log($"Gave item {itemToGive.itemName} to player.");
            }
            else
            {
                Debug.LogError($"Item {itemName} not found in ItemDatabase.");
            }
        }

        // Jos questilla on rahapalkinto
        if (quest.goldReward > 0)
        {
            playerInventory.AddGold(quest.goldReward);
            Debug.Log($"Gave {quest.goldReward} gold to player.");
        }
    }
}
