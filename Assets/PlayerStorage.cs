using System.Collections.Generic;
using UnityEngine;

public class PlayerStorage : MonoBehaviour
{
    public List<Item> storedItems = new List<Item>();  // Lista tallennetuista esineistä
    public Inventory playerInventory;  // Viittaus pelaajan inventaariin
    public GameObject slotPrefab;  // Slotti-prefabi
    public Transform slotParent;  // Slotin säilytyspaikka UI:ssa
    public List<InventorySlot> storageSlots;  // Lista arkun sloteista
    public GameObject storageParent;
    private bool isStorageOpen = false;  // Seurataan arkkujen tilaa
    public GameObject closeButton;    // Sulkuttonappi, joka sulkee arkkuikkunan
    public Camera playerCamera;  // Viittaus pelaajan kameraan


    void Start()
    {
        if (storageParent == null)
        {
            Debug.LogError("storageParent ei ole liitetty! Tarkista, että se on asetettu.");
        }
        if (closeButton == null)
        {
            Debug.LogError("closeButton ei ole liitetty! Tarkista, että se on asetettu.");
        }

        storageSlots = new List<InventorySlot>();
        storageParent.SetActive(false);

        // Luodaan 63 slottia arkkuun
        for (int i = 0; i < 63; i++)
        {
            GameObject newSlot = Instantiate(slotPrefab, slotParent);  // Luodaan slotti
            InventorySlot inventorySlot = newSlot.GetComponent<InventorySlot>();
            storageSlots.Add(inventorySlot);  // Lisätään slotti listaan
            newSlot.SetActive(true); // Piilotetaan aluksi slotit
        }

        UpdateStorageUI(); // Varmistetaan, että UI päivittyy aluksi
    }

    void Update()
    {
        // Tarkistetaan, onko pelaaja klikannut arkkuun
        if (Input.GetMouseButtonDown(0))  // Vasemman hiiren painikkeen klikkaus
        {
            if (playerCamera != null)  // Varmistetaan, että pelaajan kamera on määritelty
            {
                RaycastHit hit;
                Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);  // Käytä pelaajan kameraa

                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.collider.CompareTag("Storage"))  // Tarkistetaan, onko osunut arkkuun
                    {
                        ToggleStorage();  // Avaa tai sulje arkku
                    }
                }
            }
            else
            {
                Debug.LogError("Pelaajan kamera ei ole määritelty! Aseta pelaajan kamera public-muuttujaan.");
            }
        }
    }

    // Lisää tavara arkkuun ja poistaa sen pelaajan inventaariosta
    public void StoreItem(Item itemToStore)
    {
        Debug.Log("Aloitean storageen lisääminen" + itemToStore + " x " + itemToStore.quantity);

        // Etsitään, onko varastossa jo kyseinen itemi
        Item existingItem = storedItems.Find(item => item.itemName == itemToStore.itemName);

        if (existingItem != null && existingItem.isStackable)
        {
            // Jos item löytyy ja se on stackable, päivitetään määrä
            existingItem.quantity += itemToStore.quantity;
            Debug.Log($"Päivitettiin {itemToStore.itemName} määrää: {existingItem.quantity}");
            Item newItem = Instantiate(itemToStore);
            newItem.quantity = itemToStore.quantity;
            playerInventory.RemoveItem(newItem, newItem.quantity);
        }
        else
        {
            // Jos itemiä ei ole varastossa tai se ei ole stackable, lisätään uusi esine
            Item newItem = Instantiate(itemToStore);
            newItem.quantity = itemToStore.quantity;
            storedItems.Add(newItem);
            playerInventory.RemoveItem(newItem, newItem.quantity);
            Debug.Log($"Lisättiin uusi itemi: {itemToStore.itemName} (määrä: {itemToStore.quantity})");
        }

        UpdateStorageUI();  // Päivitetään UI, koska esine lisättiin tai määrä päivitettiin
    }


    // Ota tavara arkusta ja lisää se pelaajan inventaariin
    public void RetrieveItem(Item itemToRetrieve)
    {
        Debug.Log("Haetaan storagesta item: " + itemToRetrieve.itemName + " x " + itemToRetrieve.quantity);
        
        Item storedItem = storedItems.Find(i => i.itemName == itemToRetrieve.itemName);

        if (storedItem != null && storedItem.quantity >= itemToRetrieve.quantity)
        {
            // Päivitetään määrä tai poistetaan item
            if (storedItem.quantity > itemToRetrieve.quantity)
            {
                storedItem.quantity -= itemToRetrieve.quantity;
            }
            else
            {
                storedItems.Remove(storedItem);
            }

            // Siirretään itemi pelaajan inventaariin
            Item retrievedItem = Instantiate(itemToRetrieve);
            retrievedItem.quantity = itemToRetrieve.quantity;
            playerInventory.AddItem(retrievedItem);

            Debug.Log($"{itemToRetrieve.itemName} x{itemToRetrieve.quantity} otettu arkusta.");
            UpdateStorageUI();
        }
        else
        {
            Debug.LogWarning("Ei löytynyt tavaraa arkusta tai määrä ei riitä!");
        }
    }


    // Päivittää arkku-UI:n esittämällä slotit ja esineet
void UpdateStorageUI()
{
    Debug.Log("Updating storage!");
    // Tyhjennä kaikki slotit ensin
    for (int i = 0; i < storageSlots.Count; i++)
    {
        if (i < storedItems.Count)
        {
            storageSlots[i].SetItem(storedItems[i]);
            storageSlots[i].gameObject.SetActive(true);
        }
        else
        {
            storageSlots[i].ClearSlot();  // Tyhjennä slotti ja aseta default-sprite
            storageSlots[i].gameObject.SetActive(true); // Pidä slotti näkyvissä
        }
    }
    Debug.Log("Vielä UI update:");
    if (playerInventory.inventoryUI != null)
    {
        playerInventory.inventoryUI.UpdateUI();
    }

}



    // Vaihda arkku auki/suljettu tilaan
    public void ToggleStorage()
    {
        isStorageOpen = !isStorageOpen;  // Vaihdetaan tilaa
        storageParent.SetActive(isStorageOpen);  // Näytetään tai piilotetaan arkku
        closeButton.SetActive(isStorageOpen);   // Näytetään sulkuttonappi vain, jos arkku on avoinna
        Debug.Log(isStorageOpen ? "Arkku avattu" : "Arkku suljettu");
    }

    // Tämä metodi sulkee arkku ikkunan, kun "X" painetaan
    public void CloseStorage()
    {
        isStorageOpen = false;
        storageParent.SetActive(false);  // Piilotetaan arkku
        closeButton.SetActive(false);   // Piilotetaan sulkuttonappi
        Debug.Log("Arkku suljettu");
    }
}
