using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System;


public class InventoryUI : MonoBehaviour
{
    public GameObject slotPrefab;
    public Transform slotParent;
    public Inventory playerInventory;
    public List<InventorySlot> slots; //new List<InventorySlot>();
    public TextMeshProUGUI playerGold;
    public GameObject confirmationPanelParent;
    public GameObject confirmationPanel;
    public Image itemToDelete;
    public Button yesButton;
    public Button noButton;


    void Start()
    {
        playerInventory = FindObjectOfType<Inventory>();
        UpdateUI();
        slotParent.gameObject.SetActive(false);
        //confirmationPanel.SetActive(false);

    }
       // public GameObject GetConfirmationPanel() => confirmationPanel;
       // public Button GetYesButton() => yesButton;
       // public Button GetNoButton() => noButton;

public void ShowConfirmationPanel(Sprite itemIcon, Action onConfirm, Action onCancel)
{
    confirmationPanel.SetActive(true);
    itemToDelete.sprite = itemIcon;
    itemToDelete.enabled = true;

    yesButton.onClick.RemoveAllListeners();
    noButton.onClick.RemoveAllListeners();

    yesButton.onClick.AddListener(() => {
        onConfirm?.Invoke();
        confirmationPanel.SetActive(false);
    });

    noButton.onClick.AddListener(() => {
        onCancel?.Invoke();
        confirmationPanel.SetActive(false);
    });
}

public void UpdateUI()
{
    
    if (playerInventory == null)
    {
        
        return;
    }

    for (int i = 0; i < slots.Count; i++)
    {
        if (i < playerInventory.items.Count)
        {
            Item item = playerInventory.items[i];
            slots[i].SetItem(item);
            slots[i].gameObject.SetActive(true);
         
        }
        else
        {
            
            slots[i].ClearSlot(); // Tyhjennä slotti ja näytä default-sprite
            slots[i].gameObject.SetActive(true);
        }
    }

    playerGold.text = playerInventory.playerMoney.ToString();
}



}
