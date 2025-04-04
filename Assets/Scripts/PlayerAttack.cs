using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerAttack : MonoBehaviour
{
    public int attackDamage = 1;
    public int magickAttackDamage = 1;
    public float critChance = 0;
    public int hitRating = 0;
    public int baseHitRating = 50;
    public float attackSpeed = 2f;
    public float attackSpeedReduction = 0f;
    public float weaponAttackSpeed = 0f;
    public Animator animator;
    public bool isAttacking;
    public bool isCasting = false; // Tila castaamisen seuraamiseen
    public bool isChanneling = false;
    public EnemyHealth targetedEnemy;
    public int targetingRange = 80;
    public int attackRange;
    public int weaponRange;  // kyhää eri aseita
    public int buffRange;
    public int baseRange = 5;
    public float castingRange = 30f;
    public float critMultiplier = 2f;
    private ActionbarPanel actionbarPanel;
    public Element autoaAttackElement;
    public Item arrowTest;


    public GameObject skillEffectPrefab;
    public GameObject enemySkillEffectPrefab;
    public GameObject outOfRange;

    private List<EnemyHealth> enemiesInRange = new List<EnemyHealth>();
    private int currentTargetIndex = 0;

    public bool isMeleeOnCooldown = false; // Viive-tilan seuraaminen
    public SkillDatabase skillDatabase;
    public bool wasCritHit = false;
    public bool wasMagicCritHit = false;

    public GameObject arrowPrefab; // Projektiili-prefab
    public Transform castPoint; // Paikka, josta projektiili syntyy
    public bool isRangedAttack = false;
    public PlayerHealth playerHealth;
    public EquipmentManager equipmentManager;
    public Inventory playerInventory;
    public PlayerStats playerStats;
    public HunterSkillManager hunterSkillManager;
    public EnemyBuffManager enemyBuffManager;
    public bool showEnemyHealthBar = false;
    public AudioSource audioSource;
    public AudioClip shootArrowSound;
    public BuffManager buffManager;
    public BuffDatabase buffDatabase;
    public PlayerMovement playerMovement;


    private void Start()
    {
        playerMovement = FindObjectOfType<PlayerMovement>();
        buffManager = GetComponent<BuffManager>();
        audioSource = gameObject.AddComponent<AudioSource>();
        autoaAttackElement = Element.Neutral;
        playerInventory = FindObjectOfType<Inventory>();
        equipmentManager = FindObjectOfType<EquipmentManager>();
        playerHealth = FindObjectOfType<PlayerHealth>();
        playerStats = FindObjectOfType<PlayerStats>();
        enemyBuffManager = FindObjectOfType<EnemyBuffManager>();
        isAttacking = false;
        outOfRange.SetActive(false);
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }
                
        if (audioSource == null)
        {
            Debug.LogError("AudioSource komponentti puuttuu pelaajalta!");
        }

  
        actionbarPanel = FindObjectOfType<ActionbarPanel>();
        if (actionbarPanel == null)
        {
            Debug.LogError("ActionBar-komponenttia ei löytynyt pelaajan lapsista.");
        }
    }

    void Update()
    {
        attackRange = baseRange + weaponRange + buffRange;
        hitRating = baseHitRating + playerStats.hit;
        if (weaponAttackSpeed < 0.3f){
            weaponAttackSpeed = 1.5f; // attackspeed ilman asetta
        }
        if (weaponAttackSpeed - attackSpeedReduction < 0.1f)
        {
            attackSpeed = 0.1f;  //ranged attakspeed kusee
        }
        else
        {
            attackSpeed = weaponAttackSpeed - attackSpeedReduction;

        }

        
        if(isCasting)
        {
            //FaceEnemy();
            //animator.SetTrigger("isCasting");
           // animator.SetBool("isCastingBool", true);

        }
        if (!isCasting)
        {
            //animator.SetBool("isCastingBool", false);
            
        }
        // Targetoi vihollinen Fire1-näppäimellä (vasen hiiripainike)
        if (Input.GetButtonDown("Fire1"))
        {
            if (!isCasting)
            {
            
               // TargetEnemy();
            }
        }

        // Vaihda kohdetta Tab-näppäimellä
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (!isCasting)
            {
                CycleTargets();
            }
        }

        // Hyökkää, jos pelaaja on jo targetoinut vihollisen ja on tarpeeksi lähellä
        if (Input.GetKeyDown(KeyCode.Q) && targetedEnemy != null && !isAttacking && !isCasting && !targetedEnemy.isDead) // Aloita hyökkäys hiiren oikealla
        {
            float distanceToEnemy = Vector3.Distance(transform.position, targetedEnemy.transform.position);
    
            if (distanceToEnemy <= attackRange)
            {
                if (targetedEnemy.isDead)
                {
                    
                }
                else
                {
                    // Tarkistetaan, että pelaajalla on ase varustettuna
                    Equipment rightHandEquipment = EquipmentManager.instance.currentEquipment[(int)SlotType.RightHand];
                    Equipment leftHandEquipment = EquipmentManager.instance.currentEquipment[(int)SlotType.LeftHand];

                    if (rightHandEquipment == null && leftHandEquipment == null)
                    {
                        
                        return; // Ei voi hyökätä ilman asetta
                    }
                    
                    StartCoroutine(PerformAutoAttack());
                    
                }
            }
            else
            {
                actionbarPanel.ShowInfoText("Out of range!");
                StopMeleeAttack();
            }
        }
        else
        {
           
        }

        if (isAttacking)
        {
           // FaceEnemy();
        }

        if (Input.GetKeyDown(KeyCode.H)) // Keskeytä hyökkäys H-näppäimellä
        {
            StopMeleeAttack();
        }

        // Tarkista näppäimet 1-5 taitojen käyttöön
        for (int i = 0; i < 12; i++)
        {
            if (Input.GetKeyDown((KeyCode)KeyCode.Alpha1 + i))
            {
                if (isCasting)
                {
                    
                }
                else
                {
                    actionbarPanel.UseSkill(i);
                }
                    
                    

                
                //UseSkill(i);
            }
        }
    }
    void FaceEnemy()
    {
        if (targetedEnemy != null) // Tarkista, että vihollinen on targetoituna
        {
            Vector3 direction = (targetedEnemy.transform.position - transform.position).normalized; // Suunta viholliseen
            direction.y = 0; // Poistetaan pystysuuntainen komponentti, jotta kääntyminen on vaakatasossa

            if (direction.magnitude > 0.1f) // Tarkistetaan, että suunta ei ole nolla
            {
                Quaternion lookRotation = Quaternion.LookRotation(direction); // Luodaan rotaatio
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 10f); // Sulava rotaatio
            }
        }
        else
        {
            Debug.LogWarning("No enemy targeted to face.");
        }
    }

    private IEnumerator PerformAutoAttack()
    {
        Debug.Log("autoattack lähtee");
        if (targetedEnemy.isDead)
        {
            
            isAttacking = false;
            yield break;
        }
        isAttacking = true;

     //   while (isAttacking)
     //   {
            if (attackRange > 30 && targetedEnemy != null) // Ranged attack
            {
                isRangedAttack = true;


            }

            float distanceToEnemy = Vector3.Distance(transform.position, targetedEnemy.transform.position);

            if (targetedEnemy == null || targetedEnemy.isDead || distanceToEnemy > attackRange)
            {
                
                StopMeleeAttack();
                yield break;
            }

            if (!isMeleeOnCooldown) // Suorita hyökkäys vain, jos cooldown ei ole käynnissä
            {
                StartCoroutine(MeleeCoolDown());
            }

           yield return new WaitForSeconds(attackSpeed); // Odota attackSpeed ennen seuraavaa hyökkäystä
       // }

        isAttacking = false; // Varmista, että tila nollataan lopuksi
   
    }

        public void StopMeleeAttack()
    {
        
        isAttacking = false;
        isMeleeOnCooldown = false; // Varmista, että cooldown nollataan
        isRangedAttack = false;
        
        animator.ResetTrigger("isMeleeAttacking");
        animator.ResetTrigger("isRangedAttacking");
        animator.SetBool("isAttacking", false);
        animator.speed = 1f;
        StopAllCoroutines(); // Lopettaa kaikki aktiiviset coroutinet
        
    }
    public void NoArrows()
    {
        Debug.Log("Out of arrows");
        actionbarPanel.ShowInfoText("Equip Arrows");
    }

    private IEnumerator MeleeCoolDown()
    {
        if (isMeleeOnCooldown)
        yield break; // Estä päällekkäiset cooldown-korutiinit
        
        //float castTime = attackSpeed; //vaihda tähän attackspeed
        isMeleeOnCooldown = true; // Aloita cooldown-tila
        if (isRangedAttack)
        {       
                Item arrow = equipmentManager.IsArrowEquipped();
                Equipment arrowEquipment = equipmentManager.currentEquipment[(int)SlotType.Arrow];

                if (arrowEquipment != null && arrowEquipment.quantity > 0)
                {
                    // Vähennetään yksi nuoli
                    arrowEquipment.quantity--;

                    // Päivitetään UI:n arrowAmount
                    equipmentManager.arrowCount = arrowEquipment.quantity;
                    equipmentManager.arrowAmount.text = $"{equipmentManager.arrowCount} ea.";

                    // Voit myös käsitellä ammunnan logiikkaa täällä
                    // Esimerkiksi: ShootArrowImplementation();
                }
                else
                {
                                    //playerInventory.RemoveItem(arrow, 1); 
               // Equipment arrow2 = arrow as Equipment; // Muunna Item Equipment-tyypik
               // equipmentManager.UpdateArrowCount(arrow2);
                NoArrows();        
                StopMeleeAttack();
                //StopMeleeAttack();
                yield break;
                
                }

        }
        animator.SetTrigger("isRangedAttacking");
                

        //yield return new WaitForSeconds(castTime); //  viive lähitaisteluiskujen välillä
        actionbarPanel.castBar.fillAmount = 1f;
        actionbarPanel.castBar.gameObject.SetActive(true);
        float elapsed = 0f;
        while (elapsed < attackSpeed)
        {            
            animator.speed = 1f / attackSpeed;
            elapsed += Time.deltaTime;
            actionbarPanel.castBar.fillAmount = 1f - (elapsed / attackSpeed); // Päivitä täyttö
            if (actionbarPanel.castTimeText != null)
            {
                actionbarPanel.castTimeText.text = $"{Mathf.Max(0f, attackSpeed - elapsed):0.0} s"; // Näytä jäljellä oleva aika
            }
            yield return null;
        }
        if (isRangedAttack)
        {
            StartCoroutine(ShootArrows(targetedEnemy, arrowPrefab));  // shoot arrow animation stuff
        }
        else
        {
            animator.SetTrigger("isMeleeAttacking");
            animator.speed = 1f / attackSpeed;
        }
        
        
    // Castaus valmis
        actionbarPanel.castBar.fillAmount = 0f;
        actionbarPanel.castBar.gameObject.SetActive(false);

        StartCoroutine(DealDamageAfterDelayMelee());
        isMeleeOnCooldown = false; // Cooldown-tila päättyy
        isRangedAttack = false;
    }
    private void PlayShootSound()
    {
        if (audioSource != null && shootArrowSound != null)
        {
            
            audioSource.PlayOneShot(shootArrowSound);// Soittaa AudioSourceen asetetun klipin
        }
        else
        {
            Debug.LogWarning("Ääniklipin toisto epäonnistui. Varmista että AudioSource ja ääni ovat asetettu.");
        }
    }

    public IEnumerator<GameObject> ShootArrows(EnemyHealth otherEnemy, GameObject arrowPrefab)
    {
        //animator.SetTrigger("isRangedAttacking");
        //animator.speed = 1f / attackSpeed;

        PlayShootSound();

        if (targetedEnemy == null || otherEnemy == null)
        {
            Debug.LogWarning("Targeted enemy is null. Cancelling arrow shot.");
            yield break;
        }

        Vector3 spawnPosition = castPoint.position + castPoint.forward * 0.5f + castPoint.up * 3.5f;
        Vector3 directionToTarget = (otherEnemy.transform.position - spawnPosition).normalized;

        // Instanssioidaan nuoli annetusta prefabista
        GameObject projectile = Instantiate(arrowPrefab, spawnPosition, Quaternion.identity);

        // Asetetaan nuolen kierto niin, että se osoittaa vihollista kohti
        projectile.transform.forward = directionToTarget;

        // Alusta nuoli omilla arvoilla, kuten nopeus ja suunta
        projectile.GetComponent<Projectile>().Initialize(otherEnemy.transform);

        yield return projectile; // Palautetaan projektiili
    }
    public IEnumerator<GameObject> ShootExplosiveArrows(EnemyHealth otherEnemy, GameObject arrowPrefab, Skill skill, bool isCrit)
    {
    // animator.SetTrigger("isRangedAttacking");
    // animator.speed = 1f / attackSpeed;

        PlayShootSound();

        if (targetedEnemy == null || otherEnemy == null)
        {
            Debug.LogWarning("Targeted enemy is null. Cancelling arrow shot.");
            yield break;
        }

        Vector3 spawnPosition = castPoint.position + castPoint.forward * 0.5f + castPoint.up * 3.5f;
        Vector3 directionToTarget = (otherEnemy.transform.position - spawnPosition).normalized;

        GameObject projectile = Instantiate(arrowPrefab, spawnPosition, Quaternion.identity);
        projectile.transform.forward = directionToTarget;

        // Alustetaan räjähtävä nuoli
        projectile.GetComponent<ExplosiveProjectile>().Initialize(otherEnemy.transform, skill, true, isCrit);

        yield return projectile; // Palautetaan projektiili
    }






    public void UseSkill(int skillIndex)  // viittaa actionbarin slottiin 1-5
    {
        Debug.Log("using skill");
        if (isCasting) 
        {
            Debug.Log("Is casting, ei voi castaa");
            return; // Estä taitojen käyttö, jos castaaminen on jo käynnissä
        }


        if (skillIndex < skillDatabase.GetSkillCount())
        {
            Skill skill = skillDatabase.GetSkillByIndex(skillIndex);

            if (skillDatabase.IsHealType(skill))
            {
                actionbarPanel.UseSkill(skillIndex);
            } 
            else if (skillDatabase.IsDamageType(skill))
            {

            
            if (targetedEnemy != null)
            {
                float distanceToEnemy = Vector3.Distance(transform.position, targetedEnemy.transform.position);
                if (distanceToEnemy <= 40f && skill.noTarget == true )
                {
                    Debug.Log("skill lähtee");
                    outOfRange.SetActive(false);
                   // actionBar.UseSkill(skillIndex);
                   
                    actionbarPanel.UseSkill(skillIndex);
                }
                else
                {
                    Debug.Log("liian kaukana");
                    outOfRange.SetActive(true);
                   
                }
            }
            else
            {
                Debug.Log("No enemy targeted.");
            }
            }
        }
    }

