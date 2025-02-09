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
            Debug.Log("CombatText löytyi onnistuneesti.");
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
        healthText.text = $"{playerHealth.currentHealth:F1} / {Mathf.RoundToInt(playerHealth.maxHealth)}";
        manaText.text = $"{playerHealth.currentMana:F1} / {Mathf.RoundToInt(playerHealth.maxMana)}";
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
public void ShowTextForDuration(TextMeshProUGUI textElement, float amount)
{
    if (textElement == playerHealthBar.takeDamageText)
    {
        
        // Luodaan uusi tekstielementti vahinkotekstille ja asetetaan se combatText-objektin lapseksi
        TextMeshProUGUI newTextElement = Instantiate(takeDamageText, combatText);
        newTextElement.gameObject.SetActive(true);
        newTextElement.text = $"{amount:F1}"; // Päivittää arvon vahinkoa varten
        newTextElement.color = Color.red; // Asetetaan väri punaiseksi vahinkoa varten

        StartCoroutine(MoveTextUp(newTextElement));
        StartCoroutine(HideTextAfterDelay(newTextElement)); // Piilottaa sen 2 sekunnin kuluttua
    }
    else
    { 
        if (textElement == hpRegenText)
        {

        TextMeshProUGUI newTextElement = Instantiate(hpRegenText, combatText);
        newTextElement.text = $"+ {amount:F1}"; // Päivittää arvon
        newTextElement.gameObject.SetActive(true);
        StartCoroutine(MoveTextUp(newTextElement));
        StartCoroutine(HideRegenAfterDelay(newTextElement));
        }
        if (textElement == manaRegenText)
        {
        TextMeshProUGUI newTextElement = Instantiate(manaRegenText, combatText);
        newTextElement.text = $"+ {amount:F1}"; // Päivittää arvon
        newTextElement.gameObject.SetActive(true);
        StartCoroutine(MoveTextUp(newTextElement));
        StartCoroutine(HideRegenAfterDelay(newTextElement));
        }

    }
}
    private IEnumerator HideRegenAfterDelay(TextMeshProUGUI textElement)
    {
        // Odottaa 3 sekuntia ja piilottaa sitten tekstin
        yield return new WaitForSeconds(1f);
        
        // Piilota teksti
        textElement.gameObject.SetActive(false);
        if (textElement != hpRegenText && textElement.transform.parent == combatText)
        {
            Destroy(textElement.gameObject);
        }
        if (textElement != manaRegenText && textElement.transform.parent == combatText)
        {
            Destroy(textElement.gameObject);
        }


    }

private IEnumerator HideTextAfterDelay(TextMeshProUGUI textElement)
{
    // Odottaa 3 sekuntia ja piilottaa sitten tekstin
    yield return new WaitForSeconds(0.45f);
    
    // Piilota teksti
    textElement.gameObject.SetActive(false);

    // Tuhotaan tekstielementti, jos se on instanssi eikä alkuperäinen (takeDamageText)
    if (textElement != takeDamageText && textElement.transform.parent == combatText)
    {
        Destroy(textElement.gameObject);
    }
}

    // Liikuttaa Take Damage -tekstiä ylöspäin hitaasti
    private IEnumerator MoveTextUp(TextMeshProUGUI textElement)
    {
        if (textElement != null)
            {
                Vector3 originalPosition = textElement.rectTransform.position;

                // Määritä offset arvot
                float horizontalOffset = isNextRight ? 4f : -4f; // Siirtymä oikealle tai vasemmalle
                float verticalOffset = 1.5f; // Siirtymä ylöspäin
                Vector3 targetPosition = originalPosition + new Vector3(horizontalOffset, verticalOffset, 0);
                isNextRight = !isNextRight;

                float elapsedTime = 0;
                float duration = 1.2f; // Kesto, kuinka nopeasti teksti liikkuu

                while (elapsedTime < duration)
                {
                    if (textElement == null)
                    {
                        yield break; // Lopetetaan, jos objekti on tuhottu
                    }

                    textElement.rectTransform.position = Vector3.Lerp(originalPosition, targetPosition, elapsedTime / duration);
                    elapsedTime += Time.deltaTime;
                    yield return null;
                }

                // Lopullinen tarkistus
                if (textElement != null)
                {
                    textElement.rectTransform.position = targetPosition;
                }
            }
        }

}
