using UnityEngine;
using TMPro;
using UnityEngine.UI;  // Lisätty, jotta pääsemme käsiksi Button-komponenttiin
using UnityEngine.EventSystems;

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


    void Start()
    {
        showTooltip = false;
        playerInventory = FindObjectOfType<Inventory>();
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
        }
        quantityText.text = "";
    }
    // Piilotetaan slotti, jos item on tyhjä
    gameObject.SetActive(false);
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
            currentItem.quantity--;

            // Jos määrä on nolla tai vähemmän, poistetaan item
            if (currentItem.quantity <= 0)
            {
                // Poistetaan item inventorylistasta
                Debug.Log("Removing item: " + currentItem.itemName);
                playerInventory.items.Remove(currentItem);

                // Tyhjennetään slotin UI
                ClearSlot();

                // Päivitetään UI
                inventoryUI.UpdateUI();
            }
            else
            {
                // Vain päivitämme UI:n määrä
                quantityText.text = currentItem.quantity.ToString();
                inventoryUI.UpdateUI();
            }
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
                RemoveItem(); // Poistetaan muu varuste
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

        initialItemPosition = itemRectTransform.position;
        itemImage.raycastTarget = false;
        Debug.Log("Drag started!");
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (currentItem == null) return;

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

    itemImage.raycastTarget = true;


    // Piilotetaan vedettävä esine, jotta se ei ole näkyvissä, kun vahvistusikkuna avautuu
    itemImage.gameObject.SetActive(false);

    // Tarkista, onko vetäminen päättynyt inventoryn ulkopuolella
    if (!RectTransformUtility.RectangleContainsScreenPoint(playerInventoryPanel, eventData.position, eventData.pressEventCamera))
    {
        if (inventoryUI.confirmationPanelParent != null)
        {
            inventoryUI.confirmationPanel.SetActive(true); // Avaa vahvistusikkuna

            // Asetetaan vahvistus- ja peruutuspainikkeet
            inventoryUI.yesButton.onClick.RemoveAllListeners();  // Poistetaan mahdolliset vanhat kuuntelijat
            inventoryUI.noButton.onClick.RemoveAllListeners();

            // Asetetaan itemToDelete kuvan avulla
            inventoryUI.itemToDelete.sprite = currentItem.icon; // Asetetaan kuva itemistä
            inventoryUI.itemToDelete.enabled = true;  // Varmistetaan, että kuva näkyy

            // Kun "Yes" painetaan, poistetaan item
            inventoryUI.yesButton.onClick.AddListener(() => {
                RemoveItem(); // Poistetaan item pelaajan inventorysta
                inventoryUI.confirmationPanel.SetActive(false); // Suljetaan varmistusikkuna
                itemRectTransform.position = initialItemPosition; // Palautetaan alkuperäinen sijainti
                itemImage.gameObject.SetActive(true);
            });

            // Kun "No" painetaan, palautetaan item alkuperäiseen sijaintiin
            inventoryUI.noButton.onClick.AddListener(() => {
                inventoryUI.confirmationPanel.SetActive(false); // Suljetaan varmistusikkuna
                itemRectTransform.position = initialItemPosition; // Palautetaan alkuperäinen sijainti
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
        Debug.Log("Dropped inside inventory");
        itemRectTransform.position = initialItemPosition;
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
