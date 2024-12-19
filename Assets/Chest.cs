using UnityEngine;
using System.Collections; // Lisää tämä rivi

public class Chest : MonoBehaviour
{
    public Animator animator;            // Arkkuun liitetty Animator
    public Transform player;             // Pelaajan Transform
    public float openDistance = 10f;     // Etäisyys, jolla arkku voidaan avata
    public GameObject lootPrefab;        // Prefab loottiobjektille
    public int lootCount = 3;            // Määrä luotavia loottiobjekteja
    public float lootSpawnRadius = 2.5f; // Etäisyys arkusta, johon lootti ilmestyy

    private bool canOpen = false;        // Tarkistaa, onko pelaaja riittävän lähellä
    private bool isOpened = false;       // Estää arkun avaamisen useampaan kertaan

    private void Start()
    {
        if (animator == null)
        {
            animator = GetComponent<Animator>(); // Hakee animaattorin, jos ei ole asetettu
        }
    }

    private void Update()
    {
        // Tarkistaa pelaajan etäisyyden
        if (Vector3.Distance(transform.position, player.position) <= openDistance)
        {
            canOpen = true;
        }
        else
        {
            canOpen = false;
        }

        // Avaa arkku, jos pelaaja on riittävän lähellä ja painaa "E"-näppäintä
        if (canOpen && Input.GetKeyDown(KeyCode.E) && !isOpened)
        {
            animator.SetTrigger("Open");  // Käynnistää avausanimaation
            isOpened = true;              // Estää uudelleen avaamisen
            StartCoroutine(SpawnLootWithDelay(1f)); // Käynnistää lootin spawnauksen 1 sekunnin viiveellä
        }
    }

    private IEnumerator SpawnLootWithDelay(float delay)
    {
        // Odota määritetty viive ennen lootin spawnia
        yield return new WaitForSeconds(delay);
        
        SpawnLoot(); // Kutsu lootin spawn-funktiota
    }

    void SpawnLoot()
    {
        for (int i = 0; i < lootCount; i++)
        {
            // Määrittää satunnaisen sijainnin arkun ympärillä
            Vector3 spawnPosition = transform.position;
            spawnPosition.x += Random.Range(-lootSpawnRadius, lootSpawnRadius); // Satunnainen paikka arkun sivuille (x-akselilla)
            spawnPosition.z += Random.Range(-lootSpawnRadius, lootSpawnRadius); // Satunnainen paikka arkun sivuille (z-akselilla)

            // Aseta lootin y-koordinaatti arkun y-koordinaatin yläpuolelle
            spawnPosition.y = transform.position.y + 1.5f; // Muuta 0.5f tarvittaessa, jotta lootit ovat tarpeeksi korkealla

            // Luo loottiobjektin spawnPosition-sijaintiin
            Instantiate(lootPrefab, spawnPosition, Quaternion.identity);
        }
    }
}
