using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.AI;
using System.Collections;

public class PlayerStats : MonoBehaviour
{
    public float totalWeaponDamage;
    public float weaponAttack = 0;
    public float aspd;
    public int strength = 3;
    public int vitality = 3;
    public int intellect = 3;
    public int agility = 3;
    public int dexterity = 3;
    public int spirit = 3;
    public int statBaseValue = 3;
    public float attack = 0;
    public float magickAttack = 0;
    public float hit = 0;
    public float hpRegen = 0;
    public float spRegen = 0;
    public float tempHP;
    public float tempSP;
    public float crit;
    public float dodge;
    public float def;
    public float castSpeed;

    public int buffAgi = 0;
    public int buffDex = 0;
    public int itemStr = 0;
    public int itemAgi = 0;
    public int itemInt = 0;
    public int itemVit = 0;
    public int itemDex = 0;
    public int itemSpirit = 0;
    public float itemHpRegValue = 0f;
    public float itemSpRegValue = 0f;
    public float itemCritValue = 0f;
    public float itemHitValue = 0f;
    public float itemDodgeValue = 0f;
    public float itemCastSpeedValue = 0f;
    public float itemDefValue = 0f;
    public float itemWeaponDamage = 0f;
    public float skillHpReg = 0f;
    public float skillSpReg = 0f;
    public float skillCrit = 0f;
    public float skillDodge = 0f;
    public float skillCastSpeed = 0f;
    public float skillHitValue = 0f;
    public float skillDef = 0f;
    public float skillWeaponDamage = 0f;

 
   
    public TextMeshProUGUI skillPointsLeftText;
    public TextMeshProUGUI attackText;
    public TextMeshProUGUI magickAttackText;
    public TextMeshProUGUI attackSpeedText;
    public TextMeshProUGUI hitText;
    public TextMeshProUGUI dexterityText;
    public TextMeshProUGUI hpRegenText;
    public TextMeshProUGUI spRegenText;
    public TextMeshProUGUI strengthText;
    public TextMeshProUGUI vitalityText;
    public TextMeshProUGUI intellectText;
    public TextMeshProUGUI agilityText;
    public TextMeshProUGUI spiritText;
    public TextMeshProUGUI statpointsText;
    public TextMeshProUGUI critText;
    public TextMeshProUGUI dodgeText;
    public TextMeshProUGUI defText;
    public TextMeshProUGUI castSpeedText;
    public Button strengthPlus;
    public Button vitalityPlus;
    public Button intellectPlus;
    public Button agilityPlus;
    public Button dexterityPlus;
    public Button spiritPlus;
    public Button strengthMinus;
    public Button vitalityMinus;
    public Button intellectMinus;
    public Button agilityMinus;
    public Button dexterityMinus;
    public Button spiritMinus;
    public Button saveStatsButton;

    public int statusPoints;
    public int skillPoints;
    public int level = 1;
    public float currentExperience = 0;
    public float experienceToNextLevel = 100;
    public PlayerHealth playerHealth;
    public PlayerAttack playerAttack;
    public GameObject characterStats;
    public PlayerUI playerUI;
    public PlayerHealthBar playerHealthBar;
    public SkillDatabase skillDatabase;
    public Button closeStatsButton;

    void Start()
    {   
        characterStats.SetActive(false);
        skillDatabase.ResetSkills();
        UpdateSkillPoints();
        playerHealth = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHealth>();
        playerAttack = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerAttack>();
        playerUI = FindObjectOfType<PlayerUI>();
        playerHealthBar = FindObjectOfType<PlayerHealthBar>();
        UpdateStatTexts();
        UpdateStats();
        playerHealth.currentHealth = playerHealth.maxHealth;
        playerHealth.currentMana = playerHealth.maxMana;
    }

    void Update()
    {

        UpdateStatTexts();
        

        
        if (Input.GetKeyDown(KeyCode.C))
        {
            characterStats.SetActive(!characterStats.activeSelf);
            if (characterStats.activeSelf)
            {
            if (statusPoints > 0 ){
                strengthPlus.gameObject.SetActive(true);
                vitalityPlus.gameObject.SetActive(true);
                intellectPlus.gameObject.SetActive(true);
                dexterityPlus.gameObject.SetActive(true);
                agilityPlus.gameObject.SetActive(true);
                spiritPlus.gameObject.SetActive(true);

                strengthMinus.gameObject.SetActive(true);
                vitalityMinus.gameObject.SetActive(true);
                intellectMinus.gameObject.SetActive(true);
                agilityMinus.gameObject.SetActive(true);
                dexterityMinus.gameObject.SetActive(true);
                spiritMinus.gameObject.SetActive(true);

                saveStatsButton.gameObject.SetActive(true);
                UpdateStatButtons();
        }
                
            }
        }
    }

