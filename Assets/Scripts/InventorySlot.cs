using UnityEngine;
using TMPro;
using UnityEngine.UI;  // Lisätty, jotta pääsemme käsiksi Button-komponenttiin
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System;


public class InventorySlot : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Transform itemTransform;  // Tämä on objekti, joka näyttää itemin ikonin
    public Item currentItem;
    private Inventory playerInventory;
    public TextMeshProUGUI quantityText;
    public Image itemImage;  // Muutettu SpriteRenderer Image-komponentiksi
    private Button button;  // Lisätty Button-komponentti
    private PlayerHealth playerHealth;
    public InventoryUI inventoryUI;
    public ItemTooltipManager itemTooltipManager; // TooltipManager viittaus
    private bool showTooltip;

    private float lastClickTime = 0f;  // Aikaleima edellisestä klikkauksesta
    public float doubleClickTimeLimit = 0.5f;  // Aikaraja tuplaklikkaukselle
    private Canvas parentCanvas;
    private RectTransform itemRectTransform;
    private Vector3 initialItemPosition;
    [SerializeField] private RectTransform playerInventoryPanel; // Viittaus pelaajan inventory-paneliin
    public GameObject confirmationPanel;
    public Sprite defaultSprite; 
    private Transform initialParent;
    public PlayerStorage playerStorage;
    public Animator glowAnimator; // Lisää tämä muuttuja luokan yläosaan Inspectorille näkyväksi
    public Outline slotOutline; 
    public Image itemFrame;
    public bool isMouseRightHeld = false;  // Tietää onko oikea nappi pohjassa



void Start()
{
    showTooltip = false;
    playerInventory = FindObjectOfType<Inventory>();
    playerStorage = FindObjectOfType<PlayerStorage>();
    playerHealth = FindObjectOfType<PlayerHealth>();
    inventoryUI = FindObjectOfType<InventoryUI>();
    itemTooltipManager = FindObjectOfType<ItemTooltipManager>();

    if (itemTransform != null)
    {
        itemImage = itemTransform.GetComponent<Image>();
        itemRectTransform = itemTransform.GetComponent<RectTransform>();

        if (itemImage == null)
        {
            Debug.LogError("itemTransform doesn't have an Image component attached!");
        }
        else
        {
           
        }
    }
    else
    {
        Debug.LogError("itemTransform is not assigned in the InventorySlot!");
    }

    button = GetComponent<Button>();
    parentCanvas = GetComponentInParent<Canvas>();

    if (inventoryUI != null)
    {
        confirmationPanel = inventoryUI.confirmationPanel;
        inventoryUI.UpdateUI();
    }
    if (playerInventoryPanel == null && inventoryUI != null)
    {
        playerInventoryPanel = inventoryUI.GetComponent<RectTransform>();
    }

    // Testaa täällä myös itemImage heti, jos se on NULL
    if (itemImage == null)
    {
        Debug.LogError("itemImage on edelleen NULL Startin jälkeen!");
    }
}

    void Update()
    {
        if (Input.GetMouseButton(1)) // 1 = oikea hiiren nappi
        {
            // Varmistetaan, että tooltip näytetään vain kerran, kun oikea nappi menee pohjaan
            if (!isMouseRightHeld)
            {
                ShowItemToolTip();  // Näytä tooltip
                isMouseRightHeld = true;  // Merkitään, että oikea nappi on pohjassa
            }
        }
        else if (Input.GetMouseButtonUp(1)) // 1 = oikea hiiren nappi vapautettu
        {
            // Kun oikea nappi vapautetaan
            if (isMouseRightHeld)
            {
                HideItemTooltip();  // Piilota tooltip
                isMouseRightHeld = false;  // Merkitään, että oikea nappi ei ole enää pohjassa
            }
        }
    }

    private Color GetGlowColor(Rarity rarity)
    {
        switch (rarity)
        {
            case Rarity.Uncommon:
                return new Color(0.5f, 1f, 0.5f, 1f); // vaalean vihreä
            case Rarity.Rare:
                return new Color(0.5f, 0.5f, 1f, 1f); // vaalean sininen
            case Rarity.Epic:
                return new Color(1f, 0.5f, 1f, 1f);   // vaalean magenta
            case Rarity.Legendary:
                return new Color(1f, 0.84f, 0f, 1f);    // kultainen
            default:
                return Color.white; // Common tai muu oletus
        }
    }
