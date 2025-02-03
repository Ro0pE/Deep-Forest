using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq; // Lisää tämä rivin yläosaan
using System.Collections;
using System.Collections.Generic;

public class EnemyHealthBar : MonoBehaviour
{
    public Transform buffParent;
    public GameObject miniBuffPrefab;
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
    private Coroutine removeBuffCoroutine;
    public List<Buff> activeBuffIcons = new List<Buff>();




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
            int healthPercentRounded = Mathf.RoundToInt(healthPercent * 100);
            healthText.text = $"{healthPercentRounded}%";
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

    public void AddBuffIcon(Buff buff)
    {
        Debug.Log("Adding buff to " + enemyHealth);
    // Hae kaikki EnemyHealthBarBuff komponentit buffParentin lapsista (enemyBuffs)
    var existingBuffUIs = buffParent.GetComponentsInChildren<EnemyHealthBarBuff>();

    // Debuggaus: tulostetaan löytyneet komponentit
    Debug.Log($"Found {existingBuffUIs.Length} buff UIs.");

    foreach (var buffUI in existingBuffUIs)
    {
        Debug.Log($"Found buff UI with name: {buffUI.buffName} from  {enemyHealth}");
    }

    // Etsi komponentti, jonka buffName vastaa current buffin nimeä
    var existingBuffUI = existingBuffUIs.FirstOrDefault(b => b.buffName == buff.name);

    if (existingBuffUI != null && removeBuffCoroutine != null)
    {
        Debug.Log("Buff found, removing the old one.");
        // Poistetaan vanha buff UI-elementti
        Destroy(existingBuffUI.gameObject); 
        activeBuffIcons.Remove(buff);
        StopCoroutine(removeBuffCoroutine);
        Debug.Log("Vanha buff poistettu ja korutiini keskeytetty.");
    }
    else
    {
        Debug.Log("Buff lisätään ekalla kerralla");
    }

    // Luodaan uusi buff UI
    GameObject newBuffUI = Instantiate(miniBuffPrefab, buffParent);
    EnemyHealthBarBuff buffUIComponent = newBuffUI.GetComponent<EnemyHealthBarBuff>();

    // Asetetaan buffin ikoni ja päivitetään UI
    buffUIComponent.buffIcon.sprite = buff.buffIcon;
    buffUIComponent.buffName = buff.name;  // Aseta buffName tähän
    buffUIComponent.Initialize(buff);

    // Liitetään UI-komponentti buffiin, jos tarpeen
    activeBuffIcons.Add(buff);

    // Käynnistetään uusi korutiini
    removeBuffCoroutine = StartCoroutine(RemoveBuffUI(buff, buffUIComponent));
    }


public IEnumerator RemoveBuffUI(Buff buff, EnemyHealthBarBuff buffUIComponent)
{
    Debug.Log("REMOVE BUFF UI ICON STARTED" + enemyHealth);
    
    // Odotetaan, että buffin kesto loppuu
    yield return new WaitForSeconds(buff.duration);
    

    // Poistetaan UI-elementti
    if (buffUIComponent != null)
    {
        
        Destroy(buffUIComponent.gameObject);
    }
    Debug.Log("buff ui comp null");

    // Tyhjennetään viittaus poisto-korutiiniin
    removeBuffCoroutine = null;
    Debug.Log("Removing buff from "  + enemyHealth);
    activeBuffIcons.Remove(buff);
}



    public void ShowTextForDuration(TextMeshProUGUI textElement, float amount, bool isCritical, bool isMiss)
    {
        // Luodaan uusi tekstielementti vahinkotekstille ja asetetaan se combatText-objektin lapseksi
        TextMeshProUGUI newTextElement = Instantiate(textElement, combatText);
        newTextElement.gameObject.SetActive(true);
        string formattedAmount = Mathf.Abs(amount).ToString();
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
            newTextElement.text = $"{formattedAmount:F1}";
            newTextElement.color = Color.yellow;
            newTextElement.fontSize = 35f;
        }
        else
        {
            
            newTextElement.text = $"{formattedAmount:F1}";
        }

        // Aloitetaan coroutine-funktiot
        StartCoroutine(MoveTextUp(newTextElement));
        StartCoroutine(HideTextAfterDelay(newTextElement)); // Piilottaa sen 3 sekunnin kuluttua
    }

    private IEnumerator HideTextAfterDelay(TextMeshProUGUI textElement)
    {
        // Odottaa 3 sekuntia ja piilottaa sitten tekstin
        yield return new WaitForSeconds(1.5f);
        
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
            float horizontalOffset = isNextRight ? 2f : -2f; // Siirtymä oikealle tai vasemmalle
            float verticalOffset = 1f; // Siirtymä ylöspäin
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
