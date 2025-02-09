using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC_vendor : MonoBehaviour
{
    public GameObject vendorPanel; // Paneli, joka avataan
    public float interactionRange = 25f; // Maksimi etäisyys pelaajasta
    private GameObject player; // Pelaajan objekti
    public VendorManager vendorManager;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player"); // Etsi pelaaja tagin avulla
        vendorManager = FindObjectOfType<VendorManager>();
    }

    void Update()
    {
        // Tämä voi jäädä pois, koska emme enää tarvitse 'E' painiketta, mutta jos haluat
        // jatkaa etäisyyden tarkistusta, voit pitää sen
        if (player != null)
        {
            float distance = Vector3.Distance(player.transform.position, transform.position); // Laske etäisyys pelaajaan
        
            if (distance <= interactionRange)
            {
                // Tarkista, onko pelaaja klikkaamassa vendorin objektia
                // Tämä ei ole enää tarpeen, koska käytämme OnMouseDownia
            }
        }
    }

    void OnMouseDown() 
    {
        // Tämä kutsuu ToggleVendorPanel, kun pelaaja klikkaa vendorin objektia
        ToggleVendorPanel();
    }

    void ToggleVendorPanel()
    {
        if (vendorPanel != null)
        {
            vendorManager.UpdateVendorInventory();
            vendorPanel.SetActive(!vendorPanel.activeSelf); // Vaihda panelin tila
        }
    }


}
