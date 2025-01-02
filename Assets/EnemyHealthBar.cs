using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class EnemyHealthBar : MonoBehaviour
{
    public Image healthBar; // Terveyspalkki
    public TextMeshProUGUI monsterName;
    public TextMeshProUGUI healthText; // Tekstikenttä terveyden näyttämiseksi
    public EnemyHealth enemyHealth; // Viittaus vihollisen terveyteen
    public TextMeshProUGUI enemyTakeDamageText;
    public Transform combatText; // Viittaa combatText-objektiin PlayerHealthin alla
    private static bool isNextRight = true; // Staattinen muuttuja vuorotteluun
    Camera targetCamera;
    public Transform healthBarParent; // Viittaus healthBarin parentiin (Canvas)
    public Camera playerCamera; // Viittaus pelaajan kameraan, annetaan Inspectorissa
    public Vector3 healthBarOffset = new Vector3(0f, 6f, 0f); // Terveyspalkin offset vihollisen päältä




    void Start()
    {
        // Hae EnemyHealth-komponentti viholliselta
        targetCamera = FindObjectOfType<Camera>();
        enemyHealth = GetComponentInParent<EnemyHealth>();
        enemyTakeDamageText.gameObject.SetActive(false);

        // Hae healthBarin parent-objekti (Canvas)
        
    }

    void Update()
    {
        if (enemyHealth != null)
        {
            // Päivitetään terveyspalkki
            float healthPercent = (float)enemyHealth.currentHealth / enemyHealth.maxHealth;
            healthBar.fillAmount = healthPercent;
            monsterName.text = enemyHealth.monsterName;
            healthText.text = $"{enemyHealth.currentHealth:F1} / {enemyHealth.maxHealth:F1}";

            // Hae vihollisen maailman koordinaatit
            Vector3 worldPosition = enemyHealth.transform.position + healthBarOffset;

            // Siirretään terveyspalkki oikealle sijainnille
            healthBarParent.position = worldPosition;

            // Estetään pyöriminen, jotta terveyspalkki ei käänny väärin
            healthBarParent.rotation = Quaternion.identity;

            // Aseta terveyspalkki aina kohti pelaajan kameraa
            Vector3 directionToPlayerCamera = playerCamera.transform.position - healthBarParent.position;
            healthBarParent.forward = directionToPlayerCamera;
            healthBarParent.Rotate(0, 180, 0);
        }
    }




    public void ShowTextForDuration(TextMeshProUGUI textElement, float amount, bool isCritical, bool isMiss)
    {
        // Luodaan uusi tekstielementti vahinkotekstille ja asetetaan se combatText-objektin lapseksi
        TextMeshProUGUI newTextElement = Instantiate(textElement, combatText);
        newTextElement.gameObject.SetActive(true);

        if (isMiss)
        {
            newTextElement.text = "Miss";
            newTextElement.color = Color.grey;
            newTextElement.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        }
        else if (amount <= 0)
        {
            newTextElement.text = "Immune";
        }
        else if (isCritical)
        {
            newTextElement.text = $"{amount:F1}";
            newTextElement.color = Color.yellow;
            newTextElement.fontSize = 35f;
        }
        else
        {
            newTextElement.text = $"{amount:F1}";
        }

        // Aloitetaan coroutine-funktiot
        StartCoroutine(MoveTextUp(newTextElement));
        StartCoroutine(HideTextAfterDelay(newTextElement)); // Piilottaa sen 3 sekunnin kuluttua
    }

    private IEnumerator HideTextAfterDelay(TextMeshProUGUI textElement)
    {
        // Odottaa 3 sekuntia ja piilottaa sitten tekstin
        yield return new WaitForSeconds(3f);
        
        // Piilota teksti
        textElement.gameObject.SetActive(false);

        // Tuhotaan tekstielementti, jos se on instanssi eikä alkuperäinen (enemyTakeDamageText)
        if (textElement != enemyTakeDamageText && textElement.transform.parent == combatText)
        {
            Destroy(textElement.gameObject);
        }
    }

    public IEnumerator MoveTextUp(TextMeshProUGUI textElement)
    {
        // Tarkistetaan ensin, että textElement ei ole null
        if (textElement != null)
        {
            Vector3 originalPosition = textElement.rectTransform.position;

            // Määritä offset arvot
            float horizontalOffset = isNextRight ? 8f : -8f; // Siirtymä oikealle tai vasemmalle
            float verticalOffset = 4f; // Siirtymä ylöspäin
            Vector3 targetPosition = originalPosition + new Vector3(horizontalOffset, verticalOffset, 0);
            isNextRight = !isNextRight;

            float elapsedTime = 0;
            float duration = 2f; // Kesto, kuinka nopeasti teksti liikkuu

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
