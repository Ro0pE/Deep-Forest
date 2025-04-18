using System.Collections;
using UnityEngine;

public class FrostWyrmLeaderAI : EnemyAI
{
    public PlayerMovement playerMovement;
    public BuffDatabase buffDatabase;
    private BuffManager buffManager;
    public bool playerSlowed = false;
 



    void Start()
    {
        base.Start();
        buffManager = FindObjectOfType<BuffManager>();
        playerMovement = FindObjectOfType<PlayerMovement>();
       

    }

    public override void AttackPlayer()
    {
        // Mukautettu hyökkäyslogiikka
        float distanceToPlayer = Vector3.Distance(player.position, transform.position);

        if (distanceToPlayer <= attackRange) // Jos pelaaja on hyökkäysetäisyydellä
        {
            Vector3 directionToPlayer = (player.position - transform.position).normalized;

            Ray ray = new Ray(transform.position, directionToPlayer);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, attackRange, playerLayer))
            {
                if (hit.collider.CompareTag("Player"))
                {
                    if (enemyHealth.isDead)
                    {
                        return;
                    }
                    else
                    {
                        // Hyökkäyksen suorittaminen
                        IcyTouch();
                        playerHealth.TakeDamage(enemyHealth.attackDamage);  // Vahingon tekeminen pelaajalle
                        Debug.Log("FrostWyrmLeader teki lisävahinkoa!");
                    }
                }
            }
        }
    }

    public void ApplySlowEffect(float slowEffect)
    {
        Debug.Log("Apply slow");
        float playerSlowedSpeed = playerMovement.moveSpeed * slowEffect;
        playerMovement.SetPlayerSpeed(playerSlowedSpeed);
    }

    public void RemoveSlowEffect()
    {
        Debug.Log("Remove slow");
        playerSlowed = false;
        playerMovement.ReturnPlayerSpeed();
    }

    public void IcyTouch()
    {
        if (!playerSlowed)  // Varmistetaan, että hidastusta ei ole jo sovellettu
        {
            Buff slowBuffData = buffDatabase.GetBuffByName("Slow");
            if (slowBuffData == null)
            {
                Debug.LogError("Slow buff not found in the BuffDatabase.");
                return;
            }

            Buff slowBuff = new Buff(
                slowBuffData.name,                // Buffin nimi
                7,                                // Modattu kesto
                slowBuffData.isStackable,         // Voiko pinota
                slowBuffData.stacks,
                slowBuffData.maxStacks,              // Pinojen määrä
                slowBuffData.buffIcon,            // Kuvake
                BuffType.Debuff,                  // Buffin tyyppi
                slowBuffData.damage,
                slowBuffData.effectText,
                slowBuffData.effectValue,         // Hidastusprosentti
                () => ApplySlowEffect(slowBuffData.effectValue),  // Efektin soveltaminen
                RemoveSlowEffect                  // Efektin poistaminen
            );

            if (slowBuff == null)
            {
                Debug.LogError("Slow buff creation failed!");
            }
            else
            {
                buffManager.AddBuff(slowBuff);
                playerSlowed = true;
            }
        }
    }
}