public void SetPotion(Item newItem)
{
    if (itemTransform == null)
        {
            Debug.LogError("itemTransform on NULL! Se on todennäköisesti tuhottu.");
        }
        else
        {
            Debug.Log("itemTransform elossa: " + itemTransform.name + " | ID: " + itemTransform.GetInstanceID());
        }
        if (newItem == null)
        {
            Debug.LogError("SetPotion kutsuttu NULL-itemillä!");
            return;
        }

  
    currentItem = newItem;
  

    
    if (itemImage != null)
    {
        if (currentItem.icon == null)
        {
            Debug.LogError("Itemillä " + currentItem.itemName + " ei ole asetettua spriteä!");
            return;
        }
        
        itemImage.sprite = currentItem.icon;
        itemImage.enabled = true;
        quantityText.text = currentItem.isStackable ? currentItem.quantity.ToString() : "";
        
        Debug.Log("Potion sprite asetettu: " + itemImage.sprite.name);
    }
    else
    {
        if (itemImage == null)
    {
        Debug.LogError("itemImage on NULL! Tämä tarkoittaa, että Image-komponenttia ei ole liitetty.");
    }
    else
    {
        Debug.Log("itemImage löytyy ja on asetettu oikein: " + itemImage.name);
    }
       // Debug.LogError("itemImage on NULL! Tarkista, onko Image-komponentti linkitetty oikein Inspectorissa.");
    }

    // Pakotetaan UI päivittymään
    Canvas.ForceUpdateCanvases();

    if (itemTransform.GetComponent<Collider2D>() == null)
    {
        itemTransform.gameObject.AddComponent<BoxCollider2D>();
    }

    // Tarkistetaan rarity ja päivitetään ulkoasu
    if (currentItem.rarity == Rarity.Common)
    {
       
        itemFrame.color = Color.white;

        if (slotOutline != null)
        {
            slotOutline.enabled = false;
        }
        if (glowAnimator != null)
        {
            glowAnimator.enabled = false;
        }
    }
    else
    {
   
        
        if (slotOutline != null)
        {
            slotOutline.enabled = true;
            slotOutline.effectColor = GetGlowColor(currentItem.rarity);
            itemFrame.color = GetGlowColor(currentItem.rarity);
        }
        if (glowAnimator != null)
        {
            glowAnimator.enabled = true;
            glowAnimator.speed = 1.8f;
        }
    }
}




public void SetItem(Item newItem)
{
   
    currentItem = newItem;

    if (currentItem != null)
    {
        if (itemImage != null)
        {
            itemImage.sprite = currentItem.icon;
            quantityText.text = currentItem.isStackable ? currentItem.quantity.ToString() : "";
        }

        if (itemTransform.GetComponent<Collider2D>() == null)
        {
            itemTransform.gameObject.AddComponent<BoxCollider2D>();
        }

        // Tarkistetaan, onko itemin rarity Common vai ei
        if (currentItem.rarity == Rarity.Common)
        {
           
            itemFrame.color = Color.white;
            // Jos item on Common, poistetaan glow kokonaan (ei Outlinea eikä animaatiota)
            if (slotOutline != null)
            {
                slotOutline.enabled = false;
                
            }
            if (glowAnimator != null)
            {
                glowAnimator.enabled = false;
            }
        }
        else
        {
            
            // Muussa tapauksessa asetetaan glow-väri rarityn mukaan ja käynnistetään glow-animaatio
            if (slotOutline != null)
            {
                slotOutline.enabled = true;
                slotOutline.effectColor = GetGlowColor(currentItem.rarity);
                itemFrame.color = GetGlowColor(currentItem.rarity);
            }
            if (glowAnimator != null)
            {
                glowAnimator.enabled = true;
                glowAnimator.speed = 1.8f;  // Hitaampi animaatio; säädä arvoa tarpeen mukaan
            }
        }
    }
    else
    {
        quantityText.text = "";

        // Jos slot on tyhjä, poistetaan glow
        if (slotOutline != null)
        {
            slotOutline.enabled = false;
        }
        if (glowAnimator != null)
        {
            glowAnimator.enabled = false;
        }
    }
}


