using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class InventoryUI : MonoBehaviour
{
    public GameObject slotPrefab;
    public Transform slotParent;
    public Inventory playerInventory;
    public List<InventorySlot> slots; //new List<InventorySlot>();
    public TextMeshProUGUI playerGold;


    void Start()
    {
        playerInventory = FindObjectOfType<Inventory>();
        UpdateUI();
        slotParent.gameObject.SetActive(false);
    }


    public void UpdateUI()
    {
        if (playerInventory == null)
        {
            Debug.Log("PlayerInventory is null! Make sure it is assigned or available in the scene.");
            return;
        }

        if (playerInventory.inventoryPanel == null)
        {
            Debug.Log("Inventory panel is not assigned in the Inventory script!");
            return;
        }
        playerInventory.inventoryPanel.SetActive(true);
        // Käydään läpi kaikki slotit ja asetetaan esineet
        for (int i = 0; i < slots.Count; i++)
        {
            if (i < playerInventory.items.Count)
            {
                Item item = playerInventory.items[i];

                // Jos esine löytyy, käytetään SetItem-metodia
                if (item != null)
                {
                    
                    
                    slots[i].gameObject.SetActive(true); // Varmistetaan, että slotti on näkyvissä
                    slots[i].SetItem(item);
                    
                }
                else
                {
                    
                    slots[i].gameObject.SetActive(false); // Piilotetaan tyhjä slotti
                }
            }
            else
            {
                slots[i].quantityText.text = "";
                slots[i].gameObject.SetActive(false); // Piilotetaan tyhjä slotti, jos ei ole esinettä
            }
        }
        playerGold.text = playerInventory.playerMoney.ToString();
        
       
    }


}
