using UnityEngine;

public class EquipmentUI : MonoBehaviour
{
    private EquipmentManager equipmentManager;
    private EquipmentSlot[] slots;

    void Start()
    {
        equipmentManager = EquipmentManager.instance;

        // Hae kaikki EquipmentSlotit EquipmentManagerista
        slots = equipmentManager.equipmentSlots;

        UpdateUI(); // Päivitä UI heti
    }

    void UpdateUI()
    {
        // Käy läpi kaikki slotit ja päivitä niiden sisältö
        for (int i = 0; i < slots.Length; i++)
        {
            slots[i].UpdateSlot();
        }
    }
    
}