public void ClearSlot()
{ 
    
    if (currentItem != null)
    {
        Debug.Log("Clearing slot for " + currentItem.itemName);
        currentItem = null;
        if (itemImage != null)
        {
            Debug.Log("Asetetaan default sprite itemille ");
            itemImage.sprite = defaultSprite;  // Tyhjennetään ikoni
             itemFrame.color = Color.white;
        }
        quantityText.text = "";
    }
    if (slotOutline != null)
    {
        slotOutline.enabled = false;
    }
    // Piilotetaan slotti, jos item on tyhjä
    //gameObject.SetActive(false);
}


    public bool IsEmpty()
    {
        return currentItem == null;
    }

    public void RemoveItem()
    {
        if (currentItem != null)
        {
            // Vähennetään itemin määrä
            Debug.Log("Itemin määrä vähenee");
            currentItem.quantity--;

            // Jos määrä on nolla tai vähemmän, poistetaan item
            if (currentItem.quantity <= 0)
            {
                // Poistetaan item inventorylistasta
                Debug.Log("Removing item: " + currentItem.itemName);
                playerInventory.items.Remove(currentItem);

                // Tyhjennetään slotin UI
                //ClearSlot();

                // Päivitetään UI
                inventoryUI.UpdateUI();
            }
            else
            {
                // Vain päivitämme UI:n määrä
                quantityText.text = currentItem.quantity.ToString();
                //inventoryUI.UpdateUI();
            }
        }
    }
        public void RemoveItemStack()
    {
        if (currentItem != null)
        {
            Debug.Log("Poistetaan itemi inventorystä  " + currentItem.itemName );
            playerInventory.items.Remove(currentItem);
            //ClearSlot();
            
            //inventoryUI.UpdateUI();
            // Vähennetään itemin määrä

        }
    }

    // Tämä metodi käsittelee klikkauksen ja tuplaklikkauksen tunnistamisen
public void OnSlotClick()
{
    float currentTime = Time.time;

    // Jos viimeinen klikkaus tapahtui riittävän nopeasti, tunnistetaan tuplaklikkaus
    if (currentTime - lastClickTime <= doubleClickTimeLimit)
    {
        Debug.Log("Double Click Detected!");

        if (currentItem != null)
        {
            // Ajetaan metodit vain tuplaklikkauksen aikana
            HandleItemUse();
        }
        else
        {
            Debug.Log("Slot is empty.");
        }
    }
    lastClickTime = currentTime;  // Päivitetään viimeinen klikkausaika
}

    public void HandleItemUse()
    {
        string itemName = currentItem.itemName;


        // Tarkistetaan, onko käytettävä itemi potion
        if (currentItem is Potion potion)
        {
            if (playerHealth != null)
            {
                playerHealth.Heal(potion.healAmount);
                playerHealth.RestoreMana(potion.manaAmount);
                RemoveItem();
                Debug.Log($"Used potion: {itemName} and healed for {potion.healAmount} HP.");
                QuestManager questManager = FindObjectOfType<QuestManager>();
                if (questManager != null)
                {
                    Debug.Log("Quest manager pitäs updatee");
                    // Tämän karhun tappaminen lisää progression Quest "DamnBearsQuestID" tavoitteelle
                    questManager.UpdateCollectQuestProgress(currentItem);
                    
                    
                }
            }
        }

        // Tarkistetaan, onko itemi Equipment ja varustetaan pelaaja
        if (currentItem is Equipment equipment)
        {
            // Jos varuste on nuoli, älä poista sitä inventaariosta
            if (equipment.slot == SlotType.Arrow)
            {
                Debug.Log("Aktivoidaan nuoli käyttöön ");
                EquipmentManager.instance.Equip(equipment); // Varustetaan nuolilla
            }
            else
            {
                EquipmentManager.instance.Equip(equipment); // Varustetaan muilla esineillä
                //RemoveItem(); // Poistetaan muu varuste
            }
        }

    }

    public void ShowItemToolTip()
    {
        if (currentItem == null)
        {
           
            return;
        }
    // Varmistetaan, että tooltip näytetään vain oikealla slotilla
    if (itemTransform != null && RectTransformUtility.RectangleContainsScreenPoint(itemRectTransform, Input.mousePosition, null))
    {
        
        itemTooltipManager.ShowTooltip(currentItem);
    }

    }

    public void HideItemTooltip()
    {
        itemTooltipManager.HideTooltip();
    }
        // Aloita vetäminen
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (currentItem == null) return;

        if (inventoryUI.confirmationPanel.activeSelf)
        {
            
            return;
        }

        initialItemPosition = itemRectTransform.position;
        initialParent = transform; // Tallennetaan lähtöpaikka
        itemImage.raycastTarget = false;
        
    }

