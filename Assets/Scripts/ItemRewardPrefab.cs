using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using System.Threading.Tasks;  // For async/await

public class ItemRewardPrefab : MonoBehaviour, IPointerDownHandler
{
    public Image itemIcon; // Esineen kuva
    public TextMeshProUGUI itemQuantityText; // Esineen määrä
    public string itemRewardName;
    public ItemTooltipManager itemTooltipManager;
    public Item currentItem;
    private ItemDatabase itemDatabase;
    public Animator glowAnimator; // Lisää tämä muuttuja luokan yläosaan Inspectorille näkyväksi
    public Outline slotOutline; 
    public Image itemFrame;

    private async void Start()
    {
        itemTooltipManager = FindObjectOfType<ItemTooltipManager>();
        itemDatabase = FindObjectOfType<ItemDatabase>();
        await LoadDatabaseAndStart();
        // Haetaan ItemTooltipManager ja ItemDatabase vain kerran


        if (itemDatabase == null)
        {
            Debug.LogError("ItemDatabase ei löytynyt.");
        }
        else
        {
            Debug.Log("ItemRewardPrefabin itemRewardName " + itemRewardName);
            currentItem = itemDatabase.GetItemByName(itemRewardName);
            Debug.Log("ItemRewardPrefabin current item " + currentItem);
        }
        if (currentItem != null)
        {



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
                glowAnimator.speed = 0.5f;
            }
        }
                }
    }

    private async Task LoadDatabaseAndStart()
    {
        await itemDatabase.LoadDatabaseFromSheetsAsync();
        
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

    public void OnPointerDown(PointerEventData eventData)
    {
        if (itemDatabase == null || itemTooltipManager == null) return;

        // Haetaan oikea esine
        currentItem = itemDatabase.GetItemByName(itemRewardName);

        if (currentItem == null)
        {
            Debug.LogWarning("Ei löytynyt esinettä nimellä: " + itemRewardName);
            return;
        }

        // Tarkistetaan oikea klikkaus
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            Debug.Log("Nyt näytetään esine: " + currentItem.itemName);
            itemTooltipManager.ShowTooltip(currentItem);
        }
    }
}
