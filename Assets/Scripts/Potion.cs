using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "New Potion", menuName = "Inventory/Potion", order = 1)]
public class Potion : Item
{
    public int healAmount;
    public int manaAmount;


    public override void Use()
    {
        base.Use();
        Heal();
    }

    void Heal()
    {
        Debug.Log($"Healed for {healAmount} points!");
        // Lisää logiikka pelaajan terveyden parantamiseen
        PlayerHealth playerHealth = FindObjectOfType<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.Heal(healAmount);
            playerHealth.RestoreMana(manaAmount);
        }
    }

    public new Potion Clone()
    {
        Potion clonedPotion = Instantiate(this) as Potion; // Luo uusi instanssi
        clonedPotion.healAmount = this.healAmount; // Kopioi Potion-erikoiskentät
        clonedPotion.manaAmount = this.manaAmount;
        return clonedPotion;
    }
}
