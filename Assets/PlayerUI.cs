using UnityEngine;
using UnityEngine.UI; // Tai TextMeshPro-komponentti
using TMPro;

public class PlayerUI : MonoBehaviour
{
    public PlayerStats playerStats;
    public TextMeshProUGUI levelText; // Viittaus UI-tekstiin
    public TextMeshProUGUI expText;
    public Image experience;
    public Image expBorder;
    public GameObject skillTreeUI; // UI-elementti skill tree:lle

    private void Start()
    {

        skillTreeUI.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            // Tarkistaa, onko paneli tällä hetkellä aktiivinen ja kääntää sen tilan
            skillTreeUI.SetActive(!skillTreeUI.activeSelf);
        }
        // Päivitä UI näyttämään pelaajan taso ja kokemuspisteet
        float expAmount = playerStats.currentExperience / playerStats.experienceToNextLevel;
        experience.fillAmount = expAmount;
        levelText.text = "Level: " + playerStats.level;
        expText.text = "XP: " + playerStats.currentExperience + "/" + playerStats.experienceToNextLevel;
    }
}