    public void UpdateStatTexts()
    {
        strengthText.text = $"{strength}";
        vitalityText.text = $"{vitality}";
        intellectText.text = $"{intellect}";
        agilityText.text = $"{agility}";
        dexterityText.text = $"{dexterity}"; 
        spiritText.text = $"{spirit}";
        statpointsText.text = $"{statusPoints}";
        attackText.text = $"{attack + weaponAttack} (" + $"{attack} + " + $"{weaponAttack})";
        attackSpeedText.text = $"{playerAttack.attackSpeed} (-" +$"{playerAttack.attackSpeedReduction}s)";
        magickAttackText.text = $"{magickAttack}";
        hpRegenText.text = hpRegen.ToString("F1") + " 5s";
        spRegenText.text = spRegen.ToString("F1") + " 5s";
        critText.text = playerAttack.critChance.ToString("F1") + "%";
        dodgeText.text = playerHealth.dodgeChance.ToString("F1") +"%";
        defText.text =  $"{playerHealth.defence}";
        hitText.text = $"{playerAttack.hitRating}";
        castSpeedText.text = "- "+castSpeed.ToString("F2") + "s";
    }

    void UpdateStatButtons()
    {
        bool canAdd = statusPoints > 0;
        strengthPlus.interactable = canAdd;
        vitalityPlus.interactable = canAdd;
        intellectPlus.interactable = canAdd;
        agilityPlus.interactable = canAdd;
        dexterityPlus.interactable = canAdd;
        spiritPlus.interactable = canAdd;

        strengthMinus.interactable = strength > statBaseValue;
        vitalityMinus.interactable = vitality > statBaseValue;
        intellectMinus.interactable = intellect > statBaseValue;
        agilityMinus.interactable = agility > statBaseValue;
        dexterityMinus.interactable = dexterity > statBaseValue;
        spiritMinus.interactable = spirit > statBaseValue;
    }

    public void IncreaseStat(string stat)
    {
        if (statusPoints <= 0) return;

        switch (stat)
        {
            case "strength":
                strength++;

                break;
            case "vitality":
                vitality++;
 
             
                break;
            case "intellect":
                intellect++;

                break;
            case "dexterity":
                dexterity++;   

                break;
            case "agility":
                agility++;

                break;
            case "spirit":
                spirit++;

                break;
        }
        statusPoints--;
        UpdateStatButtons();
        UpdateStatTexts();
        UpdateStats();

    }

    public void UpdateStats()
    {
        attack = (strength * 2);
        playerAttack.attackDamage = attack + weaponAttack;
        aspd = (agility / 200f);
        tempHP = (vitality * 13);  
        def = (vitality / 6) + itemDefValue + skillDef;
        playerHealth.defence = def;
        playerHealth.maxHealth = tempHP + playerHealth.baseHealth; 
        magickAttack = (intellect * 2);
        tempSP = (intellect * 8);
        playerHealth.maxMana = tempSP + playerHealth.baseMana;
        playerAttack.magickAttackDamage = magickAttack;
        dodge = (agility / 8f);
        crit = (agility / 8f);
        playerHealth.dodgeChance = dodge + itemDodgeValue + skillDodge;
        playerAttack.critChance = crit + itemCritValue + skillCrit;
        castSpeed = (spirit / 25f) + itemCastSpeedValue + itemCastSpeedValue;
        hpRegen = (spirit / 8) + (level * 0.3f) + (itemHpRegValue) + (skillHpReg);
        spRegen = (spirit / 10) + (level * 0.2f) + (itemSpRegValue) + (skillSpReg);
        hit = dexterity  + itemHitValue + skillHitValue;  // 5 dex == 1 hit eli 1 % lisää osumatarkkuutta
        playerHealth.healthRegen = hpRegen;
        playerHealth.manaRegen = spRegen;
        playerAttack.attackSpeedReduction = aspd;

        totalWeaponDamage = playerAttack.attackDamage;
        if (castSpeed > 2f) // huom cast speed reduction!
        {
            Debug.Log("Cast speed mieletön");
            castSpeed = 2f;
        }

        Debug.Log("Player aspd =  " + playerAttack.weaponAttackSpeed);



    }

