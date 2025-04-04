using UnityEngine;
using System.Collections.Generic;


public class Loot : MonoBehaviour
{
    public Item itemName; // Esineen nimi
    public float pickUpRange = 3f; // Etäisyys, jonka sisällä pelaaja voi poimia lootin

    private Transform playerTransform;

    private void Start()
    {
        // Etsi pelaajan transform
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }
        else
        {
            Debug.LogWarning("Pelaaja-objektia ei löytynyt! Varmista, että pelaajalla on tag 'Player'.");
        }
    }

    private void OnMouseDown()
    {
        if (playerTransform == null)
        {
            Debug.LogWarning("Pelaajan transform on määrittelemättä.");
            return;
        }

        // Varmista, että pelaaja on riittävän lähellä
        if (Vector3.Distance(playerTransform.position, transform.position) <= pickUpRange)
        {
            // Etsi PlayerInventory-objekti ja sen Inventory-komponentti
            Inventory inventory = playerTransform.Find("PlayerInventory")?.GetComponent<Inventory>();

            if (inventory != null)
            {
                Debug.Log("Lisätään esine inventaarioon.");
                inventory.AddItem(itemName); // Käytä Inventory-komponenttia
                //FindObjectOfType<InventoryUI>().UpdateUI(); // Päivitä UI
                Debug.Log("Tuhoamassa esineen: " + gameObject.name);
                Destroy(gameObject); // Poista esine pelistä
            }
            else
            {
                Debug.LogWarning("Inventaariojärjestelmää ei löytynyt PlayerInventory-objektilta!");
            }
        }
        else
        {
            Debug.Log("Olet liian kaukana nostaaksesi " + itemName.itemName);
        }
    }
}
