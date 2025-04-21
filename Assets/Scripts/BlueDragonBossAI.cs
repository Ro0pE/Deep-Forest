using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using TMPro;
using UnityEngine.UI;


public class BlueDragonBossAI : MonoBehaviour
{
    public Image castBar; // Viittaus CastBar-kuvakkeeseen
    public TextMeshProUGUI castTimeText; // Viittaus CastBarin ajan näyttöön
    public TextMeshProUGUI castBarSkillText;
    public GameObject enemyCastBarPanel;
    public float detectionRange = 80f; // Etäisyys, jolla vihollinen havaitsee pelaajan
    public float attackRange = 25f; // Etäisyys, jolla vihollinen voi hyökätä pelaajaan
    public float wanderRadius = 100f; // Vaeltamisen säde
    public float wanderInterval = 25f; // Vaeltamisen aikaväli
    public LayerMask playerLayer; // Pelaajan kerros
    public LayerMask groundLayer; // Maatason tarkistuskerros
    public float attackDamage = 35f; // Hyökkäysvoima
    public float groundCheckDistance = 1.5f; // Maan tarkistuksen etäisyys
    public float basicAttackCooldown = 1.2f; // Hyökkäysväli
    private bool isMeleeAttackOnCooldown = false;

    [Header("Bosse Casting")]
    public float flameBreatchCooldownTimer = 10f;
    public float flameBreathTimer = 0f;
    public float flameBreathChannelTime = 4f;
    public bool isCastingFlameBreath = false;
    public bool playerSlowed = false;
    [Header("Ice Charge")]
    public  float iceChargeCooldownTimer = 5f;
    public float iceChargerTimer = 0f;
    public float iceChargeChannelTime = 2f;
    public bool isCastingIceCharge = false;
    public bool isChargingToPlayer = false;
    public float chargeSpeed = 1f; // Nopeus, jolla Ice Dragon menee pelaajaan

    public bool isAgentStopped;


    private BuffManager buffManager;
    private Buff burningHeartBuff; // Buff tallennetaan vain kerran

    public UnityEngine.AI.NavMeshAgent agent;
    public Transform player;
    public PlayerHealth playerHealth;
    public EnemyHealth enemyHealth;
    public Animator animator;
    public float wanderTimer;
    public bool isAttacking = false;
    public BuffDatabase buffDatabase;
    public PlayerMovement playerMovement;

    void Start()
    {
        //buffDatabase = FindObjectOfType<BuffDatabase>();
        playerMovement = FindObjectOfType<PlayerMovement>();
        buffManager = FindObjectOfType<BuffManager>();
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        playerHealth = player.GetComponent<PlayerHealth>();
        enemyHealth = GetComponent<EnemyHealth>();
        animator = GetComponent<Animator>();
        wanderTimer = wanderInterval;
        playerSlowed = false;

        castBar.gameObject.SetActive(false);

        // Luo Buff vain kerran

    }