    public void DecreaseStat(string stat)
    {
        Debug.Log("Decrease stats  " + stat);
        switch (stat)
        {
            case "strength":
                if (strength > (statBaseValue+itemStr)) { strength--; statusPoints++; }

                break;
            case "vitality":
                if (vitality > (statBaseValue+itemVit)) { vitality--; statusPoints++; }   
                break;
            case "intellect":
                if (intellect > (statBaseValue+itemInt)) { intellect--; statusPoints++; }
                break;
            case "dexterity":
                if (dexterity > (statBaseValue+itemDex)) { dexterity--; statusPoints++; }
                break;  
            case "agility":
                if (agility > (statBaseValue+itemAgi)) { agility--; statusPoints++; }
                break;
            case "spirit":
                if (spirit > (statBaseValue+itemSpirit)) { spirit--; statusPoints++; }
                break;
        }
        UpdateStatButtons();
        UpdateStatTexts();
        UpdateStats();

    }

    public void SaveStats()
    {
        // Lukitse statukset ja piilota plus- ja miinus-napit
        if (statusPoints > 0 ){
            Debug.Log("Use all status points!");
        }
        else
        {


        strengthPlus.gameObject.SetActive(false);
        vitalityPlus.gameObject.SetActive(false);
        intellectPlus.gameObject.SetActive(false);
        agilityPlus.gameObject.SetActive(false);
        spiritPlus.gameObject.SetActive(false);
        dexterityPlus.gameObject.SetActive(false);

        strengthMinus.gameObject.SetActive(false);
        vitalityMinus.gameObject.SetActive(false);
        intellectMinus.gameObject.SetActive(false);
        agilityMinus.gameObject.SetActive(false);
        spiritMinus.gameObject.SetActive(false);
        dexterityMinus.gameObject.SetActive(false);


        saveStatsButton.gameObject.SetActive(false);
    }
        }


    // Päivittää pelaajan kokemuspisteet ja tarkistaa, nouseeko taso
    public void AddExperience(float amount)
    {
        currentExperience += amount;
        Debug.Log("Kokemuspisteet: " + currentExperience + "/" + experienceToNextLevel);

        // Tarkista, riittävätkö kokemuspisteet tason nousuun
        while (currentExperience >= experienceToNextLevel)
        {
            LevelUp();
        }
    }
    public int CalculateStatusPoints(int playerLevel)
    {
        // Määritä kerroin: joka kymmenes taso kasvattaa kerrointa yhdellä
        int multiplier = (playerLevel / 10) + 1;
        
        // Lasketaan statuspisteet kaavalla: 2 * kerroin
        int newPoints = 2 + (2 * multiplier);
        Debug.Log("Stat points coming:"  + newPoints);

        return newPoints;
    }

    // Tason nousu
    private void LevelUp()
    {
        level++;
        currentExperience -= experienceToNextLevel; // Vähennetään kokemuspisteet
        experienceToNextLevel = Mathf.RoundToInt(experienceToNextLevel * 1.2f); // Pyöristä kokonaisluvuksi
        statusPoints = statusPoints + CalculateStatusPoints(level);
        skillPoints++;
        Debug.Log("Tasosi nousi! Nykyinen taso: " + level);
        FlashExpBar(); // Kutsutaan välähdysanimaatio
        // Tarkista, nouseeko taso useita kertoja
        while (currentExperience >= experienceToNextLevel)
        {
            currentExperience -= experienceToNextLevel;
            level++;
            skillPoints++;
            experienceToNextLevel = Mathf.RoundToInt(experienceToNextLevel * 1.2f);
            Debug.Log("Tasosi nousi! Nykyinen taso: " + level);
            FlashExpBar(); // Kutsutaan välähdysanimaatio
        }
        
        playerHealth.baseHealth = (playerHealth.baseHealth * 1.2f); 
        playerHealth.baseMana = (playerHealth.baseMana * 1.2f);
        playerHealth.currentHealth = playerHealth.baseHealth;
        playerHealth.currentMana = playerHealth.baseMana;
        skillPointsLeftText.text = $"Skill points: {skillPoints}";
        // Voit myös lisätä tason nousuun liittyviä muita toimintoja, kuten pelaajan statuksien parantaminen
    }

    private void FlashExpBar()
    {
        StartCoroutine(FlashCoroutine());
    }

