using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SkillDatabase", menuName = "Game/Skill Database")]
public class SkillDatabase : ScriptableObject
{
    public List<Skill> skills; // Lista kaikista taidoista
    


    // Hakee taidon nimen perusteella

    public Skill GetSkillByName(Skill skill)
    {
        return skills.Find(skill => skill.skillName == name);
    }
    public bool IsDamageType(Skill skill)
    {
        return skill.spellType == SpellType.Damage;
    }
    public bool IsHealType(Skill skill)
    {   
 
        return skill.spellType == SpellType.Heal;
    }
    public List<Skill> GetSkillList()
    {
        return skills;
    }
    public int GetSkillCount()
    {
        
        return skills.Count;
    }
    public void ResetSkills()
    {
        foreach (var skill in skills)
        {
            skill.ResetToDefaults(); // Palauttaa skilleille oletusarvot
        }
    }
    public Skill GetSkillByIndex(int index)
    {
        if (index >= 0 && index < skills.Count)
        {
            Debug.Log("skill : " + skills[index]);
            return skills[index];
        }
        else
        {
            Debug.LogWarning("Skill index out of range!");
            return null;
        }
    }

    // Muut metodit


}