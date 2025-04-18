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
    private GameObject hitEffect;

    // Alusta nuoli ja määritä, onko se räjähtävä
    public void Initialize(Transform targetTransform, Skill skillData, bool explosive = false, bool critical = false, GameObject effectPrefab = null)
    {
        target = targetTransform;
        skill = skillData;
        isExplosive = explosive;
        isCrit = critical;
        hitEffect = effectPrefab;  // Talletetaan efekti
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
            if (Vector3.Distance(transform.position, targetPosition) <= 6.5f)
            {
                Debug.Log("Update räjähys");
                Explode(); // Käynnistetään räjähdys
                
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
        private void Explode()
        {


            //Destroy(gameObject); // Poistetaan projektiili
            StartCoroutine(ExplosiveDelay());
        }

    IEnumerator ExplosiveDelay()
    {
        Debug.Log("EXPLOSION DELAY!!");

        // Viivästys ennen räjähdyksen jälkeisiä toimintoja
        yield return new WaitForSeconds(0.05f); // 0.1 sekunnin viive ennen räjähdystä

        // Ladataan räjähdysanimaatio (tässä voidaan käyttää parametrina saatua prefabia)
        if (hitEffect != null)
        {
            Debug.Log("Mitä helve");
            Debug.Log("RÄjähdyksen nimi :  " + hitEffect);
            GameObject explosionInstance = Instantiate(hitEffect, target.position, Quaternion.identity);
            explosionInstance.transform.SetParent(target);
            

            if (explosionInstance != null)
            {
                Destroy(explosionInstance, 1.3f); // Räjähdysinstanssi poistetaan 1.3 sekunnin kuluttua
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
        Debug.Log("Metodi loppu");
        Destroy(gameObject);
    }
    

}
