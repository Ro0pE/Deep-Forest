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
        if (player != null)
        {
            float distance = Vector3.Distance(player.transform.position, transform.position); // Laske etäisyys pelaajaan
    
            if (distance <= interactionRange && Input.GetKeyDown(KeyCode.E)) // Jos pelaaja on tarpeeksi lähellä ja painaa E
            {
                ToggleVendorPanel();
            }
        }
    }

    void ToggleVendorPanel()
    {
        if (vendorPanel != null)
        {
            vendorManager.UpdateVendorInventory();
            vendorPanel.SetActive(!vendorPanel.activeSelf); // Vaihda panelin tila
            
        }
    }

    void OnDrawGizmosSelected()
    {
        // Piirrä gizmo etäisyysalueen havainnollistamiseksi Scene-näkymässä
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, interactionRange);
    }
}
