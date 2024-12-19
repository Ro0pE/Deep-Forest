using UnityEngine;

public class BigTreeHealth : MonoBehaviour
{
    public int health = 300; // Puun alkuperäinen terveys
    

    // Metodi, jota kutsutaan, kun puuta hyökätään
    public void TakeDamage(int damage)
    {
        health -= damage;
        Debug.Log("Puu saa vahinkoa: " + damage + " | Jäljellä oleva terveys: " + health);

        // Tarkista, onko puu tuhoutunut
        if (health <= 0)
        {
            Die();
        }
    }

    // Metodi, joka kutsutaan, kun puu tuhoutuu
    private void Die()
    {
        Debug.Log("Puu on tuhoutunut!");
        // Täällä voit lisätä logiikkaa puun poistamiseksi tai kaatamiseksi
        Destroy(gameObject); // Poistaa puun pelistä
    }
}
