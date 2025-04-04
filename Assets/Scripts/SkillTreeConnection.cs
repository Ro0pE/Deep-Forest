using UnityEngine;
using UnityEngine.UI;

public class SkillTreeConnection : MonoBehaviour
{
    public Skill fromSkill;  // Nyt tämä on Skill-objekti
    public Skill toSkill;    // Nyt tämä on Skill-objekti
    public Transform fromSkillTransform;
    public Transform toSkillTransform;
    public RectTransform rectTransform;
    public Image lineImage;

    void Start()
    {
        // Lisätään RectTransform ja Image komponentit
        rectTransform = gameObject.AddComponent<RectTransform>();
        lineImage = gameObject.AddComponent<Image>();

        // Ladataan sprite Resources-kansiosta nimellä "SkillConnection"
        lineImage.sprite = Resources.Load<Sprite>("SkillConnection");

        // Tarkistetaan, että sprite on ladattu oikein
        if (lineImage.sprite == null)
        {
            Debug.LogError("SkillTreeConnection: SkillConnection spritea ei löytynyt Resources-kansiosta!");
        }

        // Asetetaan RectTransform oikeaan paikkaan
        rectTransform.SetParent(transform.parent, false);
        transform.SetAsLastSibling(); // Tuodaan ylimmäksi UI-hierarkiassa

        // Varmistetaan, että Z-koordinaatti on 0, jotta se ei mene väärään syvyyteen
        rectTransform.position = new Vector3(rectTransform.position.x, rectTransform.position.y, 0);

        UpdatePosition();
    }

    void Update()
    {
        UpdatePosition();  // Päivitetään viivan sijainti jatkuvasti
    }

    void UpdatePosition()
    {
        if (fromSkill == null || toSkill == null)
        {
            Debug.LogError("SkillTreeConnection: fromSkill tai toSkill puuttuu!");
            return;
        }

        // Lasketaan keskipiste ja etäisyys
        Vector2 startPos = fromSkillTransform.position; // Käytetään SkillButtonin sijaintia
        Vector2 endPos = toSkillTransform.position;   // Käytetään SkillButtonin sijaintia
        startPos.x += 46f; // Säätää viivan lähtöpistettä jne
        endPos.x -= 46f;
        Vector2 midPoint = (startPos + endPos) / 2f;
        float distance = Vector2.Distance(startPos, endPos);

        // Asetetaan RectTransform
        rectTransform.position = midPoint;
        rectTransform.sizeDelta = new Vector2(distance, 11); // Paksumpi viiva

        // Asetetaan pivot oikeaan paikkaan
        rectTransform.pivot = new Vector2(0.5f, 0.5f);

        // Käännetään viiva oikeaan kulmaan
        float angle = Mathf.Atan2(endPos.y - startPos.y, endPos.x - startPos.x) * Mathf.Rad2Deg;
        rectTransform.rotation = Quaternion.Euler(0, 0, angle);

        // Muutetaan väri riippuen siitä, onko skill saatavilla ja preSkill opittu
        if (toSkill.isAvailable)
        {
             lineImage.color = new Color(0f, 1f, 0f, 1f); // Kirkas vihreä
        }
        else
        {

            lineImage.color = Color.red;
            Color newColor = lineImage.color;
            newColor.a = 0.3f; // 30% näkyvä
            lineImage.color = newColor;

             // Väri, jos taito tai sen esitaidot eivät ole opittuja
        }
    }
}
