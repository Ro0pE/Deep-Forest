using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class FrostWyrmLeaderAI : EnemyAI
{
    public BuffDatabase buffDatabase;
    private BuffManager buffManager;
    public Image castBar;
    public TextMeshProUGUI castTimeText;
    public TextMeshProUGUI castBarSkillText;
    public GameObject enemyCastBarPanel;
    public EnemySkillManager enemySkillManager;
    public float frostChargeCooldown = 15f;
    public float glacialNovaCooldown = 15f;
    public float icyPrisonCooldown = 4f;

    public float frostChargeTimer;
    public float glacialNovaTimer;
    public float icyPrisonTimer;

    public float frostChargeCastTime = 2f;
    public float glacialNovaCastTime = 1.5f;
    public float glacialNovaChannelTime = 6f;
    public float icyPrisonCastTime = 2f;


    public AudioSource audioSource; // Perus audioSource
    public AudioSource fireBlastRoarSource; // FireBlast Roar ääni
    public AudioSource fireBoltSource; // FireBlast Roar ääni
    public AudioSource attackRoarSource; // Attack Roar ääni
    public AudioSource randomRoarSource; // Random Roar ääni

    public AudioClip fireBoltCastSound;
    public AudioClip fireBoltHitSound;
    public AudioClip fireBlastSound; // Fire Blast ääni
    public AudioClip fireBlastRoarSound; // Fire Blast Roar ääni
    public AudioClip attackRoarSound; // Attack Roar ääni
    public AudioClip randomRoaringSound; // Random Roaring ääni
    public Transform glacialNovaCastPoint;
    private Coroutine roarCoroutine;
    private bool isRoaring = false;


    void Start()
    {
        base.Start();
        roarCoroutine = StartCoroutine(RandomRoarRoutine());
        audioSource = gameObject.AddComponent<AudioSource>();
        fireBlastRoarSource = gameObject.AddComponent<AudioSource>();
        attackRoarSource = gameObject.AddComponent<AudioSource>();
        randomRoarSource = gameObject.AddComponent<AudioSource>();
        fireBoltSource = gameObject.AddComponent<AudioSource>();
        glacialNovaTimer = glacialNovaCooldown;
        icyPrisonTimer = icyPrisonCooldown;
        frostChargeTimer = frostChargeCooldown;
    }

    void Update()
    {
        base.Update();
        

        glacialNovaTimer -= Time.deltaTime;
        icyPrisonTimer -= Time.deltaTime;
        frostChargeTimer -= Time.deltaTime;
        if (enemyHealth.isStunned || isCasting || isRoaring)
        {
            return;
        }
        float distanceToPlayer = Vector3.Distance(playerHealth.transform.position, transform.position);

        if (distanceToPlayer < detectionRange)
        {
            FacePlayer();
            Enemy enemy = enemyHealth as Enemy;

            // Lähitaisteluetäisyys → käytetään vain basea ja debuffia
            if (distanceToPlayer <= attackRange)
            {
                if (glacialNovaTimer <= 0f && enemy.monsterData.skills.Contains("Glacial Nova") && !isCasting)
                {
                    
                    StartCoroutine(GlacialNova());
                }
                if (icyPrisonTimer <= 0f && enemy.monsterData.skills.Contains("Icy Prison") && !isCasting)
                {
                    
                    StartCoroutine(IcyPrison());

                }

                return; // Älä tee muita skillejä kun ollaan ihan lähellä
            }

            // Etäisyys yli 25 yksikköä mutta vielä detect rangella
            if (distanceToPlayer > 25f)
            {
                if (frostChargeTimer <= 0f && enemy.monsterData.skills.Contains("Frost Charge") && !isCasting)
                {
                   
                    StartCoroutine(FrostCharge());
                }
            }

            // Fire Debuff toimii myös kauempana
                if (glacialNovaTimer <= 0f && enemy.monsterData.skills.Contains("Glacial Nova") && !isCasting)
                {
                    
                    StartCoroutine(GlacialNova());
                }
                if (icyPrisonTimer <= 0f && enemy.monsterData.skills.Contains("Icy Prison") && !isCasting)
                {
                 
                    StartCoroutine(IcyPrison());

                }
            
        }
    }

    public override void AttackPlayer()
    {
        base.AttackPlayer(); // Kutsuu EnemyAI:n versiota
        attackRoarSource.PlayOneShot(attackRoarSound); // Oman luokan lisätoiminto
    }
    private IEnumerator IcyPrison()
    {
        yield return null;
    }
    private IEnumerator FrostCharge()
    {
    Debug.Log("Frost Charge  lähtee!");
    fireBoltSource.PlayOneShot(fireBoltCastSound);
    animator.SetTrigger("isCastingFrostCharge");
    
    enemyCastBarPanel.SetActive(true);
    agent.isStopped = true;
    isCasting = true;
    castBarSkillText.text = "Frost Charge";
    castBar.fillAmount = 1f;
    castBar.gameObject.SetActive(true);
    Vector3 lockedTargetPosition = playerHealth.transform.position; // pelaajan sijainti talteen



    // CASTING: Näytetään cast-time ennen kanavointia
    float castElapsed = 0f;
    while (castElapsed < frostChargeCastTime)
    {
        if (enemyHealth.isInterrupted)
        {
            castBar.gameObject.SetActive(false);
            isCasting = false;
            fireBoltSource.Stop();
            agent.isStopped = false;

            
            yield break;
        }

        castBar.fillAmount = 1f - (castElapsed / frostChargeCastTime);

        if (castTimeText != null)
        {
            castTimeText.text = $"Casting:{Mathf.Max(0f, frostChargeCastTime - castElapsed):0.0} s";
        }

        castElapsed += Time.deltaTime;
        yield return null;
    }
    GameObject frostChargeEffect = null;
    GameObject frostChargeEffectPrefab = Resources.Load<GameObject>("UI_Effects/FrostChargeEffect");

    if (frostChargeEffectPrefab != null)
    {
        frostChargeEffect = Instantiate(frostChargeEffectPrefab, glacialNovaCastPoint.position, Quaternion.identity);
        frostChargeEffect.transform.SetParent(enemyHealth.transform);
        Destroy(frostChargeEffect, 3f);
    }

    // Ennen kanavointia kutsutaan enemySkillManagerin GlacialNova-metodia
    yield return new WaitForSeconds(0.1f);

// Asetetaan "isFrostCharging" triggeri
    animator.SetTrigger("isFrostCharging");
    StartCoroutine(enemySkillManager.ExecuteEnemySkill("Frost Charge", playerHealth, enemyHealth, lockedTargetPosition));

    // CHANNELING: Kanavointi alkaa




    castBar.gameObject.SetActive(false);
    isCasting = false;
    fireBoltSource.Stop();
    agent.isStopped = false;
    frostChargeTimer = frostChargeCooldown;
    yield return new WaitForSeconds(0.4f);
    animator.ResetTrigger("isCastingFrostCharge");
    animator.ResetTrigger("isFrostCharging");

    //yield return new WaitForSeconds(1f);


    //if (glacialEffect != null) Destroy(glacialEffect);
    }

private IEnumerator GlacialNova()
{
    Debug.Log("Glacial Nova lähtee!");
    fireBoltSource.PlayOneShot(fireBoltCastSound);
    animator.SetTrigger("isCastingGlacial");
    enemyCastBarPanel.SetActive(true);
    agent.isStopped = true;
    isCasting = true;
    castBarSkillText.text = "Glacial Nova";
    castBar.fillAmount = 1f;
    castBar.gameObject.SetActive(true);



    // CASTING: Näytetään cast-time ennen kanavointia
    float castElapsed = 0f;
    while (castElapsed < glacialNovaCastTime)
    {
        if (enemyHealth.isInterrupted)
        {
            castBar.gameObject.SetActive(false);
            isCasting = false;
            fireBoltSource.Stop();
            agent.isStopped = false;

            
            yield break;
        }

        castBar.fillAmount = 1f - (castElapsed / glacialNovaCastTime);

        if (castTimeText != null)
        {
            castTimeText.text = $"Casting:{Mathf.Max(0f, glacialNovaCastTime - castElapsed):0.0} s";
        }

        castElapsed += Time.deltaTime;
        yield return null;
    }
    GameObject glacialEffect = null;
    GameObject glacialEffectPrefab = Resources.Load<GameObject>("Explosions/GlacialNovaEffect");

    if (glacialEffectPrefab != null)
    {
        glacialEffect = Instantiate(glacialEffectPrefab, glacialNovaCastPoint.position, Quaternion.identity);
    }

    // Ennen kanavointia kutsutaan enemySkillManagerin GlacialNova-metodia
    StartCoroutine(enemySkillManager.ExecuteEnemySkill("Glacial Nova", playerHealth, enemyHealth));

    // CHANNELING: Kanavointi alkaa
    animator.SetTrigger("isChannelingGlacialStart");
    float elapsed = 0f;
    animator.SetTrigger("isChannelingGlacial");
    while (elapsed < glacialNovaChannelTime)
    {
        if (enemyHealth.isInterrupted)
        {
            castBar.gameObject.SetActive(false);
            isCasting = false;
            fireBoltSource.Stop();
            agent.isStopped = false;

            if (glacialEffect != null) Destroy(glacialEffect);
            yield break;
        }

        // Katse pelaajaan
        Vector3 lookDirection = (playerHealth.transform.position - transform.position).normalized;
        lookDirection.y = 0;
        if (lookDirection != Vector3.zero)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookDirection), Time.deltaTime * 5f);
        }

        elapsed += Time.deltaTime;
        castBar.fillAmount = 1f - (elapsed / glacialNovaChannelTime);

        if (castTimeText != null)
        {
            castTimeText.text = $"Channeling:{Mathf.Max(0f, glacialNovaChannelTime - elapsed):0.0} s";
        }

        yield return null;
    }

    castBar.gameObject.SetActive(false);
    isCasting = false;
    fireBoltSource.Stop();
    agent.isStopped = false;
    glacialNovaTimer = glacialNovaCooldown;
    animator.ResetTrigger("isCastingGlacial");
    animator.ResetTrigger("isChannelingGlacial");
    animator.ResetTrigger("isChannelingGlacialStart");

    if (glacialEffect != null) Destroy(glacialEffect);
}





    void FacePlayer()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        direction.y = 0;
        if (direction.magnitude > 0.1f)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 10f);
        }
    }

    bool CanSeePlayer()
    {
        // Esimerkki näkyvyystarkistus (voit käyttää omiasi)
        return Vector3.Distance(transform.position, player.position) < 30f;
    }
    public void PlayFireBlastRoar()
    {
        if (fireBlastRoarSource != null && fireBlastRoarSound != null)
        {
            fireBlastRoarSource.PlayOneShot(fireBlastRoarSound);
        }
    }
    public void PlayRandomRoar()
    {
        if (audioSource != null && randomRoaringSound != null)
        {
            audioSource.PlayOneShot(randomRoaringSound);
        }
    }


    // Metodi FireBlast äänen toistamiseen
    public void PlayFireBlastSound()
    {
        if (audioSource != null && fireBlastSound != null)
        {
            audioSource.PlayOneShot(fireBlastSound);
        }
    }

    // Metodi Attack Roar äänen toistamiseen
    public void PlayAttackRoarSound()
    {
        if (attackRoarSource != null && attackRoarSound != null)
        {
            attackRoarSource.PlayOneShot(attackRoarSound);
        }
    }

    // Metodi Random Roar äänen toistamiseen
    public void PlayRandomRoaringSound()
    {
        if (randomRoarSource != null && randomRoaringSound != null)
        {
            randomRoarSource.PlayOneShot(randomRoaringSound);
        }
    }

    // Metodi lopettaa kaikki äänet
    public void StopAllSounds()
    {
        fireBlastRoarSource.Stop();
        attackRoarSource.Stop();
        randomRoarSource.Stop();
        audioSource.Stop();
    }
    private IEnumerator StopFireBoltSoundAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        fireBoltSource.Stop();
    }
    private IEnumerator RandomRoarRoutine()
    {
        while (true)
        {
            float waitTime = Random.Range(7f, 26f);
            yield return new WaitForSeconds(waitTime);

            if (!audioSource.isPlaying && !isAttacking && !isCasting && !isRoaring)
            {
                isRoaring = true;
                agent.isStopped = true;

                animator.SetTrigger("isRoaring");
                audioSource.PlayOneShot(randomRoaringSound);
                Debug.Log("Vihollinen murisee!");

                yield return new WaitForSeconds(randomRoaringSound.length);

                isRoaring = false;
                agent.isStopped = false;
            }
        }
    }

}