    void Update()
    {
        isAgentStopped = agent.isStopped;
        flameBreathTimer += Time.deltaTime;
        iceChargerTimer +=Time.deltaTime;
        float distanceToPlayer = Vector3.Distance(player.position, transform.position);
        
        if (enemyHealth.isDead)
        {
            return;
        }



        // Tarkistetaan, onko pelaaja detection range -alueella
        if (distanceToPlayer <= detectionRange)
        {
            if (enemyHealth.currentHealth < 200f && !playerSlowed)
            {
          
                IcyTouch();
            }

            if (flameBreathTimer >= flameBreatchCooldownTimer && !isCastingFlameBreath && !isCastingIceCharge)
            {
                StartCoroutine(FlameBreathCasting());
            }
            if (isCastingFlameBreath)
            {
                return; // Estetään kaikki muut toiminnot castauksen aikana
            }

            if (iceChargerTimer >= iceChargeCooldownTimer && !isCastingIceCharge && !isCastingFlameBreath)
            {
                StartCoroutine(IceChargeCasting());
            }
            if (isCastingIceCharge)
            {
                return; // Estetään kaikki muut toiminnot castauksen aikana
            }
            if (distanceToPlayer <= attackRange) 
            {
                StartCoroutine(MeleeAttack());
            }        

            if (enemyHealth.isDead != true)
            {
                ChasePlayer(distanceToPlayer);
            }
        }
        else
        {
            Wander();
        }

    }
    public IEnumerator MeleeAttack()
    {
        if (isMeleeAttackOnCooldown)
        Debug.Log("BREAK?!");
        yield break; // Jos hyökkäys on jäähdytyksessä, ei tehdä mitään
        isMeleeAttackOnCooldown = true;
        Debug.Log("MELEE INC");
        animator.SetBool("isAttacking", true);
        playerHealth.TakeDamage(attackDamage);
        yield return new WaitForSeconds(basicAttackCooldown);
        isMeleeAttackOnCooldown = false;
        animator.SetBool("isAttacking", false);

    }
    public void IceCharge()
    {
        // Tallenna pelaajan sijainti ennen kuin vihollinen alkaa liikkua
        Vector3 targetPosition = player.position;

        isChargingToPlayer = true;

        // Ota agentti liikkeelle ja säädä sen nopeutta ja kiihtyvyyttä
        agent.isStopped = false;  // Varmista, että agentti ei ole pysähdyksissä
        agent.speed = 140f;        // Nopeus
        agent.acceleration = 120f; // Kiihtyvyys
        agent.SetDestination(targetPosition);  // Aseta tavoite
        animator.SetTrigger("chargeAttack");

        StartCoroutine(ChargeTowardsPlayer(targetPosition));
        
    }

    private IEnumerator ChargeTowardsPlayer(Vector3 targetPosition)
    {
        
        // Liikuta agentti kohti pelaajaa
        while (isChargingToPlayer && Vector3.Distance(transform.position, targetPosition) > 1f)
        {

            agent.SetDestination(targetPosition); // Jatka liikettä kohti pelaajaa
            yield return null;  // Odota seuraavaa framea
        }
        isChargingToPlayer = false;
        // Kun agentti on lähellä pelaajaa, tarkista osuuko se pelaajaan
        if (Vector3.Distance(transform.position, player.position) < 25f) // Pelaaja on lähellä
        {
            if (playerHealth != null)
            {
                IcyTouch();
                float damage = 100f; // Määritä vahinkosumma
                Debug.Log("Pelaaja ottaa chargesta");
                playerHealth.TakeDamage(damage); // Pelaaja ottaa vahinkoa
            }
        }

        // Loppu, isChargingToPlayer asetetaan false
        Debug.Log("charge loppu");


        // Kun Ice Dragon on valmis ja lopettaa liikkumisen
        agent.speed = 30f;  // Palauta alkuperäinen nopeus
        agent.acceleration = 95f;  // Palauta alkuperäinen kiihtyvyys
        //agent.isStopped = true;  // Pysäytä agentti
    }

