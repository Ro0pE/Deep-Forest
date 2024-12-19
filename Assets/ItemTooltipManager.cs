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
    public TextMeshProUGUI agi; // Viittaus tooltipin tekstikenttään
    public TextMeshProUGUI spr; // Viittaus tooltipin tekstikenttään    
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
    public TextMeshProUGUI agiLabel;
    public TextMeshProUGUI sprLabel;
    public TextMeshProUGUI hpRegLabel;
    public TextMeshProUGUI spRegLabel;
    public TextMeshProUGUI critLabel;
    public TextMeshProUGUI dodgeLabel;
    public TextMeshProUGUI defLabel;


    public TextMeshProUGUI restoreInfo;
    public TextMeshProUGUI healAmountText;
    public TextMeshProUGUI manaAmountText;
    public Image itemImage; 
    public Image equipmentImage;   

    void Start()
    {
        itemPanel.SetActive(false); // Piilota tooltip aluksi
        equipmentPanel.SetActive(false);
    }

    void Update()
    {
    // Jos tooltip on näkyvissä ja käyttäjä klikkaa jotain muuta kuin tooltipia
        if ((itemPanel.activeSelf || equipmentPanel.activeSelf) && Input.GetMouseButtonDown(0))
        {
            bool clickedOutsideItemPanel = !RectTransformUtility.RectangleContainsScreenPoint(itemPanel.GetComponent<RectTransform>(), Input.mousePosition, Camera.main);
            bool clickedOutsideEquipmentPanel = !RectTransformUtility.RectangleContainsScreenPoint(equipmentPanel.GetComponent<RectTransform>(), Input.mousePosition, Camera.main);

            // Piilota tooltip, jos klikataan tooltipien ulkopuolelle
            if (clickedOutsideItemPanel && clickedOutsideEquipmentPanel)
            {
                HideTooltip();
            }
    }
    }

    // Funktio näytettävän tekstin asettamiseksi
    /*public void ShowTooltip(string name, int price, string info, string usage, Sprite sprite, string healInfo = "", string manaInfo = "")
    {
        itemName.text = name; // Aseta tooltipin teksti
        sellPrice.text =  $"{price}"; // Aseta tooltipin teksti
        infoText.text = info; // Aseta tooltipin teksti
        usageText.text = usage;
        itemImage.sprite = sprite;

        // Jos on potion, näytetään healAmount ja manaAmount
        if (!string.IsNullOrEmpty(healInfo))
        {
            Debug.Log("yks");
            healAmountText.text = healInfo;
            healAmountText.gameObject.SetActive(true); // Näytetään healAmount
            restoreInfo.gameObject.SetActive(true); // Näytetään restoreInfo
            usagePanel.SetActive(false); // Piilotetaan usageText
        }
        else
        {
            healAmountText.gameObject.SetActive(false); // Piilotetaan healAmount
        }

        if (!string.IsNullOrEmpty(manaInfo))
        {
            Debug.Log("kaks");
            manaAmountText.text = manaInfo;
            manaAmountText.gameObject.SetActive(true); // Näytetään manaAmount
            restoreInfo.gameObject.SetActive(true); // Näytetään restoreInfo
            usagePanel.SetActive(false); // Piilotetaan usageText
        }
        else
        {
            manaAmountText.gameObject.SetActive(false); // Piilotetaan manaAmount
        }

        // Jos ei ole potiona, varmistetaan että usageText on näkyvissä
        if (string.IsNullOrEmpty(healInfo) && string.IsNullOrEmpty(manaInfo))
        {
            Debug.Log("yks");
            usagePanel.SetActive(true); // Näytetään usageText, jos ei ole potion
            restoreInfo.gameObject.SetActive(false); // Piilotetaan restoreInfo
        }

        itemPanel.SetActive(true); // Näytetään tooltip
    } */
public void ShowTooltip(Item item)
{ 


    // Handle Potions (if it's a Potion)
    if (item is Potion potion)
    {
            Debug.Log("Item name:  "+ item.itemName + " " + item.sellPrice);
        itemName.text = item.itemName; // Set item name
        sellPrice.text = item.sellPrice.ToString(); // Set sell price
        infoText.text = item.infoText; // Set item info
        itemImage.sprite = item.icon;
        manaAmountText.text = "+"+potion.manaAmount.ToString();
        healAmountText.text = "+" +potion.healAmount.ToString();
        manaAmountText.gameObject.SetActive(true);
        restoreInfo.gameObject.SetActive(true);
        healAmountText.gameObject.SetActive(true);
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

        SetStatValue(weaponDamage, equipment.damageValue);
        SetStatValue(magickDamage, equipment.magickDamageValue);
        SetStatValue(aspd, equipment.weaponAttackSpeed);
        SetStatValue(str, equipment.strValue);
        SetStatValue(vit, equipment.vitValue);
        SetStatValue(intellect, equipment.intValue);
        SetStatValue(agi, equipment.agiValue);
        SetStatValue(spr, equipment.spiritValue);
        SetStatValue(hpReg, equipment.hpRegValue);
        SetStatValue(spReg, equipment.spRegValue);
        SetStatValue(crit, equipment.critValue);
        SetStatValue(dodge, equipment.dodgeValue);
        SetStatValue(def, equipment.defValue);
        lvlreq.text = equipment.requiredLevel.ToString(); // This stays in int format

        // Show rarity color for equipment only
        SetRarityColor(equipmentName, equipment.rarity);

        equipmentPanel.SetActive(true);
        usagePanel.SetActive(false);
    }

    // Handle other item types
    else
    {
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

        Debug.Log("Value for " + textElement.name + ": " + value);

        if (value != 0)
        {
            textElement.text = value.ToString("F1"); // Format value with 1 decimal place

            // Tarkista, onko kyseessä weaponDamage tai aspd
            if (textElement != weaponDamage && textElement != aspd)
            {
                // Aseta väri vain, jos teksti ei ole weaponDamage tai aspd
                textElement.color = value > 0 ? Color.green : Color.white;
            }
            else
            {
                // Pidä väri aina valkoisena näille elementeille
                textElement.color = Color.white;
            }

            textElement.transform.parent.gameObject.SetActive(true); // Show the text element
        }
        else
        {
            textElement.transform.parent.gameObject.SetActive(false); // Hide the text element if value is zero
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
        Debug.Log("Hiding tooltip");
        itemPanel.SetActive(false); // Piilota tooltip
        equipmentPanel.SetActive(false);
    }
}