public void OnDrag(PointerEventData eventData)
{
    if (currentItem == null) return;
    if (inventoryUI.confirmationPanel.activeSelf)
    {
        return;
    }

    Vector3 mousePosition;
    RectTransformUtility.ScreenPointToWorldPointInRectangle(
        parentCanvas.transform as RectTransform,
        eventData.position,
        eventData.pressEventCamera,
        out mousePosition
    );
    itemRectTransform.position = mousePosition;

    // Käydään läpi kaikki slottit, jos esine menee päälle
    foreach (var slot in playerInventory.potionSlots)
    {
        RectTransform slotRect = slot.GetComponent<RectTransform>();
        if (slotRect != null && slotRect.rect.Contains(slotRect.InverseTransformPoint(mousePosition)))
        {
            // Jos esine menee slottiin, suurennetaan slotin skaalaa
            slotRect.localScale = new Vector3(1.2f, 1.2f, 1);
        }
        else
        {
            // Palautetaan slotin skaala normaaliin kokoon
            slotRect.localScale = new Vector3(1f, 1f, 1);
        }
    }
}



public void OnEndDrag(PointerEventData eventData)
{
    if (currentItem == null) return;
    if (inventoryUI.confirmationPanel.activeSelf) return;

    itemImage.raycastTarget = true;
    Transform dropTarget = null;

    // Käydään läpi kaikki raycasttitulokset ja etsitään oikea kohde
    List<RaycastResult> results = new List<RaycastResult>();
    EventSystem.current.RaycastAll(eventData, results);

    foreach (var result in results)
    {
        Transform hitTransform = result.gameObject.transform;
        Debug.Log("Mihin osu: " + hitTransform);

        // Tarkistetaan, osuuko kohde oikeaan paikkaan
        if (hitTransform.name.StartsWith("storageSlotPrefab"))
        {
            dropTarget = hitTransform;
            break;
        }
        if (hitTransform.name == "Inventory")
        {
            dropTarget = hitTransform;
            break;
        }
        if (hitTransform.name.StartsWith("PotionSlot"))
        {
            dropTarget = hitTransform;
            break;
        }
    }

    // Käydään läpi kaikki slottit ja palautetaan ne alkuperäiseen kokoon
    foreach (var slot in playerInventory.potionSlots)
    {
        RectTransform slotRect = slot.GetComponent<RectTransform>();
        if (slotRect != null)
        {
            // Palautetaan slotin skaala normaaliin kokoon
            slotRect.localScale = new Vector3(1f, 1f, 1);
        }
    }

    // Jos dropTarget on null, käsitellään pudotus ulkopuolelle
    if (dropTarget == null)
    {
        HandleOutsideDrop(eventData);
        return;
    }

    if (dropTarget.name.StartsWith("storageSlotPrefab"))
    {
        HandleStorageDrop(dropTarget);
        return;
    }

    if (dropTarget.name == "Inventory")
    {
        HandleInventoryDrop();
        return;
    }

    if (dropTarget.name.StartsWith("PotionSlot"))
    {
        Debug.Log("Potion SLot löytyi");
        HandlePotionDrop(dropTarget);
        return;
    }

    // Jos mikään ei täsmännyt, palautetaan item alkuperäiseen paikkaan
    ResetItemPosition();
}


