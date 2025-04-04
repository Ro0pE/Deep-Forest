using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using System.Linq; 



public class HunterSkillTree : MonoBehaviour
{
    public SkillDatabase skillDatabase;  // Viittaa SkillDatabase ScriptableObjectiin
    public GameObject skillButtonPrefab; // Prefabi, joka sisältää UI-elementit
    public Transform tier1;  // Vanhempi, johon painikkeet lisätään (Cost 1)
    public Transform tier2;  // Vanhempi, johon painikkeet lisätään (Cost 2)
    public Transform tier3;  // Vanhempi, johon painikkeet lisätään (Cost 3)
    public Transform tier4;  // Vanhempi, johon painikkeet lisätään (Cost 5)
    public Transform tier5; // Vanhempi, johon painikkeet lisätään (Cost 14)
    public Transform tier6;  // Vanhempi, johon painikkeet lisätään (Cost 5)
    public Transform tier7; // Vanhempi, johon painikkeet lisätään (Cost 14)

    public TextMeshProUGUI skillPointsLeft; // Skillin nimi
    public PlayerStats playerStats;
    public PlayerAttack playerAttack;
    private ActionbarPanel actionbarPanel;
    public GameObject skillTreeUI; // UI-elementti skill tree:lle
    public List<Skill> filteredSkills;

    void Start()
    {
        actionbarPanel = FindObjectOfType<ActionbarPanel>();
        playerStats = FindObjectOfType<PlayerStats>();
        playerAttack = FindObjectOfType<PlayerAttack>();
        skillPointsLeft.text = "Skill points left " + playerStats.skillPoints.ToString();

        // Täytetään UI vain Hunter-tyyppisillä skilleillä
        PopulateSkillTree(SkillSchool.Hunter);
        UpdateAvailableSkills();
    }
    void Update()
    {
        skillPointsLeft.text = "Skill points left " + playerStats.skillPoints.ToString();
    }

public void UpdatePlayerSkillsInfo()
{

    foreach (var skill in filteredSkills)
    {
        if (skill != null)
        {
            

            // Tallennetaan alkuperäinen teksti VAIN JOS sitä ei ole jo tallennettu
            if (string.IsNullOrEmpty(skill.updatedInfoText))
            {
                skill.updatedInfoText = skill.infoText;
            }

            // Päivitetään infoText uudella tiedolla
            skill.updatedInfoText = skill.infoText
                .Replace("skillLevel", skill.skillLevel.ToString())
                .Replace("ATK", $"<color=#FF0000>{skill.damage}</color>"); // Punainen



            //Debug.Log("Updated infoText: " +skill.skillName +" " + skill.updatedInfoText);
        }
    }
}


void PopulateSkillTree(SkillSchool skillSchool)
{
    List<Skill> allSkills = skillDatabase.GetInitializedSkillList();
    Dictionary<string, GameObject> skillButtons = new Dictionary<string, GameObject>(); // Tallennetaan painikkeet
    
    // ✅ Tyhjennetään listat ennen uutta täyttöä, jotta skillejä ei kloonata
    filteredSkills.Clear();
    skillButtons.Clear();

    foreach (Skill skill in allSkills)
    {
        skill.Initialize(playerStats, playerAttack);
        if (skill.skillSchool == skillSchool)
        {
            filteredSkills.Add(skill);
        }
    }

    foreach (Skill skill in filteredSkills)
    {
        Transform parent = GetParentTransformBasedOnCost(skill);
        GameObject skillButton = Instantiate(skillButtonPrefab, parent);
        SkillButton skillButtonComponent = skillButton.GetComponent<SkillButton>();
        skillButtonComponent.Setup(skill);

        Button button = skillButton.GetComponent<Button>();
        button.onClick.AddListener(() => OnSkillButtonClick(button));

        Transform learnTransform = skillButton.transform.Find("LearnButton");
        Transform increaseTransform = skillButton.transform.Find("IncreaseLevel");
        Transform decreaseTransform = skillButton.transform.Find("DecreaseLevel");

        if (learnTransform == null || increaseTransform == null || decreaseTransform == null)
        {
            Debug.LogError($"SkillButtonista puuttuu napit: {skill.skillName}");
            continue;
        }

       // Button learnButton = learnTransform.GetComponent<Button>();
        Button increaseLevelButton = increaseTransform.GetComponent<Button>();
        Button decreaseLevelButton = decreaseTransform.GetComponent<Button>();

        if (increaseLevelButton == null || decreaseLevelButton == null)
        {
            Debug.LogError($"Napilta puuttuu Button-komponentti: {skill.skillName}");
            continue;
        }

        //learnButton.onClick.AddListener(() => LearnSkill(skill, parent));
        increaseLevelButton.onClick.AddListener(() => IncreaseSkillLevel(skill, parent));
        decreaseLevelButton.onClick.AddListener(() => DecreaseSkillLevel(skill, parent));
        decreaseLevelButton.gameObject.SetActive(false);
        increaseLevelButton.gameObject.SetActive(true);

        // ✅ Tallennetaan painike dictionaryyn, jotta siihen voidaan viitata myöhemmin
        skillButtons.Add(skill.skillName, skillButton);
    }

    // ✅ Korjataan, että yhteydet menevät oikeaan UI-parentiin
    // Skill connections removed
       /* foreach (Skill skill in filteredSkills)
        {
            if (!string.IsNullOrEmpty(skill.preSkill) && skillButtons.ContainsKey(skill.preSkill))
            {
                GameObject connection = new GameObject("SkillConnection");
                connection.transform.SetParent(skillButtons[skill.skillName].transform.parent, false); // ✅ Nyt pysyy UI:n sisällä
                SkillTreeConnection connectionScript = connection.AddComponent<SkillTreeConnection>();

                // Liitetään fromSkill ja toSkill skill-objekteiksi, e
                connectionScript.fromSkill = filteredSkills.FirstOrDefault(s => s.skillName == skill.preSkill);
                connectionScript.toSkill = skill;
                connectionScript.fromSkillTransform = skillButtons[skill.preSkill].transform;
                connectionScript.toSkillTransform = skillButtons[skill.skillName].transform;

               
            }
        }*/

}



