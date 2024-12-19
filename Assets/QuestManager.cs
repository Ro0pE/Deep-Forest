using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QuestManager : MonoBehaviour
{
    public TextMeshProUGUI questDescriptionText;
    public Button acceptButton;
    public Button declineButton;
    public Button rewardButton;
    public TextMeshProUGUI rewardText;
    public GameObject questPanel;
    public GameObject questProgresPanel;
    public GameObject questRewardPanel;
    public TextMeshProUGUI questProgressText; // Viittaus UI-tekstiin
    public PlayerStats playerStats;

    private int bearsKilled = 0;
    private int bearKillCount = 0;
    private int bearsToKill = 10;
    private bool questAccepted = false;
    private int rewardAmount = 10; // Palkkio kultakolikoina
    public bool questCompleted = false;

    private void Start()
    {
        questPanel.SetActive(false); // Piilota quest-paneeli aluksi
        questRewardPanel.SetActive(false);
        questProgresPanel.SetActive(false);

        acceptButton.onClick.AddListener(AcceptQuest);
        declineButton.onClick.AddListener(DeclineQuest);
        rewardButton.onClick.AddListener(AcceptReward);
        
        rewardText.text = $"Reward: {rewardAmount} gold";
    }
    public void resetQuest()
    {
        bearsKilled = 0;
        bearKillCount = 0;
        questAccepted = false;
        questRewardPanel.SetActive(true);
        questProgresPanel.SetActive(false);
        questPanel.SetActive(false);
        questProgressText.text = "Bears killed: " + bearsKilled + "/" + bearsToKill;
    }

    public void ShowQuest()
    {
        questPanel.SetActive(true);
        //questDescriptionText.text = $"Kill {bearsToKill} bears to get {rewardAmount} gold.";
    }

    private void AcceptQuest()
    {
        questProgresPanel.SetActive(true);
        questAccepted = true;
        questPanel.SetActive(false);
        Debug.Log("Quest hyväksytty!");
    }
    private void AcceptReward()
{
    questProgresPanel.SetActive(false);
    questRewardPanel.SetActive(false);
    Debug.Log("Quest completed! GOOD JOB");
    playerStats.AddExperience(300);
}

    private void DeclineQuest()
    {
        questAccepted = false;
        questPanel.SetActive(false);
        Debug.Log("Quest hylätty.");
    }

    public void OnBearKilled()
    {
        if (bearsKilled < bearsToKill)
        {
        bearsKilled++;
        UpdateQuestProgress();

        }

        if (bearsKilled >= bearsToKill)
        {
            questProgressText.text = "Bears killed: " + bearsKilled + "/" + bearsToKill + " Complete!";
           CompleteQuest();
            // Tässä voit päivittää palkinnon tai näyttää ilmoituksen pelaajalle
        }
    }

    private void UpdateQuestProgress()
    {
        if (questProgressText != null)
        {
            questProgressText.text = "Bears killed: " + bearsKilled + "/" + bearsToKill;
        }
        else
        {
            Debug.LogWarning("Quest progress text not assigned in QuestManager.");
        }
    }

    private void CompleteQuest()
    {
        Debug.Log("Quest suoritettu! Saat " + rewardAmount + " kultakolikkoa ja hitusen expaa.");
        // Palkitse pelaaja (voit yhdistää tämän pelaajan inventaariosysteemiin)
        questAccepted = false;
        bearKillCount = 0;
        questCompleted = true;
        
    }
}
