using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Threading.Tasks;  // For async/await

public class Inventory : MonoBehaviour
{
    public List<Item> items = new List<Item>();  // This list can contain any items, including potions
    public List<Item> potions = new List<Item>(); // Lista potioneista
    public GameObject potionSlotPrefab; // Aseta tämä Inspectorissa
    public Transform potionSlotParent;  // Paneeli johon slotit instansoidaan
    public List<GameObject> potionSlots = new List<GameObject>(); // Lista sloteista
    public ItemDatabase itemDatabase;
    public GameObject inventoryPanel;
    public int maxItems = 20;
    public int playerMoney;
    public InventoryUI inventoryUI;

    private async void Start()
    {
        if (itemDatabase == null)
        {
            Debug.LogError("ItemDatabase on null!");
            return;
        }
        for (int i = 0; i < 6; i++)
        {
            potions.Add(null);
        }

        UpdatePotionInventory(); // Päivitetään inventory heti
        // Await loading database asynchronously
        await LoadDatabaseAndStart();

        // Check if the database was loaded correctly
        if (!itemDatabase.isDatabaseLoaded)
        {
            Debug.LogError("ItemDatabase failed to load!");
            return;
        }

        // Ensure InventoryUI is found in the scene
        inventoryUI = FindObjectOfType<InventoryUI>();
        if (inventoryUI == null)
        {
            Debug.LogError("InventoryUI not found in the scene!");
            return;
        }

        // Add items to the inventory
        AddItemToInventoryByName("Minor Healing Potion", 5);
        AddItemToInventoryByName("Frenzy Bow", 1);
        AddItemToInventoryByName("Minor Mana Potion", 4);
        AddItemToInventoryByName("Wooden Bow", 1);
        AddItemToInventoryByName("Arrow", 1000);
        AddItemToInventoryByName("Fire Arrow", 1000);
        AddItemToInventoryByName("Frost Arrow", 1000);
        AddItemToInventoryByName("Shoulders of the bear", 1);
        AddItemToInventoryByName("Tunica Of Quickness", 1); 
         AddItemToInventoryByName("Novice Hat", 1);
        AddItemToInventoryByName("Worn Shirt", 1);
        AddItemToInventoryByName("Training Sword", 1);
        AddItemToInventoryByName("Work Shoes", 1);
        AddItemToInventoryByName("Shoulders of the bear", 1);
        AddItemToInventoryByName("Tunica Of Quickness", 1); 
    }

    // Async method to load the database
    private async Task LoadDatabaseAndStart()
    {
        await itemDatabase.LoadDatabaseFromSheetsAsync();
        
    }
public void UpdatePotionInventory()
{
    // Varmista, että potionSlots sisältää 6 slotia
    if (potionSlots.Count != 6)
    {
        Debug.LogError("Potion slotteja ei ole määritelty 6 kappaletta Inspectorissa!");
        return;
    }

    // Päivitetään slotit
    for (int i = 0; i < potionSlots.Count; i++)
    {
        InventorySlot slotScript = potionSlots[i].GetComponent<InventorySlot>();

        if (slotScript != null)
        {
            if (i < potions.Count && potions[i] is Potion potion)
            {
                if (potion.quantity > 0)
                {
                    slotScript.SetPotion(potion);
                }
                else
                {
                    slotScript.ClearSlot();
                    potions[i] = null;  // Poistetaan tyhjä potion listasta
                }
            }
            else
            {
                slotScript.ClearSlot();
            }
        }
    }
}





    // Add item to inventory by name
    public void AddItemToInventoryByName(string itemName, int quantity)
    {
        Item item = itemDatabase.GetItemByName(itemName);
       
        if (item != null)
        {
            Item newItem = Instantiate(item);
            newItem.quantity = quantity;
            items.Add(newItem);
            
        }
        else
        {
            Debug.LogError($"Item {itemName} not found!");
        }
    }
    void UsePotion(int index)
    {
        if (index >= 0 && index < potionSlots.Count)
        {
            InventorySlot slotScript = potionSlots[index].GetComponent<InventorySlot>();
            if (slotScript != null && slotScript.currentItem != null)
            {
                slotScript.HandleItemUse(); // Käytä potionin HandleItemUse-metodia
                //potions[index] = null; // Poista potion listasta
                UpdatePotionInventory(); // Päivitä käyttöliittymä
            }
        }
    }

    // Handle inventory toggling when pressing the B key
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            inventoryPanel.SetActive(!inventoryPanel.activeSelf);

