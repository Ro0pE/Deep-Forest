using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.AI;
using System.Collections;

public class PlayerStats : MonoBehaviour
{
    public int totalWeaponDamage;
    public int weaponAttack = 0;
    public float aspd;
    public int totalDex;
    public int totalAgi;
    public int totalStr;
    public int totalInt;
    public int totalVit;
    public int strength = 0;
    public int vitality = 0;
    public int intellect = 0;
    public int agility = 0;
    public int dexterity = 0;
    public int baseAgi = 1;
    public int baseStr = 1;
    public int baseInt = 1;
    public int baseVit = 1;
    public int baseDex = 1;
    public int baseRangedRage = 30;
    public int attack = 0;
    public int magickAttack = 0;
    public int hit = 0;
    public int hpRegen = 0;
    public int spRegen = 0;
    public int tempHP;
    public int tempSP;
    public float crit;
    public float dodge;
    public float def;
    public float castSpeed;

    public int buffAgi = 0;
    public int buffDex = 0;
    public int buffStr = 0;
    public int buffInt = 0;
    public int buffVit = 0;
    public int buffHP = 0;
    public int buffSP = 0;
    public int buffAttack;

    public int itemStr = 0;
    public int itemAgi = 0;
    public int itemInt = 0;
    public int itemVit = 0;
    public int itemDex = 0;
    public int itemHpRegValue = 0;
    public int itemSpRegValue = 0;
    public float itemCritValue = 0;
    public int itemHitValue = 0;
    public float itemDodgeValue = 0;
    public float itemCastSpeedValue = 0;
    public float itemDefValue = 0;
    public int itemWeaponDamage = 0;
    
    public int skillAgi = 0;
    public int skillDex = 0;
    public int skillVit = 0;
    public int skillInt = 0;
    public int skillStr = 0;
    public int skillHpReg = 0;
    public int skillSpReg = 0;
    public float skillCrit = 0;
    public float skillDodge = 0;
    public float skillCastSpeed = 0;
    public int skillHitValue = 0;
    public int skillDef = 0;
    public int skillWeaponDamage = 0;
    public int skillRangedRange = 0;

 
   
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
    public Button strengthMinus;
    public Button vitalityMinus;
    public Button intellectMinus;
    public Button agilityMinus;
    public Button dexterityMinus;
    public Button saveStatsButton;

    public int statusPoints;
    public int skillPoints;
    public int skillPointsSpend;
    public int level = 1;
    public int currentExperience = 0;
    public int experienceToNextLevel = 100;
    public PlayerHealth playerHealth;
    public PlayerAttack playerAttack;
    public GameObject characterStats;
    public PlayerUI playerUI;
    public PlayerHealthBar playerHealthBar;
    public SkillDatabase skillDatabase;
    public Button closeStatsButton;
    public AudioSource audioSource;
    public AudioClip levelUpSound;
    public HunterSkillTree hunterSkillTree;