private void HandleStorageDrop(Transform dropTarget)
{
    if (initialParent.name.StartsWith("storageSlotPrefab"))
    {
        Debug.Log("Dropped into storage slot, palautetaan.");
        ResetItemPosition();
        return;
    }

    Debug.Log("Siirretään itemi storageen.");
    playerStorage.StoreItem(currentItem);
    ResetItemPosition();
}

private void HandleInventoryDrop()
{
    int oldIndex = playerInventory.potions.IndexOf(currentItem);
    Debug.Log("Siirretään itemi inventoryyn.");
    playerInventory.AddItem(currentItem);
    playerInventory.potions[oldIndex] = null;
    ResetItemPosition();
    playerInventory.UpdatePotionInventory();
}

private void HandlePotionDrop(Transform dropTarget)
{
    Debug.Log("HandlePotionDrop");
    if (!(currentItem is Potion))
    {
        Debug.Log("Item ei ole potion, palautetaan.");
        ResetItemPosition();
        return;
    }

    int targetIndex = playerInventory.potionSlots.IndexOf(dropTarget.gameObject);
    if (targetIndex == -1) return;

    int oldIndex = playerInventory.potions.IndexOf(currentItem);

    // Tarkistetaan, onko lähtöslotti sama kuin kohdeslotin indeksi
    if (oldIndex == targetIndex)
    {
        Debug.Log("Lähtöslotti on sama kuin kohdeslotti, ei tehdä mitään.");
        ResetItemPosition();
        return;
    }

    if (playerInventory.potions[targetIndex] != null)
    {
        Item oldItem = playerInventory.potions[targetIndex];
        if (currentItem.itemName == oldItem.itemName)
        {
            currentItem.quantity += oldItem.quantity;
            if (oldIndex != -1)
            {
                playerInventory.potions[oldIndex] = null;
            }
        }
        else
        {
            if (initialParent.name.StartsWith("inventorySlotPrefab"))
            {
                Debug.Log("jos initial parent on inventoryslot");
                playerInventory.AddItem(oldItem);
                //playerInventory.potions[oldIndex] = null;
            }
            else
            {
                Debug.Log("Eli ei inventory slot niin");
                playerInventory.potions[oldIndex] = oldItem;
            }
        }
    }

    if (oldIndex != -1 && oldIndex != targetIndex && playerInventory.potions[targetIndex] == null)
    {
        playerInventory.potions[oldIndex] = null;
    }

    playerInventory.potions[targetIndex] = currentItem;

    if (initialParent.name.StartsWith("inventorySlotPrefab"))
    {
        playerInventory.RemoveItem(currentItem, currentItem.quantity);
    }

    playerInventory.UpdatePotionInventory();
    ResetItemPosition();
}


private void HandleOutsideDrop(PointerEventData eventData)
{
    Vector2 localPoint;
    RectTransformUtility.ScreenPointToLocalPointInRectangle(playerInventoryPanel, eventData.position, eventData.pressEventCamera, out localPoint);

    if (!playerInventoryPanel.rect.Contains(localPoint))
    {
    inventoryUI.ShowConfirmationPanel(currentItem.icon, () =>
    {
        RemoveItem();
        ResetItemPosition();
    },
    () => ResetItemPosition());

    }
    else if (initialParent.name.StartsWith("storageSlotPrefab"))
    {
        Debug.Log("Siirretään takaisin storageen.");
        playerStorage.RetrieveItem(currentItem);
        ResetItemPosition();
    }
    else
    {
        ResetItemPosition();
    }
}

private void ResetItemPosition()
{
    itemRectTransform.position = initialItemPosition;
    itemImage.gameObject.SetActive(true);
}




    private void ConfirmItemRemoval()
    {
        Debug.Log("Item removed");
        RemoveItem();
        //confirmationPanel.SetActive(false);
        //itemRectTransform.position = initialItemPosition;
        
    }

    private void CancelItemRemoval()
    {
        Debug.Log("Item removal canceled");
       // confirmationPanel.SetActive(false);
        itemRectTransform.position = initialItemPosition;
        
    }
}