            if (inventoryPanel.activeSelf && inventoryUI != null)
            {
                
                inventoryUI.UpdateUI();
            }
        }
        for (int i = 1; i <= 6; i++)  
        {
            if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))  
            {
                if (Input.GetKeyDown(KeyCode.Alpha1 + (i - 1)))  
                {
                    UsePotion(i - 1); // Käytä slottia (0-indeksoitu)
                }
            }
        }
    }

    // Equip an item (add it to inventory, if needed)
    public void EquipItem(Item newItem)
    {
        if (newItem == null)
        {
            Debug.LogWarning("Trying to add a null item to inventory!");
            return;
        }

        // Check if item is stackable
        if (!newItem.isStackable)
        {
            // If inventory has space, clone the item
            if (items.Count < maxItems)
            {
                Item clonedItem = Instantiate(newItem);
                items.Add(clonedItem);
                inventoryUI.UpdateUI();
            }
            else
            {
                Debug.LogWarning("Inventory full, cannot add item: " + newItem.itemName);
            }
        }
        else
        {
            // If the item is stackable, check if we already have it
            Item existingItem = items.Find(item => item.itemName == newItem.itemName);

            if (existingItem != null)
            {
                existingItem.quantity += newItem.quantity; // Increase quantity
            }
            else
            {
                // Add new item if there is space
                if (items.Count < maxItems)
                {
                    Item clonedItem = Instantiate(newItem);
                    clonedItem.quantity = newItem.quantity;
                    items.Add(clonedItem);
                }
                else
                {
                    Debug.LogWarning("Inventory full, cannot add item: " + newItem.itemName);
                }
            }
        }

        // Update UI
        inventoryUI?.UpdateUI();
    }

    // Remove item from equipment and return to inventory
    public void RemoveEquipment(Item item)
    {
        if (item != null && items.Contains(item))
        {
            items.Remove(item);
            EquipItem(item); // Return to inventory
        }

        if (inventoryPanel.activeSelf)
        {
            inventoryUI.UpdateUI(); // Update UI immediately
        }
    }

    // Add an item to the inventory, handling stacking and cloning
    public void AddItem(Item item)
    {
        
        if (item == null)
        {
            Debug.LogWarning("Tried to add a null item to inventory!");
            return;
        }

        // Handle stackable items
        if (item.isStackable)
        {
            Item existingItem = items.Find(i => i.itemName == item.itemName);

            if (existingItem != null)
            {
                existingItem.quantity += item.quantity; // Add to existing item stack
               
            }
            else
            {
                if (items.Count < maxItems)
                {
                    Item clonedItem = Instantiate(item);
                    clonedItem.quantity = item.quantity;
                    items.Add(clonedItem);
                }
                else
                {
                    Debug.LogWarning("Inventory full, cannot add item: " + item.itemName);
                }
            }
        }
        else
        {
            // Handle non-stackable items
            if (items.Count < maxItems)
            {
                Item clonedItem = Instantiate(item);
                items.Add(clonedItem);
            }
            else
            {
                Debug.LogWarning("Inventory full, cannot add item: " + item.itemName);
            }
        }


        // Update UI

        QuestManager questManager = FindObjectOfType<QuestManager>();
        if (questManager != null)
        {
            
            // Tämän karhun tappaminen lisää progression Quest "DamnBearsQuestID" tavoitteelle
            questManager.UpdateCollectQuestProgress(item);
            
            
        }
         inventoryUI.UpdateUI();
        

    }

    // Remove an item by a specified quantity
    public void RemoveItem(Item item, int quantity)
    {
        Debug.Log("Removing item from inventory?");
        if (item == null || quantity <= 0)
        {
            Debug.LogWarning("Invalid item or quantity when removing item!");
            return;
        }

        Item stack = items.Find(i => i.itemName == item.itemName);

        if (stack != null)
        {
            if (stack.quantity <= quantity)
            {
                items.Remove(stack); // Remove the entire stack
            }
            else
            {
                stack.quantity -= quantity; // Reduce the stack size
            }

            if (inventoryPanel.activeSelf && inventoryUI != null)
            {
                inventoryUI.UpdateUI(); // Update UI
            }
        }
        else
        {
            Debug.LogWarning("Item not found in inventory: " + item.itemName);
        }
        QuestManager questManager = FindObjectOfType<QuestManager>();
        if (questManager != null)
        {
            // Tämän karhun tappaminen lisää progression Quest "DamnBearsQuestID" tavoitteelle
            questManager.UpdateCollectQuestProgress(item);
            
            
        }
    }

    // Get the quantity of a specific equipment
    public int GetEquipmentCount(Equipment equipment)
    {
        if (equipment == null)
        {
            Debug.LogWarning("Equipment is null");
            return 0;
        }

        return items.Where(item => item.itemName == equipment.itemName).Sum(item => item.quantity);
    }
    public int GetItemCount(Item itemToFind)
    {
        
        if (itemToFind == null)
        {
            Debug.LogWarning("Equipment is null");
            return 0;
        }
        
        return items.Where(item => item.itemName == itemToFind.itemName).Sum(item => item.quantity);
    }    
    public void AddGold(int amount)
    {
        playerMoney = playerMoney + amount;
    }
}
