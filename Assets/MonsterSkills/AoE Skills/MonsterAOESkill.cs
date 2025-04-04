using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewAOESkill", menuName = "MonsterSkills/AOESkill")]
public class MonsterAOESkill : MonsterSkill
{
    public float aoeRadius;
    public float damage;
    public float duration;
    public float tickInterval;

    public override void Activate(Transform caster, Transform target)
    {
        Debug.Log($"{caster.name} käyttää {skillName}-AOE-skilliä!");
        // Lisää vahinkomekaniikka tähän
    }
}