    // Castausmetodi
    public IEnumerator IceChargeCasting()
    {
        enemyCastBarPanel.SetActive(true);
        enemyHealth.isInterrupted = false;
        iceChargerTimer = 0f;
        isCastingIceCharge = true;
        //agent.isStopped = true;  // Pysäytä agentti castausajaksi
        agent.velocity = Vector3.zero; // Aseta nopeus nollaksi
        agent.acceleration = 0f;      // Estä hidastuva liike

        castBarSkillText.text = "ICE CHARGE";
        castBar.fillAmount = 1f;
        castBar.gameObject.SetActive(true);
        float elapsed = 0f;
        animator.SetTrigger("isCastingCharge");

        // Kun castaus keskeytetään, estä debuffin asettaminen
        while (elapsed < iceChargeChannelTime && !enemyHealth.isInterrupted)
        {
            elapsed += Time.deltaTime;
            castBar.fillAmount = 1f - (elapsed / iceChargeChannelTime); // Päivitä CastBar
            if (castTimeText != null)
            {
                castTimeText.text = $"{Mathf.Max(0f, iceChargeChannelTime - elapsed):0.0} s"; // Näytä jäljellä oleva aika
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
            agent.isStopped = false;  // Palauta liike
            agent.acceleration = 95f;  // Palauta alkuperäinen kiihtyvyys
            isCastingIceCharge = false;
            yield return new WaitForSeconds(1);
            castBar.gameObject.SetActive(false);
            yield break;  // Lopeta korutiini, jotta ei aseteta debuffia
        }
        


        // Kun castaus on valmis
        castBar.fillAmount = 0f;
        castBarSkillText.text = "";
        castBar.gameObject.SetActive(false);
        agent.isStopped = false;  // Palauta liike
        agent.acceleration = 95f; // Palauta alkuperäinen kiihtyvyys
        isCastingIceCharge = false;

        // Aloita IceCharge-metodi
        Debug.Log("Ice charge lähtee!");
        enemyCastBarPanel.SetActive(false);
        IceCharge();
    }



    public void ApplySlowEffect(float slowEffect){
        Debug.Log("Apply slow");
        float playerSlowedSpeed = playerMovement.moveSpeed * slowEffect;
       // playerMovement.SetPlayerSpeed(playerSlowedSpeed);
    }
    public void RemoveSlowEffect(){
        Debug.Log("Remove slow");
        playerSlowed = false;
       // playerMovement.ReturnPlayerSpeed();
    }
    public void IcyTouch()
    {
        if (!playerSlowed){

            Buff slowBuffData  = buffDatabase.GetBuffByName("Slow");
            Buff slowBuff = new Buff(
                slowBuffData.name,               // Buffin nimi
                slowBuffData.duration,           // Kesto
                slowBuffData.isStackable,        // Voiko pinota
                slowBuffData.stacks,
                slowBuffData.maxStacks,             // Pinojen määrä
                slowBuffData.buffIcon,           // Kuvake
                BuffType.Debuff,                 // Buffin tyyppi (Debuff, koska hidastus on yleensä debuff)
                slowBuffData.damage,
                slowBuffData.effectText,
                slowBuffData.effectValue,        // EffectValue (esim. hidastusprosentti)
                () => ApplySlowEffect(slowBuffData.effectValue),                 // Efektin soveltaminen
                RemoveSlowEffect                 // Efektin poistaminen
            );

            if (slowBuff == null) {
                Debug.LogError("Slow buff creation failed!");
            } else {        
                buffManager.AddBuff(slowBuff);
                playerSlowed = true;
            }
        }
    }

    public IEnumerator FlameBreathCasting()
    {
        enemyCastBarPanel.SetActive(true);
        isChargingToPlayer = false;
        animator.SetTrigger("isCastingFlame");
        enemyHealth.isInterrupted = false;
        flameBreathTimer = 0f;
        isCastingFlameBreath = true;
        //agent.isStopped = true;
        agent.velocity = Vector3.zero; // Aseta nopeus nollaan
        agent.acceleration = 0f;      // Estä hidastuva liike
        castBarSkillText.text = "FLAME BREATH";
        castBar.fillAmount = 1f;
        castBar.gameObject.SetActive(true);
        float elapsed = 0f;

        // Kun castaus keskeytetään, estä debuffin asettaminen
        while (elapsed < flameBreathChannelTime && !enemyHealth.isInterrupted)
        {
            FacePlayer();

            elapsed += Time.deltaTime;
            castBar.fillAmount = 1f - (elapsed / flameBreathChannelTime); // Päivitä CastBar
            if (castTimeText != null)
            {
                castTimeText.text = $"{Mathf.Max(0f, flameBreathChannelTime - elapsed):0.0} s"; // Näytä jäljellä oleva aika
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
            isCastingFlameBreath = false;
            yield return new WaitForSeconds(1);
            castBar.gameObject.SetActive(false);
         
            yield break; // Lopeta korutiini, jotta ei aseteta debuffia
        }

        castBar.fillAmount = 0f;
        castBarSkillText.text = "";
        castBar.gameObject.SetActive(false);
        agent.isStopped = false;
        agent.acceleration = 95f; // Palauta alkuperäinen kiihtyvyys
        isCastingFlameBreath = false;
        enemyCastBarPanel.SetActive(false);

        // Aloita polttovahinko hyökkäys
       
        FlameBreathAttack();
    }


    public void FlameBreathAttack()
    {
            isChargingToPlayer = false;
            Buff burningHeartBuffData  = buffDatabase.GetBuffByName("BurningHeart");
            Buff burningHeartBuff = new Buff(
                burningHeartBuffData.name,
                burningHeartBuffData.duration,
                burningHeartBuffData.isStackable,
                burningHeartBuffData.stacks,
                burningHeartBuffData.maxStacks,
                burningHeartBuffData.buffIcon,
                BuffType.Debuff,  // Esimerkki: Jos tämä on debuff, käytä BuffType.Debuff
                burningHeartBuffData.damage,  // Tässä oletetaan, että "damage" on mukana BuffDatabase:ssa
                burningHeartBuffData.effectText,
                burningHeartBuffData.effectValue,  // Sama effectValue, jos se liittyy johonkin muuhun efektiin
                ApplyBurningEffect,  // Efektin soveltaminen
                RemoveBurningEffect  // Efektin poistaminen
            );

            if (burningHeartBuff == null) {
                Debug.LogError("BurningHeart buff creation failed!");
            } else {
                // Lisää BurningHeart buffi pelaajalle

                buffManager.AddBuff(burningHeartBuff);
            }
    }

    private void ApplyBurningEffect()
    {
  
        StopAllCoroutines();
        StartCoroutine(ApplyBurningDamage());
    }

    private void RemoveBurningEffect()
    {
        // Pysäytetään polttovahingon korutiinit
        StopCoroutine(ApplyBurningDamage());
    }

    private IEnumerator ApplyBurningDamage()
    {
   
        float burnDamagePerSecond = 2f; // Polttovahinko joka sekunti
        float tickInterval = 1f;       // Vahinkoa kerran sekunnissa
        
        while (true)
        {
            Buff burningHeartBuff = buffManager.activeBuffs.Find(b => b.name == "BurningHeart");

            if (burningHeartBuff == null || burningHeartBuff.duration <= 0f)
            {
                Debug.Log("BurningHeart buff expired or not found, stopping DoT.");
                yield break; // Lopeta korutiini, kun buffi ei ole enää aktiivinen
            }

          
            playerHealth.TakeDamage(burnDamagePerSecond * burningHeartBuff.stacks);
            //burningHeartBuff.duration -= tickInterval;

            yield return new WaitForSeconds(tickInterval);
        }
    }





    public  void ChasePlayer(float distanceToPlayer)
    {
       
         if (isCastingFlameBreath) return; // Estä liike castauksen aikana
         if (isChargingToPlayer) return;
         if (isCastingIceCharge) return;
        agent.speed = 55f;
        animator.SetBool("isRunning", true);
        animator.SetBool("isWalking", false);
        animator.SetBool("isAttacking", false);
        if (player != null)
        {

            if (distanceToPlayer <= attackRange)
            {
                animator.SetBool("isRunning", false);
                //agent.isStopped = true;
                agent.velocity = Vector3.zero; // Aseta nopeus nollaan
                agent.acceleration = 0f;      // Estä hidastuva liike
                FacePlayer();
                // Pysäytä agentti heti, kun ollaan hyökkäysetäisyydellä
              // agent.isStopped = true;

                if (!isAttacking)
                {
                    StartCoroutine(DelayedAttack());
                }
            }
            else
            {
                
                agent.isStopped = false;
                agent.acceleration = 95f;      // Estä hidastuva liike                
                agent.SetDestination(player.position);

            }
        }
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
    public virtual IEnumerator DelayedAttack()
    {
        
        isAttacking = true; // Aseta hyökkäystila true
        animator.SetBool("isAttacking", true);
        animator.SetBool("isRunning", false);
        animator.SetBool("isWalking", false);
        yield return new WaitForSeconds(basicAttackCooldown); // Voit vähentää tai poistaa tämän testataksesi
        AttackPlayer();
        isAttacking = false; // Palauta hyökkäystila false
    }
    public void AttackPlayer()
    {


        float distanceToPlayer = Vector3.Distance(player.position, transform.position);
        
        if (distanceToPlayer <= attackRange) // Jos karhu on tarpeeks lähellä
        {
            Vector3 directionToPlayer = (player.position - transform.position).normalized;
            Ray ray = new Ray(transform.position, directionToPlayer);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, attackRange, playerLayer))
            {
                if (hit.collider.CompareTag("Player"))
                {
                    playerHealth.TakeDamage(1);
                }
                
                }
            }
        }
    

    public virtual void Wander()
    {
         if (isCastingFlameBreath) return; // Estä liike castauksen aikana
        
        agent.speed = 25f;
        wanderTimer += Time.deltaTime;
        animator.SetBool("isWalking", true);
        animator.SetBool("isRunning", false);
        animator.SetBool("isAttacking", false);

        // Jos vihollinen on saavuttanut määränpäänsä tai vaeltaminen on kestänyt tarpeeksi pitkään, valitse uusi kohde
        if (wanderTimer >= wanderInterval || agent.remainingDistance < 0.5f)
        {
           

            // Luo satunnainen pysähtymisaika 1-12 sekuntia
            float stopTime = Random.Range(0.5f, 2f);
            StartCoroutine(PauseBeforeNextMove(stopTime));
            
            // Luo uusi satunnainen paikka vaeltamista varten
            Vector3 newPos = RandomNavSphere(transform.position, wanderRadius, -1);
            agent.SetDestination(newPos);
            wanderTimer = 0;
        }

    }
    public static Vector3 RandomNavSphere(Vector3 origin, float dist, int layermask)
    {
        Vector3 randomDirection = Random.insideUnitSphere * dist;
        randomDirection += origin;
        NavMeshHit navHit;

        if (NavMesh.SamplePosition(randomDirection, out navHit, dist, layermask))
        {
            return navHit.position;
        }
        else
        {
            return origin; // Palautetaan alkuperäinen sijainti, jos sopivaa paikkaa ei löydy
        }
    }

    // Tauko liikkeen välissä
    private IEnumerator PauseBeforeNextMove(float duration)
    {
        agent.isStopped = true; // Pysäytä agentti

        yield return new WaitForSeconds(duration); // Odota satunnainen aika
         agent.isStopped = false; // Pysäytä agentti
    }  
    } 
     

    

