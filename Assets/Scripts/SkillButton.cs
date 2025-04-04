using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class SkillButton : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerDownHandler, IPointerUpHandler
{
    public Skill skill; // Skill-objekti, joka liittyy tähän painikkeeseen
    public Image skillIcon; // Ikoni
    public TextMeshProUGUI skillNameText; // Skillin nimi
    public TextMeshProUGUI skillLevel;
   // public TextMeshProUGUI skillLevelText; // Taso
    //public Button levelUpButton; // Levelup-painike
    public Button learnButton;
    public Button increaseButton;
    public Button decreaseButton;
 

    private Vector2 originalIconPosition; // Ikonin alkuperäinen sijainti
    private RectTransform iconRectTransform; // Ikonin RectTransform
    private RectTransform rectTransform; // Koko painikkeen RectTransform

    private Transform originalParent; // Alkuperäinen parent (action barin slotti)
    public SkillTooltipManager skillTooltipManager; // TooltipManager viittaus
    private bool isPointerInside = false;
    public GameObject transparentTriggerArea;
    public PlayerStats playerStats;


    private void Start()
    {
        // Alustetaan tarvittavat komponentit
        playerStats = FindObjectOfType<PlayerStats>();

        //skill.UpdateInfoText(); // Päivitä info tekstin aluksi
    
        iconRectTransform = skillIcon.GetComponent<RectTransform>();
        skillTooltipManager = FindObjectOfType<SkillTooltipManager>();
        rectTransform = GetComponent<RectTransform>();
        originalParent = transform.parent;

        originalIconPosition = iconRectTransform.anchoredPosition;
        UpdateUI();
    }

    public void Setup(Skill skill)
    {
        this.skill = skill;
        skillIcon.sprite = skill.skillIcon;
        skillNameText.text = skill.skillName;
        skillLevel.text = $"{skill.skillLevel} / {skill.skillMaxLevel}";
        UpdateUI();
    }

    public void UpdateUI()
    {
        
        //skillLevelText.text = $"{skill.skillLevel}/{skill.skillMaxLevel}";

       // levelUpButton.interactable = skill.skillLevel < skill.skillMaxLevel;
       // levelUpButton.gameObject.SetActive(skill.isLearned);
        //learnButton.gameObject.SetActive(!skill.isLearned);
        increaseButton.gameObject.SetActive(skill.isAvailable);
        //decreaseButton.gameObject.SetActive(skill.isLearned);
        skillLevel.text = $"{skill.skillLevel} / {skill.skillMaxLevel}";
        if (skill.skillLevel == skill.skillMaxLevel)
        {
           skillLevel.color = Color.green;
        }
        else
        {
            skillLevel.color = Color.yellow;
            // Tähän skillelvel.color --> orginal color
        }
        if (skill.skillLevel == skill.skillMaxLevel)
        {
            increaseButton.gameObject.SetActive(false);
        }
        //skill.UpdateInfoText(); // Päivitä info tekstin aluksi
        UpdateSkillIconVisibility();
    }
    private void UpdateSkillIconVisibility()
    {
        
        Color iconColor = skillIcon.color;
        iconColor.a = skill.isAvailable ? 1.2f : 0.1f; // 1.0 täysin näkyvä, 0.5 haalea
        skillIcon.color = iconColor;
    }

public void OnBeginDrag(PointerEventData eventData)
{

    if (skill.isPassive)
    {

        return;
    }

    if (!skill.isLearned)
    {

        return;
    }

 
    // Siirretään ikoni canvasin päälle, jotta se näkyy päällimmäisenä
    iconRectTransform.SetAsLastSibling();

    // Piilotetaan ikoni raahaamisen ajaksi
    skillIcon.raycastTarget = false;
}

    public void OnDrag(PointerEventData eventData)
    {
    if (skill.isPassive)
    {
   
        return;
    }

    if (!skill.isLearned)
    {

        return;
    }

        // Päivitetään ikonin sijaintia raahaamisen aikana
        iconRectTransform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (skill.isPassive)
        {

            return;
        }

        if (!skill.isLearned)
        {
       
            return;
        }
        // Tarkistetaan, onko pudotettu oikeaan slottiin
        iconRectTransform.anchoredPosition = originalIconPosition;  
        // Palautetaan ikonin vuorovaikutus
        skillIcon.raycastTarget = true;
    }

    private void UpdateSkillSlot(Transform targetSlot)
    {
        Debug.Log("Päivitetään skill slot");
        // Asetetaan skill oikeaan action barin slottiin
        transform.SetParent(targetSlot); // Asetetaan uusi parent
        rectTransform.localPosition = Vector3.zero; // Asetetaan slotti oikeaan paikkaan (keskelle)

        // Varmistetaan, että skillin ikoni on näkyvissä oikeassa paikassa
        iconRectTransform.anchoredPosition = Vector2.zero;

    }
    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            float skillDamage = (skill.baseDamage + (skill.damagePerLevel * (skill.skillLevel - 1))) * playerStats.magickAttack;
            
            string skillname = skill.skillName;
            int minLvl = skill.skillLevel;
            int maxLvl = skill.skillMaxLevel;
            float manaCost = skill.manaCost;
            string info = "Deals damage " 
                + (skill.baseDamage + (skill.damagePerLevel * (skill.skillLevel - 1))) 
                + " * MATK (<color=red>" + skillDamage + "</color>)\n"
                + "Increase damage " + (100 * skill.damagePerLevel) + "% per level\n"
                + "Increase manacost <color=#1C81CF>" + skill.manaCostPerLevel + "</color> per level";

            string element = skill.element.ToString();
            float cd = skill.cooldown;
            Sprite icon = skill.skillIcon;
            skillTooltipManager.ShowTooltip(skill);
            //tooltipManager.ShowTooltip(skillname,minLvl,maxLvl,manaCost,info,element,cd,icon,Input.mousePosition);
        }
    }


    public void OnPointerUp(PointerEventData eventData)
    {
        
        skillTooltipManager.HideTooltip();
    }

}
