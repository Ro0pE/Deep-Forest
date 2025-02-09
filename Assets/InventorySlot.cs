using UnityEngine;
using TMPro;
using UnityEngine.UI;  // Lisätty, jotta pääsemme käsiksi Button-komponenttiin
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class InventorySlot : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
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
            //confirmationPanel.SetActive(false);

            //Button yesButton = inventoryUI.GetYesButton();
            //Button noButton = inventoryUI.GetNoButton();

            inventoryUI.UpdateUI();
        }
        if (playerInventoryPanel == null && inventoryUI != null)
        {
            playerInventoryPanel = inventoryUI.GetComponent<RectTransform>();
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





public void SetItem(Item newItem)
{
    Debug.Log("Setting item : " + newItem.itemName);
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
            Debug.Log("RAriry on common  ? " + currentItem.itemName);
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
            Debug.Log("Rarirty ei ole  common  ? " + currentItem.itemName);
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
                glowAnimator.speed = 0.5f;  // Hitaampi animaatio; säädä arvoa tarpeen mukaan
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
        currentItem = null;
        if (itemImage != null)
        {
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
        Debug.Log("SLOTKLIK");
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

        // Poista esine oikealla klikkauksella
        if (Input.GetMouseButtonDown(1))  // Oikea klikkaus
        {
            if (currentItem is not Equipment)
            {
                RemoveItem();
                Debug.Log($"Removed item: {itemName}");
            }
        }
    }

    public void ShowItemToolTip()
    {
        if (currentItem == null)
        {
            Debug.Log("Slot is empty");
            return;
        }
        Debug.Log("Item clicked: " + currentItem);

        /*string itemName = currentItem.itemName;
        int sellPrice = currentItem.sellPrice;
        string infoText = currentItem.infoText;
        string usageText = currentItem.usageText;
        Sprite icon = currentItem.icon;*/
        itemTooltipManager.ShowTooltip(currentItem);

       /* // Tarkistetaan, onko itemi potion
        if (currentItem is Potion potion)
        {
            // Potion erikoiskentät
            //string infoText = potion.infoText;
            string healInfo = $"{potion.healAmount}";
            string manaInfo = $"{potion.manaAmount}";

            // Näytetään tooltip potionspesifisillä kentillä
            //itemTooltipManager.ShowTooltip(itemName, sellPrice, infoText, usageText, icon, healInfo, manaInfo);
        }
        else if ( currentItem is Equipment equipment)
        {
            Debug.Loh("equipment");

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
        // Aloita vetäminen
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (currentItem == null) return;

        if (inventoryUI.confirmationPanel.activeSelf)
        {
            Debug.Log("Confirmationpanel is active");
            return;
        }

        initialItemPosition = itemRectTransform.position;
        initialParent = transform; // Tallennetaan lähtöpaikka
        itemImage.raycastTarget = false;
        Debug.Log("Drag started!");
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (currentItem == null) return;
        if (inventoryUI.confirmationPanel.activeSelf)
        {
            Debug.Log("Confirmationpanel is active");
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
    }

public void OnEndDrag(PointerEventData eventData)
{
    if (currentItem == null) return;
    if (inventoryUI.confirmationPanel.activeSelf)
    {
        Debug.Log("Confirmationpanel is active");
        return;
    }

    itemImage.raycastTarget = true;

    // Raycast check to see what was hit
    PointerEventData pointerEventData = eventData;
    List<RaycastResult> results = new List<RaycastResult>();
    EventSystem.current.RaycastAll(pointerEventData, results);

    bool isItemStorageBackgroundHit = false;
    Transform dropParent = null;

    foreach (var result in results)
    {
        Debug.Log("Raycast hit: " + result.gameObject.name);

        // Tarkistetaan, osuuko raycast ItemStorageBackgroundiin
        if (result.gameObject.name == "storageSlotPrefab(Clone)") // Varmista, että käytät oikeaa nimeä
        {
            Debug.Log("Storage check");
            dropParent = result.gameObject.transform;
            isItemStorageBackgroundHit = true;
            break; // Jos osui, ei tarvitse tarkistaa enempää
        }
        else if (result.gameObject.name == "Inventory") // Varmista, että käytät oikeaa nimeä
        {
            Debug.Log("Inventory check");
            dropParent = result.gameObject.transform;
            //isItemStorageBackgroundHit = true;
            break; // Jos osui, ei tarvitse tarkistaa enempää
        }
    }
    Debug.Log("CHECK:");
    Debug.Log(dropParent);
    Debug.Log(initialParent);
    if (dropParent != null && dropParent.name.StartsWith("storageSlotPrefab") && initialParent.name.StartsWith("storageSlotPrefab"))
    {
        Debug.Log("Dropped into storage slot prefab");
        itemRectTransform.position = initialItemPosition;
        return;
        // Tee haluamasi toimenpiteet
    }

    // Jos itemi osui ItemStorageBackgroundiin
    if (isItemStorageBackgroundHit)
    {
        // Siirretään item PlayerStorageen
        playerStorage.StoreItem(currentItem);  // Siirretään item PlayerStorageen

        // Poistetaan item inventaariosta
        itemRectTransform.position = initialItemPosition;
        //RemoveItemStack();  // Tämä poistaa vain, jos siirto onnistui
        //ClearSlot();
        //Debug.Log($"{currentItem.quantity} x {currentItem.itemName} siirretty PlayerStorageen.");
      
    }
    else
    {
        // Jos itemiä ei ole pudotettu storageen, tarkistetaan, onko se pudotettu inventoryn ulkopuolelle
        Vector2 playerInventoryPosition;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(playerInventoryPanel, eventData.position, eventData.pressEventCamera, out playerInventoryPosition);

        if (!playerInventoryPanel.rect.Contains(playerInventoryPosition))
        {
            Debug.Log("Tämä tekee jotain!!!!");
            // Esine ei ole pudotettu storageen eikä inventoryyn, joten voidaan poistaa
            if (inventoryUI.confirmationPanelParent != null)
            {
                inventoryUI.confirmationPanel.SetActive(true);
                itemImage.gameObject.SetActive(false);
                inventoryUI.yesButton.onClick.RemoveAllListeners();
                inventoryUI.noButton.onClick.RemoveAllListeners();

                inventoryUI.itemToDelete.sprite = currentItem.icon;
                inventoryUI.itemToDelete.enabled = true;

                inventoryUI.yesButton.onClick.AddListener(() => {
                    RemoveItem();  // Poistetaan itemi, kun hyväksytään
                    
                    inventoryUI.confirmationPanel.SetActive(false);
                    itemRectTransform.position = initialItemPosition;
                    itemImage.gameObject.SetActive(true);
                });

                inventoryUI.noButton.onClick.AddListener(() => {
                    inventoryUI.confirmationPanel.SetActive(false);
                    itemRectTransform.position = initialItemPosition;
                    itemImage.gameObject.SetActive(true);
                });
            }
            else
            {
                Debug.Log("Confirmation panel is null");
            }
        }
        else
        {
            if (initialParent.name.StartsWith("storageSlotPrefab"))
            {

            Debug.Log("Storagesta -> inventory");
            playerStorage.RetrieveItem(currentItem);
            itemRectTransform.position = initialItemPosition;
            
          
            }
            else
            {
                            // Jos item on pudotettu inventoryyn, palautetaan se alkuperäiseen paikkaansa
            Debug.Log("palautetaan itemi");
            itemRectTransform.position = initialItemPosition;
            itemImage.gameObject.SetActive(true);
            }

        }
    }

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
