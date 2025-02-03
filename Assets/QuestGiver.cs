using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.Collections.Generic;
using TMPro;

public class QuestGiver : MonoBehaviour
{
    public GameObject questInProgress;
    public GameObject questCompleted;
    public GameObject questHereMark;
    public Transform markPanel;
    public Camera playerCamera; // Pelaajan kamera
    public Vector3 nameOffset = new Vector3(0, 2f, 0); // Offset NPC:n yläpuolelle

    public List<QuestData> questToGive; // Useampi questi
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
    public Image completeReward1; // Palkintoesine 1
    public Image completeReward2; // Palkintoesine 2
    public TextMeshProUGUI questRewardExp; // Palkintokokemus
    public TextMeshProUGUI questRewardGold; // Palkintoraha
    public Button acceptButton; // Hyväksy-nappi
    public Button rejectButton; // Hylkää-nappi

    public GameObject questCompletedWindow;
    public TextMeshProUGUI questCompleteTitleText; // Questin otsikko
    public TextMeshProUGUI questCompletedText;
    public Button collectReward;
    public GameObject availableQuests;
    public GameObject availableQuestPrefab;
    public GameObject questLists;
    private List<QuestData> availableQuestsToShow;

    public int questIndex = 0;

    void Start()
    {
        questManager = FindObjectOfType<QuestManager>(); // Etsi QuestManager
        playerStats = FindObjectOfType<PlayerStats>();
        playerInventory = FindObjectOfType<Inventory>();
        questInProgress.SetActive(false);
        questCompleted.SetActive(false);
        questHereMark.SetActive(true);
        questLists.SetActive(false);

        // Piilotetaan quest-ikkuna aluksi
        questWindow.SetActive(false);
        questCompletedWindow.SetActive(false);
    }

void Update()
{
    // Päivitetään quest-merkintöjen sijainti ja suunta pelaajaa kohti
    if (playerCamera != null)
    {
        Vector3 worldPosition = transform.position + nameOffset;
        markPanel.position = worldPosition;
        markPanel.rotation = Quaternion.LookRotation(markPanel.position - playerCamera.transform.position);
        markPanel.Rotate(0, 180, 0);
    }

    // Päivitä merkit questin tilan mukaan
    if (questAccepted && IsQuestCompleted())
    {
        questCompleted.SetActive(true);
    }

    // Tarkistetaan questgiverin questien tila suhteessa pelaajaan
    bool hasInProgressQuests = questToGive.Any(quest =>
        questManager.activeQuests.Any(activeQuest => 
            activeQuest.questID == quest.questID && !activeQuest.isReadyForCompletion
        )
    );

    bool allQuestsReadyForCompletion = questToGive.All(quest =>
        questManager.activeQuests.Any(activeQuest =>
            activeQuest.questID == quest.questID && activeQuest.isReadyForCompletion
        )
    );

    bool allQuestsReturned = questToGive.All(quest =>
        questManager.completedQuests.Any(completedQuest => 
            completedQuest.questID == quest.questID
        )
    );

    // Päivitä questInProgress tila
    if (allQuestsReturned || (!hasInProgressQuests && !allQuestsReadyForCompletion))
    {
        questInProgress.SetActive(false); // Ei aktiivisia tai kaikki palautettu
    }
    else
    {
        questInProgress.SetActive(true); // On aktiivisia, keskeneräisiä questeja
    }

    // Tarkistetaan, onko saatavilla uusia questeja
    bool hasAvailableQuests = questToGive.Any(quest =>
        !questManager.activeQuests.Any(activeQuest => activeQuest.questID == quest.questID) &&
        !questManager.completedQuests.Any(completedQuest => completedQuest.questID == quest.questID)
    );

    if (hasAvailableQuests)
    {
        questHereMark.SetActive(true); // Näytä merkki uusista questeista
    }
    else
    {
        questHereMark.SetActive(false); // Piilota, jos uusia questeja ei ole
    }
}