void CycleTargets()
{
    UpdateEnemiesInRange(); // Päivitä lista varmasti

    // Poistetaan kuolleet viholliset
    enemiesInRange.RemoveAll(enemy => enemy == null || enemy.isDead);

    if (enemiesInRange.Count > 1) // Varmista, että on vähintään kaksi validia vihollista
    {
        if (targetedEnemy != null)
        {
            targetedEnemy.targetIndicator.SetActive(false);
        }

        do
        {
            currentTargetIndex = (currentTargetIndex + 1) % enemiesInRange.Count;
        } 
        while (enemiesInRange[currentTargetIndex] == targetedEnemy); // Etsitään eri targetti

        targetedEnemy = enemiesInRange[currentTargetIndex];
        
    }
    else if (enemiesInRange.Count == 1)
    {
        targetedEnemy = enemiesInRange[0]; // Valitaan ainoa vihollinen
        targetedEnemy.targetIndicator.SetActive(true);
    }
    else
    {
       
        targetedEnemy = null;
    }

    // Päivitä avatar ja actionbar, jos uusi target on löytynyt
    if (targetedEnemy != null)
    {
        targetedEnemy.targetIndicator.SetActive(true);

        AvatarManager avatarManager = FindObjectOfType<AvatarManager>();
        if (avatarManager != null)
        {
            avatarManager.avatarPanel?.SetActive(true);
            avatarManager.AssignEnemy(targetedEnemy, targetedEnemy.enemySprite);
        }

        actionbarPanel?.SetTargetedEnemy(targetedEnemy);
    }
}



