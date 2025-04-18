using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using TMPro;
using UnityEngine.UI;

public class EnemyAI : MonoBehaviour
{

    public float detectionRange = 30f; // Etäisyys, jolla vihollinen havaitsee pelaajan
    public float attackRange = 7f; // Etäisyys, jolla vihollinen voi hyökätä pelaajaan
    public float wanderRadius = 30f; // Vaeltamisen säde
    public float wanderInterval = 2f; // Vaeltamisen aikaväli
    public LayerMask playerLayer; // Pelaajan kerros
    public LayerMask groundLayer; // Maatason tarkistuskerros
    public float groundCheckDistance = 1.5f; // Maan tarkistuksen etäisyys
    public float attackCooldown = 1.5f; // Hyökkäysväli
    public float walkSpeed = 7f; // Oletusnopeus vaeltamiseen
    public float runSpeed = 12f; // Oletusnopeus pelaajaa jahdatessa
    public float orginalWalkSpeed = 7f;
    public float orginalRunSpeed = 12f;

    public NavMeshAgent agent;
    public Transform player;
    public PlayerHealth playerHealth;
    public EnemyHealth enemyHealth;
    public Animator animator;
    public float wanderTimer;
    public bool isAttacking; // Tieto siitä, onko vihollinen parhaillaan hyökkäämässä
    public float randomAttack;
    public float distanceToPlayer; 
    public bool isChasingPlayer = false;
    public bool isWandering = false;
    private Coroutine attackCoroutine;
    public bool isOnCooldown = false;
    public bool isCasting = false;

    public void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        playerHealth = player.GetComponent<PlayerHealth>();
        enemyHealth = GetComponent<EnemyHealth>();
        animator = GetComponent<Animator>();
        if (animator == null)
        {
            Debug.Log("Animatori löytykin lapsesta");
            animator = GetComponentInChildren<Animator>();
        }
        wanderTimer = wanderInterval;
        isAttacking = false; // Vihollinen ei hyökkää alussa

    }

    public void Update()
    {
        distanceToPlayer = Vector3.Distance(transform.position, playerHealth.transform.position);

        if (enemyHealth.isStunned || enemyHealth.isDead || isCasting)
            return;

        distanceToPlayer = Vector3.Distance(player.position, transform.position);

        if (distanceToPlayer <= attackRange)
        {
            StartAttacking();
        }
        else if (distanceToPlayer <= detectionRange)
        {
            ChasePlayer(distanceToPlayer);
            StopAttacking(); // Varmista ettei hyökkäys jatku, jos pelaaja poistui hyökkäysetäisyydeltä
        }
        else
        {
            StopAttacking(); // Pelaaja liian kaukana
            Wander();
        }
    }


    private void UpdatePositionToGround()
    {
        RaycastHit hit;
        // Suorita raycast alaspäin tarkistaaksesi maan korkeus
        if (Physics.Raycast(transform.position, Vector3.down, out hit, groundCheckDistance, groundLayer))
        {
            // Aseta vihollisen Y-koordinaatti maan pinnalle
            Vector3 newPosition = transform.position;
            newPosition.y = hit.point.y + 0.5f; // Voit säätää tätä arvoa tarvittaessa
            transform.position = newPosition;
        }
    }

    public virtual void StartAttacking()
    {
 
      
        if (!isOnCooldown)
        {
            attackCoroutine = StartCoroutine(AttackLoop());
          
        }
    }
    private IEnumerator CooldownRoutine()
    {
        isOnCooldown = true;
        yield return new WaitForSeconds(attackCooldown);
        isOnCooldown = false;
    }


    public virtual void StopAttacking()
    {
        if (attackCoroutine != null)
        {
            StopCoroutine(attackCoroutine);
            attackCoroutine = null;
        }

        isAttacking = false;
        agent.isStopped = false;
        animator.ResetTrigger("isAttacking");
        animator.SetBool("isAttacking", false);
    }


    public virtual void AttackPlayer()
    {
        Debug.Log("Attack");
        animator.SetTrigger("isAttacking");
        isAttacking = true;
        isOnCooldown = true;

        float distanceToPlayer = Vector3.Distance(player.position, transform.position);
        
        if (distanceToPlayer <= attackRange) // Jos karhu on liian lähellä
        {
            Vector3 directionToPlayer = (player.position - transform.position).normalized;

            // Rajoita karhun lähestymistä pelaajaan


            // Törmäystarkistus
            Ray ray = new Ray(transform.position, directionToPlayer);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, attackRange, playerLayer))
            {
                if (hit.collider.CompareTag("Player"))
                {
                    if (enemyHealth.isDead)
                    {
                        Debug.Log("Enemy is dead");
                    }
                    else
                    {
                        playerHealth.TakeDamage((enemyHealth.attackDamage));
                    }
                }
            }
        }

         
    }
    public virtual IEnumerator AttackLoop()
    {
        if (isOnCooldown) yield break;

        isAttacking = true;
        isOnCooldown = true;

        animator.SetBool("isAttacking", true);
        animator.SetBool("isRunning", false);
        animator.SetBool("isWalking", false);
        agent.isStopped = true;

        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 25f);

        AttackPlayer(); // tehdään hyökkäys heti

        // Käynnistä cooldown
        StartCoroutine(CooldownRoutine());

        yield return new WaitForSeconds(0.1f); // pieni odotus että ei spämmiä

        isAttacking = false;
        attackCoroutine = null;
    }




    public virtual void ChasePlayer(float distanceToPlayer)
    {
        //Debug.Log("Chasing");
        if (player == null) return;

        isWandering = false;
        isChasingPlayer = true;
        detectionRange = 70;
        agent.speed = runSpeed;

        animator.SetBool("isRunning", true);

        // Jatka seuraamista
        if (!agent.isStopped)
        {
            agent.SetDestination(player.position);
        }
        else
        {
            Debug.Log("agent is stopped, not chasing");
        }
            
    }



    public virtual void Wander()
    {
        //Debug.Log("Wandering");
        if (enemyHealth.isDead)
        {
            //Debug.Log("Cant wander while dead");
            return;
        }
        isWandering = true;
        isChasingPlayer = false;
        detectionRange = 40;
        agent.speed = walkSpeed;
        //animator.SetFloat("Speed", agent.velocity.magnitude);
        animator.SetBool("isRunning", false);
        animator.SetBool("isWalking", true);
        wanderTimer += Time.deltaTime;


        // Jos vihollinen on saavuttanut määränpäänsä tai vaeltaminen on kestänyt tarpeeksi pitkään, valitse uusi kohde
        if (wanderTimer >= wanderInterval || agent.remainingDistance < 0.5f)
        {
            // Luo satunnainen pysähtymisaika 1-12 sekuntia
            float stopTime = Random.Range(1f, 5f);
            StartCoroutine(PauseBeforeNextMove(stopTime));
            
            // Luo uusi satunnainen paikka vaeltamista varten
            Vector3 newPos = RandomNavSphere(transform.position, wanderRadius, -1);
            agent.SetDestination(newPos);
            wanderTimer = 0;
        }
      

        // Animaation hallinta: kävelyanimaatio on päällä, kun agentti liikkuu
        
    }

    // Tauko liikkeen välissä
    private IEnumerator PauseBeforeNextMove(float duration)
    {
        //agent.isStopped = true; // Pysäytä agentti
        //animator.SetBool("WalkForward", false); // Lopeta kävelyanimaatio

        yield return new WaitForSeconds(duration); // Odota satunnainen aika

        //agent.isStopped = false; // Jatka agentin liikettä
    }

    bool IsGrounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, groundCheckDistance, groundLayer);
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
}