    private IEnumerator FlashCoroutine()
    {
        Image fillImage = playerUI.expBorder; 
        Image avatarImage = playerHealthBar.playerAvatarBackground;
        Color avatarOrg = avatarImage.color;
        Color avatarFlash = Color.yellow;
        Color originalColor = fillImage.color;
        Color flashColor = Color.yellow; // Valitse väläytyksen väri

        // Tee 3 syklistä värinvaihtoa
        for (int i = 0; i < 3; i++)
        {
            avatarImage.color = flashColor;
            fillImage.color = flashColor;
            yield return new WaitForSeconds(0.1f);
            avatarImage.color = originalColor;
            fillImage.color = originalColor;
            yield return new WaitForSeconds(0.1f);
        }
    }
    public void EquipItem(Equipment item)
    {
        if (item != null)
        {
            itemWeaponDamage += item.damageValue;
            playerAttack.weaponAttackSpeed = item.weaponAttackSpeed;
            itemStr += item.strValue; // Example: item boosts strength
            itemAgi += item.agiValue;  // Example: item boosts vitality
            itemInt += item.intValue; // Example: item boosts strength
            itemVit += item.vitValue;  // Example: item boosts vitality
            itemSpirit += item.spiritValue; // Example: item boosts strength
            itemDex += item.dexValue;
            itemHpRegValue += item.hpRegValue;
            itemSpRegValue += item.spRegValue;
            itemCritValue += item.critValue;
            itemDodgeValue += item.dodgeValue;
            itemCastSpeedValue += item.castSpeedValue;
            itemDefValue += item.defValue;
            itemHitValue += item.hitValue;

            weaponAttack += item.damageValue;
            strength += item.strValue;
            agility += item.agiValue;
            intellect += item.intValue;
            dexterity += item.dexValue;
            vitality += item.vitValue; // tämä tekeee jotain
            spirit += item.spiritValue;
            //hpRegen += item.hpRegValue;
            //spRegen += item.spRegValue;
            //crit += item.critValue;
            //dodge += item.dodgeValue;
            //def += item.defValue;
            //castSpeed += item.castSpeedValue;
              
            UpdateStats();
            UpdateStatTexts();
        }
    }

    public void RemoveItem(Equipment item)
    {
        if (item !=null)
        {
            playerAttack.weaponAttackSpeed -= item.weaponAttackSpeed;
            itemStr -= item.strValue; // Example: item boosts strength
            itemAgi -= item.agiValue;  // Example: item boosts vitality
            itemInt -= item.intValue; // Example: item boosts strength
            itemVit -= item.vitValue;  // Example: item boosts vitality
            itemDex -= item.dexValue;
            itemSpirit -= item.spiritValue; // Example: item boosts strength 
            itemHpRegValue -= item.hpRegValue;
            itemSpRegValue -= item.spRegValue;
            itemCritValue -= item.critValue;
            itemDodgeValue -= item.dodgeValue;
            itemCastSpeedValue -= item.castSpeedValue;
            itemDefValue -= item.defValue;   
            itemHitValue -= item.hitValue;
            itemWeaponDamage -= item.damageValue; 

            weaponAttack -=item.damageValue;
            dexterity -= item.dexValue;
            strength -= item.strValue; // Vähennetään varusteen antama strength
            agility -= item.agiValue;  // Vähennetään varusteen antama agility
            intellect -= item.intValue; // Vähennetään varusteen antama intellect
            vitality -= item.vitValue;
            spirit -= item.spiritValue; 
            //hpRegen -= item.hpRegValue;
            //spRegen -= item.spRegValue;
            //crit -= item.critValue;
            //dodge -= item.dodgeValue;
            //def -= item.defValue;
            //castSpeed -= item.castSpeedValue;     
                   
            UpdateStats();
            UpdateStatTexts();

        }
    }
    public void AddBuffStat()
    {
        Debug.Log("Buff stats ADded");
        agility = agility + buffAgi;
        dexterity = dexterity + buffDex;
        UpdateStatTexts();
        UpdateStats();
    }
    public void RemoveBuffStat()
    {
        agility = agility - buffAgi;
        dexterity = dexterity - buffDex;
        UpdateStatTexts();
        UpdateStats();
    }
    public void RemoveSkillPoint()
    {
        skillPoints--;
    }
    public void UpdateSkillPoints()
    {
        skillPointsLeftText.text = $"Skill points: {skillPoints}";
    }
    public void Close()
    {
        characterStats.SetActive(false);

    }
}
