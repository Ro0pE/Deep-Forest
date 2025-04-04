using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class EmberWyrmLeaderAI : EnemyAI
{
    public PlayerMovement playerMovement;
    public BuffDatabase buffDatabase;
    private BuffManager buffManager;
    public bool playerSlowed = false;
    public Image castBar; // Viittaus CastBar-kuvakkeeseen
    public TextMeshProUGUI castTimeText; // Viittaus CastBarin ajan näyttöön
    public TextMeshProUGUI castBarSkillText;
    public GameObject enemyCastBarPanel;
    [Header("Fire Ball")]
    public float fireBallCooldownTimer = 10f;
    public float fireBallTimer = 0f;
    public float fireBallChannelTime = 4f;
    public bool isCastingFireBall = false;
    public GameObject frostSpellPrefab;
    public Transform castPoint; // Paikka, josta projektiili syntyy




    void Start()
    {
        base.Start();
        buffManager = FindObjectOfType<BuffManager>();
        playerMovement = FindObjectOfType<PlayerMovement>();
    }
    void Update()
    {
        fireBallTimer += Time.deltaTime;
        if (distanceToPlayer <= detectionRange)
        {
            
            if (fireBallTimer >= fireBallCooldownTimer && !isCastingFireBall)
            {
                StartCoroutine(FireBallCasting());
            }
        } 
        base.Update();
        
   
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
                        
                        playerHealth.TakeDamage(attackDamage);  // Vahingon tekeminen pelaajalle
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
                slowBuffData.stacks,              // Pinojen määrä
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
    public IEnumerator FireBallCasting()
    {
        Debug.Log("Casting fireball");
        enemyCastBarPanel.SetActive(true);
        animator.SetTrigger("isCasting");
        enemyHealth.isInterrupted = false;
        fireBallTimer = 0f;
        isCastingFireBall = true;
        //agent.isStopped = true;
        agent.velocity = Vector3.zero; // Aseta nopeus nollaan
        agent.acceleration = 0f;      // Estä hidastuva liike
        castBarSkillText.text = "Fire Ball";
        castBar.fillAmount = 1f;
        castBar.gameObject.SetActive(true);
        float elapsed = 0f;

        // Kun castaus keskeytetään, estä debuffin asettaminen
        while (elapsed < fireBallChannelTime && !enemyHealth.isInterrupted)
        {
            FacePlayer();

            elapsed += Time.deltaTime;
            castBar.fillAmount = 1f - (elapsed / fireBallChannelTime); // Päivitä CastBar
            if (castTimeText != null)
            {
                castTimeText.text = $"{Mathf.Max(0f, fireBallChannelTime - elapsed):0.0} s"; // Näytä jäljellä oleva aika
            }

            yield return null;
        }

        // Jos castaus keskeytettiin, estä debuffin asettaminen
        if (enemyHealth.isInterrupted)
        {
            Debug.Log("SKILL INTERRUPTED WOHOO!!");
            castBar.fillAmount = 1f;
            castTimeText.text = "";
            castBarSkillText.text = "Interrupted";
            agent.isStopped = false;
            agent.acceleration = 95f; // Palauta alkuperäinen kiihtyvyys
            isCastingFireBall = false;
            yield return new WaitForSeconds(1);
            castBar.gameObject.SetActive(false);
         
            yield break; // Lopeta korutiini, jotta ei aseteta debuffia
        }

        castBar.fillAmount = 0f;
        castBarSkillText.text = "";
        castBar.gameObject.SetActive(false);
        agent.isStopped = false;
        agent.acceleration = 95f; // Palauta alkuperäinen kiihtyvyys
        isCastingFireBall = false;
        enemyCastBarPanel.SetActive(false);

        // Aloita polttovahinko hyökkäys
       
        FireBall();
    }

    public void FireBall()
    {
        Debug.Log("Fireball Inc!");
        Vector3 spawnPosition = castPoint.position + castPoint.forward * 0.5f + castPoint.up * 3.5f;
        GameObject projectile = Instantiate(frostSpellPrefab, spawnPosition, Quaternion.identity);
        projectile.GetComponent<Projectile>().Initialize(player.transform);

    }
        void FacePlayer()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        direction.y = 0; // Poistetaan pystysuuntainen komponentti, jotta vihollinen ei kallistu.
        if (direction.magnitude > 0.1f)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 10f); // Sulava käännös
        }
    }
}