    private void ShowQuestCompletedPanel()
    {
        Item item1 = FindObjectOfType<ItemDatabase>().GetItemByName(questToGive[questIndex].itemRewardNames.Count > 0 ? questToGive[questIndex].itemRewardNames[0] : null);
        Item item2 = FindObjectOfType<ItemDatabase>().GetItemByName(questToGive[questIndex].itemRewardNames.Count > 1 ? questToGive[questIndex].itemRewardNames[1] : null);

        // Näytetään esineiden kuvat (sprite) UI:ssa
        completeReward1.sprite = item1 != null ? item1.icon : null;
        completeReward2.sprite = item2 != null ? item2.icon : null;

        questCompletedWindow.SetActive(true); // Näytä paneeli
        questCompleteTitleText.text = questToGive[questIndex].title;
        questCompletedText.text = questToGive[questIndex].completeText;
        Debug.Log($"Quest '{questToGive[questIndex].title}' is already completed.");
    }

    void OnMouseDown()
    {
        if (IsQuestCompleted()) // Tarkista, onko quest jo suoritettu
        {
            collectReward.onClick.AddListener(CollectRewards);
            ShowQuestCompletedPanel(); // Näytä suoritetun questin paneeli
        }
        else if (questToGive[questIndex] != null)
        {
            ShowAvailableQuestsWindow();
            //ShowQuestWindow(); // Näytä quest-ikkuna, jos quest ei ole hyväksytty
        }
    }
    public void CollectRewards()
    {
        Debug.Log("Complete quest!");
        if (questManager != null && questToGive[questIndex] != null)
        {
            Quest questInstance = questManager.activeQuests.Find(q => q.questID == questToGive[questIndex].questID);
            if (questInstance != null && questInstance.isReadyForCompletion)
            {
                questManager.CompleteQuest(questInstance); // Siirretään quest completed-listalle
                GiveRewardsToPlayer(questInstance); // Annetaan palkinnot
            }
            else
            {
                Debug.Log("Quest is not ready for completion yet.");
            }
        }

        questCompletedWindow.SetActive(false);
        questInProgress.SetActive(false);
        questCompleted.SetActive(false);
    }
    private void ShowAvailableQuestsWindow()
    {
        // Tyhjennetään vanhat napit
        foreach (Transform child in availableQuests.transform)
        {
            Destroy(child.gameObject);
        }

        // Suodatetaan questit, joita ei ole aktiivisissa eikä suoritetuissa questeissa
        availableQuestsToShow = questToGive
            .Where(quest => !questManager.activeQuests.Any(activeQuest => activeQuest.questID == quest.questID)
                        && !questManager.completedQuests.Any(completedQuest => completedQuest.questID == quest.questID))
            .ToList();

        // Tarkistetaan, onko jäljellä questtejä, jotka voi antaa
        if (availableQuestsToShow.Count == 0)
        {
            Debug.Log("Ei ole enää questtejä annettavana.");
            return; // Ei näytetä quest-listaa, jos ei ole saatavilla questtejä
        }

        // Luodaan napit jäljellä oleville questeille
        for (int i = 0; i < availableQuestsToShow.Count; i++)
        {
            QuestData quest = availableQuestsToShow[i];

            // Luodaan uusi nappi prefabista
            GameObject questButton = Instantiate(availableQuestPrefab, availableQuests.transform);

            // Muutetaan napin teksti vastaamaan questin otsikkoa
            TextMeshProUGUI buttonText = questButton.GetComponentInChildren<TextMeshProUGUI>();
            if (buttonText != null)
            {
                buttonText.text = quest.title;
            }

            // Varmistetaan, että Button-komponentti on olemassa ja lisätään kuuntelija
            Button button = questButton.GetComponent<Button>();
            if (button != null)
            {
                int currentIndex = questToGive.IndexOf(quest); // Haetaan alkuperäinen indeksi
                button.onClick.AddListener(() =>
                {
                    Debug.Log($"Clicked quest button: {quest.title}");
                    questIndex = currentIndex;
                    ShowQuestWindow();
                });
            }
            else
            {
                Debug.LogError("Button component missing on quest prefab!");
            }
        }

        // Näytetään lista
        questLists.SetActive(true);
    }

