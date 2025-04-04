using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEditor;

public enum SpellType
{
    Damage,
    Heal,
    Buff
    
}
public enum SkillType
{
    Melee,
    Ranged,
    Spell,
    Passive
    
}
public enum SkillSchool
{
    Warrior,
    Hunter,
    Mage,
    Priest,
    Assassin
    
}
[System.Serializable]
public class Skill
{
    public string skillName;
    public Sprite skillIcon;
    public float baseDamage; // Perusvahinko tason 1 tasolla
    public float damagePerLevel; // Kuinka paljon vahinko kasvaa per taso
    public int baseHeal;
    public float healPerLevel;
    public float cooldown;
    public float castTime;
    public int manaCost;
    public int manaCostPerLevel;
    public int damageRange;
    public int skillLevel;
    public int skillMaxLevel;
    public int totalHeal;
    public int totalDamage;
    public float addCrit;
    public float addBaseCrit;
    public float addDodge;
    public float addBaseDodge;
    public int addAgi;
    public int addDex;
    public int addStr;
    public int addVit;
    public int addInt;
    public int addRegenHP;
    public int addRegenSP;
    public int addRangedRange;
    public int addCastingRange;
    public bool isPassive;
    public string infoText;
    public string updatedInfoText;
    public Element element;
    public SpellType spellType;
    public SkillType skillType;
    public SkillSchool skillSchool;
    public bool isLearned = false;
    public bool canInterrup;
    public int skillCost;
    private PlayerStats playerStats;
    private PlayerAttack playerAttack;
    public bool noTarget = false;
    public string preSkill = null;
    public int preSkillLevel = 0;
    public int skillTier;
    public bool isAvailable = true;



    // Vahinko lasketaan suoraan skillLevel-muuttujasta
    void Start()
    {

        isLearned = false;
  
    }

    public Skill Clone()
    {
        return (Skill)MemberwiseClone();
    }
    public void Initialize(PlayerStats player, PlayerAttack attack)
    {
        playerStats = player;
        playerAttack = attack;
    }
    public void ResetToDefaults()
    {
        skillLevel = 0; // Oletustaso
        manaCost = manaCostPerLevel; // Oletus manakustannus
        isLearned = false;
        

    }
    public int damage
    {
        get
        {
            if (skillType == SkillType.Melee || skillType == SkillType.Ranged) // rangedille oma joskus ?
            {         
            int tempDmg = Mathf.RoundToInt((baseDamage + (damagePerLevel * (skillLevel - 1))) * playerStats.totalWeaponDamage);
            // 0.4 + (0.2 * 0) = 0.4 * 4
           
            return tempDmg;  // 1 + 0.2 * 1 * 6 = 1.2*6;
            }
            else if (skillType == SkillType.Spell)
            {
                int tempMatk = Mathf.RoundToInt((baseDamage + (damagePerLevel * (skillLevel - 1))) * playerStats.magickAttack);
            return tempMatk;  // 1 + 0.2 * 1 * 6 = 1.2*6;
            }
            else
            {
                return Mathf.RoundToInt(baseDamage + (damagePerLevel * (skillLevel - 1)));
            }
            
        }
    }

    public int heal 
    {
        get
        {
            return Mathf.RoundToInt(baseHeal + (((playerStats.magickAttack) * (skillLevel - 1)) * healPerLevel)); // 1 + 6 * 1 * 0.2
        }
    }

    // Taitotason nosto
    public void DecreaseLevel(PlayerStats playerStats)
    {
        if (skillLevel > 1)
        {
            skillLevel--;
            manaCost = manaCost - manaCostPerLevel;
            UpdatePassiveEffects(playerStats);
        }
        else
        {
            skillLevel--;
            manaCost = manaCost - manaCostPerLevel;
            UpdatePassiveEffects(playerStats);
            isLearned = false;
           
        }
    }
    public void Levelup(PlayerStats playerStats)
    {
        if (isLearned)
        {
            if (skillLevel < skillMaxLevel)
            {
                
                skillLevel++;
                manaCost = manaCostPerLevel * skillLevel;
                UpdatePassiveEffects(playerStats);
                
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
        UpdatePassiveEffects(playerStats);
        isLearned = true;
        }
    }

    public void UpdatePassiveEffects(PlayerStats playerStats)
    {
        
        if (isPassive)
        {

            if (addCrit > 0)
            {
                
                playerStats.skillCrit = addCrit * skillLevel;
                            
            }
                
            if (addDodge > 0)
            {
                
                playerStats.skillDodge = addDodge * skillLevel;
            }
            if (addRegenHP > 0)
            {
        
                playerStats.skillHpReg = addRegenHP * skillLevel;
            }
            if (addRegenSP > 0)
            {
                
                playerStats.skillSpReg = addRegenSP * skillLevel;
            }
            if (addDex > 0)
            {
                playerStats.skillDex = (addDex * skillLevel);
                //playerStats.AddBuffStatDex();
            }
            if (addRangedRange > 0)
            {
                playerAttack.buffRange = (addRangedRange * skillLevel);
                
            }

            playerStats.UpdateStatTexts();
            playerStats.UpdateStats();
            
        }
    }



}
