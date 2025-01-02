using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillManager : MonoBehaviour
{
    PlayerAttack playerAttack;
    // Start is called before the first frame update
    void Start()
    {
        playerAttack = FindObjectOfType<PlayerAttack>();
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ExecuteSkill(Skill skill)
    {
        Debug.Log("Skill name : " +skill.skillName);
        if(skill.skillName == "Multi Arrow")
        {
            MultiArrow(skill);
        }
        else if (skill.skillName == "Double Strike")
        {
            StartCoroutine(DoubleStrike(skill));
        }
        else if (skill.skillName == "Stunning Arrow")
        {
            StartCoroutine(StunningArrow(skill));
        }
        else if (skill.skillName == "Bulls Eye")
        {
            StartCoroutine(BullsEye(skill));
        }
        else
        {
            Debug.Log("Invalid skill");
        }

    }

    public void MultiArrow(Skill skill)
    {       
        Debug.Log("Multi arrow inc!");
    // Laukaistaan nuoli kohteeseen
    playerAttack.StartCoroutine(playerAttack.ShootArrows(playerAttack.targetedEnemy,playerAttack.arrowPrefab));
    playerAttack.StartCoroutine(playerAttack.DealDamageAfterDelayMagic(skill, playerAttack.IsCriticalHit()));

    // Tarkistetaan vahinkosäde kohteen ympäriltä
    if (skill.damageRange > 0f && playerAttack.targetedEnemy != null)
    {
        Collider[] hitColliders = Physics.OverlapSphere(playerAttack.targetedEnemy.transform.position, skill.damageRange);
        Debug.Log("Vihuja lähellä: " + hitColliders.Length);

        foreach (Collider hitCollider in hitColliders)
        {
            // Tarkista, onko kyseessä toinen vihollinen
            EnemyHealth otherEnemy = hitCollider.GetComponent<EnemyHealth>();
            if (otherEnemy != null && otherEnemy != playerAttack.targetedEnemy) // Ei tehdä vahinkoa alkuperäiselle kohteelle
            {
                // Tee vahinkoa muille vihollisille rinnakkain
                playerAttack.StartCoroutine(playerAttack.DealDamageAfterDelayMagic(skill, playerAttack.IsCriticalHit(), otherEnemy));

                // Laukaise nuoli visuaalisesti muita vihollisia kohti, jos haluat
                playerAttack.StartCoroutine(playerAttack.ShootArrows(otherEnemy,playerAttack.arrowPrefab));
            }
        }
    }
        
    }
    public IEnumerator DoubleStrike(Skill skill)
    {
        // Ensimmäinen nuoli ja vahinko
        yield return playerAttack.StartCoroutine(playerAttack.ShootArrows(playerAttack.targetedEnemy,playerAttack.arrowPrefab));
        // Lyhyt viive
        yield return new WaitForSeconds(0.1f);
        // Toinen nuoli ja vahinko
        yield return playerAttack.StartCoroutine(playerAttack.ShootArrows(playerAttack.targetedEnemy,playerAttack.arrowPrefab));
        yield return playerAttack.StartCoroutine(playerAttack.DealDamageAfterDelayMagic(skill, playerAttack.IsCriticalHit()));
        yield return playerAttack.StartCoroutine(playerAttack.DealDamageAfterDelayMagic(skill, playerAttack.IsCriticalHit()));
    }

    public IEnumerator StunningArrow(Skill skill)
    {
        yield return playerAttack.StartCoroutine(playerAttack.ShootArrows(playerAttack.targetedEnemy,playerAttack.arrowPrefab));
        yield return playerAttack.StartCoroutine(playerAttack.DealDamageAfterDelayMagic(skill, playerAttack.IsCriticalHit()));
        playerAttack.targetedEnemy.agent.isStopped = true;
        yield return new WaitForSeconds(3f);
        playerAttack.targetedEnemy.agent.isStopped = false;
        
    }

public IEnumerator BullsEye(Skill skill)
{
    // Lataa tai aseta BullsEyeArrow-prefab
    GameObject bullsEyeArrowPrefab = Resources.Load<GameObject>("Projectiles/BullsEyeArrow");

    if (bullsEyeArrowPrefab == null)
    {
        Debug.LogError("BullsEyeArrow prefab not found!");
        yield break;
    }

    // Käynnistetään ShootArrows coroutine ja odotetaan sen palauttamista
    GameObject projectile = null;

    // Odotetaan ShootArrows coroutinea ja palautetaan projektiili
    yield return StartCoroutine(playerAttack.ShootArrows(playerAttack.targetedEnemy, bullsEyeArrowPrefab));

    // Tässä vaiheessa 'projectile' on käytettävissä, jos se palautettiin ShootArrows metodista
    if (projectile != null)
    {
        Renderer renderer = projectile.GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material.color = Color.red; // Muuta esimerkiksi nuolen väri punaiseksi
        }

        // Voit lisätä muita efektejä tai ominaisuuksia
    }

    // Vahingon käsittely
    yield return playerAttack.StartCoroutine(playerAttack.DealDamageAfterDelayMagic(skill, playerAttack.IsCriticalHit()));
}




}
