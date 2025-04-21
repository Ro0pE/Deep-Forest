using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class PlayerHealthBar : MonoBehaviour
{
    public Image healthBar;
    public TextMeshProUGUI healthText;
    public Image manaBar;
    public TextMeshProUGUI manaText;
    public PlayerHealth playerHealth;
    public TextMeshProUGUI hpRegenText;
    public TextMeshProUGUI manaRegenText;
    public TextMeshProUGUI takeDamageText; // Alkuperäinen tekstielementti, jota käytetään mallina
    public Transform combatText; // Viittaa compaText-objektiin PlayerHealthin alla
    public Image playerAvatarBackground;
    public PlayerHealthBar playerHealthBar;
    public PlayerAttack playerAttack;
    public Image elementImage; // Elementin kuva
    public Sprite[] playerElementSprites; // Elementteihin liittyvät spritekuvat (Fire, Water, Earth, jne.)
    private static bool isNextRight = true; // Staattinen muuttuja vuorotteluun
    public PlayerStats playerStats;
    public TextMeshProUGUI playerLevel;

    void Start()
    {
        // Hae PlayerHealth-komponentti pelaajalta
        playerAttack = FindObjectOfType<PlayerAttack>();
        playerHealthBar = FindObjectOfType<PlayerHealthBar>();
        playerStats = FindObjectOfType<PlayerStats>();
        playerHealth = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHealth>();
        //combatText = playerHealth.transform.Find("CombatText"); // Hakee compaText-objektin pelaajan sisältä
        if (combatText == null)
        {
            Debug.LogError("CombatText-objektia ei löydy PlayerHealthin alta. Varmista, että objektin nimi on 'CombatText' ja että se on PlayerHealthin lapsi.");
        }
        else
        {
            
        }

        // Aluksi kaikki tekstit piiloon
        hpRegenText.gameObject.SetActive(false);
        manaRegenText.gameObject.SetActive(false);
        takeDamageText.gameObject.SetActive(false);
    }

    void Update()
    {
        // Päivitä terveyspalkki pelaajan terveyden mukaan
        float healthPercent = (float)playerHealth.currentHealth / playerHealth.maxHealth;
        healthBar.fillAmount = healthPercent;
        float manaPercent = (float)playerHealth.currentMana / playerHealth.maxMana;
        manaBar.fillAmount = manaPercent;

        // Päivitä terveyden ja manan tekstit
        healthText.text = $"{playerHealth.currentHealth} / {playerHealth.maxHealth}";
        manaText.text = $"{playerHealth.currentMana} / {playerHealth.maxMana}";
        playerLevel.text = $"{playerStats.level}";
        SetElementImage();
    }
        private void SetElementImage()
        {
        
        if (playerAttack.autoaAttackElement != null && elementImage != null)
        {
            // Aseta elementtikuvan sprite oikean elementin mukaan
            switch (playerAttack.autoaAttackElement)
            {
                case Element.Fire:
                    elementImage.sprite = playerElementSprites[0]; // Fire sprite
                    break;
                case Element.Water:
                    elementImage.sprite = playerElementSprites[1]; // Water sprite
                    break;
                case Element.Earth:
                    elementImage.sprite = playerElementSprites[2]; // Earth sprite
                    break;
                case Element.Wind:
                    elementImage.sprite = playerElementSprites[3]; // Wind sprite
                    break;
                case Element.Shadow:
                    elementImage.sprite = playerElementSprites[4]; // Shadow sprite
                    break;
                case Element.Holy:
                    elementImage.sprite = playerElementSprites[5]; // Holy sprite
                    break;
                case Element.Melee:
                    elementImage.sprite = playerElementSprites[6]; // Combat sprite
                    break;
                case Element.Ranged:
                    elementImage.sprite = playerElementSprites[7]; // Combat sprite
                    break;                    
                case Element.Defense:
                    elementImage.sprite = playerElementSprites[8]; // Defense sprite
                    break;
                case Element.Neutral:
                    elementImage.sprite = playerElementSprites[9]; // Defense sprite
                    break;      
                default:
                    elementImage.sprite = null; // Jos elementtiä ei ole, jätä kuva tyhjäksi
                    break;
            }
        }
    }

    // Tämä metodi näyttää tekstin ja piilottaa sen 2 sekunnin kuluttua
public void ShowTextForDuration(TextMeshProUGUI textElement, float amount, string skillName = null)
{
    if (amount == 0) return;

    // DAMAGE
    if (textElement == playerHealthBar.takeDamageText)
    {
        TextMeshProUGUI newTextElement = Instantiate(takeDamageText, combatText);
        newTextElement.gameObject.SetActive(true);
        newTextElement.text = $"{amount:F0}";
        newTextElement.color = Color.red;

        float damageScale = Mathf.Clamp(amount, 1f, 10000f);
        float fontSize = Mathf.Lerp(18f, 50f, (damageScale - 1f) / (10000f - 1f));
        newTextElement.fontSize = fontSize;

        if (!string.IsNullOrEmpty(skillName))
        {
            newTextElement.text = $"{newTextElement.text} <color=#FFFFFF>(</color><color=#FFA500>{skillName}</color><color=#FFFFFF>)</color>";
        }

        StartCoroutine(MoveTextUp(newTextElement, amount));
        StartCoroutine(HideTextAfterDelay(newTextElement, amount));
    }

    // HP REGEN
    else if (textElement == hpRegenText && amount > 0)
    {
        TextMeshProUGUI newTextElement = Instantiate(hpRegenText, combatText);
        newTextElement.text = $"+{amount:F0}";
        newTextElement.color = Color.green;

        float regenScale = Mathf.Clamp(amount, 1f, 10000f);
        float fontSize = Mathf.Lerp(18f, 50f, (regenScale - 1f) / (10000f - 1f));
        newTextElement.fontSize = fontSize;

        if (!string.IsNullOrEmpty(skillName))
        {
            newTextElement.text = $"<color=#FFFFFF>(</color><color=#FFA500>{skillName}</color><color=#FFFFFF>)</color> {newTextElement.text}";
        }

        newTextElement.gameObject.SetActive(true);
        StartCoroutine(MoveTextUp(newTextElement, amount));
        StartCoroutine(HideRegenAfterDelay(newTextElement, amount));
    }

    // MANA REGEN
    else if (textElement == manaRegenText && amount > 0)
    {
        TextMeshProUGUI newTextElement = Instantiate(manaRegenText, combatText);
        newTextElement.text = $"+{amount:F0}";
        newTextElement.color = new Color(0.2f, 0.6f, 1f); // Mana-väritys

        float regenScale = Mathf.Clamp(amount, 1f, 10000f);
        float fontSize = Mathf.Lerp(18f, 50f, (regenScale - 1f) / (10000f - 1f));
        newTextElement.fontSize = fontSize;

        if (!string.IsNullOrEmpty(skillName))
        {
            newTextElement.text = $"<color=#FFFFFF>(</color><color=#FFA500>{skillName}</color><color=#FFFFFF>)</color> {newTextElement.text}";
        }

        newTextElement.gameObject.SetActive(true);
        StartCoroutine(MoveTextUp(newTextElement, amount));
        StartCoroutine(HideRegenAfterDelay(newTextElement, amount));
    }
}



    private IEnumerator HideRegenAfterDelay(TextMeshProUGUI textElement, float amount)
    {
        // Skaalaa viive amountin mukaan (1 -> 0.5s, 1000 -> 2.5s)
        float duration = Mathf.Lerp(2.5f, 4.5f, Mathf.Clamp(amount, 0f, 10000f) / 10000f);

        yield return new WaitForSeconds(duration);
        
        // Piilota teksti
        textElement.gameObject.SetActive(false);

        // Tuhotaan, jos ei ole alkuperäinen prefab
        if (textElement != hpRegenText && textElement.transform.parent == combatText)
        {
            Destroy(textElement.gameObject);
        }

        if (textElement != manaRegenText && textElement.transform.parent == combatText)
        {
            Destroy(textElement.gameObject);
        }
    }


    private IEnumerator HideTextAfterDelay(TextMeshProUGUI textElement, float amount)
    {
        float duration = Mathf.Lerp(2.5f, 4.5f, Mathf.Clamp(amount, 0f, 10000f) / 10000f);
        yield return new WaitForSeconds(duration);

        textElement.gameObject.SetActive(false);

        if (textElement != takeDamageText && textElement.transform.parent == combatText)
        {
            Destroy(textElement.gameObject);
        }
    }

private IEnumerator MoveTextUp(TextMeshProUGUI textElement, float amount)
{
    if (textElement != null)
    {
        // Satunnainen lähtösiirto, jotta tekstit eivät ole täysin päällekkäin
        float initialYOffset = Random.Range(-2f, 2f);
        float initialXOffset = Random.Range(-1f, 1f);

        Vector3 originalPosition = textElement.rectTransform.position + new Vector3(initialXOffset, initialYOffset, 0f);

        float horizontalOffset = Random.Range(-2f, 2f);
        float verticalOffset = Random.Range(8f, 15f);

        float duration = 2f;
        Vector3 targetPosition = originalPosition + new Vector3(horizontalOffset, verticalOffset, 0);

        float elapsedTime = 0f;
        Color originalColor = textElement.color;

        while (elapsedTime < duration)
        {
            if (textElement == null)
                yield break;

            float t = elapsedTime / duration;
            textElement.rectTransform.position = Vector3.Lerp(originalPosition, targetPosition, t);

            if (elapsedTime > 1f)
            {
                float fadeT = (elapsedTime - 1f) / 1f;
                textElement.color = new Color(originalColor.r, originalColor.g, originalColor.b, Mathf.Lerp(1f, 0f, fadeT));
            }
            else
            {
                textElement.color = originalColor;
            }

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        textElement.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0f);
    }
}



}
