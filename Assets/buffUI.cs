using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.UI;

public class buffUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public TextMeshProUGUI buffDuration; // Buffin kesto
    public TextMeshProUGUI buffStacks; // Buffin kesto
    public Image buffIcon; // Buffin kuvake

    public GameObject tooltipPanel;       // Tooltip-paneli
    public TextMeshProUGUI buffNameText;  // Tooltipin nimi
    public TextMeshProUGUI buffEffectText;// Tooltipin vaikutus

    void Start()
    {
        tooltipPanel.SetActive(false);
    }

    public void UpdateDuration(float duration, float stacks)
    {
        buffDuration.text = $"{duration:0.0}s";
        buffStacks.text = stacks > 1 ? $"{stacks}" : "";
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        tooltipPanel.SetActive(true); // Näytä tooltip
        Debug.Log($"Mouse entered: {gameObject.name}");
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        tooltipPanel.SetActive(false); // Piilota tooltip
        Debug.Log($"Mouse exited: {gameObject.name}");
    }
    public void Initialize(Buff buff)
    {
        // Tooltip tiedot
        buffNameText.text = buff.name;
        buffEffectText.text = buff.effectText;

        // UI:n kuvake ja kesto
        buffIcon.sprite = buff.buffIcon;
        UpdateDuration(buff.duration, buff.stacks);
    }
}
