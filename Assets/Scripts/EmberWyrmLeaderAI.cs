using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class EmberWyrmLeaderAI : EnemyAI
{
    public BuffDatabase buffDatabase;
    private BuffManager buffManager;
    public Image castBar;
    public TextMeshProUGUI castTimeText;
    public TextMeshProUGUI castBarSkillText;
    public GameObject enemyCastBarPanel;
    public EnemySkillManager enemySkillManager;
    public float fireBoltCooldown = 425f;
    public float fireBlastCooldown = 15f;
    public float fireBoltTimer;
    public float fireBlastTimer;
    public float fireBoltCastTime = 2f;
    public float fireBlastCastTime = 4f;
    public float fireBuffTimer;
    public float fireBuffCooldown = 257f;
    public float fireBuffCastTime = 1.5f;
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
    public Transform fireBlastCastPoint;
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
        fireBoltTimer = fireBoltCooldown;
        fireBlastTimer = fireBlastCooldown;
        fireBuffTimer = fireBuffCooldown;
    }

    void Update()
    {
        base.Update();
        

        fireBoltTimer -= Time.deltaTime;
        fireBlastTimer -= Time.deltaTime;
        fireBuffTimer -= Time.deltaTime;
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
                if (fireBuffTimer <= 0f && enemy.monsterData.skills.Contains("Fire Debuff") && !isCasting)
                {
                    Debug.Log("Burning heartin aika (lähellä)");
                    StartCoroutine(FireDebuff());
                }
                if (fireBlastTimer <= 0f && enemy.monsterData.skills.Contains("Fire Blast") && !isCasting)
                {
                    Debug.Log("Fire Blastin aika");
                    StartCoroutine(CastFireBlast());

                }

                return; // Älä tee muita skillejä kun ollaan ihan lähellä
            }

            // Etäisyys yli 25 yksikköä mutta vielä detect rangella
            if (distanceToPlayer > 25f)
            {
                if (fireBoltTimer <= 0f && enemy.monsterData.skills.Contains("Fire Bolt") && !isCasting)
                {
                    Debug.Log("Fireboltin aika");
                    StartCoroutine(CastFireBolt());
                }
            }

            // Fire Debuff toimii myös kauempana
            if (fireBuffTimer <= 0f && enemy.monsterData.skills.Contains("Fire Debuff") && !isCasting)
            {
                Debug.Log("Burning heartin aika (kauempaa)");
                StartCoroutine(FireDebuff());
            }
            if (fireBlastTimer <= 0f && enemy.monsterData.skills.Contains("Fire Blast") && !isCasting)
            {
                Debug.Log("Fire Blastin aika");
                StartCoroutine(CastFireBlast());

            }
            
        }
    }

    public override void AttackPlayer()
    {
        base.AttackPlayer(); // Kutsuu EnemyAI:n versiota
        attackRoarSource.PlayOneShot(attackRoarSound); // Oman luokan lisätoiminto
    }
    private IEnumerator FireDebuff()
    {
        animator.SetTrigger("isCasting");
        enemyCastBarPanel.SetActive(true);
        agent.isStopped = true;
        isCasting = true;
        castBarSkillText.text = "Burning Heart";
        castBar.fillAmount = 1f;
        castBar.gameObject.SetActive(true);

        float elapsed = 0f;
        while (elapsed < fireBuffCastTime)
        {
            if (enemyHealth.isInterrupted)
            {
                castBar.gameObject.SetActive(false);
                isCasting = false;
                yield break; // Lopeta coroutine heti
            }

            elapsed += Time.deltaTime;
            castBar.fillAmount = 1f - (elapsed / fireBuffCastTime);
            if (castTimeText != null)
            {
                castTimeText.text = $"{Mathf.Max(0f, fireBuffCastTime - elapsed):0.0} s";
            }
            yield return null;
        }

        castBar.gameObject.SetActive(false);

        if (!enemyHealth.isInterrupted)
        {
            enemySkillManager.ExecuteEnemySkill("Fire Debuff", playerHealth, enemyHealth);
            fireBuffTimer = fireBuffCooldown;
        }

        isCasting = false;
        agent.isStopped = false;
    }


