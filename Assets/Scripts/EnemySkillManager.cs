using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySkillManager : MonoBehaviour
{
    PlayerAttack playerAttack;
    PlayerStats playerStats;
    PlayerMovement playerMovement;
    BuffManager buffManager;
    public BuffDatabase buffDatabase;
    // Start is called before the first frame update
    void Start()
    {
        playerAttack = FindObjectOfType<PlayerAttack>();
        playerStats = FindObjectOfType<PlayerStats>();
        playerMovement = FindObjectOfType<PlayerMovement>();
        buffManager = FindObjectOfType<BuffManager>();

        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void ExecuteEnemySkill(string skillName, PlayerHealth playerHealth, EnemyHealth enemyHealth)
    {
        
        if(skillName == "Fire Bolt")
        {
            StartCoroutine(FireBolt(playerHealth, enemyHealth));
        }
        else if (skillName == "Fire Charge")
        {
            StartCoroutine(FireCharge(playerHealth, enemyHealth));
        }    
        else if (skillName == "Fire Debuff")
        {
            StartCoroutine(FireDebuff(playerHealth, enemyHealth));
        }                                                                    
        else
        {
            Debug.Log("Invalid skill");
        }

    }
    private void ApplyBurningEffect(PlayerHealth playerHealth)
    {
  
        StopAllCoroutines();
        StartCoroutine(ApplyBurningDamage(playerHealth));
    }

    private void RemoveBurningEffect(PlayerHealth playerHealth)
    {
        // Pysäytetään polttovahingon korutiinit
        StopCoroutine(ApplyBurningDamage(playerHealth));
    }

    private IEnumerator ApplyBurningDamage(PlayerHealth playerHealth)
    {
   
        float tickInterval = 1f;       // Vahinkoa kerran sekunnissa
        
        while (true)
        {
            Buff burningHeartBuff = buffManager.activeBuffs.Find(b => b.name == "BurningHeart");

            if (burningHeartBuff == null || burningHeartBuff.duration <= 0f)
            {
                Debug.Log("BurningHeart buff expired or not found, stopping DoT.");
                yield break; // Lopeta korutiini, kun buffi ei ole enää aktiivinen
            }

          
            playerHealth.TakeDamage(burningHeartBuff.damage * burningHeartBuff.stacks);
            burningHeartBuff.duration -= tickInterval;

            yield return new WaitForSeconds(tickInterval);
        }
    }
    public IEnumerator FireDebuff(PlayerHealth playerHealth, EnemyHealth enemyHealth)
    {
                Buff burningHeartBuffData  = buffDatabase.GetBuffByName("BurningHeart");
                Buff burningHeartBuff = new Buff(
                    burningHeartBuffData.name,
                    burningHeartBuffData.duration,
                    burningHeartBuffData.isStackable,
                    burningHeartBuffData.stacks,
                    burningHeartBuffData.maxStacks,
                    burningHeartBuffData.buffIcon,
                    BuffType.Debuff,  // Esimerkki: Jos tämä on debuff, käytä BuffType.Debuff
                    burningHeartBuffData.damage * enemyHealth.attackDamage,  // Tässä oletetaan, että "damage" on mukana BuffDatabase:ssa
                    burningHeartBuffData.effectText,
                    burningHeartBuffData.effectValue,  // Sama effectValue, jos se liittyy johonkin muuhun efektiin
                    () => ApplyBurningEffect(playerHealth), // Käytetään lambda-funktiota
                    () => RemoveBurningEffect(playerHealth) // Sama täällä
                );

                if (burningHeartBuff == null) {
                    Debug.LogError("BurningHeart buff creation failed!");
                } else {
                    // Lisää BurningHeart buffi pelaajalle

                    buffManager.AddBuff(burningHeartBuff);
                }

        yield break;

    }
    public IEnumerator FireBolt(PlayerHealth playerHealth, EnemyHealth enemyHealth)
    {
        Debug.Log("FIRE BOOOLT!");
    

        GameObject boltPrefab = Resources.Load<GameObject>("Projectiles/FireBolt");
        if (boltPrefab == null)
        {
            Debug.LogError("FireBolt prefab ei löytynyt Resourcesista!");
            yield break;
        }

        Vector3 spawnPosition = enemyHealth.transform.position + Vector3.down * 4f;

        GameObject bolt = Instantiate(boltPrefab, spawnPosition, Quaternion.identity);

        Projectile projectile = bolt.GetComponent<Projectile>();
        if (projectile != null)
        {
            projectile.Initialize(playerHealth.transform, ShooterType.Enemy);
            projectile.damage = enemyHealth.attackDamage;
        }

    yield return new WaitForSeconds(1f);
    Destroy(projectile);

        // 🔥 Ladataan ja luodaan "fireHit" efekti pelaajan päälle
        GameObject hitEffectPrefab = Resources.Load<GameObject>("Explosions/Explosion2");
        if (hitEffectPrefab != null)
        {
            Vector3 hitPosition = playerHealth.transform.position + Vector3.up * 1.0f; // hieman pelaajan yläpuolelle
            GameObject hitEffect = Instantiate(hitEffectPrefab, hitPosition, Quaternion.identity);
            Destroy(hitEffect, 1f); // tuhoaa efektin automaattisesti 2 sek kuluttua
        }
        else
        {
            Debug.LogWarning("FireHit-efektiä ei löytynyt Resources/Effects-kansiosta!");
        }
        

        
        Debug.Log("DAMAGE " + enemyHealth.attackDamage);

        playerHealth.TakeDamage(enemyHealth.attackDamage/2);
    }


    public IEnumerator FireCharge(PlayerHealth playerHealth, EnemyHealth enemyHealth)
    {
        Debug.Log("FIRE CHARGEE!");
         yield break;
    }

}