    private void ShowQuestWindow()
    {
        questLists.SetActive(false);
        Debug.Log("Show toimii");
        questWindow.SetActive(true); // Näytetään quest-ikkuna

        // Näytetään questin tiedot
        questTitleText.text = questToGive[questIndex].title;
        questDescriptionText.text = questToGive[questIndex].description;

        Item item1 = FindObjectOfType<ItemDatabase>().GetItemByName(questToGive[questIndex].itemRewardNames.Count > 0 ? questToGive[questIndex].itemRewardNames[0] : null);
        Item item2 = FindObjectOfType<ItemDatabase>().GetItemByName(questToGive[questIndex].itemRewardNames.Count > 1 ? questToGive[questIndex].itemRewardNames[1] : null);

        // Näytetään esineiden kuvat (sprite) UI:ssa
        questRewardItem1.sprite = item1 != null ? item1.icon : null;
        questRewardItem2.sprite = item2 != null ? item2.icon : null;

        questRewardExp.text = questToGive[questIndex].experienceReward > 0 ? questToGive[questIndex].experienceReward.ToString() : "-";
        questRewardGold.text = questToGive[questIndex].goldReward > 0 ? questToGive[questIndex].goldReward.ToString() : "-";

        // Asetetaan napit toimimaan
        acceptButton.onClick.AddListener(AcceptQuest);
        rejectButton.onClick.AddListener(RejectQuest);
    }

private void AcceptQuest()
{
    if (questManager != null && questToGive[questIndex] != null)
    {
        // Muunna QuestData Quest-objektiksi
        Quest questInstance = questToGive[questIndex].ToQuest();

        // Tarkista, onko quest jo aktiivisissa questeissä
        bool alreadyActive = questManager.activeQuests.Any(q => q.questID == questInstance.questID);

        if (!alreadyActive)
        {
            questManager.AddQuest(questInstance);
            questAccepted = true;
            questWindow.SetActive(false); // Piilotetaan quest-ikkuna
            questInProgress.SetActive(true);
            questHereMark.SetActive(false);
            Debug.Log($"Quest '{questToGive[questIndex].title}' accepted!");
        }
        else
        {
            Debug.LogWarning($"Quest '{questToGive[questIndex].title}' is already active and won't be added again.");
        }
    }
}


    private void RejectQuest()
    {
        questWindow.SetActive(false); // Piilotetaan quest-ikkuna
        Debug.Log("Quest rejected.");
    }

    private void CompleteQuest()
    {
        Debug.Log("Lisätäään comp listaan");
        if (questManager != null)
        {
            Debug.Log("manager ei null");
            // Muunna QuestData Quest-objektiksi
            Quest questInstance = new Quest(questToGive[questIndex]); // Käytämme aiemmin lisättyä konstruktoria
            
            // Merkitse quest suoritetuksi
            questManager.CompleteQuest(questInstance);
            questCompleted.SetActive(true);
            questAccepted = false;
            GiveRewardsToPlayer(questInstance);
            Debug.Log($"Quest '{questToGive[questIndex].title}' completed!");
        }
    }

    private bool IsQuestCompleted()
    {
        if (questManager != null)
        {
            // Tarkista kaikki NPC:llä olevat questit
            foreach (var quest in questToGive)
            {
                // Etsi, onko quest aktiivinen ja valmis suoritettavaksi
                Quest activeQuest = questManager.activeQuests.Find(q => q.questID == quest.questID);
                if (activeQuest != null && activeQuest.isReadyForCompletion)
                {
                    // Päivitä indeksi tähän questiin
                    questIndex = questToGive.IndexOf(quest);
                    return true; // Ainakin yksi quest on valmis suoritettavaksi
                }
            }
        }
        return false; // Ei valmiita questeja
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
