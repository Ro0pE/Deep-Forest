using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 10f; // Projektiilin nopeus
    public float lifetime = 1f; // Aika, jonka jälkeen projektiili tuhoutuu

    private Transform target; // Vihollinen, johon projektiili lentää

    public void Initialize(Transform targetTransform)
    {
        target = targetTransform; // Aseta kohde
    }

    void Start()
    {
        Destroy(gameObject, lifetime); // Tuhotaan projektiili automaattisesti tietyn ajan kuluttua
    }
    void Update()
    {
        if (target != null)
        {
            // Lisätään korkeuden offset kohteeseen
            Vector3 targetPosition = target.position;
            targetPosition.y += 1.5f; // Lisää 1.5 yksikköä korkeutta, jotta projektiili osuu ylemmäs

            // Tarkista, onko projektiili tarpeeksi lähellä kohdetta
            if (Vector3.Distance(transform.position, targetPosition) <= 1.5f) // Säätöetäisyys 1.5
            {
                Debug.Log("Projektiili osui kohteeseen!");
                //Invoke("DestroyProjectile", 0.3f); // Tuhotaan 0.3 sekunnin kuluttua
                //Destroy(gameObject); // Tuhotaan, jos kohde katoaa
                return;
            }

            // Liikuta projektiilia kohti kohteen korotettua sijaintia
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

            // Käännä projektiili kohti kohdetta
            transform.LookAt(targetPosition);
        }
        else
        {
            Destroy(gameObject); // Tuhotaan, jos kohde katoaa
        }
    }

    // Metodi projektiilin tuhoamiseen
    void DestroyProjectile()
    {
        Destroy(gameObject); // Tuhotaan projektiili
    }



    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy")) // Varmistetaan, että osutaan viholliseen
        {
            Debug.Log("Projektiili osui viholliseen!s");
            Destroy(gameObject); // Tuhotaan projektiili
            // Lisätoiminnallisuus: Viholliselle vahinkoa
            EnemyHealth enemyHealth = other.GetComponent<EnemyHealth>();
            if (enemyHealth != null)
            {
                Debug.Log("osuma!");
               //enemyHealth.TakeDamage(10); // Esimerkkiarvo vahingolle
            }
        }
        else if (other.CompareTag("Player"))
        {
            Destroy(gameObject); // Tuhotaan, jos kohde katoaa
            Debug.Log("pelaajaan osu");
        }
    }
}
