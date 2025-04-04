using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    // Aseiden GameObjectit
    public GameObject weaponHolderL; // Aseta tämä Inspectorissa WeaponShield_L:ään
    public GameObject weaponHolderR; // Aseta tämä Inspectorissa WeaponShield_R:ään
    private List<GameObject> leftHandWeapons = new List<GameObject>();
    private List<GameObject> rightHandWeapons = new List<GameObject>();
    private GameObject currentWeaponL;
    private GameObject currentWeaponR;

    // Armorit GameObjectit
    public GameObject armorHolder; // Aseta tämä Inspectorissa ArmorHolder:iin
    private List<GameObject> armors = new List<GameObject>();


    // Nykyiset varusteet
    private GameObject currentHeadArmor;
    private GameObject currentShoulderArmor;
    private GameObject currentGlovesArmor;
    private GameObject currentBeltArmor;
    private GameObject currentBootsArmor;
    private GameObject currentChestArmor;
    private GameObject currentNeckArmor;
    private GameObject currentFingerArmor;

    void Start()
    {
        // Lisää kaikki aseet vasempaan ja oikeaan käteen
        foreach (Transform weapon in weaponHolderL.transform)
        {
            leftHandWeapons.Add(weapon.gameObject);
            weapon.gameObject.SetActive(false); // Poista kaikki aseet näkyvistä alussa
        }
        foreach (Transform weapon in weaponHolderR.transform)
        {
            rightHandWeapons.Add(weapon.gameObject);
            weapon.gameObject.SetActive(false); // Poista kaikki aseet näkyvistä alussa
        }

        // Lisää kaikki armoreiden osat
        foreach (Transform armor in armorHolder.transform)
        {
            armors.Add(armor.gameObject);
        }
            //armorObject.SetActive(false); // Poista kaikki armoreiden osat näkyvistä alussa
    }
    

    // Varustaa aseen
    public void EquipWeapon(string weaponName, bool isLeftHand)
    {
        GameObject currentWeapon = isLeftHand ? currentWeaponL : currentWeaponR;
        List<GameObject> weaponList = isLeftHand ? leftHandWeapons : rightHandWeapons;

        // Piilota nykyinen ase
        if (currentWeapon != null)
        {
            currentWeapon.SetActive(false);
        }

        // Etsi ase listasta
        foreach (GameObject weapon in weaponList)
        {
            if (weapon.name == weaponName)
            {
                weapon.SetActive(true);
                if (isLeftHand)
                    currentWeaponL = weapon;
                else
                    currentWeaponR = weapon;
                return;
            }
        }

        Debug.LogWarning("Asetta " + weaponName + " ei löytynyt!");
    }

    // Varustaa armoreita
    public void EquipArmor(string armorName, string slot)
    {
        GameObject currentArmor = GetCurrentArmor(slot);
        Debug.Log("Armor lenght " + armors.Count);
        // Piilota nykyinen varuste
        if (currentArmor != null)
        {
            currentArmor.SetActive(false);
        }

        // Etsi ja varusta oikean tyyppinen varuste
        foreach (GameObject armor in armors)
        {
            Debug.Log("Armor:  " + armor.name);
            if (armor.name == armorName)
            {
                armor.SetActive(true);
                SetCurrentArmor(slot, armor);
                return;
            }
        }

        Debug.LogWarning("Armoreita ei löytynyt tyyppiä: " + slot + " ja nimeä: " + armorName);
    }

    // Hakee nykyisen varusteen slotin mukaan
    private GameObject GetCurrentArmor(string slot)
    {
        switch (slot)
        {
            case "Head": return currentHeadArmor;
            case "Shoulders": return currentShoulderArmor;
            case "Gloves": return currentGlovesArmor;
            case "Belt": return currentBeltArmor;
            case "Boots": return currentBootsArmor;
            case "Chest": return currentChestArmor;
            case "Neck": return currentNeckArmor;
            case "Finger": return currentFingerArmor;
            default: return null;
        }
    }

    // Asettaa nykyisen varusteen slotin mukaan
    private void SetCurrentArmor(string slot, GameObject armor)
    {
        switch (slot)
        {
            case "Head": currentHeadArmor = armor; break;
            case "Shoulders": currentShoulderArmor = armor; break;
            case "Gloves": currentGlovesArmor = armor; break;
            case "Belt": currentBeltArmor = armor; break;
            case "Boots": currentBootsArmor = armor; break;
            case "Chest": currentChestArmor = armor; break;
            case "Neck": currentNeckArmor = armor; break;
            case "Finger": currentFingerArmor = armor; break;
        }
    }

    // Hakee armoreiden listan slotin mukaan

}