    // Määrittää oikean parentin skillin cost-arvon mukaan
    Transform GetParentTransformBasedOnCost(Skill skill)
    {
        switch (skill.skillTier)
        {
            case 1:
                return tier1;
            case 2:
                return tier2;
            case 3:
                return tier3;
            case 4:
                return tier4;
            case 5:
                return tier5;
            case 6:
                return tier6;
            case 7:
                return tier7;
            default:
                return null;
        }
    }

    // Skillin painikkeen klikkaus
    void OnSkillButtonClick(Button button)
    {
        // Tämä voisi olla lisälogiikkaa painikkeen käsittelyyn
    }
    public void DecreaseSkillLevel(Skill skill, Transform parent)
    {
        if (skill.isLearned)
        {
            skill.DecreaseLevel(playerStats);
            playerStats.AddSkillPoint(skill.skillCost);
            UpdateSkillUI(skill, parent);
            playerStats.UpdateSkillPoints();
        }
    }
    public void IncreaseSkillLevel(Skill skill, Transform parent)
    {
        bool preSkillStatus = true; // Oletetaan, että preskill vaatimuksia ei ole
        if (!skill.isLearned && preSkillStatus)
        {
            if (playerStats != null && skill != null) // Null-tarkistus
            {
                Skill preSkill = GetFilteredSkillByName(skill.preSkill);
                int requiredPreSkillLevel = skill.preSkillLevel;

                
                if (preSkill != null) // Tarkistetaan, että preSkill löytyy
                {
                    if (preSkill.skillLevel >= requiredPreSkillLevel) 
                    {
                        preSkillStatus = true; // Preskill on riittävällä tasolla
                    }
                    else
                    {
                        preSkillStatus = false; // Preskill ei ole vaaditulla tasolla
                    }
                }
                else if (!string.IsNullOrEmpty(skill.preSkill))
                {
                    Debug.LogError("Edeltävää skilliä '" + skill.preSkill + "' ei löytynyt!");
                    return; // Lopetetaan suoritus, jos vaadittu preSkill puuttuu
                }
                if (playerStats.skillPoints >= skill.skillCost) // Tarkistetaan, onko tarpeeksi skillipisteitä
                {
                    skill.LearnSkill(playerStats);
                    playerStats.RemoveSkillPoint(skill.skillCost);
                    playerStats.skillPointsSpend += skill.skillCost;
                    UpdateSkillUI(skill, parent);
                    playerStats.UpdateSkillPoints();
                    UpdateAvailableSkills();
                    
                    
                }
                else
                {
                    Debug.Log("Ei tarpeeksi skillipisteitä!");
                }
            }
            else
            {
                Debug.LogError("playerStats tai skill on null!");
            }

        }
        else
        {
            if (playerStats.skillPoints >= skill.skillCost)
            {
                if (skill.skillLevel != skill.skillMaxLevel)
                {

                skill.Levelup(playerStats);
                playerStats.RemoveSkillPoint(skill.skillCost);
                playerStats.skillPointsSpend += skill.skillCost;
                UpdateSkillUI(skill,parent);
                playerStats.UpdateSkillPoints(); 
                UpdateAvailableSkills();         
                }
                else
                {
                    Debug.Log("skill is maxed");
                }
            }

        }
    }
   /* public void LearnSkill(Skill skill, Transform parent)
    {
        // Haetaan edeltävä skilli filtered listasta


        // Varmistetaan, ettei skilli ole jo opittu ja että preSkill-ehdot täyttyvät
        if (!skill.isLearned && preSkillStatus)
        {
            if (playerStats != null && skill != null) // Null-tarkistus
            {
                if (playerStats.skillPoints >= skill.skillCost) // Tarkistetaan, onko tarpeeksi skillipisteitä
                {
                    skill.LearnSkill(playerStats);
                    playerStats.RemoveSkillPoint(skill.skillCost);
                    playerStats.skillPointsSpend += skill.skillCost;
                    UpdateSkillUI(skill, parent);
                    playerStats.UpdateSkillPoints();
                    UpdateAvailableSkills();
                }
                else
                {
                    Debug.Log("Ei tarpeeksi skillipisteitä!");
                }
            }
            else
            {
                Debug.LogError("playerStats tai skill on null!");
            }
        }
        else
        {
            Debug.Log("Preskill name: " + skill.preSkill);
            Debug.Log("Preskill level required: " + requiredPreSkillLevel);
            Debug.Log("Preskill current level: " + (preSkill != null ? preSkill.skillLevel.ToString() : "N/A"));
            Debug.Log("Skill on jo opittu tai preskill ei ole vaaditulla tasolla");
        }
    }*/
    public void UpdateAllSkillUIs()
    {
        UpdatePlayerSkillsInfo();
        // Käydään läpi kaikki tier-alueet ja päivitetään niiden UI:t
        Transform[] allTiers = { tier1, tier2, tier3, tier4, tier5 };

        foreach (Transform tier in allTiers)
        {
            // Käydään läpi kaikki skillien painikkeet tässä parentissa
            foreach (Transform button in tier)
            {
                SkillButton skillButtonComponent = button.GetComponent<SkillButton>();
                if (skillButtonComponent != null)
                {
                    skillButtonComponent.UpdateUI(); // Päivitetään skillin UI
                }
            }
        }

        // Päivitetään skill-pisteet
        skillPointsLeft.text = "Skill points left " + playerStats.skillPoints.ToString();
    }
    private void UpdateAvailableSkills()
    {
        
        foreach (Skill skill in filteredSkills) // filteredSkills = lista skilleistä, jotka on suodatettu
        {
            // Jos preSkill on tyhjä, skilli on aina saatavilla
            if (string.IsNullOrEmpty(skill.preSkill))
            {
                skill.isAvailable = true;
       
            }
            else
            {
                Skill preSkill = GetFilteredSkillByName(skill.preSkill);
                
                // Tarkistetaan, että preSkill löytyy ja sen taso täyttää vaatimukset
                if (preSkill != null && preSkill.skillLevel >= skill.preSkillLevel)
                {
                    skill.isAvailable = true; // Skilli on saatavilla, koska preSkillin taso täyttyy
                   
                }
                else
                {
                    skill.isAvailable = false; // Skilli ei ole saatavilla, koska preSkillin taso ei täyty
                }
            }
         
        }
        
        // Päivitetään UI kaikille skilleille
        UpdateAllSkillUIs();
    }





