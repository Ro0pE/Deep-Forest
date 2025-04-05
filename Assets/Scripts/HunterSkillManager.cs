using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class HunterSkillManager : MonoBehaviour
{
    PlayerAttack playerAttack;
    PlayerStats playerStats;
    PlayerMovement playerMovement;
    BuffManager buffManager;
    public BuffDatabase buffDatabase;
    private Coroutine bleedCoroutine;
    private Dictionary<EnemyHealth, Coroutine> activeBleedCoroutines = new Dictionary<EnemyHealth, Coroutine>();
    public int hawkEyeAgi;
    public int hawkEyeDex;
    public int recoMasterHP;
    public int recoMasterSP;
    public float windrunnerSpeed;
    public float trapDistance = 5f; // Etäisyys, johon trap asetetaan
    public LayerMask groundLayer; // Maa-kerros, johon ansa voidaan asettaa
    public GameObject trapPrefab;
    public AudioSource audioSource;
    public AudioClip windRunnerSound;
    private Renderer enemyRenderer;
    private Color originalColor; // Tallennetaan alkuperäinen väri
    [SerializeField] private GameObject rainOfArrowPrefab;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        playerAttack = FindObjectOfType<PlayerAttack>();
        playerStats = FindObjectOfType<PlayerStats>();
        playerMovement = FindObjectOfType<PlayerMovement>();
        buffManager = FindObjectOfType<BuffManager>();
        enemyRenderer = GetComponent<Renderer>();


    }

    // Update is called once per frame
    void Update()
    {
        
    }
            
    

    private void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            
            audioSource.PlayOneShot(clip);// Soittaa AudioSourceen asetetun klipin
        }
        else
        {
            Debug.LogWarning("Ääniklipin toisto epäonnistui. Varmista että AudioSource ja ääni ovat asetettu.");
        }
    }
private IEnumerator ApplyBleedDamage(EnemyHealth targetEnemy, Skill skill)
{
    Debug.Log("BLEED DAMAGE LÄHTEE: " + targetEnemy);
    float tickInterval = 1f; // Vahinkoa kerran sekunnissa

    while (true)
    {
        // Tarkista BuffManager ja Bleed-buff
        EnemyBuffManager buffManager = targetEnemy.GetComponent<EnemyBuffManager>();
        if (buffManager == null)
        {
            Debug.LogError("EnemyBuffManager not found on target!");
            yield break;
        }

        Buff bleedBuff = buffManager.activeBuffs.Find(b => b.name == "Bleed");
        if (bleedBuff == null || bleedBuff.duration <= 0f)
        {
            Debug.Log($"Bleed effect ended for: {targetEnemy}");
            yield break; // Lopeta korutiini, kun buffi ei ole enää aktiivinen
        }

        // Vahingon käsittely
        Debug.Log($"Dealing bleed damage from {skill.skillName}, BaseDamage: {skill.baseDamage} to target: {targetEnemy}");
        targetEnemy.TakeDamage(skill, false);

        yield return new WaitForSeconds(tickInterval);
    }
}

private void ApplyBleedEffect(EnemyHealth targetEnemy, Skill skill)
{
    Debug.Log($"Applying bleed effect to: {targetEnemy}");

    // Tarkista, onko kohteella jo aktiivinen korutiini
    if (activeBleedCoroutines.TryGetValue(targetEnemy, out Coroutine existingCoroutine))
    {
        StopCoroutine(existingCoroutine); // Pysäytä aiempi korutiini
    }

    // Käynnistä uusi korutiini ja lisää se sanakirjaan
    Coroutine newCoroutine = StartCoroutine(ApplyBleedDamage(targetEnemy, skill));
    activeBleedCoroutines[targetEnemy] = newCoroutine;
}

private void RemoveBleedEffect(EnemyHealth targetEnemy, Skill skill)
{
    Debug.Log($"Removing bleed effect from: {targetEnemy}");

    // Lopeta ja poista aktiivinen korutiini, jos sellainen löytyy
    if (activeBleedCoroutines.TryGetValue(targetEnemy, out Coroutine existingCoroutine))
    {
        StopCoroutine(existingCoroutine);
        activeBleedCoroutines.Remove(targetEnemy);
    }
}
    private void ApplyWindrunnerEffect(Skill skill)
    {
        windrunnerSpeed = playerMovement.originalSpeed * 2f;
        playerMovement.SetPlayerSpeed(windrunnerSpeed);
        
    }

    private void RemoveWindrunnerEffect(Skill skill)
    {
        playerMovement.ReturnPlayerSpeed();
    }
private void ApplySlowEffect(EnemyHealth targetEnemy)
{
    GameObject frozenEffectPrefab = Resources.Load<GameObject>("Explosions/FrozenEffect");
    if (frozenEffectPrefab != null)
    {
        if (targetEnemy.frozenEffectInstance == null) // Tarkistetaan, ettei efektiä ole jo
        {
            GameObject frozenEffect = Instantiate(frozenEffectPrefab, targetEnemy.transform.position, Quaternion.identity);
            frozenEffect.transform.SetParent(targetEnemy.transform, false);
            frozenEffect.transform.localPosition = Vector3.zero;

            targetEnemy.frozenEffectInstance = frozenEffect; // Tallennetaan viite EnemyHealthiin
        }
    }
    if (targetEnemy.enemyRenderer != null)
    {
        Debug.Log("Laitetaan vähän sinistä väriä");
        Color frostColor = Color.Lerp(targetEnemy.originalColor, Color.blue, 0.5f); // 50% sinisemmäksi
        targetEnemy.enemyRenderer.material.color = frostColor;
        
    }

    targetEnemy.enemyAI.walkSpeed *= 0.5f;
    targetEnemy.enemyAI.runSpeed *= 0.5f;
}