void UpdateEnemiesInRange()
{
    enemiesInRange.Clear(); // Tyhjennetään lista
    Collider[] colliders = Physics.OverlapSphere(transform.position, targetingRange);
    foreach (var collider in colliders)
    {
        EnemyHealth enemy = collider.GetComponent<EnemyHealth>();
        if (enemy != null && !enemy.isDead) // Lisää vain elossa olevat viholliset
        {
            enemiesInRange.Add(enemy);
        }
    }

    currentTargetIndex = 0; // Palauta alkuun, jos lista on tyhjä
}



public IEnumerator Buff(Skill skill)
{
    if (skill != null)
    {
        Debug.Log("buff kyl lähtee");
        hunterSkillManager.ExecuteSkill(skill);
    }
     yield break;
}

public IEnumerator Attack(Skill skill)
{
    Debug.Log("Attack lähti");
    animator.SetTrigger("isAttacking");
    isAttacking = true;

    if (skill != null)
    {
        hunterSkillManager.ExecuteSkill(skill);
    }
    else
    {
        Debug.Log("Skill oli null");
        yield return StartCoroutine(DealDamageAfterDelayMagic(skill, IsCriticalHit()));
    }

    // Lopetetaan coroutine
    yield break;
}


    public IEnumerator DealDamageAfterDelayMelee()
    {
        if (targetedEnemy != null)
        {
            //Element element = Element.Combat;
            float finalDamage = CalculateDamageMelee();
            //float randomVariance = Random.Range(-5f, 5f); // Satunnainen arvo välillä -5 ja 
           // finalDamage += randomVariance;
           
          
           // Element weaponElement = equipmentManager.WeaponCurrentElement();
            targetedEnemy.TakeDamageMelee(finalDamage, wasCritHit, autoaAttackElement);
           
        }
        yield return null;

        //StartCoroutine(ResetAttack());
    }
    public IEnumerator DealDamageAfterDelaySkill(Skill skill, bool isCrit, EnemyHealth targetEnemy = null)
    {
        
        // Jos targetEnemy on null, käytä targetedEnemy
        if (targetEnemy == null)
        {
            targetEnemy = targetedEnemy;
        }

        

        if (targetEnemy != null)
        {
            
            float finalDamage = CalculateDamageSkill(skill.damage);
            Debug.Log("Final damage " + finalDamage);
            //Element weaponElement = equipmentManager.WeaponCurrentElement();
            targetEnemy.TakeDamageMelee(finalDamage, wasCritHit, autoaAttackElement);
            
            // Toteuta vahinko määritetylle viholliselle
           
        }


        // Käynnistä ResetAttack vain kerran pääkohteelle
        if (targetEnemy == targetedEnemy)
        {
            StartCoroutine(ResetAttack());
        }
        yield return null;
    }

    public IEnumerator DealDamageAfterDelayMagic(Skill skill, bool isCrit, EnemyHealth targetEnemy = null)
    {
        
        // Jos targetEnemy on null, käytä targetedEnemy
        if (targetEnemy == null)
        {
            targetEnemy = targetedEnemy;
        }

        yield return new WaitForSeconds(0.2f);

        if (targetEnemy != null)
        {
            // Toteuta vahinko määritetylle viholliselle
            targetEnemy.TakeDamage(skill, isCrit);
        }



        // Käynnistä ResetAttack vain kerran pääkohteelle
        if (targetEnemy == targetedEnemy)
        {
            StartCoroutine(ResetAttack());
        }
    }


    public IEnumerator ResetAttack()
    {
        Debug.Log("REssataan cast");
        
        
        yield return new WaitForSeconds(0.2f);
        isAttacking = false;
        isCasting = false;
        animator.SetTrigger("notAttacking");
    }

    public void OnDestroy()
    {
        if (targetedEnemy != null)
        {
            //targetedEnemy.HideHealthBar();
        }
    }

    private float CalculateDamageMelee()
    {
        bool isCriticalHit = IsCriticalHit();
        float randomPercentage = Random.Range(0.9f, 1.1f);
        float damage = attackDamage * randomPercentage;
     

        if (isCriticalHit)
        {
            wasCritHit = true;
            damage = (attackDamage * critMultiplier);
            return damage;
        }
        else
        {
        wasCritHit = false;
        }
        return damage;
    }
    private float CalculateDamageSkill(float skillDamage)
    {
        bool isCriticalHit = IsCriticalHit();
        float damage = skillDamage;
     

        if (isCriticalHit)
        {
            wasCritHit = true;
            damage = (damage * critMultiplier);
            return damage;
        }
        else
        {
        wasCritHit = false;
        }
        return damage;
    }

    public float CalculateDamageMagic(float skillDamage)
    {
        bool isCriticalHit = IsCriticalHit();
        float damage = skillDamage;

        if (isCriticalHit)
        {
            wasMagicCritHit = true;
            damage = (skillDamage * critMultiplier);
            return damage;
        }
        else
        {
            wasMagicCritHit = false;
             return damage;
        }

    }

    public bool IsCriticalHit()
    {
        float randomValue = Random.Range(0f, 100f); // Satunnaisluku väliltä 0-100

        return randomValue < critChance; // Kriittinen osuma, jos satunnaisluku on alle critChance:n
    }

}
