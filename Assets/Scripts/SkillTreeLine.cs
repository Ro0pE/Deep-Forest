using UnityEngine;
using UnityEngine.UI;


public class SkillTreeLine : MonoBehaviour
{
    public RectTransform skill;  // Kohdeskilli
    public RectTransform preskill; // Vaadittu skilli
    private RectTransform line;
    private Image lineImage;

    public Color lockedColor = Color.gray; // Väri, jos skilli ei ole avattu
    public Color unlockedColor = Color.green; // Väri, kun skilli avataan
    public float animationTime = 0.5f; // Animaation kesto

    void Start()
    {
        line = GetComponent<RectTransform>();
        lineImage = GetComponent<Image>();
        lineImage.color = lockedColor; // Aluksi harmaa viiva

        UpdateLine();
    }

    void UpdateLine()
    {
        Vector2 startPos = skill.anchoredPosition;
        Vector2 endPos = preskill.anchoredPosition;
        Vector2 difference = endPos - startPos;

        line.anchoredPosition = startPos + (difference / 2);
        line.sizeDelta = new Vector2(0, line.sizeDelta.y); // Aloitetaan viiva 0-pituisena
        line.localRotation = Quaternion.Euler(0, 0, Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg);
    }

    public void UnlockSkill()
    {
        Vector2 startPos = skill.anchoredPosition;
        Vector2 endPos = preskill.anchoredPosition;
        float distance = Vector2.Distance(startPos, endPos);

        lineImage.color = unlockedColor;
        line.sizeDelta = new Vector2(0, line.sizeDelta.y);
       // line.DOSizeDelta(new Vector2(distance, line.sizeDelta.y), animationTime).SetEase(Ease.OutQuad);
    }
}
