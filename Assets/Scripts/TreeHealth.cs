using System.Collections;
using UnityEngine;

public class TreeHealth : MonoBehaviour
{
    public int health = 100; // Puun kestopisteet
    public GameObject[] lootPrefabs; // Array loot-esineistä

    // Metodi hyökkäykselle
    public void TakeDamage(int damage)
    {
        Debug.Log("Chopping tree with " + damage + "dmg");
        health -= damage;

        // Aloita tärinäefekti
        StartCoroutine(ShakeTree());

        if (health <= 0)
        {
            Debug.Log("Tree is down!");
            Die();
        }
    }

    // Tuhoutuessa kutsuttava metodi
    void Die()
    {
        if (lootPrefabs.Length > 0)
        {
            int randomIndex = Random.Range(0, lootPrefabs.Length);
            GameObject selectedLoot = lootPrefabs[randomIndex];

            // Instansioi loot-objekti puun sijaintiin
            GameObject loot = Instantiate(selectedLoot, transform.position, Quaternion.identity);

            // Lisää lisävoimaa alaspäin putoamisen nopeuttamiseksi
            Rigidbody rb = loot.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddForce(Vector3.down * 100f, ForceMode.Impulse); // Säädä arvoa 100f nopeuden mukaan
            }
        }

        // Tuhotaan puu-objekti
        Destroy(gameObject);
    }

    // Tärinäefekti lyönnin yhteydessä
    private IEnumerator ShakeTree()
    {
        Vector3 originalPosition = transform.position;
        float shakeDuration = 0.2f; // Tärinän kesto
        float shakeMagnitude = 0.1f; // Tärinän voimakkuus
        float elapsed = 0.0f;

        while (elapsed < shakeDuration)
        {
            float x = Random.Range(-1f, 1f) * shakeMagnitude;
            float y = Random.Range(-1f, 1f) * shakeMagnitude;
            transform.position = new Vector3(originalPosition.x + x, originalPosition.y + y, originalPosition.z);

            elapsed += Time.deltaTime;
            yield return null;
        }

        // Palauta alkuperäiseen sijaintiin
        transform.position = originalPosition;
    }
}
