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
        
    // Hae kaikki EnemyHealthBarBuff komponentit buffParentin lapsista (enemyBuffs)
    var existingBuffUIs = buffParent.GetComponentsInChildren<EnemyHealthBarBuff>();

    // Debuggaus: tulostetaan löytyneet komponentit
    

    foreach (var buffUI in existingBuffUIs)
    {
       
    }

    // Etsi komponentti, jonka buffName vastaa current buffin nimeä
    var existingBuffUI = existingBuffUIs.FirstOrDefault(b => b.buffName == buff.name);

    if (existingBuffUI != null && removeBuffCoroutine != null)
    {
        
        // Poistetaan vanha buff UI-elementti
        Destroy(existingBuffUI.gameObject); 
        activeBuffIcons.Remove(buff);
        StopCoroutine(removeBuffCoroutine);
        
    }
    else
    {
        
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
   
    
    // Odotetaan, että buffin kesto loppuu
    yield return new WaitForSeconds(buff.duration);
    

    // Poistetaan UI-elementti
    if (buffUIComponent != null)
    {
        
        Destroy(buffUIComponent.gameObject);
    }
    

    // Tyhjennetään viittaus poisto-korutiiniin
    removeBuffCoroutine = null;
    
    activeBuffIcons.Remove(buff);
}


    public void ShowTextForDuration(TextMeshProUGUI textElement, int amount, bool isCritical, bool isMiss, Element element)
    {
        TextMeshProUGUI newTextElement = Instantiate(textElement, combatText);
        newTextElement.gameObject.SetActive(true);

        if (isMiss)
        {
            newTextElement.text = "Miss";
            newTextElement.fontSize = 24f;
            newTextElement.color = Color.grey;
           // newTextElement.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        }
        else if (amount == 0)
        {
            newTextElement.text = "Immune";
            newTextElement.fontSize = 24f;
            newTextElement.color = Color.grey;
           // newTextElement.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        }
        else if (amount < 1)
        {
            newTextElement.text = string.Empty; // Ei näytetä mitään, jos damage on liian pieni
        }
        else
        {
            newTextElement.text = $"{amount}";

            // Fonttikoko dynaamisesti damagen perusteella
            float minFontSize = 20f;
            float maxFontSize = 80f;
            int minDamage = 1;
            int maxDamage = 10000;
            float clampedDamage = Mathf.Clamp(amount, minDamage, maxDamage);
            float normalized = (clampedDamage - minDamage) / (float)(maxDamage - minDamage);
            newTextElement.fontSize = Mathf.Lerp(minFontSize, maxFontSize, normalized);

            // Väri elementin mukaan
            newTextElement.color = GetElementColor(element);

            // Korostetaan kriittistä vielä kirkkaammalla sävyllä (valinnainen)
        if (isCritical)
        {
            newTextElement.color = new Color(1f, 0.92f, 0.016f); // Kirkas kulta/keltainen

            // Kasvata fonttikokoa isommaksi
            if (newTextElement.fontSize < 30f)
            {
                newTextElement.fontSize = 30f;
            }
            else
            {
                newTextElement.fontSize += 3f;
            }

            newTextElement.fontStyle = FontStyles.Bold;

            // Voimakkaampi musta outline
            newTextElement.outlineColor = Color.black;
            newTextElement.outlineWidth = 0.5f; // was 0.3f

            // Terävämpi ja näkyvämpi varjo (Underlay)
            newTextElement.fontMaterial = new Material(newTextElement.fontSharedMaterial);
            newTextElement.fontMaterial.EnableKeyword("UNDERLAY_ON");
            newTextElement.fontMaterial.SetColor("_UnderlayColor", Color.black);
            newTextElement.fontMaterial.SetFloat("_UnderlaySoftness", 0.2f); // vähemmän blur
            newTextElement.fontMaterial.SetFloat("_UnderlayOffsetX", 0.3f);
            newTextElement.fontMaterial.SetFloat("_UnderlayOffsetY", -0.3f);
        }

        }

        StartCoroutine(MoveTextUp(newTextElement));
        StartCoroutine(HideTextAfterDelay(newTextElement));
    }



    private IEnumerator HideTextAfterDelay(TextMeshProUGUI textElement)
    {
        // Odottaa 3 sekuntia ja piilottaa sitten tekstin
        yield return new WaitForSeconds(1f);
        
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
            float horizontalOffset = isNextRight ? 1f : -1f; // Siirtymä oikealle tai vasemmalle
            float verticalOffset = 1f; // Siirtymä ylöspäin
            Vector3 targetPosition = originalPosition + new Vector3(horizontalOffset, verticalOffset, 0);
            isNextRight = !isNextRight;

            float elapsedTime = 0;
            float duration = 0.1f; // Kesto, kuinka nopeasti teksti liikkuu

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
    private Color GetElementColor(Element element)
    {
        switch (element)
        {
            case Element.Fire:
                return new Color(1f, 0.5f, 0f); // oranssi
            case Element.Wind:
                return new Color(1f, 1f, 0.7f); // vaalea keltainen
            case Element.Water:
                return new Color(0.6f, 0.8f, 1f); // vaalea sininen
            case Element.Earth:
                return new Color(0.7f, 0.5f, 0.3f); // vaalea ruskea
            case Element.Shadow:
                return Color.black;
            case Element.Holy:
                return new Color(1f, 0.95f, 0.6f); // kirkas keltainen
            case Element.Neutral:
            default:
                return Color.white;
        }
    }

}