    /*
    private IEnumerator FireBreathCasting()
    {
        fireCooldwonTimer = 0f;
        isCastingFire = true;
        agent.isStopped = true;
        Debug.Log("FIRE BREATH");
        castBarSkillText.text = "FIRE BREAT!";
        castBar.fillAmount = 1f;
        castBar.gameObject.SetActive(true);
        float elapsed = 0f;
        while (elapsed < fireBreathTime)
        {
            //playerMovement.SetPlayerSpeed();
            elapsed += Time.deltaTime;
            castBar.fillAmount = 1f - (elapsed / fireBreathTime); // Päivitä CastBar
            if (castTimeText != null)
            {
                castTimeText.text = $"{Mathf.Max(0f, fireBreathTime - elapsed):0.0} s"; // Näytä jäljellä oleva aika
            }
            yield return null;
        }
        castBar.fillAmount = 0f;
        castBarSkillText.text = "";
        castBar.gameObject.SetActive(false);  
        StartCoroutine(PerformFireBreath());
        agent.isStopped = false;
        isAttacking = false;              
    }

    private IEnumerator ChargeAttack()
    {
  
        isCharging = true;
        agent.isStopped = true;
        attackRange = 25f;
        
        // Latausaika animaatio ja logiikka
        Debug.Log("Charge attack animaatio");
        animator.SetTrigger("ChargeAttack"); // Käynnistä lataus-animaatio
        
        castBarSkillText.text  = "Triple Strike!";
        castBar.fillAmount = 1f;
        castBar.gameObject.SetActive(true);

        // Castausaika
        float elapsed = 0f;
        while (elapsed < chargeTime)
        {
            //playerMovement.SetPlayerSpeed();
            elapsed += Time.deltaTime;
            castBar.fillAmount = 1f - (elapsed / chargeTime); // Päivitä CastBar
            if (castTimeText != null)
            {
                castTimeText.text = $"{Mathf.Max(0f, chargeTime - elapsed):0.0} s"; // Näytä jäljellä oleva aika
            }
            yield return null;
        }

        // Castaus valmis
       // SpawnProjectile(slot);
        castBar.fillAmount = 0f;
        castBarSkillText.text = "";
        castBar.gameObject.SetActive(false);
        Debug.Log("Times up!!");
        //yield return new WaitForSeconds(chargeTime); // Odota latausaika

        // Kun lataus on valmis, suorita hyökkäys

        StartCoroutine(PerformTripleAttack()); // Käynnistä Coroutine

        
        attackRange = 10f;
        agent.isStopped = false;
        isAttacking = false;

        

        
    }
    private IEnumerator PerformFireBreath()
    {
        
        // Näytä tulihengitysefekti
        if (fireEffect != null)
        {
            Debug.Log("Fire-effect ei oo null");
        fireEffect.Play();
        yield return new WaitForSeconds(fireEffect.main.duration); // Odota efektin loppumista
        fireEffect.Stop();
        }

        isCastingFire = false; // Nollaa tila

        // Tarkista, osuuko pelaaja hyökkäysalueeseen
        Collider[] hitColliders = Physics.OverlapSphere(fireOrigin.position, fireRange, fireBreathPlayerLayer);
        Debug.Log($"Player layer is {playerLayer.value}, FireBreath layer mask is {fireBreathPlayerLayer.value}");
        foreach (var collider in hitColliders)
        {
            Transform target = collider.transform;
            Vector3 directionToTarget = (target.position - fireOrigin.position).normalized;
            
                Vector3 directionToPlayer = target.position - fireOrigin.position;
                Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);
                fireOrigin.rotation = Quaternion.Slerp(fireOrigin.rotation, targetRotation, Time.deltaTime * 15);
                float angleToTarget = Vector3.Angle(fireOrigin.forward, directionToTarget);
            if (angleToTarget <= fireAngle / 2f)
            {

                // Pelaaja on cone-alueella
                PlayerHealth playerHealth = target.GetComponent<PlayerHealth>();
                if (playerHealth != null)
                {
                    playerHealth.TakeDamage(fireDamage);
                    Debug.Log("Fire breath hit the player!");
                }
            }
        }
        yield break;
    }
        
    
        private IEnumerator PerformTripleAttack()
        {
            for (int i = 0; i < 3; i++)
            {
                if (Vector3.Distance(player.position, transform.position) <= attackRange)
                {
                    PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
                    if (playerHealth != null)
                    {
                        playerHealth.TakeDamage(15f); // Pelaaja ottaa vahinkoa
                        animator.SetTrigger("AttackHit");
                    }
                }
                else
                {
                    Debug.Log("Player moved out of range!");
                    break; // Lopeta hyökkäys, jos pelaaja poistuu alueelta
                }
                yield return new WaitForSeconds(attackInterval);
            }
            isCharging = false; // Nollaa tila hyökkäyksen jälkeen
        }


    public override void ChasePlayer(float distanceToPlayer)
    {
        if (isCharging || isAttacking)
        {
            return; // Ei jahdata pelaajaa, jos ollaan lataamassa tai hyökkäämässä
        }

        // Suoritetaan normaalit jahtauslogiikat, jos ei ole latauksessa tai hyökkäyksessä
        base.ChasePlayer(distanceToPlayer); // Kutsuu peruslogiikan EnemyAI:sta
        agent.speed = 16;
    }
    public override IEnumerator DelayedAttack()
    {
        if (isCharging) // Tarkista, onko chargetimer kesken
        {
            yield break; // Älä suorita hyökkäystä
        }

        animator.SetBool("isAttackRange", true);
        animator.SetBool("ChasePlayer", false);
        isAttacking = true; // Aseta hyökkäystila true
        animator.SetTrigger("BasicAttack");
        AttackPlayer();
        yield return new WaitForSeconds(attackCooldown); // Hyökkäyksen cooldown
        animator.SetBool("isAttacking", false);

        isAttacking = false; // Palauta hyökkäystila false
    }
}*/

    