private void RemoveSlowEffect(EnemyHealth targetEnemy)
{
    targetEnemy.enemyAI.walkSpeed = targetEnemy.enemyAI.orginalWalkSpeed;
    targetEnemy.enemyAI.runSpeed = targetEnemy.enemyAI.orginalRunSpeed;
     if (targetEnemy.enemyRenderer != null)
        {
            targetEnemy.enemyRenderer.material.color = targetEnemy.originalColor; // Palautetaan alkuperäinen väri
        }
    if (targetEnemy.frozenEffectInstance != null)
    {
        Destroy(targetEnemy.frozenEffectInstance);
        targetEnemy.frozenEffectInstance = null; // Nollataan viite, jotta se voidaan asettaa uudelleen
    }
}
    private void ApplyHuntersResilienceBuff(Skill skill)
    {
        Debug.Log("Hunters resi päällä!");
    }

    private void RemoveHuntersResilienceBuff(Skill skill)
    {
        Debug.Log("Hunters resi haihtuuu");

    }
    private void ApplyLeechingEffect(Skill skill)
    {
        Debug.Log("Leech  päällä!");
    }

    private void RemoveLeechingEffect(Skill skill)
    {
        Debug.Log("Leech  haihtuuu");

    }
    private void ApplyStunEffect(EnemyHealth targetEnemy)
    {
        targetEnemy.isStunned = true;
        targetEnemy.agent.isStopped = true;
        targetEnemy.animator.SetBool("isStunned", true);
        
    }

    private void RemoveStunEffect(EnemyHealth targetEnemy)
    {
        Debug.Log("Removing stun");
        targetEnemy.isStunned = false;
        targetEnemy.animator.SetBool("isStunned", false);
        targetEnemy.agent.isStopped = false;
    }
    private void ApplyHawkEye(Skill skill)
    {
        Debug.Log("Apply haek eye metod");
        hawkEyeAgi = Mathf.RoundToInt(playerStats.totalAgi * 0.25f);
        hawkEyeDex = Mathf.RoundToInt(playerStats.totalDex * 0.25f);
        playerStats.buffAgi = playerStats.buffAgi + hawkEyeAgi;
        playerStats.buffDex = playerStats.buffDex + hawkEyeDex;
        playerStats.UpdateStatTexts();
        playerStats.UpdateStats();
        
    }

    private void RemoveHawkEye(Skill skill)
    {
        playerStats.buffAgi = playerStats.buffAgi - hawkEyeAgi;
        playerStats.buffDex = playerStats.buffDex - hawkEyeDex;
        playerStats.UpdateStatTexts();
        playerStats.UpdateStats();
        hawkEyeAgi = 0;
        hawkEyeDex = 0;

    }
        private void ApplyRecoveryMasteryEffect(Skill skill)
    {
        recoMasterHP = 20;
        recoMasterSP = 20;
       
        playerStats.buffHP = playerStats.buffHP + recoMasterHP;
        playerStats.buffSP = playerStats.buffSP + recoMasterSP;
        playerStats.AddBuffStatHP();
        playerStats.AddBuffStatSP();
        
    }

    private void RemoveRecoveryMasteryEffect(Skill skill)
    {
        playerStats.buffHP = (playerStats.buffHP - recoMasterHP);
        playerStats.buffSP = (playerStats.buffSP - recoMasterSP);
        playerStats.RemoveBuffStatHP();
        playerStats.RemoveBuffStatSP();
        recoMasterHP = 0;
        recoMasterSP = 0;
    }


    public void ExecuteSkill(Skill skill)
    {
        
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
        else if (skill.skillName == "Hawk Eye")
        {
           
            StartCoroutine(HawkEye(skill));
        }
        else if (skill.skillName == "Bleeding Strike")
        {
           
            StartCoroutine(BleedingStrike(skill));
        }
        else if (skill.skillName == "Recovery Mastery")
        {
           
            StartCoroutine(RecoveryMastery(skill));
        }
        else if (skill.skillName == "Windrunner")
        {
           
            StartCoroutine(Windrunner(skill));
        }
        else if (skill.skillName == "Chillshot")
        {
           
            StartCoroutine(Chillshot(skill));
        }  
        else if (skill.skillName == "Rapid Barrage")
        {
           
            StartCoroutine(RapidBarrage(skill));
        }
        else if (skill.skillName == "Meteor Arrows")
        {
           
            StartCoroutine(MeteorArrows(skill));
        }  
        else if (skill.skillName == "Freezing Trap")
        {
           
            StartCoroutine(FreezingTrap(skill));
        }
        else if (skill.skillName == "Shock Trap")
        {
           
            StartCoroutine(ShockTrap(skill));
        }
        else if (skill.skillName == "Bonecrusher Trap")
        {
           
            StartCoroutine(BonecrusherTrap(skill));
        } 
        else if (skill.skillName == "Hunter's Resilience")
        {
           
            StartCoroutine(HuntersResilience(skill));
        }  
        else if (skill.skillName == "Shadow Step")
        {
           
            StartCoroutine(ShadowStep(skill));
        } 
        else if (skill.skillName == "Leeching Arrows")
        {
           
            StartCoroutine(LeechingArrows(skill));
        }
        else if (skill.skillName == "Sharp Shooter")
        {
           
            StartCoroutine(SharpShooter(skill));
        }  
        else if (skill.skillName == "Rain of Arrows")
        {
           
            StartCoroutine(RainOfArrows(skill));
        }
        else if (skill.skillName == "Ice Trap")
        {
           
            StartCoroutine(IceTrap(skill));
        }                                                                       
        else
        {
            Debug.Log("Invalid skill");
        }

    }

    public void MultiArrow(Skill skill)
    {       
        
    // Laukaistaan nuoli kohteeseen
    playerAttack.StartCoroutine(playerAttack.ShootArrows(playerAttack.targetedEnemy,playerAttack.arrowPrefab));
    playerAttack.StartCoroutine(playerAttack.DealDamageAfterDelaySkill(skill, playerAttack.IsCriticalHit()));

    // Tarkistetaan vahinkosäde kohteen ympäriltä
    if (skill.damageRange > 0 && playerAttack.targetedEnemy != null)
    {
        Collider[] hitColliders = Physics.OverlapSphere(playerAttack.targetedEnemy.transform.position, skill.damageRange);
        

        foreach (Collider hitCollider in hitColliders)
        {
            // Tarkista, onko kyseessä toinen vihollinen
            EnemyHealth otherEnemy = hitCollider.GetComponent<EnemyHealth>();
            if (otherEnemy != null && otherEnemy != playerAttack.targetedEnemy && !otherEnemy.isDead) // Ei tehdä vahinkoa alkuperäiselle kohteelle
            {
                Debug.Log("HIT " + otherEnemy);
                // Tee vahinkoa muille vihollisille rinnakkain
                playerAttack.StartCoroutine(playerAttack.DealDamageAfterDelaySkill(skill, playerAttack.IsCriticalHit(), otherEnemy));

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
        yield return playerAttack.StartCoroutine(playerAttack.DealDamageAfterDelaySkill(skill, playerAttack.IsCriticalHit()));
        yield return playerAttack.StartCoroutine(playerAttack.DealDamageAfterDelaySkill(skill, playerAttack.IsCriticalHit()));
    }
public void SpawnArrowRainEffect(Transform enemyTarget)
{
    int arrowCount = 12;
    float spawnRadius = 10f;
    float heightAboveEnemy = 20f;
    float heightVariation = 5f; // Satunnainen korkeuden vaihtelu

    // Efekti, joka luodaan tippumiskohdassa
    GameObject spellEffect = Resources.Load<GameObject>("Explosions/RainArrowLocation");

    // Laske alueen keskikohta, johon efektit tulevat
    Vector3 effectCenter = enemyTarget.position + new Vector3(0, 0, 0); // Keskikohta (tässä oletetaan, että se on vihollisen sijainti, mutta se voidaan muuttaa)

    // Lisätään pieni nostokorjaus efektin korkeuteen
    float heightOffset = 1f; // Pieni nostokorjaus, voit säätää tätä arvoa
    effectCenter.y += heightOffset; // Nostataan efektiä ylös

    // Luo yksi suuri efekti alueelle
    Instantiate(spellEffect, effectCenter, Quaternion.identity);

    for (int i = 0; i < arrowCount; i++)
    {
        Vector3 randomOffset = new Vector3(
            Random.Range(-spawnRadius, spawnRadius),
            0,  // X ja Z ovat satunnaisia, mutta korkeus (Y) vaihdetaan alla
            Random.Range(-spawnRadius, spawnRadius)
        );

        // Satunnainen korkeus, joka vaihtelee heightAboveEnemy +/- heightVariation
        float randomHeight = Random.Range(heightAboveEnemy - heightVariation, heightAboveEnemy + heightVariation);
        
        // Asetetaan spawnPosition käyttämään tätä satunnaista korkeutta
        Vector3 spawnPosition = enemyTarget.position + new Vector3(0, randomHeight, 0) + randomOffset;

        // Nuolten rotaatio, joka osoittaa suoraan alaspäin
        Quaternion rotation = Quaternion.LookRotation(Vector3.down);

        // Luo nuoli
        Instantiate(rainOfArrowPrefab, spawnPosition, rotation);
    }
}







    public IEnumerator RainOfArrows(Skill skill)
    {
        EnemyHealth firstTarget = playerAttack.targetedEnemy;
        playerAttack.isAttacking = false; // nollataan castit että pelaaja voi liikkua
        playerAttack.isCasting = false;
        if (firstTarget == null) yield break; // Varmistetaan, että kohde on olemassa
        
        int wave = 0;
        int maxWaves = 4;
        WaitForSeconds waitBetweenWaves = new WaitForSeconds(0.2f);

        while (wave < maxWaves)
        {  
            SpawnArrowRainEffect(firstTarget.transform);
            yield return new WaitForSeconds(1f); // Odotetaan, että nuolet putoavat maahan
            
            // Nyt haetaan viholliset alueelta ja tehdään vahinko
            Collider[] hitColliders = Physics.OverlapSphere(firstTarget.transform.position, skill.damageRange);
            List<EnemyHealth> enemiesInRange = TakeAllEnemiesInRange(hitColliders);

            // Vahinko tulee vasta kun nuolet ovat maassa
            foreach (EnemyHealth enemy in enemiesInRange)
            {
                if (enemy != null && !enemy.isDead) // Tarkistetaan, onko vihollinen vielä hengissä
                {
                    yield return StartCoroutine(playerAttack.DealDamageAfterDelaySkill(skill, playerAttack.IsCriticalHit(), enemy));
                }
            }

            wave++;
            yield return waitBetweenWaves; // Odota ennen seuraavaa aaltoa
        }

        yield return null;
    }


    public IEnumerator LeechingArrows(Skill skill)
    {
        // Ensimmäinen nuoli ja vahinko
            Buff leechingArrowData = buffDatabase.GetBuffByName("LifeLeech");
            //GameObject stunningArrowPrefab = Resources.Load<GameObject>("Projectiles/StunningArrowPrefab");
            //EnemyHealth stunnedEnemy = playerAttack.targetedEnemy;
            //EnemyBuffManager targetBuffManager = stunnedEnemy.GetComponent<EnemyBuffManager>();
            Buff leechBuff = new Buff(
                leechingArrowData.name,
                leechingArrowData.duration,
                leechingArrowData.isStackable,
                leechingArrowData.stacks,
                leechingArrowData.buffIcon,
                BuffType.Buff, // Tämä on debuff
                leechingArrowData.damage,
                leechingArrowData.effectText,
                leechingArrowData.effectValue,
                () => ApplyLeechingEffect(skill), // Käytetään lambda-funktiota
                () => RemoveLeechingEffect(skill) // Sama täällä
                );

                if (leechBuff == null)
                {
                    Debug.LogError("Stun-buff creation failed!");
                }
                else
                {
                    
                    
                    buffManager.AddBuff(leechBuff);
                }
    yield return null; 
    }
    public IEnumerator ShadowStep(Skill skill)
    {
        Debug.Log("SHADOW STEPP!!");

        // Skillin vaikutus: teleporttaa pelaajan eteenpäin 14 yksikköä kameran suuntaan
        Vector3 currentPosition = transform.position;  // Pelaajan nykyinen sijainti

        // Käytetään kameran eteenpäin suuntaa liikkumiseen
        Vector3 cameraForward = playerMovement.mainCamera.transform.forward; // Kameran suunta
        cameraForward.y = 0; // Poistetaan y-akselin vaikutus, jotta liikkuminen on tasossa
        cameraForward.Normalize(); // Normalisoidaan suunta, jotta liikkuminen on tasapainoista

        Vector3 targetPosition = currentPosition + cameraForward * (skill.skillLevel * 1f);  // Liikkuminen 14 yksikköä kameran eteenpäin

        Debug.Log($"Current Position: {currentPosition}");
        Debug.Log($"Target Position: {targetPosition}");

        // Saadaan CharacterController-komponentti pelaajalta
        CharacterController controller = playerAttack.GetComponent<CharacterController>();  // Käytetään pelaajan omaa CharacterControlleria
        if (controller != null)
        {
            float duration = 0.5f; // Liikkumisen kesto
            float elapsedTime = 0f;

            // Interpolaatio liikkeelle
            while (elapsedTime < duration)
            {
                float step = (elapsedTime / duration);
                Debug.Log($"Elapsed Time: {elapsedTime}, Step: {step}");

                Vector3 moveVector = Vector3.Lerp(currentPosition, targetPosition, step) - currentPosition;
                Debug.Log($"Move Vector: {moveVector}");

                controller.Move(moveVector);  // Liikuttaa pelaajaa kohti kohdetta
                elapsedTime += Time.deltaTime;  // Lisätään kulunut aika

                yield return null;  // Odotetaan seuraavaa framea
            }

            // Varmistetaan, että saavutetaan tarkka kohde
            controller.Move(targetPosition - currentPosition);
            Debug.Log("Final Position Reached.");
        }
        else
        {
            // Jos CharacterControlleria ei löydy, teleportataan suoraan
            transform.position = targetPosition;
            Debug.Log("No CharacterController found. Teleporting directly.");
        }

        yield return null;
    }


public IEnumerator StunningArrow(Skill skill)
{
    Buff stunData = buffDatabase.GetBuffByName("Stun");
    GameObject stunningArrowPrefab = Resources.Load<GameObject>("Projectiles/StunningArrowPrefab");
    EnemyHealth stunnedEnemy = playerAttack.targetedEnemy;
    EnemyBuffManager targetBuffManager = stunnedEnemy.GetComponent<EnemyBuffManager>();
    Buff stunBuf = new Buff(
        stunData.name,
        skill.skillLevel,
        stunData.isStackable,
        stunData.stacks,
        stunData.buffIcon,
        BuffType.Debuff, // Tämä on debuff
        stunData.damage,
        stunData.effectText,
        stunData.effectValue,
        () => ApplyStunEffect(stunnedEnemy), // Käytetään lambda-funktiota
        () => RemoveStunEffect(stunnedEnemy) // Sama täällä
        );

        if (stunBuf == null)
        {
            Debug.LogError("Stun-buff creation failed!");
        }
        else
        {
            
            
            targetBuffManager.AddBuff(stunBuf);
        }

        yield return playerAttack.StartCoroutine(playerAttack.ShootArrows(playerAttack.targetedEnemy, stunningArrowPrefab));
        yield return playerAttack.StartCoroutine(playerAttack.DealDamageAfterDelaySkill(skill, playerAttack.IsCriticalHit()));
        
      
        
        
    }
    
    public IEnumerator SharpShooter(Skill skill)
    {
        Animator animator = playerAttack.GetComponent<Animator>();
        animator.SetTrigger("RapidShooting");
       // playerAttack.animator.speed = 1f / skill.castTime;
        // Lataa tai aseta BullsEyeArrow-prefab
        GameObject sharpShooterArrowPrefab = Resources.Load<GameObject>("Projectiles/SharpShooterArrow");

        if (sharpShooterArrowPrefab == null)
        {
            Debug.LogError("BullsEyeArrow prefab not found!");
            yield break;
        }

        // Käynnistetään ShootArrows coroutine ja odotetaan sen palauttamista
        GameObject projectile = null;
       Debug.Log("SHGARP SHOOYER");
        // Odotetaan ShootArrows coroutinea ja palautetaan projektiili

        animator.ResetTrigger("RapidShooting");
        yield return StartCoroutine(playerAttack.ShootArrows(playerAttack.targetedEnemy, sharpShooterArrowPrefab));
        // Vahingon käsittely
        yield return playerAttack.StartCoroutine(playerAttack.DealDamageAfterDelaySkill(skill, playerAttack.IsCriticalHit()));
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
        yield return playerAttack.StartCoroutine(playerAttack.DealDamageAfterDelaySkill(skill, playerAttack.IsCriticalHit()));
    }
    public IEnumerator HawkEye(Skill skill)
    {
            Buff hawkEyeData = buffDatabase.GetBuffByName("HawkEye");
            Debug.Log("Hawk eye duration " + hawkEyeData.duration * skill.skillLevel);
            Buff hawkBuf = new Buff(
                hawkEyeData.name,
                (hawkEyeData.duration * skill.skillLevel), // TESTAA! 
                hawkEyeData.isStackable,
                hawkEyeData.stacks,
                hawkEyeData.buffIcon,
                BuffType.Buff, // Tämä on buff
                hawkEyeData.damage,
                hawkEyeData.effectText,
                hawkEyeData.effectValue,
                () => ApplyHawkEye(skill), // Käytetään lambda-funktiota
                () => RemoveHawkEye(skill) // Sama täällä
                );
                buffManager.AddBuff(hawkBuf);
    yield return null; // Palauttaa null, koska coroutine odottaa palautusta.
    }
public IEnumerator BleedingStrike(Skill skill)
{
    Buff bleedData = buffDatabase.GetBuffByName("Bleed");
    if (playerAttack.targetedEnemy == null)
    {
        Debug.LogError("playerAttack.targetedEnemy is null!");
        yield break; // Lopeta korutiini
    }

    if (skill.damageRange > 0f)
    {
        Collider[] hitColliders = Physics.OverlapSphere(playerAttack.targetedEnemy.transform.position, skill.damageRange);
        

        foreach (Collider hitCollider in hitColliders)
        {
            EnemyHealth otherEnemy = hitCollider.GetComponent<EnemyHealth>();
                if (otherEnemy == null)
                {
                    
                    continue; // Ohita osuma, jos ei ole vihollinen
                }
                if (!hitCollider.CompareTag("Enemy"))
                {
                    continue; // Ohita collider, jos tagi ei ole "Enemy"
                }

                
                if (otherEnemy == null)
                {
                    continue; // Ohita collider, jos siinä ei ole EnemyHealth-komponenttia
                }

                
            if (otherEnemy != null)
            {
                EnemyBuffManager otherBuffManager = otherEnemy.GetComponent<EnemyBuffManager>();
                if (otherBuffManager != null)
                {
                    Buff newBleedBuff = new Buff(
                        bleedData.name,
                        bleedData.duration,
                        bleedData.isStackable,
                        bleedData.stacks,
                        bleedData.buffIcon,
                        BuffType.Debuff,
                        bleedData.damage,
                        bleedData.effectText,
                        bleedData.effectValue,
                        () => ApplyBleedEffect(otherEnemy, skill),
                        () => RemoveBleedEffect(otherEnemy, skill)
                    );
                    
                    otherBuffManager.AddBuff(newBleedBuff);
                }
            }
        }
    }

    yield return null;
}
    public IEnumerator HuntersResilience(Skill skill)
    {
        Debug.Log("Hunters res skill inc");
            Buff huntersResilienceBuffData = buffDatabase.GetBuffByName("HuntersResilience");
       
            Buff huntersResilienceBuff = new Buff(
                huntersResilienceBuffData.name,
                huntersResilienceBuffData.duration,
                huntersResilienceBuffData.isStackable,
                huntersResilienceBuffData.stacks,
                huntersResilienceBuffData.buffIcon,
                BuffType.Buff, // Tämä on buff
                huntersResilienceBuffData.damage,
                huntersResilienceBuffData.effectText,
                huntersResilienceBuffData.effectValue,
                () => ApplyHuntersResilienceBuff(skill), // Käytetään lambda-funktiota
                () => RemoveHuntersResilienceBuff(skill) // Sama täällä
                );
                buffManager.AddBuff(huntersResilienceBuff);
                Debug.Log("Buff listätyy");
    yield return null; // Palauttaa null, koska coroutine odottaa palautusta.
    }
    public IEnumerator RecoveryMastery(Skill skill)
    {
            Buff recoveryMasterData = buffDatabase.GetBuffByName("RecoveryMastery");
       
            Buff recoMasterBuff = new Buff(
                recoveryMasterData.name,
                recoveryMasterData.duration,
                recoveryMasterData.isStackable,
                recoveryMasterData.stacks,
                recoveryMasterData.buffIcon,
                BuffType.Buff, // Tämä on buff
                recoveryMasterData.damage,
                recoveryMasterData.effectText,
                recoveryMasterData.effectValue,
                () => ApplyRecoveryMasteryEffect(skill), // Käytetään lambda-funktiota
                () => RemoveRecoveryMasteryEffect(skill) // Sama täällä
                );
                buffManager.AddBuff(recoMasterBuff);
    yield return null; // Palauttaa null, koska coroutine odottaa palautusta.
    }
    public IEnumerator Windrunner(Skill skill)
    {
            PlaySound(windRunnerSound);
            Buff windrunnerBuffData = buffDatabase.GetBuffByName("WindrunnerSpeed");
       
            Buff windrunnerBuff = new Buff(
                windrunnerBuffData.name,
                windrunnerBuffData.duration,
                windrunnerBuffData.isStackable,
                windrunnerBuffData.stacks,
                windrunnerBuffData.buffIcon,
                BuffType.Buff, // Tämä on buff
                windrunnerBuffData.damage,
                windrunnerBuffData.effectText,
                windrunnerBuffData.effectValue,
                () => ApplyWindrunnerEffect(skill), // Käytetään lambda-funktiota
                () => RemoveWindrunnerEffect(skill) // Sama täällä
                );
                buffManager.AddBuff(windrunnerBuff);
               GameObject windrunnerEffectPrefab = Resources.Load<GameObject>("UI_Effects/windrunnerEffect");
                if (windrunnerEffectPrefab != null)
                {
                    GameObject windrunnerEffect = Instantiate(windrunnerEffectPrefab, playerAttack.transform.position, Quaternion.identity);
                    windrunnerEffect.transform.SetParent(playerAttack.transform, false);
                    windrunnerEffect.transform.localPosition = Vector3.zero;
                    Destroy(windrunnerEffect, windrunnerBuffData.duration);
                }
    yield return null; // Palauttaa null, koska coroutine odottaa palautusta.
    }
    public IEnumerator Chillshot(Skill skill)
    {
        GameObject chillingShotArrowPrefab = Resources.Load<GameObject>("Projectiles/ChillingShotArrowPrefab");
        GameObject projectile = null;

        // Odotetaan ShootArrows coroutinea ja palautetaan projektiili
        yield return StartCoroutine(playerAttack.ShootArrows(playerAttack.targetedEnemy, chillingShotArrowPrefab));

            playerAttack.StartCoroutine(playerAttack.DealDamageAfterDelaySkill(skill, playerAttack.IsCriticalHit()));
            EnemyHealth stunnedEnemy = playerAttack.targetedEnemy;
            EnemyBuffManager targetBuffManager = stunnedEnemy.GetComponent<EnemyBuffManager>();
            Buff slowBuffData = buffDatabase.GetBuffByName("Slow");

       
            Buff slowBuff = new Buff(
                slowBuffData.name,
                skill.skillLevel,// Fixed duration, 4skillLevel
                slowBuffData.isStackable,
                slowBuffData.stacks,
                slowBuffData.buffIcon,
                BuffType.Buff, // Tämä on buff
                slowBuffData.damage,
                slowBuffData.effectText,
                slowBuffData.effectValue,
                () => ApplySlowEffect(stunnedEnemy), // Käytetään lambda-funktiota
                () => RemoveSlowEffect(stunnedEnemy) // Sama täällä
                );
                
                if (slowBuff == null)
                {
                    Debug.LogError("slow-buff creation failed!");
                }
                else
                {                   
                    targetBuffManager.AddBuff(slowBuff);
                }
    yield return null; // Palauttaa null, koska coroutine odottaa palautusta.
    }

    public IEnumerator RapidBarrage(Skill skill)
    {
        Animator animator = playerAttack.GetComponent<Animator>();
        animator.SetTrigger("RapidShooting");  // Tai käytä boolia
        GameObject rapidBarrageEffectPrefab = Resources.Load<GameObject>("UI_Effects/rapidBarrageEffect");
        GameObject rapidBarrageEffect = null; // Määritellään täällä, jotta se on käytettävissä koko metodissa

        if (rapidBarrageEffectPrefab != null)
        {
            rapidBarrageEffect = Instantiate(rapidBarrageEffectPrefab, playerAttack.transform.position, Quaternion.identity);
            rapidBarrageEffect.transform.SetParent(playerAttack.transform, false);
            rapidBarrageEffect.transform.localPosition = Vector3.zero;
            
        }
        for (int i = 0; i < 10; i++)
        {
            playerAttack.isChanneling = true;

            yield return playerAttack.StartCoroutine(playerAttack.ShootArrows(playerAttack.targetedEnemy, playerAttack.arrowPrefab));

            // Lisätään vahinko jokaisen nuolen jälkeen
            yield return playerAttack.StartCoroutine(playerAttack.DealDamageAfterDelaySkill(skill, playerAttack.IsCriticalHit()));

            if (playerAttack.targetedEnemy.isDead)
            {
                Debug.Log("Vihu kuoli barrageen, break");
                break;
            }

            // Viive ennen seuraavaa nuolta
            yield return new WaitForSeconds(0.1f);
        };
        playerAttack.isChanneling = false;
        animator.ResetTrigger("RapidShooting");
        Destroy(rapidBarrageEffect);
    }

public IEnumerator MeteorArrows(Skill skill)
{
    GameObject explosiveArrowPrefab = Resources.Load<GameObject>("Projectiles/ExplosiveArrow");
    if (playerAttack.targetedEnemy == null)
    {
        yield break;
    }
        yield return StartCoroutine(playerAttack.ShootExplosiveArrows(playerAttack.targetedEnemy, explosiveArrowPrefab, skill, playerAttack.IsCriticalHit()));
        yield return StartCoroutine(playerAttack.DealDamageAfterDelaySkill(skill, playerAttack.IsCriticalHit(),playerAttack.targetedEnemy));
        yield return new WaitForSeconds(0.1f);
    Collider[] hitColliders = Physics.OverlapSphere(playerAttack.targetedEnemy.transform.position, skill.damageRange);
    List<EnemyHealth> closestEnemies = TakeClosestEnemies(hitColliders, 3);

    foreach (EnemyHealth enemy in closestEnemies)
    {
        yield return StartCoroutine(playerAttack.ShootExplosiveArrows(enemy, explosiveArrowPrefab, skill, playerAttack.IsCriticalHit()));
        yield return StartCoroutine(playerAttack.DealDamageAfterDelaySkill(skill, playerAttack.IsCriticalHit(),enemy));
        yield return new WaitForSeconds(0.3f);
    }
}




public IEnumerator FreezingTrap(Skill skill)
{

    playerAttack.isAttacking = false;  // nollaa hyökkäys
    GameObject trapObject = PlaceTrap(skill);
    Trap trapComponent = trapObject.GetComponent<Trap>();

    if (trapComponent != null)
    {
        trapComponent.OnTrapActivated += (EnemyHealth enemyHealth) =>
        {
            
            GameObject freezeEffectPrefab = Resources.Load<GameObject>("Explosions/FreezingTrapEffect");
            if (freezeEffectPrefab != null)
            {
                GameObject freezeEffect = Instantiate(freezeEffectPrefab, trapObject.transform.position, Quaternion.identity);
                Destroy(freezeEffect, 2f); // Poistetaan efekti 3 sekunnin kuluttua
            }
            else
            {
                Debug.LogError("❌ FreezingTrapEffect ei löytynyt Resources-kansiosta!");
            }
            
            Collider[] hitColliders = Physics.OverlapSphere(trapObject.transform.position, skill.damageRange);
            foreach (Collider hitCollider in hitColliders)
            {
                EnemyHealth targetEnemyHealth = hitCollider.GetComponent<EnemyHealth>();
                if (targetEnemyHealth != null)
                {
                    EnemyBuffManager targetBuffManager = targetEnemyHealth.GetComponent<EnemyBuffManager>();
                    Buff slowBuffData = buffDatabase.GetBuffByName("Slow");

                    Buff slowBuff = new Buff(
                        slowBuffData.name,
                        skill.skillLevel * 2, // Kiinteä kesto, 5 sekuntia
                        slowBuffData.isStackable,
                        slowBuffData.stacks,
                        slowBuffData.buffIcon,
                        BuffType.Buff, // Tämä on buff
                        slowBuffData.damage,
                        slowBuffData.effectText,
                        slowBuffData.effectValue,
                        () => ApplySlowEffect(targetEnemyHealth),
                        () => RemoveSlowEffect(targetEnemyHealth)
                    );

                    if (slowBuff == null)
                    {
                        Debug.LogError("slow-buff creation failed!");
                    }
                    else
                    {                   
                        targetBuffManager.AddBuff(slowBuff);
                        
                    }
                }
            }
        };
    }
    playerAttack.isCasting = false;
    yield return null;
}

public IEnumerator ShockTrap(Skill skill)
{
    playerAttack.isAttacking = false;  // nollaa hyökkäys
    GameObject trapObject = PlaceTrap(skill);
    Trap trapComponent = trapObject.GetComponent<Trap>();

    if (trapComponent != null)
    {
        trapComponent.OnTrapActivated += (EnemyHealth enemyHealth) =>
        {
            Debug.Log("⚡ Shock Trap aktivoitu! Sähköshokki osui: " + enemyHealth.monsterName);
            
            // Näytetään shokkiefekti
            GameObject shockTrapEffect = Resources.Load<GameObject>("Explosions/ShockTrapEffect");
            if (shockTrapEffect != null)
            {
                GameObject effectInstance = Instantiate(shockTrapEffect, trapObject.transform.position, Quaternion.identity);
                Destroy(effectInstance, 5f); 
            }
            else
            {
                Debug.LogError("❌ ShockTrapEffect ei löytynyt Resources-kansiosta!");
            }

            // 10% mahdollisuus lisätä Stun
            if (Random.value > 0.9f)
            {
                Buff stunData = buffDatabase.GetBuffByName("Stun");
                EnemyBuffManager targetBuffManager = enemyHealth.GetComponent<EnemyBuffManager>();

                Buff stunBuff = new Buff(
                    stunData.name,
                    skill.skillLevel,
                    stunData.isStackable,
                    stunData.stacks,
                    stunData.buffIcon,
                    BuffType.Debuff,
                    stunData.damage,
                    stunData.effectText,
                    stunData.effectValue,
                    () => ApplyStunEffect(enemyHealth),
                    () => RemoveStunEffect(enemyHealth)
                );

                if (stunBuff == null)
                {
                    Debug.LogError("❌ Stun-buffin luonti epäonnistui!");
                }
                else
                {
                    targetBuffManager.AddBuff(stunBuff);
                    Debug.Log("✅ Stun lisätty: " + enemyHealth.monsterName);
                }
            }

            // **TÄRKEIN MUUTOS**: Vasta nyt tehdään damage vaikutusalueelle!
            Collider[] hitColliders = Physics.OverlapSphere(trapObject.transform.position, skill.damageRange);
            foreach (Collider hitCollider in hitColliders)
            {
                EnemyHealth targetEnemyHealth = hitCollider.GetComponent<EnemyHealth>();
                if (targetEnemyHealth != null && !targetEnemyHealth.isDead)
                {
                    Debug.Log("Dealing damage to " + targetEnemyHealth);
                    targetEnemyHealth.TakeDamage(skill, playerAttack.IsCriticalHit());
                }
            }
        };
    }

    playerAttack.isCasting = false;
    yield break; // ✅ Tämä varmistaa, että metodi palauttaa IEnumerator-arvon
}
public IEnumerator IceTrap(Skill skill)
{
    playerAttack.isAttacking = false;  // nollaa hyökkäys
    GameObject trapObject = PlaceTrap(skill);
    Trap trapComponent = trapObject.GetComponent<Trap>();

    if (trapComponent != null)
    {
        trapComponent.OnTrapActivated += (EnemyHealth enemyHealth) =>
        {
            Buff stunData = buffDatabase.GetBuffByName("Stun");
            EnemyBuffManager targetBuffManager = enemyHealth.GetComponent<EnemyBuffManager>();

            Buff stunBuff = new Buff(
                stunData.name,
                skill.skillLevel * 2,
                stunData.isStackable,
                stunData.stacks,
                stunData.buffIcon,
                BuffType.Debuff,
                stunData.damage,
                stunData.effectText,
                stunData.effectValue,
                () => ApplyStunEffect(enemyHealth),
                () => RemoveStunEffect(enemyHealth)
            );

            if (stunBuff == null)
            {
                Debug.LogError("❌ Stun-buffin luonti epäonnistui!");
            }
            else
            {
                targetBuffManager.AddBuff(stunBuff);
                Debug.Log("✅ Stun lisätty: " + enemyHealth.monsterName);
            }

            Debug.Log("💀 Icetrap aktivoitu! kohteen: " + enemyHealth.monsterName);
            
            // Ladataan efektin prefab
            GameObject iceTrapEffect = Resources.Load<GameObject>("Explosions/IceTrapEffect");
            
            // Varmistetaan, että efekti löytyy
            if (iceTrapEffect != null)
            {
                // Luodaan efekti vihollisen sijaintiin (ei trap-objektin sisään)
                GameObject effectInstance = Instantiate(iceTrapEffect, enemyHealth.transform.position, Quaternion.identity);
                
                // Asetetaan efekti vihollisen lapseksi, jotta se ei mene trapin mukana
                effectInstance.transform.SetParent(enemyHealth.transform);

                // Efekti tuhotaan myöhemmin
                Debug.Log("effect tuhotaan sec: " + skill.skillLevel * 2);
                Destroy(effectInstance, (skill.skillLevel * 2)); 
            }
            else
            {
                Debug.LogError("❌ IceTrap efekti ei löytynyt Resources-kansiosta!");
            }

        };
    }
    playerAttack.isCasting = false;

    yield return null;
}


public IEnumerator BonecrusherTrap(Skill skill)
{
    playerAttack.isAttacking = false;  // nollaa hyökkäys
    GameObject trapObject = PlaceTrap(skill);
    Trap trapComponent = trapObject.GetComponent<Trap>();

    if (trapComponent != null)
    {
        trapComponent.OnTrapActivated += (EnemyHealth enemyHealth) =>
        {
            Debug.Log("💀 Bonecrusher Trap aktivoitu! VAhinkoa ottaa : " + enemyHealth.monsterName);
            GameObject boneCrusherTrapEffect = Resources.Load<GameObject>("Explosions/BoneCrusherTrapEffect");
            enemyHealth.TakeDamage(skill, playerAttack.IsCriticalHit());
            if (boneCrusherTrapEffect != null)
            {
                GameObject effectInstance = Instantiate(boneCrusherTrapEffect, trapObject.transform.position, Quaternion.identity);
                Destroy(effectInstance, 5f); 
            }
            else
            {
                Debug.LogError("❌ ShockTrapEffect ei löytynyt Resources-kansiosta!");
            }
           // enemyHealth.ApplyBuff(new ArmorBreakDebuff()); // Lisää panssarin särkyminen
        };
    }
    playerAttack.isCasting = false;

    yield return null;
}

    // Yleinen metodi trapin asettamiseen
private GameObject PlaceTrap(Skill skill)
{
    Vector3 trapPosition = playerAttack.transform.position;
    trapPosition.y -= 0.5f; // Lasketaan ansan korkeutta

  

    GameObject trap = Instantiate(trapPrefab, trapPosition, Quaternion.identity);
    trap.GetComponent<Trap>().PlaceTrap(trapPosition);

  

    return trap; // ✅ Palautetaan ansa GameObjectina
}





private List<EnemyHealth> TakeClosestEnemies(Collider[] hitColliders, int maxCount)
{
    List<EnemyHealth> enemies = new List<EnemyHealth>();

    foreach (Collider hitCollider in hitColliders)
    {
        EnemyHealth enemy = hitCollider.GetComponent<EnemyHealth>();
        if (enemy != null && !enemy.isDead)
        {
            enemies.Add(enemy);
        }
    }

    // Järjestetään viholliset etäisyyden mukaan pelaajasta
    enemies.Sort((a, b) =>
        Vector3.Distance(playerAttack.transform.position, a.transform.position)
        .CompareTo(Vector3.Distance(playerAttack.transform.position, b.transform.position))
    );

    return enemies.Take(maxCount).ToList();
}

private List<EnemyHealth> TakeAllEnemiesInRange(Collider[] hitColliders)
{
    List<EnemyHealth> enemies = new List<EnemyHealth>();

    foreach (Collider hitCollider in hitColliders)
    {
        EnemyHealth enemy = hitCollider.GetComponent<EnemyHealth>();
        if (enemy != null && !enemy.isDead)
        {
            enemies.Add(enemy);
        }
    }

    return enemies;
}








}
