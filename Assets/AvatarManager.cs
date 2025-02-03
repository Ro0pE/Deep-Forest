using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using TMPro;
using UnityEngine.UI;

public class AvatarManager : MonoBehaviour
{
    public Image avatarImage; // Vihollisen avatarin kuva
    public Image healthBar; // Terveyspalkki
    public Image elementImage; // Elementin kuva
    public TextMeshProUGUI takeDamageText;
    public TextMeshProUGUI healthText; // Tekstikenttä terveyden näyttämiseksi
    public TextMeshProUGUI monsterLevelText;
    public TextMeshProUGUI monsterNameText;
    public GameObject monsterIconPanel;
    public EnemyHealth enemyHealth; // Viittaus vihollisen terveyteen
    public GameObject avatarPanel; // Paneli, jossa kaikki näkyvät (kuva, palkki, teksti)
    public PlayerStats playerStats;
    public GameObject enemyCastBarPanel;
    public GameObject enemyBuffPanel;
    public GameObject buffPrefab;
    public Transform buffParent; // UI Parent for buffs (optional)

    // Varmistetaan, että paneli on piilotettu alussa
    void Start()
    {
        playerStats = FindObjectOfType<PlayerStats>();
        if (avatarPanel != null && enemyHealth == null)
        {
            avatarPanel.SetActive(false); // Piilotetaan avatar alussa, jos ei ole valittua vihollista
        }
        monsterIconPanel.SetActive(false);
        enemyCastBarPanel.SetActive(false);
    }

    public void Update()
    {
        if (enemyHealth != null)
        {
            // Näytetään avatarPanel, jos vihollinen on aktiivinen ja elossa
            if (enemyHealth.currentHealth > 0)
            {
                if (avatarPanel != null && !avatarPanel.activeSelf)
                {
                    avatarPanel.SetActive(true); // Näytetään paneli, kun vihollinen on valittu ja elossa
                    
                }

                UpdateHealthBar();
                UpdateMonsterText();
            }
            else
            {
                // Piilotetaan paneli, jos vihollinen on kuollut
                if (avatarPanel != null)
                {
                    UpdateHealthBar();
                    avatarPanel.SetActive(false); // Piilotetaan paneli, jos vihollinen kuolee
                }
            }
        }
        else
        {
            // Jos ei ole aktiivista vihollista, piilotetaan avatar
            if (avatarPanel != null)
            {
                avatarPanel.SetActive(false); // Piilotetaan paneli, jos ei ole valittua vihollista
            }
        }
    }
        public void DisplayDamage(float damage)
        {
        StartCoroutine(ShowDamageText(damage));
        }
        private IEnumerator ShowDamageText(float damage)
        {
       
        if (takeDamageText != null)
        {
            if (damage <= 0)
            {
            takeDamageText.text = "Immune";
            takeDamageText.color = Color.white;
            takeDamageText.gameObject.SetActive(true);

            } 
            else
            {
            takeDamageText.color = Color.white;
            takeDamageText.text = $"{damage:F1}";
            takeDamageText.gameObject.SetActive(true);          
            }
           


            // Animaation kesto tai viive
            yield return new WaitForSeconds(1.5f);

            // Piilotetaan teksti
            takeDamageText.gameObject.SetActive(false);
        }
        else
        {
            Debug.Log("TakedamageTExt on null");
        }
        }

    // Metodi, joka asettaa elementin kuvan sen mukaan, mitä elementtiä vihollinen edustaa
    private void SetElementImage()
    {
        
        if (enemyHealth != null && elementImage != null)
        {
            // Aseta elementtikuvan sprite oikean elementin mukaan
            switch (enemyHealth.enemyElement)
            {
                case Element.Fire:
                    elementImage.sprite = enemyHealth.elementSprites[0]; // Fire sprite
                    break;
                case Element.Water:
                    elementImage.sprite = enemyHealth.elementSprites[1]; // Water sprite
                    break;
                case Element.Earth:
                    elementImage.sprite = enemyHealth.elementSprites[2]; // Earth sprite
                    break;
                case Element.Wind:
                    elementImage.sprite = enemyHealth.elementSprites[3]; // Wind sprite
                    break;
                case Element.Shadow:
                    elementImage.sprite = enemyHealth.elementSprites[4]; // Shadow sprite
                    break;
                case Element.Holy:
                    elementImage.sprite = enemyHealth.elementSprites[5]; // Holy sprite
                    break;
                case Element.Melee:
                    elementImage.sprite = enemyHealth.elementSprites[6]; // Combat sprite
                    break;
                case Element.Ranged:
                    elementImage.sprite = enemyHealth.elementSprites[7]; // Combat sprite
                    break;
                case Element.Defense:
                    elementImage.sprite = enemyHealth.elementSprites[8]; // Defense sprite
                    break;
                default:
                    elementImage.sprite = null; // Jos elementtiä ei ole, jätä kuva tyhjäksi
                    break;
            }
        }
    }

public void AssignEnemy(EnemyHealth newEnemyHealth, Sprite newAvatarImage)
{
    // Jos vaihdetaan vihollista, piilotetaan vanhan vihollisen avatar ja poistetaan vanhat buffit
    if (enemyHealth != null && enemyHealth != newEnemyHealth)
    {
        // Piilotetaan vanhan vihollisen avatar
        if (avatarPanel != null && avatarPanel.activeSelf)
        {
            avatarPanel.SetActive(false);
        }

        // Poistetaan kaikki edelliset buffit UI:sta
        foreach (Transform child in buffParent)
        {
            Destroy(child.gameObject); // Poistetaan kaikki lapsielementit (buffit)
        }
    }

    // Asetetaan uusi vihollinen
    enemyHealth = newEnemyHealth;

    if (enemyHealth != null)
    {
        // Päivitetään elementtikuvan sprite
        SetElementImage();

        // Jos avatarPanel ei ole aktiivinen, ei päivitetä UI:tä heti
        if (avatarPanel != null && !avatarPanel.activeSelf)
        {
            avatarPanel.SetActive(true); // Aktivoi paneli vain tarvittaessa
        }

        if (avatarImage != null)
        {
            avatarImage.sprite = newAvatarImage; // Asetetaan uusi avatar-kuva
            avatarImage.enabled = true; // Varmistetaan, että kuva näkyy
        }

        UpdateHealthBar(); // Päivitetään terveys

        // Tarkistetaan ja lisätään aktiiviset buffit avatarin alle
        EnemyBuffManager enemyBuffManager = newEnemyHealth.GetComponent<EnemyBuffManager>();
        if (enemyBuffManager != null)
        {
            
            foreach (Buff buff in enemyBuffManager.activeBuffs)
            {
                Debug.Log("Haetaan buffin tiedot " + buff.name);
               // CreateBuffUI(buff); // Luodaan UI buffit
            }
        }
    }
}



