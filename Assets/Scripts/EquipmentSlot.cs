using UnityEngine;
using TMPro;
using UnityEngine.UI;  // Lisätty, jotta pääsemme käsiksi Button-komponenttiin
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System;

public class EquipmentSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Image icon;          // Ikoni, joka näyttää varusteen
    public Image slotBackground; // Taustakuva, näkyy kun ei ole varustetta
    public Button removeButton; // Poista-painike
    public Equipment currentItem;  // Käytettävä varuste
    public int slotIndex;
    public Inventory playerInventory;
    public EquipmentManager equipmentManager;
    private SlotType slotType;
    public PlayerStats playerStats;
    public InventoryUI inventoryUI;
    public ItemTooltipManager itemTooltipManager; // TooltipManager viittaus
    public bool isMouseRightHeld = false; 
    private bool isMouseOver = false; // Onko hiiri slotin päällä?

    void Start()
    {
        // Etsii pelaajan inventaarin ja varustuspaneelin
        itemTooltipManager = FindObjectOfType<ItemTooltipManager>();
        playerInventory = FindObjectOfType<Inventory>();
        playerStats = FindObjectOfType<PlayerStats>();
        equipmentManager = FindObjectOfType<EquipmentManager>();
        inventoryUI = FindObjectOfType<InventoryUI>();

        // Varmistaa, että komponentit löytyvät
        if (playerInventory == null) Debug.LogError("Player Inventory not found!");
        if (equipmentManager == null) Debug.LogError("Equipment Manager not found!");
    }
    void Update()
    {
        if (Input.GetMouseButtonDown(1)) // Kun oikea hiiren nappi painetaan
        {
            if (!isMouseRightHeld && isMouseOver) // Varmista, että hiiri on slotin päällä
            {
                ShowItemToolTip();
                isMouseRightHeld = true;
            }
        }
        else if (Input.GetMouseButtonUp(1)) // Kun oikea nappi vapautetaan
        {
            if (isMouseRightHeld)
            {
                HideItemTooltip();
                isMouseRightHeld = false;
            }
        }
    }

    public void Setup(SlotType slot)
    {
        slotType = slot;
        icon.enabled = false;  // Aluksi tyhjä ikoni
        slotBackground.enabled = true; // Taustakuva näkyy aluksi
    }

    // Päivittää slotin sisällön (ikoni ja poista-painike)
    public void UpdateSlot()
    {
        if (currentItem != null)
        {
            // Ase on asetettu: Näytetään varusteen kuva, piilotetaan taustakuva
            icon.sprite = currentItem.icon;
            icon.enabled = true;
            slotBackground.enabled = false;  // Piilotetaan taustakuva
            removeButton.gameObject.SetActive(true); // Näytetään poista-painike
        }
        else
        {
            // Ei ole varustetta: Näytetään taustakuva, piilotetaan varusteen kuva
            icon.enabled = false;
            slotBackground.enabled = true; // Taustakuva näkyy
            removeButton.gameObject.SetActive(false); // Piilotetaan poista-painike
        }
    }

    // Poista varuste slotista
    public void RemoveItem()
    {
        if (currentItem != null)
        {
            // Tarkista, onko kyseessä kahden käden ase
            if (currentItem.slot == SlotType.TwoHanded)
            {
                // Poista molemmat kädet kahden käden aseista
                equipmentManager.Unequip((int)SlotType.LeftHand);
                equipmentManager.Unequip((int)SlotType.RightHand);
            }
            else
            {
                Debug.Log("Yksittäinen " + slotIndex);
                // Poista yksittäinen varuste normaalisti
                equipmentManager.Unequip(slotIndex);
            }

            // Tyhjennä slotti
            currentItem = null;
            UpdateSlot();  // Päivitä slotti
        }
    }

    // Aseta varuste slotille
    public void SetItem(Equipment item)
    {
        if (currentItem != null)
        {
            Debug.Log($"Current item in the slot: {currentItem.itemName} + count: {currentItem.quantity}");
        }

        currentItem = item;  // Asettaa uuden varusteen slotille
        
        UpdateSlot();  // Päivittää slotti heti
    }

    // Tyhjennä varuste slotista
    public void ClearItem()
    {
        currentItem = null;  // Tyhjennä varuste
        icon.enabled = false; // Piilota ikoni
        slotBackground.enabled = true; // Näytä taustakuva
        removeButton.gameObject.SetActive(false); // Piilota poista-painike
    }
    public void ShowItemToolTip()
    {
        if (currentItem == null)
        {
           
            return;
        }
        itemTooltipManager.ShowTooltip(currentItem);
    }

    public void HideItemTooltip()
    {
        itemTooltipManager.HideTooltip();
    }
        public void OnPointerEnter(PointerEventData eventData)
    {
        isMouseOver = true; // Hiiri on slotin päällä
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isMouseOver = false; // Hiiri ei ole slotin päällä
    }
}
