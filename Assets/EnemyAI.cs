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
    public float attackDamage = 5f; // Hyökkäysvoima
    public float groundCheckDistance = 1.5f; // Maan tarkistuksen etäisyys
    public float attackCooldown = 1.5f; // Hyökkäysväli
    public float walkSpeed = 5f; // Oletusnopeus vaeltamiseen
    public float runSpeed = 10f; // Oletusnopeus pelaajaa jahdatessa

    public NavMeshAgent agent;
    public Transform player;
    public PlayerHealth playerHealth;
    public EnemyHealth enemyHealth;
    public Animator animator;
    public float wanderTimer;
    public bool isAttacking; // Tieto siitä, onko vihollinen parhaillaan hyökkäämässä
    public float randomAttack;
    public float distanceToPlayer; 
    

    public void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        playerHealth = player.GetComponent<PlayerHealth>();
        enemyHealth = GetComponent<EnemyHealth>();
        animator = GetComponent<Animator>();
        wanderTimer = wanderInterval;
        isAttacking = false; // Vihollinen ei hyökkää alussa

    }

    public void Update()
    {
        if (enemyHealth.isStunned)
        {
            return;
        }
       
        distanceToPlayer = Vector3.Distance(player.position, transform.position);

        if (distanceToPlayer <= attackRange && !isAttacking)
        {
            StartCoroutine(DelayedAttack());
        }
        else if (distanceToPlayer <= detectionRange)
        {
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

public virtual IEnumerator DelayedAttack()
{
    Vector3 directionToPlayer = (player.position - transform.position).normalized;
    Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);
    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 25f); // Aseta kääntymisnopeus tarvittaessa
    animator.SetBool("isAttacking", true);
    animator.SetBool("isWalking", false);
    animator.SetBool("isRunning", false);
    isAttacking = true; // Aseta hyökkäystila true
    
    AttackPlayer();
    yield return new WaitForSeconds(attackCooldown); // Voit vähentää tai poistaa tämän testataksesi
   

    isAttacking = false; // Palauta hyökkäystila false
    animator.SetBool("isAttacking", false);
}

    public virtual void AttackPlayer()
    {


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
                        
                    }
                    else
                    {
                if (randomAttack < 4){
                     playerHealth.TakeDamage((attackDamage * 1));
                }
                else if (randomAttack > 3  && randomAttack < 8)
                {
                     playerHealth.TakeDamage((attackDamage * 2));
                } 
                else
                {
                     playerHealth.TakeDamage((attackDamage * 3));
                }
                }
                }
            }
        }
         
    }

public virtual void ChasePlayer(float distanceToPlayer)
{
    if (player != null)
    {
        detectionRange = 70;
        agent.speed = runSpeed;
        animator.SetBool("isRunning", true);

        // Jos agentilla on voimassa oleva reitti, jatka sen seuraamista
        if (agent.hasPath && !agent.pathPending && !agent.isPathStale)
        {
            // Jos reitti on voimassa, liikutaan sen mukaan
            agent.SetDestination(player.position);
            return;
        }

        // Jos ei ole reittiä, etsitään uusi reitti
        if (distanceToPlayer <= attackRange)
        {
            animator.SetBool("isRunning", false);
            agent.isStopped = true;

            if (!isAttacking)
            {
                StartCoroutine(DelayedAttack());
            }
        }
        else if (distanceToPlayer <= detectionRange)
        {
            agent.isStopped = false;

            // Jos ei ole reittiä tai se on vanhentunut, asetetaan uusi reitti
            if (!agent.hasPath || agent.isPathStale || agent.pathPending)
            {
                Debug.Log("Reitti ei ole voimassa tai agentti odottaa reittiä.");
                agent.ResetPath();
                agent.SetDestination(player.position);
            }
        }
        else
        {
            Debug.Log("Pelaaja on liian kaukana.");
        }
    }
}




    public virtual void Wander()
    {
        if (enemyHealth.isDead)
        {
            //Debug.Log("Cant wander while dead");
            return;
        }
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
