using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemTooltipManager : MonoBehaviour
{
    public GameObject tooltip; // Viittaus tooltipin GameObjectiin
    public GameObject itemPanel;
    public GameObject usagePanel;
    public GameObject equipmentPanel;

    public TextMeshProUGUI itemName; // Viittaus tooltipin tekstikenttään
    public TextMeshProUGUI equipmentName; // Viittaus tooltipin tekstikenttään
    public TextMeshProUGUI equipmentInfo; // Viittaus tooltipin tekstikenttään
    public TextMeshProUGUI itemRarity;
    public TextMeshProUGUI equipmentRarity;
    public TextMeshProUGUI equipmentSellPrice; // Viittaus tooltipin tekstikenttään
    public TextMeshProUGUI sellPrice; // Viittaus tooltipin tekstikenttään
    public TextMeshProUGUI infoText; // Viittaus tooltipin tekstikenttään    
    public TextMeshProUGUI usageText; // Viittaus tooltipin tekstikenttään  
    public TextMeshProUGUI weaponDamage; // Viittaus tooltipin tekstikenttään
    public TextMeshProUGUI magickDamage; // Viittaus tooltipin tekstikenttään
    public TextMeshProUGUI aspd;
    public TextMeshProUGUI str; // Viittaus tooltipin tekstikenttään    
    public TextMeshProUGUI vit;
    public TextMeshProUGUI intellect; // Viittaus tooltipin tekstikenttään
    public TextMeshProUGUI dex;
    public TextMeshProUGUI agi; // Viittaus tooltipin tekstikenttään  
    public TextMeshProUGUI hpReg;
    public TextMeshProUGUI spReg; // Viittaus tooltipin tekstikenttään
    public TextMeshProUGUI crit; // Viittaus tooltipin tekstikenttään    
    public TextMeshProUGUI dodge;
    public TextMeshProUGUI def; // Viittaus tooltipin tekstikenttään    
    public TextMeshProUGUI lvlreq;

    public TextMeshProUGUI atkLabel;
    public TextMeshProUGUI aspdLabel;
    public TextMeshProUGUI matkLabel;
    public TextMeshProUGUI strLabel;
    public TextMeshProUGUI vitLabel;
    public TextMeshProUGUI intellectLabel;
    public TextMeshProUGUI dexLabel;
    public TextMeshProUGUI agiLabel;
    public TextMeshProUGUI hpRegLabel;
    public TextMeshProUGUI spRegLabel;
    public TextMeshProUGUI critLabel;
    public TextMeshProUGUI dodgeLabel;
    public TextMeshProUGUI defLabel;


    public TextMeshProUGUI restoreInfo;
    public TextMeshProUGUI healAmountText;
    public TextMeshProUGUI manaAmountText;
    public TextMeshProUGUI healLabel;
    public TextMeshProUGUI manaLabel;
    public Image itemImage; 
    public Image equipmentImage;   


    void Start()
    {

        itemPanel.SetActive(false); // Piilota tooltip aluksi
        equipmentPanel.SetActive(false);
    }

    void Update()
    {

    }

public void ShowTooltip(Item item)
{ 

    // Handle Potions (if it's a Potion)
    if (item is Potion potion)
    {
        itemName.text = item.itemName; // Set item name
        sellPrice.text = item.sellPrice.ToString(); // Set sell price
        infoText.text = item.infoText; // Set item info
        itemImage.sprite = item.icon;
        itemRarity.text = item.rarity.ToString();
        SetRarityColor(itemRarity, item.rarity);

        // Tarkistetaan, onko manaAmount suurempi kuin 0
        if (potion.manaAmount > 0)
        {
            manaAmountText.text = "+" + potion.manaAmount.ToString();
            manaLabel.gameObject.SetActive(true);
            manaAmountText.gameObject.SetActive(true);
        }
        else
        {
            manaAmountText.gameObject.SetActive(false);
            manaLabel.gameObject.SetActive(false);
        }

        // Tarkistetaan, onko healAmount suurempi kuin 0
        if (potion.healAmount > 0)
        {
            healAmountText.text = "+" + potion.healAmount.ToString();
            healLabel.gameObject.SetActive(true);
            healAmountText.gameObject.SetActive(true);
        }
        else
        {
            healAmountText.gameObject.SetActive(false);
            healLabel.gameObject.SetActive(false);
        }

        // Näytetään restoreInfo vain, jos jompikumpi (manaAmount tai healAmount) on käytössä
        restoreInfo.gameObject.SetActive(potion.manaAmount > 0 || potion.healAmount > 0);

        usagePanel.SetActive(false);
        equipmentPanel.SetActive(false);
        itemPanel.SetActive(true); // Show tooltip

    }

    // Handle Equipment (if it's an Equipment)
// Handle Equipment (if it's an Equipment)
    else if (item is Equipment equipment)
    {
        equipmentName.text = item.itemName; // Set item name
        equipmentSellPrice.text = item.sellPrice.ToString(); // Set sell price
        equipmentInfo.text = item.infoText; // Set item info
        equipmentImage.sprite = item.icon;
        equipmentRarity.text = equipment.rarity.ToString();
        SetRarityColor(equipmentRarity, equipment.rarity);
        

        SetStatValue(weaponDamage, equipment.damageValue);
        SetStatValue(magickDamage, equipment.magickDamageValue);
        SetStatValue(aspd, equipment.weaponAttackSpeed);
        SetStatValue(str, equipment.strValue);
        SetStatValue(vit, equipment.vitValue);
        SetStatValue(intellect, equipment.intValue);
        SetStatValue(dex, equipment.dexValue);
        SetStatValue(agi, equipment.agiValue);
        SetStatValue(hpReg, equipment.hpRegValue);
        SetStatValue(spReg, equipment.spRegValue);
        SetStatValue(crit, equipment.critValue);
        SetStatValue(dodge, equipment.dodgeValue);
        SetStatValue(def, equipment.defValue);
        //lvlreq.text = equipment.requiredLevel.ToString(); // This stays in int format

        // Show rarity color for equipment only
        SetRarityColor(equipmentName, equipment.rarity);

        equipmentPanel.SetActive(true);
        usagePanel.SetActive(false);
    }
    else if (item is Card cardItem)
    {
      
        itemName.text = cardItem.itemName; // Set item name
        itemImage.sprite = cardItem.icon;
        infoText.text = cardItem.infoText;
        sellPrice.text = cardItem.sellPrice.ToString();
        itemRarity.text = item.rarity.ToString();
        SetRarityColor(itemRarity, cardItem.rarity);
        usagePanel.SetActive(true);
        restoreInfo.gameObject.SetActive(false);
        equipmentPanel.SetActive(false);
        itemPanel.SetActive(true); // Show tooltip
    }

    // Handle other item types
    else
    {
     
        itemImage.sprite = item.icon;
        itemName.text = item.itemName; // Set item name
        infoText.text = item.infoText;
        usageText.text = item.infoText;
        
        usagePanel.SetActive(true);
        restoreInfo.gameObject.SetActive(false);
        equipmentPanel.SetActive(false);
        itemPanel.SetActive(true); // Show tooltip
    }

    
}

// Helper method to set stat values and only display if non-zero
private void SetStatValue(TextMeshProUGUI textElement, float value)
{
    if (textElement == null)
    {
        Debug.LogWarning("Text element is null in SetStatValue");
        return;
    }

    if (value != 0)
    {
        if (textElement != aspd)
        {
            textElement.text = "+" + value.ToString("F0"); // Kokonaisluku
        }
        else
        {
            textElement.text = value.ToString("F1"); // 1 desimaali
        }

        // Tarkista, onko kyseessä weaponDamage tai aspd
        if (textElement != weaponDamage && textElement != aspd)
        {
            textElement.color = value > 0 ? Color.green : Color.white;
        }
        else
        {
            textElement.color = Color.white;
        }

        textElement.transform.parent.gameObject.SetActive(true); // Näytä teksti
    }
    else
    {
        textElement.transform.parent.gameObject.SetActive(false); // Piilota teksti, jos arvo on 0
    }
}



// Helper method to set rarity color based on rarity
private void SetRarityColor(TextMeshProUGUI textElement, Rarity rarity)
{
    switch (rarity)
    {
        case Rarity.Common:
            textElement.color = Color.white;
            break;
        case Rarity.Uncommon:
            textElement.color = Color.green;
            break;
        case Rarity.Rare:
            textElement.color = Color.blue;
            break;
        case Rarity.Epic:
            textElement.color = new Color(0.64f, 0.21f, 0.93f); // Purple
            break;
        case Rarity.Legendary:
            textElement.color = new Color(1f, 0.5f, 0f); // Orange
            break;
    }
}


    // Piilota tooltip
    public void HideTooltip()
    {
        itemPanel.SetActive(false); // Piilota tooltip
        equipmentPanel.SetActive(false);
    }
}