    void Start()
    {   
       
        audioSource = gameObject.AddComponent<AudioSource>();
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

                strengthMinus.gameObject.SetActive(true);
                vitalityMinus.gameObject.SetActive(true);
                intellectMinus.gameObject.SetActive(true);
                agilityMinus.gameObject.SetActive(true);
                dexterityMinus.gameObject.SetActive(true);

                saveStatsButton.gameObject.SetActive(true);
                UpdateStatButtons();
        }
                
            }
        }
    }

    public void UpdateStatTexts()
    {
        strengthText.text = $"{totalStr}   ( {strength} + <color=green>{baseStr + skillStr + buffStr + itemStr} </color> )";
        vitalityText.text = $"{totalVit}   ( {vitality}  + <color=green>{baseVit + skillVit + buffVit + itemVit}</color> )";
        intellectText.text = $"{totalInt}   ( {intellect}  + <color=green>{baseInt + skillInt + buffInt + itemInt}</color> )";
        agilityText.text = $"{totalAgi}   ( {agility}  + <color=green>{baseAgi + skillAgi + buffAgi + itemAgi}</color> )";
        dexterityText.text = $"{totalDex}   ( {dexterity} + <color=green>{baseDex + skillDex + buffDex + itemDex} </color> )"; // total dex, basedex ja skilldex ei voi vähentää, dexterity on itemi ja statpoint dex ja buffdex on buffeista
        statpointsText.text = $"{statusPoints}";
        attackText.text = $" {weaponAttack + attack} (" + $"{weaponAttack} + " + $"{attack})";
        attackSpeedText.text = $" {playerAttack.attackSpeed}";
        magickAttackText.text = $" {magickAttack}";
        hpRegenText.text = hpRegen.ToString("F0") + " 5s";
        spRegenText.text = spRegen.ToString("F0") + " 5s";
        critText.text = playerAttack.critChance.ToString("F1") + "%";
        dodgeText.text = playerHealth.dodgeChance.ToString("F1") +"%";
        defText.text =  $" {playerHealth.defence}";
        hitText.text = $" {playerAttack.hitRating}";
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
 
        strengthMinus.interactable = strength > 0;
        vitalityMinus.interactable = vitality > 0;
        intellectMinus.interactable = intellect > 0;
        agilityMinus.interactable = agility > 0;
        dexterityMinus.interactable = dexterity > 0;
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
        }
        statusPoints--;
        UpdateStatButtons();
        UpdateStatTexts();
        UpdateStats();
       

    }

    public void UpdateStats()
    {
        totalDex = baseDex + dexterity + skillDex + buffDex +itemDex;
        totalAgi = baseAgi + agility + skillAgi + buffAgi + itemAgi;
        totalStr = baseStr + strength + skillStr + buffStr + itemStr;
        totalVit = baseVit + vitality + skillVit + buffVit + itemVit; 
        totalInt = baseInt + intellect + skillInt + buffInt + itemInt; 
        attack = (totalStr * 2) + (totalDex / 2) + buffAttack;
        playerAttack.attackDamage = attack + weaponAttack;
        aspd = ((totalAgi) / 200f);
        tempHP = (totalVit * 13);  
        def = (totalVit / 6) + itemDefValue + skillDef;
        playerHealth.defence = def;
        playerHealth.maxHealth = tempHP + playerHealth.baseHealth; 
        magickAttack = (totalInt * 2);
        tempSP = (totalInt * 8);
        playerHealth.maxMana = tempSP + playerHealth.baseMana;
        playerAttack.magickAttackDamage = magickAttack;
        dodge = (totalAgi / 8f);
        crit = (totalAgi / 8f);
        playerHealth.dodgeChance = dodge + itemDodgeValue + skillDodge;
        playerAttack.critChance = crit + itemCritValue + skillCrit;
        castSpeed = (totalDex / 25f) + itemCastSpeedValue + itemCastSpeedValue;
        hpRegen = Mathf.RoundToInt(itemHpRegValue + skillHpReg + buffHP + totalVit + (Mathf.Pow(vitality / 10, 2) * 2));
        spRegen = Mathf.RoundToInt(itemSpRegValue + skillSpReg + buffSP + totalInt + (Mathf.Pow(intellect / 10, 2) * 2));
        hit = totalDex + itemHitValue + skillHitValue;  
        playerHealth.healthRegen = hpRegen;
        playerHealth.manaRegen = spRegen;
        playerAttack.attackSpeedReduction = aspd;
        //playerAttack.rangedAttackRange = skillRangedRange;
        

        totalWeaponDamage = playerAttack.attackDamage;
        if (castSpeed > 2f) // huom cast speed reduction!
        {    
            castSpeed = 2f;
        }
        hunterSkillTree.UpdatePlayerSkillsInfo();

    }

    public void DecreaseStat(string stat)
    {
        
        switch (stat)
        {
            case "strength":
                if (strength > 0) { strength--; statusPoints++; }

                break;
            case "vitality":
                if (vitality > 0) { vitality--; statusPoints++; }   
                break;
            case "intellect":
                if (intellect > 0) { intellect--; statusPoints++; }
                break;
            case "dexterity":
                if (dexterity > 0) { dexterity--; statusPoints++; }
                break;  
            case "agility":
                if (agility > 0) { agility--; statusPoints++; }
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
            
        }
        else
        {


        strengthPlus.gameObject.SetActive(false);
        vitalityPlus.gameObject.SetActive(false);
        intellectPlus.gameObject.SetActive(false);
        agilityPlus.gameObject.SetActive(false);
        dexterityPlus.gameObject.SetActive(false);

        strengthMinus.gameObject.SetActive(false);
        vitalityMinus.gameObject.SetActive(false);
        intellectMinus.gameObject.SetActive(false);
        agilityMinus.gameObject.SetActive(false);
        dexterityMinus.gameObject.SetActive(false);


        saveStatsButton.gameObject.SetActive(false);
    }
        }


    // Päivittää pelaajan kokemuspisteet ja tarkistaa, nouseeko taso
    public void AddExperience(int amount)
    {
        currentExperience += amount;
        

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
        

        return newPoints;
    }
    private void PlayeLevelUpSound()
    {
        if (audioSource != null && levelUpSound != null)
        {
            
            audioSource.PlayOneShot(levelUpSound);// Soittaa AudioSourceen asetetun klipin
        }
        else
        {
            Debug.LogWarning("Ääniklipin toisto epäonnistui. Varmista että AudioSource ja ääni ovat asetettu.");
        }
    }

    // Tason nousu
    public void LevelUpEffect()
    {
        Debug.Log("level up effect");
        GameObject levelUpEffectPrefab = Resources.Load<GameObject>("UI_Effects/levelUpEffect");
        
        if (levelUpEffectPrefab != null)
        {
            Debug.Log("Ei oo null");

            // Instansioidaan efekti ja asetetaan pelaajan kohdalle
            GameObject levelUpEffect = Instantiate(levelUpEffectPrefab, playerAttack.transform.position, Quaternion.identity);
            Debug.Log("Efekti sijainti ennen parentointia: " + levelUpEffect.transform.position);


            Debug.Log("Ennen parentointia: " + levelUpEffect.transform.parent);
            levelUpEffect.transform.SetParent(playerAttack.transform, false);
            Debug.Log("Jälkeen parentoinnin: " + levelUpEffect.transform.parent);
            Debug.Log("Efekti aktiivinen? " + levelUpEffect.activeSelf);
            //levelUpEffect.SetActive(true);


            // Varmistetaan, että efekti pysyy pelaajan kohdalla (jos ei käytä world spacea)
            levelUpEffect.transform.localPosition = Vector3.zero;

            // Poistetaan efekti 15 sekunnin kuluttua
            Destroy(levelUpEffect, 15f);
        }
    }

    private void LevelUp()
    {
        level++;
        currentExperience -= experienceToNextLevel; // Vähennetään kokemuspisteet
        experienceToNextLevel = Mathf.RoundToInt(experienceToNextLevel * 1.2f); // Pyöristä kokonaisluvuksi
        statusPoints++;
        statusPoints++;
        skillPoints++;

        
        PlayeLevelUpSound();
        FlashExpBar(); // Kutsutaan välähdysanimaatio
        LevelUpEffect();
        // Tarkista, nouseeko taso useita kertoja
        while (currentExperience >= experienceToNextLevel)
        {
            currentExperience -= experienceToNextLevel;
            level++;
            skillPoints++;
            statusPoints++;
            statusPoints++;
            experienceToNextLevel = Mathf.RoundToInt(experienceToNextLevel * 1.2f);
            PlayeLevelUpSound();
            FlashExpBar(); // Kutsutaan välähdysanimaatio
            LevelUpEffect();
        }
        
        playerHealth.baseHealth = Mathf.RoundToInt((playerHealth.baseHealth * 1.1f)); 
        playerHealth.baseMana = Mathf.RoundToInt((playerHealth.baseMana * 1.05f));
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
    Debug.Log("väläytellään expaa");
    Image fillImage = playerUI.expBorder; 

    if (fillImage == null)
    {
        Debug.Log("expBorder on null!");
        yield break;
    }

    Color originalColor = fillImage.color;
    Color flashColor = Color.yellow;

    for (int i = 0; i < 3; i++)
    {
        fillImage.color = flashColor;
        yield return new WaitForSeconds(0.4f);
        fillImage.color = originalColor;
        yield return new WaitForSeconds(0.4f);
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
            itemDex += item.dexValue;
            itemHpRegValue += item.hpRegValue;
            itemSpRegValue += item.spRegValue;
            itemCritValue += item.critValue;
            itemDodgeValue += item.dodgeValue;
            itemCastSpeedValue += item.castSpeedValue;
            itemDefValue += item.defValue;
            itemHitValue += item.hitValue;

            weaponAttack += item.damageValue;
           /* strength += item.strValue;
            agility += item.agiValue;
            intellect += item.intValue;
            dexterity += item.dexValue;
            vitality += item.vitValue; // tämä tekeee jotain*/
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
            itemHpRegValue -= item.hpRegValue;
            itemSpRegValue -= item.spRegValue;
            itemCritValue -= item.critValue;
            itemDodgeValue -= item.dodgeValue;
            itemCastSpeedValue -= item.castSpeedValue;
            itemDefValue -= item.defValue;   
            itemHitValue -= item.hitValue;
            itemWeaponDamage -= item.damageValue; 

            weaponAttack -=item.damageValue;
           /* dexterity -= item.dexValue;
            strength -= item.strValue; // Vähennetään varusteen antama strength
            agility -= item.agiValue;  // Vähennetään varusteen antama agility
            intellect -= item.intValue; // Vähennetään varusteen antama intellect
            vitality -= item.vitValue;*/
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
    public void AddBuffStatHP()
    {
        
        UpdateStatTexts();
        UpdateStats();
    }
    public void AddBuffStatSP()
    {
      
        UpdateStatTexts();
        UpdateStats();
    }
    public void AddBuffStatAgi()
    {
        buffAgi = buffAgi;
        UpdateStatTexts();
        UpdateStats();
    }
    public void AddBuffStatDex()
    {
        buffDex = buffDex;
        skillDex = skillDex;
       
        UpdateStatTexts();
        UpdateStats();
    }
    public void RemoveBuffStatAgi()
    {
        buffAgi = buffAgi;
        UpdateStatTexts();
        UpdateStats();
    }
    public void RemoveBuffStatDex()
    {
        buffDex = buffDex;
        skillDex = skillDex;
        UpdateStatTexts();
        UpdateStats();
    }
    public void RemoveBuffStatHP()
    {
        
        //hpRegen = (hpRegen - buffHP);
        UpdateStatTexts();
        UpdateStats();
    }
    public void RemoveBuffStatSP()
    {
        
        //spRegen = (spRegen - buffSP);
        UpdateStatTexts();
        UpdateStats();
    }        
    public void RemoveSkillPoint(int points)
    {
        skillPoints -=  points;
    }
    public void AddSkillPoint(int points)
    {
        skillPoints +=  points;
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