    public void CreateBuffUI(Buff buff)
    {
            EnemyBuffManager buffManager = enemyHealth.GetComponent<EnemyBuffManager>();
            if (buffManager == null)
            {
                Debug.LogError("EnemyBuffManager component not found on the enemy! ENWMY: " + enemyHealth);
                
            }

            // Tarkista, onko buffi jo aktiivinen
            Buff existingBuff = buffManager.activeBuffs.Find(b => b.name == buff.name);
          /*  if (existingBuff != null)
            {
                Debug.Log($"Buff {buff.name} already exists on this enemy. Updating duration instead.");
                //existingBuff.duration = Mathf.Max(existingBuff.duration, buff.duration); // Päivitä kesto
                return; // Ei luoda uutta buffia
            }*/
        if (buffPrefab != null && buffParent != null)
        {
            Debug.Log("Luodaan avatariin buff ui");
            EnemyHealthBar enemyHealthBar = enemyHealth.GetComponentInChildren<EnemyHealthBar>();
            enemyHealthBar.AddBuffIcon(buff);
            // Luo UI-elementti buffille
            GameObject newBuffUI = Instantiate(buffPrefab, buffParent);
            buffUI buffUIComponent = newBuffUI.GetComponent<buffUI>();
            
            // Asetetaan buffin ikoni
            buffUIComponent.buffIcon.sprite = buff.buffIcon;

            // Päivitetään buffin kesto ja pinojen määrä
            buffUIComponent.UpdateDuration(buff.duration, buff.stacks);
            
            // Alustetaan UI
            buffUIComponent.Initialize(buff);
            
            // Liitetään buffin UI komponentti buffiin
            buff.uiComponent = buffUIComponent;
        }
    }

    private void UpdateHealthBar()
    {
        // Päivitetään terveyspalkki ja teksti
        if (enemyHealth != null)
        {
            float healthPercent = (float)enemyHealth.currentHealth / enemyHealth.maxHealth;
            if (healthBar != null)
            {
                healthBar.fillAmount = healthPercent;
            }

            if (healthText != null)
            {
                healthText.text = $"{enemyHealth.currentHealth:F1} / {enemyHealth.maxHealth:F1}";
            }
        }
    }
    public void UpdateMonsterText()
    {
        
            // Aseta tekstiksi vihollisen taso
    monsterLevelText.text = $"{enemyHealth.monsterLevel}";
    monsterNameText.text = $"{enemyHealth.monsterName}";

    // Laske tasoero
    int levelDifference = playerStats.level - enemyHealth.monsterLevel;

    // Värjäyksen määrittäminen tasoeron perusteella
    if (levelDifference >= -3 && levelDifference <= 3)
    {
        // Tasoero on -3 ja 3 välillä, käytetään vihreää väriä
        monsterLevelText.color = new Color(0f, 1f, 0f); // Vihreä
        monsterIconPanel.SetActive(false); // Piilotetaan pääkallo
    }
    else if (levelDifference >= -5 && levelDifference < -3)
    {
        // Tasoero on -5 ja -3 välillä, käytetään keltaista väriä
        monsterLevelText.color = new Color(1f, 1f, 0f); // Keltainen
        monsterIconPanel.SetActive(false); // Piilotetaan pääkallo
    }
    else if (levelDifference >= -7 && levelDifference < -5)
    {
        // Tasoero on -7 ja -5 välillä, käytetään oranssia väriä
        monsterLevelText.color = new Color(1f, 0.647f, 0f); // Oranssi
        monsterIconPanel.SetActive(false); // Piilotetaan pääkallo
    }
    else if (levelDifference >= -9 && levelDifference < -7)
    {
        // Tasoero on -9 ja -7 välillä, käytetään punaista väriä
        monsterLevelText.color = new Color(1f, 0f, 0f); // Punainen
        monsterIconPanel.SetActive(false); // Piilotetaan pääkallo
    }
    else if (levelDifference < -9)
    {
        // Tasoero on alle -9, näyttöön tulee pääkallo ja punainen väri
        monsterLevelText.color = new Color(1f, 0f, 0f); // Punainen
        monsterIconPanel.SetActive(true); // Näytetään pääkallo
    }
    else
    {
        // Muu tilanne (positiiviset erot ovat pelaajan eduksi, käytetään vihreää väriä)
        monsterLevelText.color = new Color(0f, 1f, 0f); // Vihreä
        monsterIconPanel.SetActive(false); // Piilotetaan pääkallo
    }


        
    }
}
