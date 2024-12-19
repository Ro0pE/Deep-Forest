using UnityEngine;
using TMPro;
using UnityEngine.UI;  // Lisätty, jotta pääsemme käsiksi Button-komponenttiin
using UnityEngine.EventSystems;

public class VendorInventorySlot : MonoBehaviour
{
    public Transform itemTransform;  // Tämä on objekti, joka näyttää itemin ikonin
    public Item currentItem;
    private Inventory playerInventory;
    public TextMeshProUGUI quantityText;
    private Image itemImage;  // Muutettu SpriteRenderer Image-komponentiksi
    private Button button;  // Lisätty Button-komponentti
    private PlayerHealth playerHealth;
    public InventoryUI inventoryUI;
    public ItemTooltipManager itemTooltipManager; // TooltipManager viittaus
    public VendorManager vendorManager;
    public Sprite defaultSprite; // Oletussprite
    private bool showTooltip;

    private float lastClickTime = 0f;  // Aikaleima edellisestä klikkauksesta
    public float doubleClickTimeLimit = 0.5f;  // Aikaraja tuplaklikkaukselle

    void Start()
    {
        showTooltip = false;
        playerInventory = FindObjectOfType<Inventory>();
        playerHealth = FindObjectOfType<PlayerHealth>();
        inventoryUI = FindObjectOfType<InventoryUI>();
        itemTooltipManager = FindObjectOfType<ItemTooltipManager>();
        vendorManager.UpdateVendorInventory();

        if (itemTransform != null)
        {
            itemImage = itemTransform.GetComponent<Image>();  // Haetaan Image-komponentti itemTransformista
            if (itemImage == null)
            {
                Debug.LogError("itemTransform doesn't have an Image component attached!");
            }
        }
        else
        {
            Debug.LogError("itemTransform is not assigned in the InventorySlot!");
        }

        button = GetComponent<Button>();
  
    }

    public void SetItem(Item newItem)
    {
        currentItem = newItem;

        if (currentItem != null)
        {
            if (itemImage != null)
            {
                itemImage.sprite = currentItem.icon;
                quantityText.text = currentItem.isStackable ? currentItem.quantity.ToString() : "";  // Näytetään määrä vain stackable esineillä
            }

            if (itemTransform.GetComponent<Collider2D>() == null)
            {
                Debug.Log("itemlille laitetaan boxcollideri");
                itemTransform.gameObject.AddComponent<BoxCollider2D>();
            }
        }
        else
        {
            quantityText.text = "";
        }
    }

    public void ClearSlot()
    {
        if (currentItem != null)
        {
            currentItem = null;
            if (itemImage != null)
            {
                itemImage.sprite = null;  // Tyhjennetään ikoni
                itemImage.sprite = defaultSprite;
            }
            quantityText.text = "";
        }
    }

    public bool IsEmpty()
    {
        return currentItem == null;
    }

    public void RemoveItem()
    {
        if (currentItem != null)
        {
                playerInventory.items.Remove(currentItem);
                ClearSlot();
                inventoryUI.UpdateUI();
                vendorManager.UpdateVendorInventory();
        }
    }

    // Tämä metodi käsittelee klikkauksen ja tuplaklikkauksen tunnistamisen
    public void OnSlotClick()
    {
        Debug.Log("Add to vendor");
        float currentTime = Time.time;

        // Jos viimeinen klikkaus tapahtui riittävän nopeasti, tunnistetaan tuplaklikkaus
        if (currentTime - lastClickTime <= doubleClickTimeLimit)
        {
            Debug.Log("Double Click Detected!");

            if (currentItem != null)
            {
                Debug.Log("Double click");
                SellItem(currentItem);
                RemoveItem();
          
            }
            else
            {
                Debug.Log("Slot is empty.");
            }
        }
        else
        {
            
            Debug.Log("no double click. showtooltip");
            if (!showTooltip)
            {
                ShowItemToolTip();
                showTooltip = true;

            }
            else
            {
                HideItemTooltip();
                showTooltip = false;
            }
        }

        lastClickTime = currentTime;  // Päivitetään viimeinen klikkausaika
    }

    private void HandleItemUse()
    {
        string itemName = currentItem.itemName;
        Debug.Log($"Clicked on item: {itemName}");

        // Tarkistetaan, onko käytettävä itemi potion
        if (currentItem is Potion potion)
        {
            if (playerHealth != null)
            {
                playerHealth.Heal(potion.healAmount);
                playerHealth.RestoreMana(potion.manaAmount);
                RemoveItem();
                Debug.Log($"Used potion: {itemName} and healed for {potion.healAmount} HP.");
            }
        }

        // Tarkistetaan, onko itemi Equipment ja varustetaan pelaaja
        if (currentItem is Equipment equipment)
        {
            Debug.Log($"Equipping: {equipment.slot}");
            EquipmentManager.instance.Equip(equipment);
            RemoveItem();
        }

    }

    public void ShowItemToolTip()
    {
       /*
        Debug.Log("Item clicked: " + currentItem);

        string itemName = currentItem.itemName;
        int sellPrice = currentItem.sellPrice;
        string infoText = currentItem.infoText;
        string usageText = currentItem.usageText;
        Sprite icon = currentItem.icon;*/
        itemTooltipManager.ShowTooltip(currentItem);
        // Tarkistetaan, onko itemi potion
      /*  if (currentItem is Potion potion)
        {
            // Potion erikoiskentät
            //string infoText = potion.infoText;
            string healInfo = $"{potion.healAmount}";
            string manaInfo = $"{potion.manaAmount}";

            // Näytetään tooltip potionspesifisillä kentillä
            itemTooltipManager.ShowTooltip(itemName, sellPrice, infoText, usageText, icon, healInfo, manaInfo);
        }
        else
        {
            Debug.Log("EI POTION");
            // Normaalit item-kentät
            itemTooltipManager.ShowTooltip(itemName, sellPrice, infoText, usageText, icon);
        }*/
    }

    public void HideItemTooltip()
    {
        itemTooltipManager.HideTooltip();
    }

    public void SellItem(Item item)
    {
        vendorManager.AddItemForSale(item);
        vendorManager.UpdateVendorInventory();
    }
}
