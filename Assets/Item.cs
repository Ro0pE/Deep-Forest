using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class Item : ScriptableObject
{
    public string itemName;
    public Sprite icon;
    public string itemType;
    public int dropChance;
    public bool isStackable = true;  // Oletuksena true, equipmentit voidaan myöhemmin määritellä ei-stackableksi
    public int quantity = 1;
    public int sellPrice;
    public string infoText;
    public string usageText;

    public Item Clone()
    {
        Item clonedItem = Instantiate(this); // Luo uusi instanssi ScriptableObjectista
        clonedItem.quantity = this.quantity; // Kopioi muut muuttujat
        return clonedItem;
    }

    public virtual void Use()
    {
        Debug.Log($"Using item: {itemName}");
        
    }
}
