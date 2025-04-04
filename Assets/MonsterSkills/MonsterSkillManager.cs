using System.Collections.Generic;
using UnityEngine;

public class MonsterSkillManager : MonoBehaviour
{
    public List<MonsterSkill> skills; // Vedetään ScriptableObjectit tähän

    public void UseSkill(int index, Transform caster, Transform target)
    {
        if (index >= 0 && index < skills.Count)
        {
            skills[index].Activate(caster, target);
        }
        else
        {
            Debug.LogWarning("Skill index out of range!");
        }
    }
    public MonsterSkill GetSkill(string skillName)
    {
        return skills.Find(skill => skill.skillName == skillName);
    }

}