    public void UpdateSkillUI(Skill skill, Transform parent)
    {
        Debug.Log("Updating UI for skill: " + skill.skillName + " in parent: " + parent.name);

        foreach (Transform button in parent)
        {
            SkillButton skillButtonComponent = button.GetComponent<SkillButton>();
            if (skillButtonComponent != null && skillButtonComponent.skill == skill)
            {
                skillButtonComponent.UpdateUI(); // Päivitetään UI tämän skillin mukaan
            }
        }
        skillPointsLeft.text = "Skill points left " + playerStats.skillPoints.ToString();
    }

        public void Close()
    {
        skillTreeUI.SetActive(false);

    }
    public bool IsSkillLearned(string skillName)
    {
        foreach (Skill skill in filteredSkills)
        {
            if (skill.skillName == skillName)
            {
                return skill.isLearned;
            }
        }

        
        return false; // Jos skilliä ei löydy, palautetaan false
    }

    public Skill GetFilteredSkillByName(string skillName)
    {
        foreach (Skill skill in filteredSkills)
        {
            if (skill.skillName == skillName)
            {
                return skill; // Palautetaan löydetty skilli
            }
        }

        return null; // Jos skilliä ei löydy, palautetaan null
    }

    public void ResetSkills()
    {
        foreach (Skill skill in filteredSkills)
        {
            skill.ResetToDefaults();
            skill.UpdatePassiveEffects(playerStats);
        }
        UpdateAvailableSkills();
        playerStats.skillPoints += playerStats.skillPointsSpend;
        playerStats.skillPointsSpend = 0;
        
        // Päivitetään kaikki parentit, jotta myös tasot 6 ja 7 päivittyvät.
        foreach (Transform tier in new Transform[] { tier1, tier2, tier3, tier4, tier5, tier6, tier7 })
        {
            foreach (Transform button in tier)
            {
                SkillButton skillButtonComponent = button.GetComponent<SkillButton>();
                if (skillButtonComponent != null)
                {
                    skillButtonComponent.UpdateUI();
                }
            }
        }

        ActionbarPanel actionbarPanel = FindObjectOfType<ActionbarPanel>();
        if (actionbarPanel != null)
        {
            foreach (ActionbarSlot slot in actionbarPanel.actionbarSlots)
            {
                slot.ClearSlot();
            }
        }
    }


}
