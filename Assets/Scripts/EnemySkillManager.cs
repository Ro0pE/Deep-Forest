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
    public AudioSource audioSource;
    public AudioSource fireBoltSource;
    public AudioClip sufferDamageSound;
    public AudioClip fireBlastSound;
    public AudioClip fireBoltCastSound;
    public AudioClip fireBoltHitSound;
    private GameObject activeBurnEffect;
    private GameObject activeGlacialNovaEffect;
    private Coroutine glacialNovaRoutine;


    // Start is called before the first frame update
    void Start()
    {
        fireBoltSource = gameObject.AddComponent<AudioSource>();
        audioSource = gameObject.AddComponent<AudioSource>();
        playerAttack = FindObjectOfType<PlayerAttack>();
        playerStats = FindObjectOfType<PlayerStats>();
        playerMovement = FindObjectOfType<PlayerMovement>();
        buffManager = FindObjectOfType<BuffManager>();

        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public IEnumerator ExecuteEnemySkill(string skillName, PlayerHealth playerHealth, EnemyHealth enemyHealth, Vector3? position = null)
    {
        if(skillName == "Fire Bolt")
        {
            yield return StartCoroutine(FireBolt(playerHealth, enemyHealth));  // FireBolt palauttaa IEnumeratorin
        }
        else if (skillName == "Fire Blast")
        {
            yield return StartCoroutine(FireBlast(playerHealth, enemyHealth));  // FireBlast palauttaa IEnumeratorin
        }    
        else if (skillName == "Fire Debuff")
        {
            yield return StartCoroutine(FireDebuff(playerHealth, enemyHealth));  // FireDebuff palauttaa IEnumeratorin
        } 
        else if (skillName == "Glacial Nova")
        {
            yield return StartCoroutine(GlacialNova(playerHealth, enemyHealth));  // GlacialNova palauttaa IEnumeratorin
        } 
        else if (skillName == "Frost Charge")
        {
            if (position.HasValue)
                yield return StartCoroutine(FrostCharge(playerHealth, enemyHealth, position.Value));
            else
                Debug.LogWarning("Frost Charge needs a target position!");
        }                                                                            
        else
        {
            Debug.Log("Invalid skill");
        }
    }
public IEnumerator FrostCharge(PlayerHealth playerHealth, EnemyHealth enemyHealth, Vector3 position)
{
    float chargeSpeed = 21f;
    float chargeDuration = 0.5f;
    float effectThreshold = 5f;  // Etäisyys, jonka vihollinen on liikkunut ennen efektin luomista
    float radius = 5f; // Pelaajan tarkistusalue viivasta
    int damageAmount = 10;  // Määrä vahinkoa, jonka pelaaja saa
    float damageDuration = 20f; // Kesto, kuinka kauan alue tekee vahinkoa

    Vector3 startPosition = enemyHealth.transform.position;
    Vector3 targetPosition = playerHealth.transform.position;

    yield return new WaitForSeconds(1f);  // Odota hetki ennen aloitusta

    float elapsed = 0f;
    float distanceTravelled = 0f;  // Tämä seuraa vihollisen liikkuman etäisyyttä

    Vector3 previousPosition = startPosition;  // Tallennetaan edellinen sijainti

    // Lasketaan viivan vektori
    Vector3 lineDirection = (targetPosition - startPosition).normalized;

    // Luo ja aktivoi FrostChargeEffect
    GameObject frostChargeEffectPrefab = Resources.Load<GameObject>("UI_Effects/FrostChargeGroundEffect");

    // Luodaan efekti ja asetetaan se paikalleen
    GameObject groundEffect = Instantiate(frostChargeEffectPrefab, startPosition, Quaternion.identity);
    groundEffect.transform.parent = null; // Poistetaan parent-suhde, jotta efekti ei seuraa vihollista

    // Poista efekti, kun se on jäänyt maahan
    Destroy(groundEffect, damageDuration);  // Poistetaan efekti nopeasti

    // Seuraa aikaa ja tarkistaa, onko pelaaja vahinkovyöhykkeellä
    while (elapsed < chargeDuration)
    {
        float t = elapsed / chargeDuration;

        // Päivitetään vihollisen sijainti Lerpillä
        enemyHealth.transform.position = Vector3.Lerp(startPosition, targetPosition, t);

        // Varmistetaan, että vihollinen on aina suunnattu kohti pelaajaa
        enemyHealth.transform.forward = lineDirection;

        // Lasketaan, kuinka paljon vihollinen on liikkunut
        distanceTravelled += Vector3.Distance(previousPosition, enemyHealth.transform.position);

        // Luodaan efekti vain, kun vihollinen on liikkunut yli `effectThreshold` yksikköä
        if (distanceTravelled >= effectThreshold)
        {
            // Luo efekti, joka jää maahan
            GameObject newGroundEffect = Instantiate(frostChargeEffectPrefab, enemyHealth.transform.position, Quaternion.identity);

            // Poista parent-suhde, jotta efekti ei seuraa vihollista
            newGroundEffect.transform.parent = null;

            // Poista efekti 20 sekunnin kuluttua
            Destroy(newGroundEffect, damageDuration);

            // Nollaa matka, jotta seuraava efekti luodaan vasta, kun vihollinen on liikkunut seuraavat 5f
            distanceTravelled = 0f;
        }

        // Päivitetään edellinen sijainti
        previousPosition = enemyHealth.transform.position;

        elapsed += Time.deltaTime;
        yield return null;
    }

    // Varmista, että vihollinen on loppupisteessä
    enemyHealth.transform.position = targetPosition;

    // Tarkistetaan etäisyys pelaajaan, jos se on alle 6 yksikköä, tapahtuu asia X
    float distanceToPlayer = Vector3.Distance(enemyHealth.transform.position, playerHealth.transform.position);
    if (distanceToPlayer <= 6f)
    { 
        // LISÄÄ EFFEKTI
        // Tässä tapahtuu "asia X" (voit määritellä mitä tämä on, esim. vahinkoa, statusvaikutuksia jne.)
        Debug.Log("Vihollinen on pelaajan läheisyydessä ja tekee jotain erityistä!");
        // Esimerkki: voi olla vahinkoa, statusvaikutuksia, tai muita toimintoja
        playerHealth.TakeSpellDamage(500, "Frost Charge");
    }

    // Alueen luominen, joka tarkistaa pelaajan sijainnin
    GameObject frostChargeArea = Instantiate(frostChargeEffectPrefab, targetPosition, Quaternion.identity);
    frostChargeArea.transform.parent = null; // Efekti ei liiku vihollisen mukana

    // Luo alue, joka tekee vahinkoa pelaajalle, kun hän menee sen lähelle
    Destroy(frostChargeArea, damageDuration);  // Poistetaan alue 20 sekunnin kuluttua

    // Alue tarkistaa pelaajan sijainnin
    StartCoroutine(DamagePlayerInArea(playerHealth, enemyHealth, startPosition, targetPosition, radius, damageAmount, damageDuration));
}


    private IEnumerator DamagePlayerInArea(PlayerHealth playerHealth, EnemyHealth enemyHealth, Vector3 startPosition, Vector3 targetPosition, float radius, int damageAmount, float duration)
    {
        float elapsed = 0f;
        float damageInterval = 2f;  // Vahinkoväli sekunneissa
        float lastDamageTime = 0f;  // Aika, jolloin viimeisin vahinko tehtiin

        // Tarkistetaan, onko pelaaja alueella ja otetaan vahinkoa
        while (elapsed < duration)
        {
            // Lasketaan viivan suunta
            Vector3 lineDirection = (targetPosition - startPosition).normalized;

            // Lasketaan etäisyys pelaajasta viivalle (vertikaalinen etäisyys)
            float distanceToLine = Mathf.Abs(Vector3.Cross(playerHealth.transform.position - startPosition, lineDirection).magnitude);

            // Tarkistetaan, onko pelaaja viivan alueella 5 yksikön säteellä
            if (distanceToLine <= radius)
            {
                // Tarkistetaan, onko pelaaja viivalla
                float projection = Vector3.Dot(playerHealth.transform.position - startPosition, lineDirection);
                if (projection >= 0 && projection <= Vector3.Distance(startPosition, targetPosition))
                {
                    // Tarkistetaan, onko kulunut tarpeeksi aikaa edellisestä vahingonotosta
                    if (elapsed - lastDamageTime >= damageInterval)
                    {
                        // Pelaaja on viivalla ja saa vahinkoa
                        Debug.Log("Pelaaja on viivalla ja saa vahinkoa.");
                        playerHealth.TakeSpellDamage(damageAmount, "Frost Field");
                        StartCoroutine(ApplyGlacialNovaDebuff(playerHealth, enemyHealth));

                        // Päivitetään viimeisin vahinkoaika
                        lastDamageTime = elapsed;
                    }
                }
            }

            elapsed += Time.deltaTime;
            yield return null;
        }
    }







    private void ApplyGlacialNovaEffect(PlayerHealth playerHealth)
    {
        ApplyGlacialNovaDamageEffect(playerHealth);
        if (glacialNovaRoutine == null)
            glacialNovaRoutine = StartCoroutine(ApplyGlacialNovaDamage(playerHealth));
    }

    private void RemoveGlacialNovaEffect(PlayerHealth playerHealth)
    {
        // Pysäytetään Glacial Nova -vahingon korutiinit
        RemoveGlacialNovaDamageEffect();
        StopCoroutine(ApplyGlacialNovaDamage(playerHealth));
    }

    private IEnumerator ApplyGlacialNovaDamage(PlayerHealth playerHealth)
    {
        float tickInterval = 1f;
        PlayerMovement movement = playerHealth.GetComponent<PlayerMovement>();

        // Haetaan buffi heti kerran ennen loopin alkua
        Buff glacialNovaBuff = buffManager.activeBuffs.Find(b => b.name == "Glacial Nova DoT");

        if (glacialNovaBuff == null)
        {
            Debug.Log("GlacialNova buff not found, stopping DoT.");
            yield break;
        }

        // Määritellään kiinteä hidastusprosentti (esim. 10%)
        //float slowAmount = glacialNovaBuff.effectValue;  // Käytetään vain effectValue-arvoa eikä stackeja

        while (true)
        {
            // Jos buffi on vanhentunut, lopetetaan DoT
            if (glacialNovaBuff.duration <= 0f)
            {
                Debug.Log("GlacialNova buff expired, stopping DoT.");
                if (movement != null)
                    movement.ReturnPlayerSpeed(playerMovement.originalSpeed);  // Palautetaan alkuperäinen nopeus
                glacialNovaRoutine = null;
                yield break;
            }

            // Lasketaan hidastusprosentti
            // Jos effectValue = 0.1 (eli 10%)
            int stacks = glacialNovaBuff.stacks;
            float effectPerStack = glacialNovaBuff.effectValue; // esim. 0.1f

            float slowAmount = effectPerStack * stacks; // 0.1 * 2 = 0.2 eli 20% hidastus
            float slowSpeed = playerMovement.originalSpeed * slowAmount;

            Debug.Log("slow speed amount " + slowSpeed);

            // Vähennetään pelaajan nopeudesta
            playerMovement.ReducePlayerSpeed(slowSpeed);

            // Lasketaan vahinko ja tehdään se
            int glacialNovaDamage = Mathf.RoundToInt(glacialNovaBuff.damage * glacialNovaBuff.stacks);
            Debug.Log($"Otettu Glacial Nova DEBUFF damage {glacialNovaDamage}, liikkumis hidastus: {slowSpeed * 100}%");
            audioSource.PlayOneShot(sufferDamageSound);
            playerHealth.TakeSpellDamage(glacialNovaDamage, glacialNovaBuff.name);

            // Vähennetään buffin kestoa
            glacialNovaBuff.duration -= tickInterval;

            // Odotetaan tickIntervalin verran ennen seuraavaa päivitystä
            yield return new WaitForSeconds(tickInterval);
        }
    }




    public IEnumerator ApplyGlacialNovaDebuff(PlayerHealth playerHealth, EnemyHealth enemyHealth)
    {
       // Haetaan Glacial Nova -buffi BuffDatabase:stä
        Buff glacialNovaBuffData = buffDatabase.GetBuffByName("Glacial Nova DoT");
        
        // Luodaan uusi GlacialNova-buffi pelaajalle
        Buff glacialNovaBuff = new Buff(
            glacialNovaBuffData.name,
            glacialNovaBuffData.duration,
            glacialNovaBuffData.isStackable,
            glacialNovaBuffData.stacks,
            glacialNovaBuffData.maxStacks,
            glacialNovaBuffData.buffIcon,
            BuffType.Debuff, // Tämä on debuff
            glacialNovaBuffData.damage * enemyHealth.attackDamage, // Vahinko perustuu vihollisen hyökkäysvoimaan
            glacialNovaBuffData.effectText,
            glacialNovaBuffData.effectValue, // Arvo voi olla esimerkiksi ylimääräinen efekti
            () => ApplyGlacialNovaEffect(playerHealth), // Efekti, joka käynnistyy
            () => RemoveGlacialNovaEffect(playerHealth) // Efekti, joka poistetaan
        );

        if (glacialNovaBuff == null)
        {
            Debug.LogError("GlacialNova buff creation failed!");
        }
        else
        {
            // Lisää Glacial Nova -buffi pelaajalle
            buffManager.AddBuff(glacialNovaBuff);
        }

        yield break;
    }


    private void ApplyBurningEffect(PlayerHealth playerHealth)
    {
  
        StopAllCoroutines();
        AddBurnDamageEffect(playerHealth);
        StartCoroutine(ApplyBurningDamage(playerHealth));
    }

    private void RemoveBurningEffect(PlayerHealth playerHealth)
    {
        // Pysäytetään polttovahingon korutiinit
        RemoveBurnDamageEffect();
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

            audioSource.PlayOneShot(sufferDamageSound);
            int burningDamage = Mathf.RoundToInt(burningHeartBuff.damage * burningHeartBuff.stacks);
             Debug.Log("otetaan Burning Heart damagea " + burningDamage);
            playerHealth.TakeSpellDamage(burningDamage, burningHeartBuff.name);
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
        GameObject boltPrefab = Resources.Load<GameObject>("Projectiles/FireBolt");
        if (boltPrefab == null)
        {
            Debug.LogError("FireBolt prefab ei löytynyt Resourcesista!");
            yield break;
        }

        Vector3 spawnPosition = enemyHealth.transform.position
                                + enemyHealth.transform.up * 6f
                                - enemyHealth.transform.forward * 2f; // taaksepäin

        GameObject bolt = Instantiate(boltPrefab, spawnPosition, Quaternion.identity);

        EnemySpellProjectile projectile = bolt.GetComponent<EnemySpellProjectile>();
        if (projectile != null)
        {
            projectile.Initialize(playerHealth.transform, ShooterType.Enemy);
            projectile.damage = enemyHealth.attackDamage;
        }
        float distance = Vector3.Distance(spawnPosition, playerHealth.transform.position);
        float timeToHit = distance / projectile.speed;

        fireBoltSource.PlayOneShot(fireBoltHitSound);
        StartCoroutine(StopFireBoltSoundAfterDelay(0.4f));

        yield return new WaitForSeconds(timeToHit); 
        

        // 🔥 Ladataan ja luodaan "fireHit" efekti pelaajan päälle
        GameObject hitEffectPrefab = Resources.Load<GameObject>("Explosions/FireBoltExplosion");
        if (hitEffectPrefab != null)
        {
            Vector3 hitPosition = playerHealth.transform.position + Vector3.up * 3.0f; // hieman pelaajan yläpuolelle
            GameObject hitEffect = Instantiate(hitEffectPrefab, hitPosition, Quaternion.identity);
            
             //Asetetaan efektin vanhemmaksi pelaaja, jotta se seuraa liikettä
            hitEffect.transform.SetParent(playerHealth.transform);
            if (projectile != null)
            {
                projectile.transform.SetParent(playerHealth.transform);
            }

            Destroy(hitEffect, 0.8f); // tuhoaa efektin automaattisesti 1 sek kuluttua
            fireBoltSource.PlayOneShot(fireBoltHitSound);
            StartCoroutine(StopFireBoltSoundAfterDelay(0.4f));
        }
        else
        {
            Debug.LogWarning("FireHit-efektiä ei löytynyt Resources/Effects-kansiosta!");
        }
        

        
        Debug.Log("DAMAGE " + enemyHealth.attackDamage);

        playerHealth.TakeSpellDamage(Mathf.RoundToInt(enemyHealth.attackDamage/2), "Fire Bolt");
        //Destroy(projectile);
    }


    public IEnumerator FireBlast(PlayerHealth playerHealth, EnemyHealth enemyHealth)
    {
        float coneAngle = 40f;
        float coneRange = 30f;
        int damage = Mathf.RoundToInt(enemyHealth.attackDamage / 30);

        Vector3 castDirection = new Vector3(enemyHealth.transform.forward.x, 0f, enemyHealth.transform.forward.z).normalized;

        Collider[] hits = Physics.OverlapSphere(enemyHealth.transform.position, coneRange);

        HashSet<PlayerHealth> affectedPlayers = new HashSet<PlayerHealth>();

        foreach (Collider hit in hits)
        {
            PlayerHealth target = hit.GetComponent<PlayerHealth>();

            if (target != null && !affectedPlayers.Contains(target))
            {
                Vector3 toTarget = (target.transform.position - enemyHealth.transform.position).normalized;
                Vector3 flatToTarget = new Vector3(toTarget.x, 0f, toTarget.z).normalized;

                float angle = Vector3.Angle(castDirection, flatToTarget);

                if (angle < coneAngle / 2f)
                {
                    Debug.Log($"🔥 Cone osui: {target.name}");
                    audioSource.PlayOneShot(sufferDamageSound);

                    target.TakeSpellDamage(damage, "Fire Blast");
                    StartCoroutine(FireDebuff(target, enemyHealth)); // HUOM! Korjattu myös oikea target

                    affectedPlayers.Add(target); // Estetään tuplaus
                }
            }
        }

        yield return new WaitForSeconds(1f);
    }
    public IEnumerator GlacialNova(PlayerHealth playerHealth, EnemyHealth enemyHealth)
    {
        float tickRate = 1f;
        int maxTicks = 5;  // Maksimi tikit (6 tikkia)
        int currentTick = 0;  // Laskuri, joka seuraa tikkejä
        bool isPlayerInRange = false;  // Tarkistetaan, onko pelaaja alueella

        while (currentTick < maxTicks)
        {
            // Laske etäisyys pelaajaan
            float distance = Vector3.Distance(enemyHealth.transform.position, playerHealth.transform.position);

            if (distance <= 30f)
            {
                if (!isPlayerInRange)
                {
                    // Pelaaja tuli alueelle
                    isPlayerInRange = true;
                    Debug.Log("Pelaaja tuli Glacial Nova -alueelle");
                }

                // Pelaaja alueella, ota vahinkoa
                DealGlacialDamage(playerHealth, enemyHealth);
                

                // Vahingon jälkeen kasvatetaan tikki-laskuria
               
            }
            else
            {
                if (isPlayerInRange)
                {
                    // Pelaaja poistui alueelta
                    Debug.Log("Pelaaja poistui Glacial Nova -alueelta");
                    isPlayerInRange = false;
                    
                }
            }
            currentTick++;
            Debug.Log("Current tick:  " + currentTick);

            yield return new WaitForSeconds(tickRate);
        }

        Debug.Log("Glacial Nova loppui 6 tikin jälkeen");
    }


    private void DealGlacialDamage(PlayerHealth playerHealth, EnemyHealth enemyHealth)
    {
        int damage = Mathf.RoundToInt(enemyHealth.attackDamage / 25f);
        Debug.Log($"Glacial Nova AOE damage: {damage}");
        StartCoroutine(ApplyGlacialNovaDebuff(playerHealth, enemyHealth));
        audioSource.PlayOneShot(sufferDamageSound);
        playerHealth.TakeSpellDamage(damage, "Glacial Nova AoE");
    }






    private IEnumerator StopFireBoltSoundAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        fireBoltSource.Stop();
    }


    public void AddBurnDamageEffect(PlayerHealth playerHealth)
    {
        GameObject burnDamageEffectPrefab = Resources.Load<GameObject>("Explosions/BurnDamageEffect");
        if (burnDamageEffectPrefab != null)
        {
            Vector3 hitPosition = playerHealth.transform.position + Vector3.up * 3.0f;
            activeBurnEffect = Instantiate(burnDamageEffectPrefab, hitPosition, Quaternion.identity);
            activeBurnEffect.transform.SetParent(playerHealth.transform);
            Destroy(activeBurnEffect, 20f);
        }
    }
    public void ApplyGlacialNovaDamageEffect(PlayerHealth playerHealth)
    {
        GameObject glacialNovaEffect = Resources.Load<GameObject>("Explosions/BurnDamageEffect");
        if (glacialNovaEffect != null)
        {
            Vector3 hitPosition = playerHealth.transform.position + Vector3.up * 3.0f;
            activeBurnEffect = Instantiate(glacialNovaEffect, hitPosition, Quaternion.identity);
            activeBurnEffect.transform.SetParent(playerHealth.transform);
            Destroy(activeBurnEffect, 10f);
        }
    }

    public void RemoveBurnDamageEffect()
    {
        if (activeBurnEffect != null)
        {
            Destroy(activeBurnEffect);
            activeBurnEffect = null;
        }
    }

    public void RemoveGlacialNovaDamageEffect()
    {
        if (activeGlacialNovaEffect != null)
        {
            Destroy(activeGlacialNovaEffect);
            activeGlacialNovaEffect = null;
        }
    }




}