private IEnumerator CastFireBolt()
{
    fireBoltSource.PlayOneShot(fireBoltCastSound);
    animator.SetTrigger("isCasting");
    enemyCastBarPanel.SetActive(true);
    agent.isStopped = true;
    isCasting = true;
    castBarSkillText.text = "Fire Bolt";
    castBar.fillAmount = 1f;
    castBar.gameObject.SetActive(true);

    float elapsed = 0f;
    while (elapsed < fireBoltCastTime)
    {
        if (enemyHealth.isInterrupted)
        {
            castBar.gameObject.SetActive(false);
            isCasting = false;
            fireBoltSource.Stop(); // pysäytetään ääni jos keskeytetään
            yield break; // Keskeytä heti
        }

        // 🔥 Katsotaan pelaajaan
        Vector3 lookDirection = (playerHealth.transform.position - transform.position).normalized;
        lookDirection.y = 0; // Poistetaan mahdollinen kallistus
        if (lookDirection != Vector3.zero)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookDirection), Time.deltaTime * 5f);
        }

        elapsed += Time.deltaTime;
        castBar.fillAmount = 1f - (elapsed / fireBoltCastTime);
        if (castTimeText != null)
        {
            castTimeText.text = $"{Mathf.Max(0f, fireBoltCastTime - elapsed):0.0} s";
        }

        yield return null;
    }

    castBar.gameObject.SetActive(false);

    if (!enemyHealth.isInterrupted)
    {
        enemySkillManager.ExecuteEnemySkill("Fire Bolt", playerHealth, enemyHealth);
        fireBoltTimer = fireBoltCooldown;
    }
    

    fireBoltSource.Stop();
    isCasting = false;
    agent.isStopped = false;
}

    private IEnumerator CastFireBlast()
    {
        fireBlastRoarSource.PlayOneShot(fireBlastRoarSound);
        audioSource.PlayOneShot(fireBlastSound);
        animator.SetTrigger("isCastingFireBlast");
        enemyCastBarPanel.SetActive(true);
        agent.isStopped = true;
        isCasting = true;
        castBarSkillText.text = "Fire Blast";
        castBar.fillAmount = 1f;
        castBar.gameObject.SetActive(true);

        float elapsed = 0f;
        float damageInterval = 1f;
        float nextDamageTime = 0f;

        // 🔥 Efekti heti kanavoinnin alkuun
        GameObject blastEffect = Resources.Load<GameObject>("Explosions/FireBlast");
        GameObject instance = null;
        if (blastEffect != null && fireBlastCastPoint != null)
        {
            instance = Instantiate(blastEffect, fireBlastCastPoint.position, fireBlastCastPoint.rotation);
            instance.transform.SetParent(fireBlastCastPoint.transform);
            Destroy(instance, fireBlastCastTime);
        }


        while (elapsed < fireBlastCastTime)
        {
            
            elapsed += Time.deltaTime;
            castBar.fillAmount = 1f - (elapsed / fireBlastCastTime);

            if (castTimeText != null)
            {
                castTimeText.text = $"{Mathf.Max(0f, fireBlastCastTime - elapsed):0.0} s";
            }

            // 🔥 Tehdään damagea tietyin välein
            if (elapsed >= nextDamageTime)
            {
                nextDamageTime += damageInterval;
                Debug.Log("FireBlast metodi runaa!");
                // Kutsutaan FireBlast coroutinea (tärkeä: StartCoroutine!)
                enemySkillManager.ExecuteEnemySkill("Fire Blast", playerHealth, enemyHealth);
              
            }

            yield return null;
        }
        audioSource.Stop();
        fireBlastRoarSource.Stop();
        castBar.gameObject.SetActive(false);
        fireBlastTimer = fireBlastCooldown;
        isCasting = false;
        agent.isStopped = false;

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
