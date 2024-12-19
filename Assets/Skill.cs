using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum SpellType
{
    Damage,
    Heal
    
}
public enum SkillType
{
    Melee,
    Ranged,
    Spell,
    Passive
    
}
[System.Serializable]
public class Skill
{
    public string skillName;
    public Sprite skillIcon;
    public float baseDamage; // Perusvahinko tason 1 tasolla
    public float damagePerLevel; // Kuinka paljon vahinko kasvaa per taso
    public float baseHeal;
    public float healPerLevel;
    public float cooldown;
    public float castTime;
    public float manaCost;
    public float manaCostPerLevel;
    public int skillLevel = 0;
    public int skillMaxLevel;
    public float totalHeal;
    public float totalDamage;
    public float addCrit;
    public float addDodge;
    public bool isPassive;
    public string infoText;
    public Element element;
    public SpellType spellType;
    public SkillType skillType;
    public bool isLearned = false;
    public bool canInterrup;
    private PlayerStats playerStats;
    //public PlayerAttack playerAttack;


    // Vahinko lasketaan suoraan skillLevel-muuttujasta
    void Start()
    {
        
        isLearned = false;
        //playerStats = FindObjectOfType<PlayerStats>();    

        
    }
    public void UpdateInfoText()
    {
        if (!isPassive)
        {
            if (spellType == SpellType.Damage)
            {
            float skillTotalDamage = (baseDamage + (damagePerLevel * (skillLevel - 1))) * playerStats.magickAttack;
            Debug.Log("Damage " + skillTotalDamage);
            infoText = "Deals damage " 
            + (baseDamage + (damagePerLevel * (skillLevel - 1))) 
            + " * MATK (<color=red>" + skillTotalDamage + "</color>)\n"
            + "Increase damage " + (100 * damagePerLevel) + "% per level\n"
            + "Increase manacost <color=#1C81CF>" + manaCostPerLevel + "</color> per level";
            }
            if (spellType == SpellType.Heal)
            {
            float skillTotalHeal = baseHeal + (((playerStats.magickAttack) * (skillLevel - 1)) * healPerLevel);
            Debug.Log("heal " + skillTotalHeal);
            infoText = "Restores " 
            + (baseHeal + (((playerStats.magickAttack) * (skillLevel - 1)) * healPerLevel)) 
            + " * MATK (<color=green>" + skillTotalHeal + "</color> health)\n"
            + "Increase healing 20% of MATK per skill level \n"
            + "Increase manacost <color=#1C81CF>" + manaCostPerLevel + "</color> per level";               
            }
        }
    }
    public void Initialize(PlayerStats player)
    {
        playerStats = player;
    }
    public void ResetToDefaults()
    {
        skillLevel = 0; // Oletustaso
        manaCost = manaCostPerLevel; // Oletus manakustannus

    }
    public float damage
    {
        get
        {
            if (skillType == SkillType.Melee || skillType == SkillType.Ranged) // rangedille oma joskus ?
            {
            return (baseDamage + (damagePerLevel * (skillLevel - 1))) * playerStats.totalWeaponDamage;  // 1 + 0.2 * 1 * 6 = 1.2*6;
            }
            else if (skillType == SkillType.Spell)
            {
            return (baseDamage + (damagePerLevel * (skillLevel - 1))) * playerStats.magickAttack;  // 1 + 0.2 * 1 * 6 = 1.2*6;
            }
            else
            {
                return baseDamage;
            }
            
        }
    }

    public float heal 
    {
        get
        {
            return baseHeal + (((playerStats.magickAttack) * (skillLevel - 1)) * healPerLevel); // 1 + 6 * 1 * 0.2
        }
    }

    // Taitotason nosto
    public void Levelup(PlayerStats playerStats)
    {
        if (isLearned)
        {
        if (skillLevel < skillMaxLevel)
        {
            skillLevel++;
            manaCost = manaCostPerLevel * skillLevel;
            ApplyPassiveEffects(playerStats);
            
        }
        }
        else 
        {
            Debug.Log("skill is not learned");
        }
    }
    public void LearnSkill(PlayerStats playerStats)
    {
        if (!isLearned)
        {
        skillLevel++;
        ApplyPassiveEffects(playerStats);
        isLearned = true;
        }
    }

    public void ApplyPassiveEffects(PlayerStats playerStats)
    {
        
        if (isPassive)
        {

            if (addCrit > 0)
            {
                addCrit++;
                playerStats.skillCrit++;
                
            }
                
            if (addDodge > 0)
            {
                addDodge++;
                playerStats.skillDodge++;
            }
            playerStats.UpdateStatTexts();
            playerStats.UpdateStats();
            
        }
    }


}
