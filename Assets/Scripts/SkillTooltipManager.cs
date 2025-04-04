using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SkillTooltipManager : MonoBehaviour
{
    public GameObject tooltip; // Viittaus tooltipin GameObjectiin
    public GameObject tooltipPanel;
    public TextMeshProUGUI name; // Viittaus tooltipin tekstikenttään
    public TextMeshProUGUI manaCost; // Viittaus tooltipin tekstikenttään    
    public TextMeshProUGUI effectText; // Viittaus tooltipin tekstikenttään
    //public TextMeshProUGUI elementType; // Viittaus tooltipin tekstikenttään     
    public TextMeshProUGUI cooldown; // Viittaus tooltipin tekstikenttään   
    public TextMeshProUGUI castTime; 
    public Image skillImage;  
    public PlayerAttack playerAttack;

    void Start()
    {
        playerAttack = FindObjectOfType<PlayerAttack>();
        tooltipPanel.SetActive(false); // Piilota tooltip aluksi
    }

    // Funktio näytettävän tekstin asettamiseksi
   /* public void ShowTooltip(string skillName, int minLvl, int maxLvl, float skillManacost, string skillEffectText, string skillElementType, float skillCd, Sprite skillSprite, Vector3 position)
    {
        string elementColor = ""; 
        string nameColored = "";

        switch (skillElementType)
        {
            case "Fire":
                elementColor = "<color=red>Fire</color>";
                nameColored = $"<color=red>{skillName}</color>";
                break;
            case "Water":
                elementColor = "<color=blue>Water</color>";
                nameColored = $"<color=blue>{skillName}</color>";
                break;
            case "Earth":
                elementColor = "<color=green>Earth</color>";
                nameColored = $"<color=green>{skillName}</color>";
                break;
            case "Wind":
                elementColor = "<color=yellow>Wind</color>";
                nameColored = $"<color=yellow>{skillName}</color>";
                break;
            case "Holy":
                elementColor = "<color=#FFFF99>Holy</color>";
                nameColored = $"<color=#FFFF99>{skillName}</color>";
                break;
            case "Shadow":
                elementColor = "<color=grey>Shadow</color>";
                nameColored = $"<color=grey>{skillName}</color>";
                break;
            default:
                elementColor = skillElementType; 
                nameColored = $"<color=blue>{skillName}</color>";
                break;
        }
        string element = elementColor;
                    
  
        name.text = nameColored; // Aseta tooltipin teksti
        skillLvl.text = $"{minLvl}" + "/" + $"{maxLvl}";// Aseta tooltipin teksti
        manaCost.text =  $"{skillManacost}"; // Aseta tooltipin teksti
        effectText.text = skillEffectText; // Aseta tooltipin teksti
        elementType.text = element; // Aseta tooltipin teksti
        cooldown.text = $"{skillCd}" + "s"; // Aseta tooltipin teksti
        skillImage.sprite = skillSprite;
       // tooltip.transform.position = position; // Siirrä tooltip oikeaan kohtaan
        tooltipPanel.SetActive(true); // Näytä tooltip
    }*/
    public void ShowTooltip(Skill skill)
    {
        string elementColor = ""; 
        string nameColored = "";
        if (skill.isPassive)
        {
            elementColor = "<color=white>Passive</color>";
            nameColored = $"<color=white>{skill.skillName}</color>";
        }
        string skillElementType = skill.element.ToString();
        switch (skillElementType)
        {
            case "Fire":
                elementColor = "<color=red>Fire</color>";
                nameColored = $"<color=red>{skill.skillName}</color>";
                break;
            case "Water":
                elementColor = "<color=blue>Water</color>";
                nameColored = $"<color=blue>{skill.skillName}</color>";
                break;
            case "Earth":
                elementColor = "<color=green>Earth</color>";
                nameColored = $"<color=green>{skill.skillName}</color>";
                break;
            case "Wind":
                elementColor = "<color=yellow>Wind</color>";
                nameColored = $"<color=yellow>{skill.skillName}</color>";
                break;
            case "Holy":
                elementColor = "<color=#FFFF99>Holy</color>";
                nameColored = $"<color=#FFFF99>{skill.skillName}</color>";
                break;
            case "Shadow":
                elementColor = "<color=grey>Shadow</color>";
                nameColored = $"<color=grey>{skill.skillName}</color>";
                break;
            default:
                elementColor = skillElementType; 
                nameColored = $"<color=white>{skill.skillName}</color>";
                break;
        }
        string element = elementColor;
                    
  
        name.text = nameColored; // Aseta tooltipin teksti
        //skillLvl.text = $"{skill.skillLevel}" + "/" + $"{skill.skillMaxLevel}";// Aseta tooltipin teksti
        if (!skill.isPassive)
        {
        manaCost.gameObject.SetActive(true);
        castTime.gameObject.SetActive(true);
        manaCost.text =  $"{skill.manaCost}"; // Aseta tooltipin teksti
        cooldown.text = $"{skill.cooldown}" + "s"; // Aseta tooltipin teksti
        cooldown.color = Color.red;
        float newCastTime = skill.castTime - playerAttack.attackSpeedReduction;
        if (newCastTime < 0.2f)
        {
            newCastTime = 0.2f;
        }
        castTime.text = $"{newCastTime:F1}" + "s";
        }
        else
        {
            cooldown.text = "PASSIVE";
            cooldown.color = Color.green;
            castTime.gameObject.SetActive(false);
            manaCost.gameObject.SetActive(false);
        }
        effectText.text = skill.updatedInfoText; // Aseta tooltipin teksti
        //elementType.text = element; // Aseta tooltipin teksti

        skillImage.sprite = skill.skillIcon;
       // tooltip.transform.position = position; // Siirrä tooltip oikeaan kohtaan
        tooltipPanel.SetActive(true); // Näytä tooltip
    }

    // Piilota tooltip
    public void HideTooltip()
    {
        
        tooltipPanel.SetActive(false); // Piilota tooltip
    }
}
