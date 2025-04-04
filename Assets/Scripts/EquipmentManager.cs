using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;

    public class EquipmentManager : MonoBehaviour
    {
        public static EquipmentManager instance;

        [SerializeField] public EquipmentSlot[] equipmentSlots; // Määritetään suoraan Inspectorissa
        public Equipment[] currentEquipment;
        public delegate void OnEquipmentChanged(Equipment newEquipment, Equipment oldEquipment);
        public OnEquipmentChanged onEquipmentChangedCallback;
        public PlayerStats playerStats;
        public PlayerAttack playerAttack;
        public Inventory playerInventory;

        public int arrowCount;
        public TextMeshProUGUI arrowAmount;
        [SerializeField] private Transform headSlotParent;
        [SerializeField] private Transform chestSlotParent;
        [SerializeField] private Transform feetSlotParent;
        [SerializeField] private Transform shouldersSlotParent;
        [SerializeField] private Transform beltSlotParent;
        [SerializeField] private Transform neckSlotParent;
        [SerializeField] private Transform glovesSlotParent;
        [SerializeField] private Transform fingerSlotParent;
        [SerializeField] private Transform rightHandSlotParent;
        [SerializeField] private Transform leftHandSlotParent;
        [SerializeField] private Transform legsSlotParent;
        [SerializeField] private Transform arrowSlotParent;


        public GameObject weaponParentRight;
        public GameObject weaponParentLeft;
        public GameObject armorParent;
        private WeaponManager weaponManager;
        private void Awake()
        {
            if (instance != null)
            {
                Debug.LogWarning("Useampi EquipmentManager löytyi!");
                return;
            }
            instance = this;
        }

        private void Start()
        {
            weaponManager = FindObjectOfType<WeaponManager>();
            playerAttack = FindObjectOfType<PlayerAttack>();
            playerInventory = FindObjectOfType<Inventory>();
            playerStats = FindObjectOfType<PlayerStats>();

            int numSlots = System.Enum.GetNames(typeof(SlotType)).Length;
            currentEquipment = new Equipment[numSlots];
        }

        public void UpdateAllSlots()
        {
            for (int i = 0; i < equipmentSlots.Length; i++)
            {
                if (equipmentSlots[i] != null)
                {
                    equipmentSlots[i].UpdateSlot();
                }
                else
                {
                    Debug.LogError("EquipmentSlot is null at index " + i);
                }
            }
        }

        // Loput metodit pysyvät lähes muuttumattomina...
    

public bool UpdateArrowCount(Equipment arrow)
{
    if (arrowAmount == null)
    {
        Debug.LogWarning("ArrowAmount text field is not assigned!");
        return false;
    }

    arrowCount = playerInventory.GetEquipmentCount(arrow);
    arrowAmount.text = $"{arrowCount} ea.";
    return arrowCount > 0;
}



public void Equip(Equipment newEquipment)
{
    if (newEquipment == null)
    {
        Debug.LogWarning("Cannot equip a null item.");
        return;
    }

    int slotIndex = (int)newEquipment.slot;

    // Nuolten käsittely
    if (newEquipment.slot == SlotType.Arrow)
    {
        HandleArrowEquipment(newEquipment);
        return;
    }

    // Kahden käden aseiden käsittely
    if (newEquipment.slot == SlotType.TwoHanded)
    {
        HandleTwoHandedEquipment(newEquipment);
        return;
    }

    // Oikean käden varusteet
    if (newEquipment.slot == SlotType.RightHand)
    {
        HandleRightHandEquipment(newEquipment);
        return;
    }

    // Varusteen yleinen käsittely muille sloteille
    EquipGenericItem(newEquipment, slotIndex);
}

public void HandleArrowEquipment(Equipment newEquipment)
{
    bool hasRangedWeapon = currentEquipment.Any(e => e != null && e.type == EquipmentType.RangedWeapon);

    if (!hasRangedWeapon)
    {
        Debug.LogWarning("Cannot equip arrows without a ranged weapon equipped.");
        return;
    }
    Unequip((int)SlotType.Arrow);

    equipmentSlots[(int)SlotType.Arrow]?.SetItem(newEquipment);
    playerAttack.autoaAttackElement = newEquipment.element;

    //arrowCount = playerInventory.GetEquipmentCount(newEquipment);
    arrowCount = newEquipment.quantity;
    arrowAmount.text = $"{arrowCount} ea.";
    currentEquipment[(int)SlotType.Arrow] = newEquipment;
    playerInventory.RemoveItem(newEquipment,arrowCount);
}

private void HandleTwoHandedEquipment(Equipment newEquipment)
{
    // Irrota nykyinen oikean käden varuste
    Unequip((int)SlotType.LeftHand);

    // Määritä uusi varuste oikealle kädelle
    currentEquipment[(int)SlotType.LeftHand] = newEquipment;
    equipmentSlots[(int)SlotType.LeftHand]?.SetItem(newEquipment);

    // Päivitä hyökkäysalue riippuen varustetyypistä
  /*  if (newEquipment.type == EquipmentType.RangedWeapon)
    {
        playerAttack.attackRange = 30f;
    }
    else
    {
        playerAttack.attackRange = 15f;
    }
 */
    playerAttack.weaponRange = newEquipment.attackRange;
    // Aseta hyökkäyksen elementti varusteelle
    playerAttack.autoaAttackElement = newEquipment.element;

    // Päivitä pelaajan tilastot
    playerStats.EquipItem(newEquipment);

    // Käytä WeaponManageria oikean käden aseiden varustamiseen
    if (weaponManager != null)
    {
        // Varusta oikea käsi uudella aseella
        weaponManager.EquipWeapon(newEquipment.modelName,true);
    }

    // Poista varuste varastosta
    playerInventory.RemoveItem(newEquipment, 1);
}

private void HandleRightHandEquipment(Equipment newEquipment)
{
    // Irrota nykyinen oikean käden varuste
    Unequip((int)SlotType.RightHand);

    // Määritä uusi varuste oikealle kädelle
    currentEquipment[(int)SlotType.RightHand] = newEquipment;
    equipmentSlots[(int)SlotType.RightHand]?.SetItem(newEquipment);

    // Päivitä hyökkäysalue riippuen varustetyypistä
   /* if (newEquipment.type == EquipmentType.RangedWeapon)
    {
        playerAttack.attackRange = 40f;
    }
    else
    {
        playerAttack.attackRange = 15f;
    } */
    playerAttack.weaponRange = newEquipment.attackRange;
    // Aseta hyökkäyksen elementti varusteelle
    playerAttack.autoaAttackElement = newEquipment.element;

    // Päivitä pelaajan tilastot
    playerStats.EquipItem(newEquipment);

    // Käytä WeaponManageria oikean käden aseiden varustamiseen
    if (weaponManager != null)
    {
        // Varusta oikea käsi uudella aseella
        weaponManager.EquipWeapon(newEquipment.modelName, false);
    }

    // Poista varuste varastosta
    playerInventory.RemoveItem(newEquipment, 1);
}


private void EquipGenericItem(Equipment newEquipment, int slotIndex)
{
    // Varmistetaan, että varuste on armori
    if (newEquipment.type != EquipmentType.Armor)
    {
        Debug.LogWarning("Tämä varuste ei ole armori!");
        return;
    }

    // Poistetaan vanha varuste
    Equipment oldEquipment = currentEquipment[slotIndex];
    Unequip(slotIndex);

    // Päivitetään uusi varuste nykyiseksi
    currentEquipment[slotIndex] = newEquipment;
    equipmentSlots[slotIndex]?.SetItem(newEquipment);

    // Varustetaan armori
    string armorName = newEquipment.modelName;
    SlotType armorSlot = newEquipment.slot; // Oletetaan, että EquipArmor tarvitsee SlotType
    weaponManager.EquipArmor(armorName, armorSlot.ToString());

    // Päivitetään pelaajan statsit
    playerStats.EquipItem(newEquipment);
    playerInventory.RemoveItem(newEquipment, 1);
}




public void Unequip(int slotIndex)
{
    if (currentEquipment[slotIndex] == null)
    {
        Debug.LogWarning($"No equipment in slot {slotIndex} to unequip.");
        return;
    }

    Equipment oldEquipment = currentEquipment[slotIndex];

    Debug.Log("Otetaan varuste pois " + oldEquipment.itemName);
    // Special handling for arrows
    if (oldEquipment.slot == SlotType.Arrow)
    {
        arrowAmount.text = ""; // Clear arrow count UI
        equipmentSlots[(int)SlotType.Arrow]?.SetItem(null);
        currentEquipment[(int)SlotType.Arrow] = null;
        playerInventory.AddItem(oldEquipment);
        return;
    }

    // Special handling for two-handed weapons
    if (oldEquipment.slot == SlotType.TwoHanded)
    {
        currentEquipment[(int)SlotType.LeftHand] = null;
        currentEquipment[(int)SlotType.RightHand] = null;
        equipmentSlots[(int)SlotType.LeftHand]?.SetItem(null);
        equipmentSlots[(int)SlotType.RightHand]?.SetItem(null);

       // arrowAmount.text = ""; // Clear arrows if applicable

      /*  if (oldEquipment.type == EquipmentType.RangedWeapon)
        {

            // Poista myös nuoli, kun ranged weapon poistetaan
            equipmentSlots[(int)SlotType.Arrow]?.SetItem(null); // Poista nuolet
            currentEquipment[(int)SlotType.Arrow] = null;  // Tyhjennä nuolen slot
            equipmentSlots[(int)SlotType.Arrow].UpdateSlot();
            
        } */
    }

    else
    {
        Debug.Log("poisteaan armoria tjs ");

        // General case for other equipment
        currentEquipment[slotIndex] = null;
        equipmentSlots[slotIndex]?.SetItem(null);

    }
    if (currentEquipment[(int)SlotType.LeftHand] != null)
    {
        Debug.Log("Ase "  + currentEquipment[(int)SlotType.LeftHand]);
        playerAttack.weaponRange = currentEquipment[(int)SlotType.LeftHand].attackRange;
    }
    else
    {
        Debug.Log("ei ole asetta kädessä, asetetaan weapon range");
        playerAttack.weaponRange = 0; // Reset to 0 if no weapon is equipped in the right hand
    }
    // Remove equipment bonuses
    playerStats.RemoveItem(oldEquipment);

    // Add the unequipped item back to the inventory

    playerInventory.AddItem(oldEquipment);



    // Invoke callback to update any listeners
    onEquipmentChangedCallback?.Invoke(null, oldEquipment);

 



}



    public Item IsArrowEquipped()
    {
        // Käydään läpi currentEquipment-taulukko ja tarkistetaan, onko joku varusteista nuoli
        foreach (var equipment in currentEquipment)
        {
            if (equipment != null && equipment.slot == SlotType.Arrow)
            {
               
                return equipment; // Nuoli on varustettu
            }
        }
        return null; // Ei ole varustettu nuolia
    }





    Transform GetSlotParentForType(SlotType slot)
    {
        switch (slot)
        {
            case SlotType.Head: return headSlotParent;
            case SlotType.Chest: return chestSlotParent;
            case SlotType.Boots: return feetSlotParent;
            case SlotType.Shoulders: return shouldersSlotParent;
            case SlotType.Belt: return beltSlotParent;
            case SlotType.Neck: return neckSlotParent;
            case SlotType.Gloves: return glovesSlotParent;
            case SlotType.Finger: return fingerSlotParent;
            case SlotType.RightHand: return rightHandSlotParent;
            case SlotType.LeftHand: return leftHandSlotParent;
            case SlotType.Legs: return legsSlotParent;
            case SlotType.Arrow: return arrowSlotParent;
            default: return null;
        }
    }

    public Element WeaponCurrentElement()
    {
        if (currentEquipment[(int)SlotType.RightHand] != null)
        {
        Equipment weapon = currentEquipment[(int)SlotType.RightHand];
        return weapon.element;
        }
        
        else
        {
        Element neutral = Element.Neutral;
        return neutral;
        }
        
    }
}
