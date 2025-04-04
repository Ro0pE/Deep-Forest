using System.Collections.Generic;
using UnityEngine;
using TMPro;

[CreateAssetMenu(fileName = "New Equipment", menuName = "Inventory/Equipment")]
public class Equipment : Item
{
    public SlotType slot;
    public EquipmentType type;
    public EquipmentModel model;
    public Element element;
    public string modelName;
    public int damageValue;
    public int magickDamageValue;
    public float weaponAttackSpeed;
    public int strValue = 0;
    public int vitValue = 0;
    public int intValue = 0;
    public int agiValue = 0;
    public int dexValue = 0;

    public int hpRegValue = 0;
    public int spRegValue = 0;
    public float critValue = 0f;
    public float dodgeValue = 0f;
    public float castSpeedValue = 0f;
    public float defValue = 0f;
    public int hitValue = 0;
    public int requiredLevel = 0;
    public int stackSize = 1;
    public int attackRange;


    public List<Card> cardSlots = new List<Card>(); // Lista korttipaikoista
    public int maxCardSlots = 0;

    public Equipment()
    {
        isStackable = false;  // Asetetaan equipmentit ei-stackableiksi
    }

    public override void Use()
    {
        base.Use();
        Equip();
    }

    void Equip()
    {
        EquipmentManager.instance.Equip(this);
    }
    
    public void SetCardSlots(int slots)
    {
        maxCardSlots = slots;

    }
    // Funktio korttien lisäämiseksi varusteeseen
    public bool AddCard(Card card)
    {
        if (cardSlots.Count < maxCardSlots)
        {
            cardSlots.Add(card);
            Debug.Log($"Kortti '{card.itemName}' lisätty varusteeseen.");
            return true;
        }
        else
        {
            Debug.LogWarning("Korttipaikat täynnä! Ei voida lisätä lisää kortteja.");
            return false;
        }
    }
}    

// Tyyppien määrittely
public enum SlotType
{
    Head,
    Shoulders,
    Gloves,
    Belt,
    Legs,
    Boots,
    LeftHand,
    RightHand,
    Chest,
    Neck,
    Finger,
    Arrow,
    TwoHanded
}
public enum EquipmentType
{
    Armor,
    MeleeWeapon,
    RangedWeapon,
    Arrow
}

public enum EquipmentModel
{
    Sword,
    Bow,
    Axe,
    Armor,
    Arrow
}


// Rarityn määrittely
public enum Rarity
{
    Common,
    Uncommon,
    Rare,
    Epic,
    Legendary
}

