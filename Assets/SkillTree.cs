using UnityEngine;
using UnityEngine.UI; // UI-komponentit kuten Button ja Image

public class SkillTree : MonoBehaviour
{
    public SkillDatabase skillDatabase;  // SkillDatabase ScriptableObject
    public GameObject skillButtonPrefab; // Prefabi, joka sisältää UI-elementit

    public Transform meleeParent;  // Vanhempi, johon painikkeet lisätään
    public Transform rangedParent;  // Vanhempi, johon painikkeet lisätään
    public Transform defenceParent;  // Vanhempi, johon painikkeet lisätään
    public Transform fireParent;  // Vanhempi, johon painikkeet lisätään
    public Transform waterParent;  // Vanhempi, johon painikkeet lisätään
    public Transform earthParent;  // Vanhempi, johon painikkeet lisätään
    public Transform windParent;  // Vanhempi, johon painikkeet lisätään
    public Transform holyParent;  // Vanhempi, johon painikkeet lisätään
    public Transform shadowParent;  // Vanhempi, johon painikkeet lisätään

    public GameObject skillTreeUI; // UI-elementti skill tree:lle
    public Button activateMelee;
    public Button activateRanged;
    public Button activateDefence;
    public Button activateFire;
    public Button activateWater;
    public Button activateEarth;
    public Button activateWind;
    public Button activateHoly;
    public Button activateShadow;


    public GameObject meleePanel;
    public GameObject rangedPanel;
    public GameObject defencePanel;
    public GameObject firePanel;
    public GameObject waterPanel;
    public GameObject earthPanel;
    public GameObject windPanel;
    public GameObject holyPanel;
    public GameObject shadowPanel;    

    private Button currentButton; // Referenssi nykyiseen painikkeeseen
    private Color normalColor = Color.white; // Normaaliväri
    private Color clickedColor = Color.red; // Väri, johon napin väri muuttuu

    private ActionbarPanel actionbarPanel; // Viite ActionbarPaneliin
    public PlayerStats playerStats;
    public Button closeSkillTreeButton;

    void Start()
    {
        meleePanel.SetActive(true);
        rangedPanel.SetActive(false);
        defencePanel.SetActive(false);
        firePanel.SetActive(false);
        waterPanel.SetActive(false);
        windPanel.SetActive(false);
        earthPanel.SetActive(false);
        holyPanel.SetActive(false);
        shadowPanel.SetActive(false);

        

        actionbarPanel = FindObjectOfType<ActionbarPanel>();
        playerStats = FindObjectOfType<PlayerStats>();

        // Täytetään UI kaikilla taidoilla, jotka on määritelty SkillDatabaseen
        PopulateSkillTree();
    }
    

    // Täyttää skill tree:n kaikilla taidoilla
    void PopulateSkillTree()
    {
        foreach (Skill skill in skillDatabase.GetSkillList())
        {
            skill.Initialize(playerStats);
            Transform parent = null;

        // Määritä parent skillin tyypin mukaan
        switch (skill.element)
        {
            case Element.Melee:
                parent = meleeParent;
                break;
            case Element.Ranged:
                parent = rangedParent;
                break;
            case Element.Defense:
                parent = defenceParent;
                break;

            case Element.Fire:
                parent = fireParent;
                break;

            case Element.Water:
                parent = waterParent;
                break;

            case Element.Earth:
                parent = earthParent;
                break;

            case Element.Wind:
                parent = windParent;
                break;

            case Element.Holy:
                parent = holyParent;
                break;

            case Element.Shadow:
                parent = shadowParent;
                break;                                                

            default:
                Debug.LogWarning($"Skill '{skill.skillName}' has an unknown type.");
                continue; // Hypätään tämän skillin yli, jos tyyppi on tuntematon
        }
            // Luo uusi skillin painike UI:hen
            GameObject skillButton = Instantiate(skillButtonPrefab, parent);

            // Täytetään painike elementeillä
            SkillButton skillButtonComponent = skillButton.GetComponent<SkillButton>();
            skillButtonComponent.Setup(skill); // Annetaan skill tiedot

            // Lisätään klikkauskuuntelija painikkeelle
            Button button = skillButton.GetComponent<Button>();
            button.onClick.AddListener(() => OnSkillButtonClick(button));
            Transform levelUpTransform = skillButton.transform.Find("LevelUpButton");
            Transform learnTransfrom = skillButton.transform.Find("LearnButton");
        if (levelUpTransform == null)
        {
            Debug.LogError("LevelUpButton not found in SkillButton prefab!");
        }
        else
        {
            Button levelUpButton = levelUpTransform.GetComponent<Button>();
            Button learnButton = learnTransfrom.GetComponent<Button>();
            if (levelUpButton == null)
            {
                Debug.LogError("Button component missing on LevelUpButton!");
            }
            levelUpButton.onClick.AddListener(() => LevelUpSkill(skill, parent));
            learnButton.onClick.AddListener(() => LearnSkill(skill, parent));
        }
            
        }
    }

    // Skillin painikkeen klikkaus
    void OnSkillButtonClick(Button button)
    {
        // Jos nykyinen painike ei ole sama kuin painettu painike
        if (currentButton != button)
        {
            // Palautetaan nykyisen painikkeen väri normaaliksi
            if (currentButton != null)
            {
                SetButtonColor(currentButton, normalColor);
            }

            // Asetetaan uuden painikkeen väri punaiseksi
            SetButtonColor(button, clickedColor);

            // Päivitetään nykyinen painike
            currentButton = button;
        }
    }

