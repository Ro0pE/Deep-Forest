using System.Collections.Generic;
using UnityEngine;

// 1. Perusluokka vihollisten skilleille
public abstract class MonsterSkill : ScriptableObject
{
    public string skillName;
    public float cooldown;
    public float castTime;
    public abstract void Activate(Transform caster, Transform target);
}
