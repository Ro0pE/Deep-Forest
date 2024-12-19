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

        private int arrowCount;
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

    arrowCount = playerInventory.GetItemCount(arrow);
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

private void HandleArrowEquipment(Equipment newEquipment)
{
    bool hasRangedWeapon = currentEquipment.Any(e => e != null && e.type == EquipmentType.RangedWeapon);

    if (!hasRangedWeapon)
    {
        Debug.LogWarning("Cannot equip arrows without a ranged weapon equipped.");
        return;
    }

    equipmentSlots[(int)SlotType.Arrow]?.SetItem(newEquipment);
    playerAttack.autoaAttackElement = newEquipment.element;

    arrowCount = playerInventory.GetItemCount(newEquipment);
    arrowAmount.text = $"{arrowCount} ea.";
    currentEquipment[(int)SlotType.Arrow] = newEquipment;
}

private void HandleTwoHandedEquipment(Equipment newEquipment)
{
    Unequip((int)SlotType.LeftHand);
    Unequip((int)SlotType.RightHand);

    currentEquipment[(int)SlotType.LeftHand] = newEquipment;
    currentEquipment[(int)SlotType.RightHand] = newEquipment;

    equipmentSlots[(int)SlotType.LeftHand]?.SetItem(newEquipment);
    equipmentSlots[(int)SlotType.RightHand]?.SetItem(newEquipment);
    equipmentSlots[(int)SlotType.LeftHand]?.UpdateSlot();
    equipmentSlots[(int)SlotType.RightHand]?.UpdateSlot();

    if (newEquipment.type == EquipmentType.RangedWeapon)
    {
        playerAttack.attackRange = 40f;
    }

    playerStats.EquipItem(newEquipment);
    ActivateModel(newEquipment);
}

private void HandleRightHandEquipment(Equipment newEquipment)
{
    Unequip((int)SlotType.RightHand);

    currentEquipment[(int)SlotType.RightHand] = newEquipment;
    equipmentSlots[(int)SlotType.RightHand]?.SetItem(newEquipment);

    if (newEquipment.type == EquipmentType.RangedWeapon)
    {
        playerAttack.attackRange = 40f;
    }
    else
    {
        playerAttack.attackRange = 15f;
    }

    playerStats.EquipItem(newEquipment);
    ActivateModel(newEquipment);
}

private void EquipGenericItem(Equipment newEquipment, int slotIndex)
{
    Equipment oldEquipment = currentEquipment[slotIndex];
    Unequip(slotIndex);

    currentEquipment[slotIndex] = newEquipment;
    equipmentSlots[slotIndex]?.SetItem(newEquipment);

    playerStats.EquipItem(newEquipment);
    ActivateModel(newEquipment);
}

private void ActivateModel(Equipment equipment)
{
    if (equipment.modelPrefab != null)
    {
        GameObject modelInstance = Instantiate(equipment.modelPrefab);
        modelInstance.SetActive(true);
        if (equipment.type == EquipmentType.RangedWeapon)
        {
            Debug.Log("Randged weapon!");
            modelInstance.transform.SetParent(weaponParentLeft.transform);
            modelInstance.transform.localPosition = Vector3.zero;
            modelInstance.transform.localRotation = Quaternion.identity;
            modelInstance.transform.localScale = Vector3.one;
            Debug.Log($"Equipped ranged weapon model: {equipment.modelPrefab.name}.");       
        }
        else
        {
        Transform parentTransform = equipment.slot == SlotType.RightHand || equipment.slot == SlotType.LeftHand
            ? weaponParentRight.transform
            : armorParent.transform;

        // Oletetaan, että 'Helm6' on lapsiobjekti parentissa (armorParent tai weaponParentRight)
        Transform helm6Transform = parentTransform.Find("Hat2");

        if (helm6Transform != null)
        {
            // Aktivoidaan 'Helm6' objektin GameObject
            helm6Transform.gameObject.SetActive(true);
            helm6Transform.localScale = new Vector3(2.5f, 2.5f, 2.5f);

            Debug.Log($"Equipped armor model: {equipment.modelPrefab.name}.");
        }
        else
        {
            Debug.LogWarning("Helm6 object not found under parent transform.");
        }

        }
            }
        }
   /* playerAttack.autoaAttackElement = newEquipment.element;
    UpdateAllSlots();
    playerAttack.StopMeleeAttack();*/



public void Unequip(int slotIndex)
{
    if (currentEquipment[slotIndex] == null)
    {
        Debug.LogWarning($"No equipment in slot {slotIndex} to unequip.");
        return;
    }

    Equipment oldEquipment = currentEquipment[slotIndex];
    Debug.Log(oldEquipment.type);

    // Special handling for arrows
    if (oldEquipment.slot == SlotType.Arrow)
    {
        arrowAmount.text = ""; // Clear arrow count UI
        equipmentSlots[(int)SlotType.Arrow]?.SetItem(null);
        currentEquipment[(int)SlotType.Arrow] = null;
        return;
    }

    // Special handling for two-handed weapons
    if (oldEquipment.slot == SlotType.TwoHanded)
    {
        currentEquipment[(int)SlotType.LeftHand] = null;
        currentEquipment[(int)SlotType.RightHand] = null;
        equipmentSlots[(int)SlotType.LeftHand]?.SetItem(null);
        equipmentSlots[(int)SlotType.RightHand]?.SetItem(null);
        arrowAmount.text = ""; // Clear arrows if applicable

        if (oldEquipment.type == EquipmentType.RangedWeapon)
        {
            Debug.Log("Poistetaan nuolet");
            // Poista myös nuoli, kun ranged weapon poistetaan
            equipmentSlots[(int)SlotType.Arrow]?.SetItem(null); // Poista nuolet
            currentEquipment[(int)SlotType.Arrow] = null;  // Tyhjennä nuolen slot
            equipmentSlots[(int)SlotType.Arrow].UpdateSlot();
            
        }
    }

    else
    {
        // General case for other equipment
        currentEquipment[slotIndex] = null;
        equipmentSlots[slotIndex]?.SetItem(null);
    }

    // Remove equipment bonuses
    playerStats.RemoveItem(oldEquipment);

    // Add the unequipped item back to the inventory
    playerInventory.AddItem(oldEquipment);


    // Invoke callback to update any listeners
    onEquipmentChangedCallback?.Invoke(null, oldEquipment);

    // Deactivate the model if applicable
if (oldEquipment.modelPrefab != null)
{
    // Logitaan SlotType
    

    // Valitaan oikea parent (vanhempi) sen mukaan, mikä ase on kyseessä
    Transform parentTransform = oldEquipment.slot switch
    {
        SlotType.RightHand => weaponParentRight.transform,
        SlotType.LeftHand => weaponParentLeft.transform,
        SlotType.TwoHanded => weaponParentLeft.transform,  // Asetetaan oikea käsi, mutta voidaan lisätä myös vasen käsi
        _ => armorParent.transform,  // Jos ei ole kädet, käytetään armorParent
    };

    // Logataan parent
 

    // Käydään läpi vanhemman kaikki lapset
    foreach (Transform child in parentTransform)
    {
       

        // Tarkistetaan, onko nimi sama kuin modelPrefab (tai clone-nimi)
        if (child.name == oldEquipment.modelPrefab.name || child.name == oldEquipment.modelPrefab.name + "(Clone)")
        {
            Destroy(child.gameObject);
           
        }
    }
}


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
        Debug.Log(weapon.element);
        return weapon.element;
        }
        
        else
        {
        Debug.Log("Weapon not equipped");
        Element neutral = Element.Neutral;
        return neutral;
        }
        
    }
}
