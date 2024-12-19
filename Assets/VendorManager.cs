using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class VendorManager : MonoBehaviour
{
    public GameObject vendorPanel;
    public GameObject playerItems;
    public GameObject vendorSlots;
    public TextMeshProUGUI totalSellValue;
    public TextMeshProUGUI itemsValue;
    public Button sellAllButton;
    public Button removeItemButton;
    public Inventory playerInventory;
    public TextMeshProUGUI playerTotalMoney;
    public List<Item> sellingItems = new List<Item>();
    public int totalSellAmount;
    public VendorInventorySlot[] slots;
    public VendorSellSlot[] vendorSellSlots;


    // Start is called before the first frame update
    void Start()
    {
        playerInventory = FindObjectOfType<Inventory>();
        slots = playerItems.GetComponentsInChildren<VendorInventorySlot>();
        vendorSellSlots = vendorSlots.GetComponentsInChildren<VendorSellSlot>();
        UpdateVendorInventory(); // Päivitä vendorin inventory heti alussa
    }

    // Update is called once per frame
    void Update()
    {
       playerTotalMoney.text = playerInventory.playerMoney.ToString();
       totalSellValue.text = totalSellAmount.ToString();
        // Voit lisätä tähän koodia myöhemmin, jos tarvitset jatkuvaa päivitystä
    }

    public void UpdateVendorInventory()
    {
        Debug.Log("Updating Vendor Inventory");

        // Käy läpi vendorin sloteista ja täytä myytävät tavarat
        for (int i = 0; i < vendorSellSlots.Length; i++)
        {
            if (i < sellingItems.Count)
            {
                vendorSellSlots[i].SetItem(sellingItems[i]); // Aseta tavara slottiin
            }
            else
            {
                vendorSellSlots[i].ClearSlot(); // Tyhjennä ylimääräiset slotit
            }
        }

        // Käy pelaajan inventory läpi ja täytä slottien tiedot
        for (int i = 0; i < slots.Length; i++)
        {
            if (i < playerInventory.items.Count)
            {
                Debug.Log("Laitetaan slottiin item: " + playerInventory.items[i]);
                slots[i].SetItem(playerInventory.items[i]); // Aseta tavara slottiin
            }
            else
            {
                slots[i].ClearSlot(); // Tyhjennä ylimääräiset slotit
            }
        }


        CalculateValue();


    }

    // Lisää tavara myyntiin
    public void AddItemForSale(Item item)
    {
        Debug.Log("Item " + item.itemName + " added to vendor");
        sellingItems.Add(item);
        UpdateVendorInventory(); // Päivitä vendorin inventory, jotta uusi tavara näkyy
    }

    public void CalculateValue()
    { 
        totalSellAmount = 0;
            foreach (Item item in sellingItems) 
            {
                totalSellAmount += (item.sellPrice * item.quantity);
            }
    }


    public void SellAllItems()
    {
        playerInventory.playerMoney += totalSellAmount;
        totalSellAmount = 0;
        Debug.Log("looppaa ja tyhjennä slotit");
        sellingItems = new List<Item>();
        UpdateVendorInventory();
    }
    public void CloseVendor()
    {
        vendorPanel.SetActive(false);
    }


}
