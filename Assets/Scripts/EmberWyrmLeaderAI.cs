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
    public float fireBoltCooldown = 4f;
    public float fireChargeCooldown = 15f;
    public float fireBoltTimer;
    public float fireChargeTimer;
    public float fireBoltCastTime = 2f;
    public float fireChargeCastTime = 1f;
    public float fireBuffTimer;
    public float fireBuffCooldown = 7f;
    public float fireBuffCastTime = 1.5f;


    void Start()
    {
        base.Start();
        fireBoltTimer = fireBoltCooldown;
        fireChargeTimer = fireChargeCooldown;
        fireBuffTimer = fireBuffCooldown;
    }

    void Update()
    {
        base.Update();

        fireBoltTimer -= Time.deltaTime;
        fireChargeTimer -= Time.deltaTime;
        fireBuffTimer -= Time.deltaTime;
        if (enemyHealth.isStunned || isCasting)
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

                if (fireChargeTimer <= 0f && enemy.monsterData.skills.Contains("Fire Charge") && !isCasting)
                {
                    Debug.Log("Fire Chargen aika");
                    enemySkillManager.ExecuteEnemySkill("Fire Charge", playerHealth, enemyHealth);
                    fireChargeTimer = fireChargeCooldown;
                }
            }

            // Fire Debuff toimii myös kauempana
            if (fireBuffTimer <= 0f && enemy.monsterData.skills.Contains("Fire Debuff") && !isCasting)
            {
                Debug.Log("Burning heartin aika (kauempaa)");
                StartCoroutine(FireDebuff());
            }
        }
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
                yield break; // Keskeytä heti
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

        // Jos ei keskeytetty, suorita skilli
        if (!enemyHealth.isInterrupted)
        {
            enemySkillManager.ExecuteEnemySkill("Fire Bolt", playerHealth, enemyHealth);
            fireBoltTimer = fireBoltCooldown;
        }

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
}
