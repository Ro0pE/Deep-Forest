using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.Collections.Generic;
using TMPro;

public class QuestGiver : MonoBehaviour
{
    public string thisNPC_ID = "";
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
    public TextMeshProUGUI questObjectiveText; // Questin kuvaus
    public GameObject questRewardItemsContainer;
    public GameObject questRewardItemsContainerComplete;
    public ItemRewardPrefab itemRewardPrefab;
    



    public TextMeshProUGUI questRewardExp; // Palkintokokemus
    public TextMeshProUGUI questRewardGold; // Palkintoraha
    public TextMeshProUGUI questRewardExpComplete; // Palkintokokemus
    public TextMeshProUGUI questRewardGoldComplete; // Palkintoraha
    public Button acceptButton; // Hyväksy-nappi
    public Button rejectButton; // Hylkää-nappi

    public GameObject questCompletedWindow;
    public TextMeshProUGUI questCompleteTitleText; // Questin otsikko
    public TextMeshProUGUI questCompletedText;
    public Button collectReward;
    public GameObject availableQuests;
    public GameObject availableQuestPrefab;
    public GameObject questLists;
    public List<QuestData> availableQuestsToShow;
    public PlayerAttack playerAttack;
    public List<QuestData> thisNpcQuestList;

    public int questIndex = 0;

    void Start()
    {
        playerAttack = FindObjectOfType<PlayerAttack>();
        questManager = FindObjectOfType<QuestManager>(); 
        playerStats = FindObjectOfType<PlayerStats>();
        playerInventory = FindObjectOfType<Inventory>();
        questInProgress.SetActive(false);
        questCompleted.SetActive(false);
        questHereMark.SetActive(false);
        questLists.SetActive(false);
        questWindow.SetActive(false);
        questCompletedWindow.SetActive(false);

        // Päivitetään markit heti alussa
        UpdateQuestMarks();
    }

    void Update()
    {
        // Päivitetään quest-merkintöjen sijainti ja suunta pelaajaa kohti
        if (playerCamera != null)
        {
            Vector3 worldPosition = transform.position + nameOffset;
            markPanel.rotation = Quaternion.LookRotation(markPanel.position - playerCamera.transform.position);
        }

        // Päivitä markit jokaisessa framessa
        UpdateQuestMarks();
    }

void UpdateQuestMarks()
{
    bool areAllQuestsActiveOrCompleted = questToGive.All(quest => 
        questManager.activeQuests.Any(activeQuest => activeQuest.questID == quest.questID && activeQuest.npcID == this.thisNPC_ID) || 
        questManager.completedQuests.Any(completedQuest => completedQuest.questID == quest.questID && completedQuest.npcID == this.thisNPC_ID)
    );

    bool hasInProgressQuests = questToGive.Any(quest =>
        questManager.activeQuests.Any(activeQuest => 
            activeQuest.questID == quest.questID && 
            activeQuest.npcID == this.thisNPC_ID && 
            !activeQuest.isReadyForCompletion
        )
    );
    
    bool hasReadyToCompleteQuests = questToGive.Any(quest =>
        questManager.activeQuests.Any(activeQuest => 
            activeQuest.questID == quest.questID && 
            activeQuest.npcID == this.thisNPC_ID && 
            activeQuest.isReadyForCompletion
        )
    );
    
    bool allQuestsReturned = questToGive.All(quest =>
        questManager.completedQuests.Any(completedQuest => 
            completedQuest.questID == quest.questID && 
            completedQuest.npcID == this.thisNPC_ID
        )
    );

    // Päivitetään quest-merki
// Aseta questHereMarkin tila riippuen siitä, onko kaikki tehtävät aktiivisia tai suoritettuja
this.questHereMark.SetActive(!areAllQuestsActiveOrCompleted); 





    this.questInProgress.SetActive(hasInProgressQuests); 
    this.questCompleted.SetActive(hasReadyToCompleteQuests);

    if (allQuestsReturned)
    {
        Debug.Log("Kaikki questit on suoritettu.");
    }
}







private void ShowQuestCompletedPanel()
{
    // Tyhjennetään mahdolliset edelliset palkinnot
    foreach (Transform child in questRewardItemsContainerComplete.transform)
    {
        Destroy(child.gameObject); // Poistetaan vanhat esineet, jos niitä on
    }

    // Käydään läpi kaikki itemRewards ja luodaan ne dynaamisesti
    foreach (ItemReward itemReward in questToGive[questIndex].itemRewards)
    {
        // Luo uusi instanssi ItemReward-prefabista
        ItemRewardPrefab itemRewardInstance = Instantiate(itemRewardPrefab, questRewardItemsContainerComplete.transform);

        // Haetaan esine ItemDatabase:sta sen nimen perusteella
        Item item = FindObjectOfType<ItemDatabase>().GetItemByName(itemReward.itemName);
        
        if (item != null)
        {
            // Asetetaan esineen kuva
            itemRewardInstance.itemIcon.sprite = item.icon;
            itemRewardInstance.itemRewardName = item.itemName;
        }

        // Asetetaan esineen määrä
        itemRewardInstance.itemQuantityText.text = itemReward.quantity.ToString();
    }
    questRewardExpComplete.text = questToGive[questIndex].experienceReward > 0 ? questToGive[questIndex].experienceReward.ToString() : "-";
    questRewardGoldComplete.text = questToGive[questIndex].goldReward > 0 ? questToGive[questIndex].goldReward.ToString() : "-";

    // Näytetään questin täydentämistiedot
    questCompletedWindow.SetActive(true); // Näytä paneeli
    questCompleteTitleText.text = questToGive[questIndex].title;
    questCompletedText.text = questToGive[questIndex].completeText;
}


