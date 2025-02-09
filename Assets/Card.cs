using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "New Card", menuName = "Inventory/Card", order = 1)]
public class Card : Item
{
    public Rarity rarity = Rarity.Legendary; // Oletuksena "Common"
    
    public override void Use()
    {
        base.Use();
       
    }


}
