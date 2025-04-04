using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using TMPro;


public class ExplosiveProjectile : MonoBehaviour
{
    public float speed = 10f; // Projektiilin nopeus
    public float lifetime = 1f; // Projektiilin elinikä
    public float explosionRadius = 7f; // Räjähdyksen säde

    private Transform target;  // Kohde, johon projektiili lentää
    private bool isExplosive = false;  // Onko nuoli räjähtävä?
    private Skill skill;  // Skill, joka aiheuttaa vahingon
    private bool isCrit; // Onko kriittinen isku?

    // Alusta nuoli ja määritä, onko se räjähtävä
    public void Initialize(Transform targetTransform, Skill skillData, bool explosive = false, bool critical = false)
    {
        target = targetTransform;
        skill = skillData;
        isExplosive = explosive;
        isCrit = critical;
    }

    void Start()
    {
        Destroy(gameObject, lifetime); // Tuhotaan projektiili automaattisesti tietyn ajan kuluttua
    }

    void Update()
    {
        if (target != null)
        {
            Vector3 targetPosition = target.position;
            targetPosition.y += 1.5f; // Pieni korotus, jotta osuu kohteen yläpuolelle

            // Tarkistetaan, onko nuoli tarpeeksi lähellä kohdetta
            if (Vector3.Distance(transform.position, targetPosition) <= 1.5f)
            {
                Debug.Log("Update räjähys");
                Explode(); // Käynnistetään räjähdys
                Destroy(gameObject);
                return;
            }

            // Liikutetaan projektiilia kohti kohdetta
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
            transform.LookAt(targetPosition);
        }
        else
        {
            Destroy(gameObject); // Tuhotaan, jos kohde katoaa
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            Debug.Log("Trigger räjähys");
            Explode();
        }
           // Destroy(gameObject);
        }
    

    // Räjähtävän nuolen AOE-vahinko
        void Explode()
        {
            Debug.Log("Nuoli räjähti!");

            // Haetaan kaikki viholliset räjähdyksen alueelta
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, explosionRadius);
            foreach (Collider hitCollider in hitColliders)
            {
                EnemyHealth enemy = hitCollider.GetComponent<EnemyHealth>();
                if (enemy != null && !enemy.isDead)
                {
                    // Välitetään vahinko räjähtävälle alueelle (esim. 20 % vahingosta)
                    Skill explosiveSkill = skill.Clone(); // Tee kopio alkuperäisestä skillistä
                    explosiveSkill.baseDamage = Mathf.RoundToInt(skill.baseDamage * 0.2f);
                    Debug.Log("Explosive damagea tulossa : " + explosiveSkill.baseDamage);
                    enemy.TakeDamage(explosiveSkill, isCrit);
                }
            }

            // Lisää viive räjähdyksen jälkeen
            StartCoroutine(ExplosiveDelay());
        }

        IEnumerator ExplosiveDelay()
        {
            Debug.Log("EXPLOSION DELAY!!");

            // Viivästys ennen räjähdyksen jälkeisiä toimintoja
            

            // Ladataan räjähdysanimaatio
            GameObject explosionEffect = Resources.Load<GameObject>("Explosions/Explosion2");
            Debug.Log("Tässä toimii ?");
            if (explosionEffect != null)
            {
                Debug.Log("Explosion prefab löytyi!");

                // Instansioidaan räjähdysanimaatio
                GameObject explosionInstance = Instantiate(explosionEffect, target.position, Quaternion.identity);
                explosionInstance.transform.SetParent(target);

                // Varmistetaan, että instansiointi on onnistunut
                if (explosionInstance != null)
                {
                    Debug.Log("Räjähdysanimaatio aloitettu!");
                    Destroy(explosionInstance, 1.4f); // Räjähdysinstanssi tuhotaan 5 sekunnin kuluttua
                }
                else
                {
                    Debug.Log("Räjähdysanimaatio ei onnistunut instansioimaan!");
                }
            }
            
            else
            {
                Debug.Log("Explosion prefabia ei löytynyt!");
            }
            Debug.Log("Toimiiko lopussa");
            yield return new WaitForSeconds(0.1f); // 0.3 sekunnin viive
        }
    

}