    void OnMouseDown()
    {
        Debug.Log("clicking questgiver");
        float distanceToNpc = Vector3.Distance(transform.position, playerAttack.transform.position);
        if (distanceToNpc > 30f)
        {
            Debug.Log("But you are too far");
            return;
        }
        if (IsQuestCompleted()) // Tarkista, onko quest jo suoritettu
        {
            collectReward.onClick.AddListener(CollectRewards);
            ShowQuestCompletedPanel(); // Näytä suoritetun questin paneeli
        }
        else if (questToGive[questIndex] != null)
        {
            Debug.Log("näytetään available quests");
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
                
                CompleteQuest(questInstance);

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
    Debug.Log("Metodi runaa");
    // Tyhjennetään vanhat napit
    foreach (Transform child in availableQuests.transform)
    {
        Destroy(child.gameObject);
    }

    // Suodatetaan questit, joita ei ole aktiivisissa eikä suoritetuissa questeissa
    availableQuestsToShow = questToGive
        .Where(quest => !questManager.activeQuests.Any(activeQuest => activeQuest.questID == quest.questID)
                    && !questManager.completedQuests.Any(completedQuest => completedQuest.questID == quest.questID)
                    && (string.IsNullOrEmpty(quest.preQuestID) || questManager.IsQuestCompleted(quest.preQuestID))) // Uusi ehto
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
    questObjectiveText.text = questToGive[questIndex].goals[0].goalDescription; // Quest can have multiple goals in future, fix this

    // Tyhjennetään mahdolliset edelliset palkinnot
    foreach (Transform child in questRewardItemsContainer.transform)
    {
        Destroy(child.gameObject); // Poistetaan vanhat esineet, jos niitä on
    }

    // Käydään läpi kaikki itemRewards ja luodaan ne dynaamisesti
    foreach (ItemReward itemReward in questToGive[questIndex].itemRewards)
    {
        // Luo uusi instanssi ItemReward-prefabista
        ItemRewardPrefab itemRewardInstance = Instantiate(itemRewardPrefab, questRewardItemsContainer.transform);
        

        // Aseta esineen kuva ja määrä
        Item item = FindObjectOfType<ItemDatabase>().GetItemByName(itemReward.itemName);
        if (item != null)
        {
            itemRewardInstance.itemIcon.sprite = item.icon; // Esineen kuva
             itemRewardInstance.itemRewardName = item.itemName;
        }
        itemRewardInstance.itemQuantityText.text = itemReward.quantity.ToString(); // Esineen määrä
    }

    // Näytetään kokemus ja kultapalkinnot
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

        // Tarkista, onko quest jo aktiivisissa tai valmiissa questeissä
        bool alreadyActive = questManager.activeQuests.Any(q => q.questID == questInstance.questID);
        bool alreadyCompleted = questManager.completedQuests.Any(q => q.questID == questInstance.questID);

        if (!alreadyActive && !alreadyCompleted)
        {
            questManager.AddQuest(questInstance);
            questAccepted = true;
            questWindow.SetActive(false); // Piilotetaan quest-ikkuna
            questInProgress.SetActive(true);
            Debug.Log($"Quest '{questToGive[questIndex].title}' accepted!");
        }
        else
        {
            Debug.LogWarning($"Quest '{questToGive[questIndex].title}' is already active or completed and won't be added again.");
        }
    }
    ShowAvailableQuestsWindow();
}
    public void CloseQuestList()
    {
        questLists.SetActive(false);
    }



