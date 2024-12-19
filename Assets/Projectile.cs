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
            // Liikuta projektiilia kohti kohdetta
            transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);

            // Käännä projektiili kohteen suuntaan (valinnainen)
            transform.LookAt(target);
        }
        else
        {
            Destroy(gameObject); // Tuhotaan, jos kohde katoaa
        }
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
    }
}