    // Asettaa napin värin
    void SetButtonColor(Button button, Color color)
    {
        ColorBlock colors = button.colors;
        colors.normalColor = color;
        colors.selectedColor = color;
        colors.highlightedColor = color;
        colors.pressedColor = color;
        button.colors = colors;
    }

    // Funktiot paneelien avaamiseen
    public void OpenMelee()
    {
        // Suljetaan kaikki muut paneelit
        CloseAllPanels();

        // Avaa vain meleePanel ja päivittää napit
        meleePanel.SetActive(true);
        ResetOtherButtons(activateMelee);
    }
    public void OpenRanged()
    {
        // Suljetaan kaikki muut paneelit
        CloseAllPanels();

        // Avaa vain meleePanel ja päivittää napit
        rangedPanel.SetActive(true);
        ResetOtherButtons(activateRanged);
    }
    public void OpenDefence()
    {
        // Suljetaan kaikki muut paneelit
        CloseAllPanels();

        // Avaa vain combatSpellPanel ja päivittää napit
        defencePanel.SetActive(true);
        ResetOtherButtons(activateDefence);
    }

    public void OpenFire()
    {
        // Suljetaan kaikki muut paneelit
        CloseAllPanels();

        // Avaa vain passivePanel ja päivittää napit
        firePanel.SetActive(true);
        ResetOtherButtons(activateFire);
    }

    public void OpenWater()
    {
        // Suljetaan kaikki muut paneelit
        CloseAllPanels();

        // Avaa vain defencePanel ja päivittää napit
        waterPanel.SetActive(true);
        ResetOtherButtons(activateWater);
    }

    public void OpenEearth()
    {
        // Suljetaan kaikki muut paneelit
        CloseAllPanels();

        // Avaa vain defenceSpellPanel ja päivittää napit
        earthPanel.SetActive(true);
        ResetOtherButtons(activateEarth);
    }

    public void OpenWind()
    {
        // Suljetaan kaikki muut paneelit
        CloseAllPanels();

        // Avaa vain defenceSpellPanel ja päivittää napit
        windPanel.SetActive(true);
        ResetOtherButtons(activateWind);
    }        
    public void OpenHoly()
    {
        // Suljetaan kaikki muut paneelit
        CloseAllPanels();

        // Avaa vain defenceSpellPanel ja päivittää napit
        holyPanel.SetActive(true);
        ResetOtherButtons(activateHoly);
    }
    public void OpenShadow()
    {
        // Suljetaan kaikki muut paneelit
        CloseAllPanels();

        // Avaa vain defenceSpellPanel ja päivittää napit
        shadowPanel.SetActive(true);
        ResetOtherButtons(activateShadow);
    }
    // Sulkee kaikki paneelit
    void CloseAllPanels()
    {
        meleePanel.SetActive(false);
        rangedPanel.SetActive(false);
        defencePanel.SetActive(false);
        firePanel.SetActive(false);
        waterPanel.SetActive(false);
        windPanel.SetActive(false);
        earthPanel.SetActive(false);
        holyPanel.SetActive(false);
        shadowPanel.SetActive(false);
    }

    // Resetoi muiden napit takaisin normaaliksi
    void ResetOtherButtons(Button activeButton)
    {
        Button[] buttons = { activateMelee, activateRanged, activateDefence, activateFire, activateWater, activateEarth, activateWind, activateHoly, activateShadow };
        foreach (Button button in buttons)
        {
            if (button != activeButton)
            {
                SetButtonColor(button, normalColor);
            }
        }
        SetButtonColor(activeButton, clickedColor); // Asetetaan vain aktivoituneelle napille punainen väri
    }

    public void LevelUpSkill(Skill skill, Transform parent)
    {
        if (skill.isLearned)
        {
        Debug.Log("Skill level up");
        if (skill.skillLevel < skill.skillMaxLevel)
        {
            if(playerStats.skillPoints > 0)
            {

            skill.Levelup(playerStats); // Nostetaan tasoa
            playerStats.RemoveSkillPoint();
            UpdateSkillUI(skill, parent); // Päivitetään UI
            playerStats.UpdateSkillPoints();


            }
            else
            {
                Debug.Log("No skillpoints left;");
            }
        }
        } 
        else
        {
            Debug.Log("skill is rly not learned dude!");
        }
    }
    public void LearnSkill(Skill skill, Transform parent) {

        if (!skill.isLearned)
        {
        if (playerStats.skillPoints > 0)
        {
            skill.LearnSkill(playerStats);
            playerStats.RemoveSkillPoint();
            UpdateSkillUI(skill,parent);
            playerStats.UpdateSkillPoints();
        }

        }
        else
        {
            Debug.Log("Skill is already learnt");
            return;
       
         }
    }

    // Päivitetään skillin UI
    public void UpdateSkillUI(Skill skill, Transform parent)
    {
       
        // Haetaan skillin painike UI:sta ja päivitetään sen elementit
        foreach (Transform button in parent)
        {
            SkillButton skillButtonComponent = button.GetComponent<SkillButton>();
            if (skillButtonComponent != null && skillButtonComponent.skill == skill)
            {
                 
                skillButtonComponent.UpdateUI(); // Päivitetään UI tämän skillin mukaan
            }
        }
    }
    public void Close()
    {
        skillTreeUI.SetActive(false);

    }
}
