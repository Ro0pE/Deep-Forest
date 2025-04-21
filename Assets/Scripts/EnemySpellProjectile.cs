using UnityEngine;

public class EnemySpellProjectile : MonoBehaviour
{
    public float speed = 55f;
    public float lifetime = 2f;
    private Transform target;
    public ShooterType shooterType;
    public int damage = 10;
    private Transform targetHitPoint;


    public void Initialize(Transform hitPointTransform, ShooterType shooter)
    {
        targetHitPoint = hitPointTransform;
        shooterType = shooter;
    }

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        if (targetHitPoint == null)
        {
            Debug.Log("Target hitpoint null");
            Destroy(gameObject);
            return;
        }

        // Siirretään targetPosition hieman eteenpäin pelaajan suuntaan
        Vector3 targetPosition = targetHitPoint.position;

        // Tsekataan pienellä etäisyydellä, ei koskaan -1.5f (negatiivinen ei toimi järkevästi)
        if (Vector3.Distance(transform.position, targetPosition) <= 1f)
        {
            Debug.Log("nyt ois osuman paikka");
            Destroy(gameObject);
            return;
        }

        transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
        transform.LookAt(targetPosition);
    }


    private void OnTriggerEnter(Collider other)
    {
        /*if (shooterType == ShooterType.Player && other.CompareTag("Enemy"))
        {
            EnemyHealth enemyHealth = other.GetComponent<EnemyHealth>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(damage);
            }
            Destroy(gameObject); 
        }
        else*/ if (shooterType == ShooterType.Enemy && other.CompareTag("Player"))
        {
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                Debug.Log("Osuu pelaajaan " + damage);
                //playerHealth.TakeDamage(damage);
            }
            //Destroy(gameObject);
        }
    }
}
