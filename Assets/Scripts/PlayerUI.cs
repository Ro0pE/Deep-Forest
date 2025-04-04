using UnityEngine;
using UnityEngine.UI; // Tai TextMeshPro-komponentti
using TMPro;

public class PlayerUI : MonoBehaviour
{
    public PlayerStats playerStats;
    public TextMeshProUGUI expText;
    public Image experience;
    public Image expBorder;
    public GameObject skillTreeUI; // UI-elementti skill tree:lle

    private void Start()
    {

        skillTreeUI.SetActive(false);
        float expAmount = (float)playerStats.currentExperience / playerStats.experienceToNextLevel;
        experience.fillAmount = expAmount;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            // Tarkistaa, onko paneli tällä hetkellä aktiivinen ja kääntää sen tilan
            skillTreeUI.SetActive(!skillTreeUI.activeSelf);
        }
        // Päivitä UI näyttämään pelaajan taso ja kokemuspisteet
        float expAmount = (float)playerStats.currentExperience / playerStats.experienceToNextLevel;
        experience.fillAmount = expAmount;
        expText.text = "XP: " + playerStats.currentExperience + "/" + playerStats.experienceToNextLevel;
    }
}
