using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class ActionbarSlot : MonoBehaviour, IDropHandler, IPointerDownHandler, IPointerUpHandler
{
    public Skill assignedSkill; // Skill, joka on liitetty tähän slottiin
    public Image skillIconImage;  // Slotin ikoni (liitetty Image-komponenttiin)
    public Sprite defaultSkillIcon;
    public Button actionButton;  // Nappi, joka käyttää taitoa
    public TextMeshProUGUI manaCostText;  // Mana-kustannuksen näyttö
    public TextMeshProUGUI cooldownText;  // Cooldownin tekstinäyttö
    public Image cooldownOverlay;  // Radiaalinen cooldown-overlay
    public PlayerAttack playerAttack;
    public PlayerHealth playerHealth;
    private float cooldownTimeRemaining = 0f;  // Jäljellä oleva cooldown-aika
    public SkillTooltipManager skillTooltipManager;

    void Start()
    {
        skillTooltipManager = FindObjectOfType<SkillTooltipManager>();
        manaCostText.text = "";
        cooldownText.text = "";
        if (cooldownOverlay != null)
            cooldownOverlay.fillAmount = 0f;  // Piilota alussa

        playerAttack = FindObjectOfType<PlayerAttack>();
        playerHealth = FindObjectOfType<PlayerHealth>();
    }

    void Update()
    {
        if (cooldownTimeRemaining > 0)
        {
            cooldownTimeRemaining -= Time.deltaTime;
            cooldownTimeRemaining = Mathf.Max(cooldownTimeRemaining, 0); // Varmista, ettei mene negatiiviseksi

            if (cooldownOverlay != null)
            {
                cooldownOverlay.fillAmount = cooldownTimeRemaining / assignedSkill.cooldown;
            }

            if (cooldownTimeRemaining == 0 && cooldownText != null)
            {
                cooldownText.text = "Ready!";
            }
        }
        else
        {
            if (cooldownOverlay != null)
            {
                cooldownOverlay.fillAmount = 0f; // Piilota overlay
            }
        }

        // Päivitä manakustannus normaalisti
        if (assignedSkill == null)
        {
            manaCostText.text = "";
            cooldownText.text = "";
        }
        else
        {
            manaCostText.text = $"{assignedSkill.manaCost}";
        }
    }

    public void ClearSlot()
    {
        assignedSkill = null;
        skillIconImage.sprite = defaultSkillIcon;  // Poista ikoni
        manaCostText.text = "";
        cooldownText.text = "";
        cooldownTimeRemaining = 0;
        
        if (cooldownOverlay != null)
            cooldownOverlay.fillAmount = 0f;  // Piilota cooldown

        actionButton.onClick.RemoveAllListeners();  // Poista vanhat eventit
    }


    public void SetSkill(Skill skill)
    {
       
        if (!skill.isLearned || skill.isPassive)
        {
            return;
        }
        else
        {
        assignedSkill = skill;
        skillIconImage.sprite = skill.skillIcon;  // Asetetaan skillin ikoni
        actionButton.onClick.AddListener(() => UseSkill());  // Lisää kuuntelija napille
        manaCostText.text = $"{skill.manaCost}";  // Näytä manakustannus
        cooldownTimeRemaining = 0;
        }
    }

    public void StartCooldown(float cooldownTime)
    {
        cooldownTimeRemaining = cooldownTime;
        if (cooldownOverlay != null)
            cooldownOverlay.fillAmount = 1f;  // Aloita täynnä
    }

    public void UseSkill()
    {
        if (assignedSkill != null)
        {
            if (cooldownTimeRemaining > 0)
            {
                Debug.Log("skill on CD");
                Debug.LogWarning("Skill is on cooldown!");
                return;
            }

            // Käytetään taito
 

            // Aloita cooldown
            Debug.Log("Strting cooldown");
            StartCooldown(assignedSkill.cooldown);

            // Käytä taitoa
            if (assignedSkill.spellType == SpellType.Damage)
            {
                Debug.Log("Skill lähtee " + assignedSkill.skillName);
                StartCoroutine(playerAttack.Attack(assignedSkill));
            }
            else if (assignedSkill.spellType == SpellType.Heal)
            {
                playerHealth.Heal(assignedSkill.heal);
            }
            else if (assignedSkill.spellType == SpellType.Buff)
            {
                StartCoroutine(playerAttack.Buff(assignedSkill));
            }
        }
        else
        {
            Debug.LogWarning("No skill assigned to this slot!");
        }
    
    }

    public void OnDrop(PointerEventData eventData)
    {
        GameObject targetObject = eventData.pointerEnter;
        Debug.Log("skill tippuu " + targetObject);
        SkillButton skillButton = eventData.pointerDrag.GetComponent<SkillButton>();
        if (skillButton != null)
        {
            if (cooldownTimeRemaining > 0)
            {
                Debug.Log("Skill is on cd");
            }
            else
            {
           
            SetSkill(skillButton.skill);
            }

        }
        else
        {
            Debug.LogWarning("No skill found to drop.");
        }
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("Pointer down");
        
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            if (assignedSkill != null && !string.IsNullOrEmpty(assignedSkill.skillName))
            {
                Debug.Log("skill " + assignedSkill.skillName);
                skillTooltipManager.ShowTooltip(assignedSkill);
            }
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        
        skillTooltipManager.HideTooltip();
    }
}