    private void RejectQuest()
    {
        questWindow.SetActive(false); // Piilotetaan quest-ikkuna
        Debug.Log("Quest rejected.");
    }

    private void CompleteQuest(Quest currentQuest)
    {
        Debug.Log("COMPLETE QUEST RUNNING");
        if (questManager != null)
        {
            Debug.Log("manager ei null");
            // Muunna QuestData Quest-objektiksi
            Quest questInstance = new Quest(questToGive[questIndex]); // Käytämme aiemmin lisättyä konstruktoria

            foreach (var goal in questInstance.goals)
            {
            if (goal.goalType == GoalType.Collect && goal.IsGoalCompleted())
            {
                foreach (var item in playerInventory.items)
                {
                    Debug.Log($"Inventaarion item: {item.itemName}");
                }
                // Etsitään item pelaajan inventaariosta
                Item itemToRemove = playerInventory.items.Find(item => item.itemName == goal.itemToCollect);

                if (itemToRemove != null)
                {
                    // Poistetaan kerätty item pelaajalta
                    playerInventory.RemoveItem(itemToRemove, goal.requiredAmount);
                    Debug.Log($"Poistettiin {goal.requiredAmount} {goal.itemToCollect} pelaajalta.");
                }
                else
                {
                    Debug.Log($"Itemiä '{goal.itemToCollect}' ei löytynyt pelaajan inventaariosta.");
                }
            }
            }
            
            // Merkitse quest suoritetuksi
            
            questManager.MarkQuestAsReadyForCompletion(currentQuest);
            questManager.CompleteQuest(currentQuest);
            questCompleted.SetActive(true);
            questAccepted = false;
            GiveRewardsToPlayer(currentQuest);
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
    Debug.Log("GIVE REWARDS RUNNING");

    // Palkintokokemus
    if (quest.experienceReward > 0)
    {
        playerStats.AddExperience(quest.experienceReward);
        Debug.Log($"Gave {quest.experienceReward} XP to player.");
    }

    // Jos questilla on varusteita tai esineitä palkintoina
    foreach (ItemReward itemReward in quest.itemRewards)
    {
        Item itemToGive = FindObjectOfType<ItemDatabase>().GetItemByName(itemReward.itemName);
        // Hae item ItemDatabase:sta sen nimellä
        if (itemToGive != null)
        {
            Debug.Log(" ItemtoGive quantity start: " + itemToGive.quantity);
            Debug.Log("Item to give: " + itemToGive.itemName);
            Debug.Log("Item reward: " + itemReward.itemName);
            Debug.Log("Item reward quantity: " + itemReward.quantity);

            // Jos Item-luokassa on quantity-kenttä, voidaan suoraan muuttaa sitä
            itemToGive.quantity = itemReward.quantity;

            Debug.Log("Updated item quantity: " + itemToGive.quantity);
        }
        else
        {
            Debug.LogError("Item not found in database.");
        }
        if (itemToGive != null)
        {
            playerInventory.AddItem(itemToGive); // Lisää itemin niin monta kertaa kuin määrä
            
            Debug.Log($"Gave {itemReward.quantity} of item {itemToGive.itemName} to player.");
        }
        else
        {
            Debug.LogError($"Item {itemReward.itemName} not found in ItemDatabase.");
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
