using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class ActionbarSlot : MonoBehaviour, IDropHandler
{
    public Skill assignedSkill = null;  // Skill, joka on liitetty tähän slottiin
    public Image skillIconImage;  // Slotin ikoni (liitetty Image-komponenttiin)
    public Button actionButton;  // Nappi, joka käyttää taitoa
    public TextMeshProUGUI manaCostText;  // Mana-kustannuksen näyttö
    public TextMeshProUGUI cooldownText;  // Cooldownin tekstinäyttö
    public Image cooldownOverlay;  // Radiaalinen cooldown-overlay
    public PlayerAttack playerAttack;
    public PlayerHealth playerHealth;
    private float cooldownTimeRemaining = 0f;  // Jäljellä oleva cooldown-aika

    void Start()
    {
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



    public void SetSkill(Skill skill)
    {
        assignedSkill = skill;
        skillIconImage.sprite = skill.skillIcon;  // Asetetaan skillin ikoni
        actionButton.onClick.AddListener(() => UseSkill());  // Lisää kuuntelija napille
        manaCostText.text = $"{skill.manaCost}";  // Näytä manakustannus
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
                Debug.LogWarning("Skill is on cooldown!");
                return;
            }

            // Käytetään taito
            Debug.Log($"Using skill: {assignedSkill.skillName}");

            // Aloita cooldown
            StartCooldown(assignedSkill.cooldown);

            // Käytä taitoa
            if (assignedSkill.spellType == SpellType.Damage)
            {
                playerAttack.Attack(assignedSkill);
            }
            else if (assignedSkill.spellType == SpellType.Heal)
            {
                playerHealth.Heal(assignedSkill.heal);
            }
        }
        else
        {
            Debug.LogWarning("No skill assigned to this slot!");
        }
        playerAttack.isCasting = false;
    }

    public void OnDrop(PointerEventData eventData)
    {
        SkillButton skillButton = eventData.pointerDrag.GetComponent<SkillButton>();
        if (skillButton != null)
        {
            SetSkill(skillButton.skill);
        }
        else
        {
            Debug.LogWarning("No skill found to drop.");
        }
    }
}
