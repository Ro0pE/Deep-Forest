using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewSingleTargetSkill", menuName = "MonsterSkills/SingleTargetSkill")]
public class MonsterSingleTargetSkill : MonsterSkill
{
    public float damage;

    public override void Activate(Transform caster, Transform target)
    {
        Debug.Log($"{caster.name} käyttää {skillName}-skilliä ja osuu {target.name}!");
        // Lisää vahinkomekaniikka tähän
    }
}
