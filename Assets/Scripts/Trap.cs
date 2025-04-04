using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class Trap : MonoBehaviour
{
    public Action<EnemyHealth> OnTrapActivated;
    private EnemyHealth lastTriggeredEnemyHealth;
    public float activationRadius = 3f; // Etäisyys, jonka sisällä viholliset aktivoivat ansan
    public bool isActive; // Onko ansa aktiivinen

    private void Start()
    {
        // Alustetaan ansa, mutta ei aktivoida heti
        isActive = true;
    }

    // Metodi ansan asettamiseen
    public void PlaceTrap(Vector3 position)
    {
        transform.position = position;
        isActive = true; // Ansasta tulee aktiivinen, kun se asetetaan paikoilleen
       
    }

    // Metodi ansan aktivointiin, jota voidaan kutsua esimerkiksi, kun vihollinen astuu ansaan
    public void ActivateTrap()
    {
        if (isActive)
        {
            Debug.Log("Ansan aktivointi!");
            isActive = false; // Ansan aktivointi tapahtui, joten se ei ole enää aktiivinen
            Destroy(gameObject, 2f); // Poistetaan ansa hetken kuluttua

            EnemyHealth enemyHealth = lastTriggeredEnemyHealth; // Ota viimeisin vihollinen
            if (enemyHealth != null && !enemyHealth.isDead)
            {
                OnTrapActivated?.Invoke(enemyHealth); // ✅ Kutsu eventti, jos siihen on liitytty
            }
        }
    }

    // Tämä voi olla vihollisten tarkistamiseen ansan ympärillä
    private void OnTriggerEnter(Collider other)
    {
        EnemyHealth enemyHealth = other.GetComponent<EnemyHealth>();

            if (enemyHealth != null && !enemyHealth.isDead)
            {
                lastTriggeredEnemyHealth = enemyHealth;
                Debug.Log(enemyHealth.monsterName + " ottaa osumaa!");
                ActivateTrap();
            }
            else
            {
                Debug.LogWarning("EnemyHealth EI löytynyt vaikka " + other +" löytyi.");
            }
        }

    
}
