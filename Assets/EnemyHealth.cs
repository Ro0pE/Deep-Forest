using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.AI;
using System.Collections.Generic;
using TMPro;



public enum Element
{
    Fire,
    Water,
    Earth,
    Wind,
    Shadow,
    Holy,
    Melee,
    Ranged,
    Defense,
    Neutral
    
}

public class EnemyHealth : MonoBehaviour
{
    public GameObject targetIndicator; // Viittaus kohteen merkkiin (punainen rinkula)
    public string monsterName = "";
    public int monsterLevel = 0;
    public Element enemyElement;
    public Sprite[] elementSprites; // Elementteihin liittyvät spritekuvat (Fire, Water, Earth, jne.)
    public Dictionary<Element, float> damageModifiers = new Dictionary<Element, float>();
    public float maxHealth = 100f; // Maksimikestopisteet
    public float currentHealth; // Nykyiset kestopisteet
    public int experiencePoints = 50; // Kokemuspisteet, jotka vihollinen antaa
    public bool isDead = false;
    public bool isInterrupted = false;
    public Animator animator;
    public PlayerAttack playerAttack;
    public GameObject healthBar; // Lisää tämä
    public float critMultiplier = 2f;
    AvatarManager avatarManager;
    private Vector3 spawnPosition; // Tallentaa karhun alkuperäisen sijainnin
    private Vector3 deathPosition;
    float finalDamage;
    public NavMeshAgent agent;

    public List<Item> lootItems; // Lista loot-esineistä
    public List<Item> droppedItems = new List<Item>(); // Lista esineistä, jotka oikeasti tippuvat


        [Header("Loot UI")]
    public GameObject lootWindow; // Loot window -objekti
    public EnemyHealth trackedBearHealth;
    public Transform lootBackground; // Loot Background
    public List<Button> lootButtons; // Painikkeet lootille
    public ItemDatabase itemDatabase;  // Viittaus ItemDatabasee
    public PlayerStats playerStats;
    public Inventory playerInventory;
    protected virtual string PrefabPath => "BrownBear"; // Oletuspolku

    public Sprite enemySprite; // Julkinen kenttä, jonka aliluokat voivat määrittää
    private const int MaxHitChance = 95;
    private const int MinHitChance = 5;
    public bool isMiss = false;
    public bool isHealthBarActive;
    public bool isStunned = false;
    public EnemyAI enemyAI;
    public PlayerMovement playerMovement;
    



    public virtual void Start()
    {
        playerMovement = FindObjectOfType<PlayerMovement>();
        agent = GetComponent<NavMeshAgent>();
        playerStats = FindObjectOfType<PlayerStats>();
        avatarManager = FindObjectOfType<AvatarManager>();
        //enemySprite = Resources.Load<Sprite>("DefaultEnemyAvatar");
        playerAttack = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerAttack>();
        playerInventory = FindObjectOfType<Inventory>();
        currentHealth = maxHealth;
        animator = GetComponent<Animator>();
        itemDatabase = FindObjectOfType<ItemDatabase>();
       
        
        spawnPosition = transform.position; // Tallentaa alkuperäisen sijainnin

        //HideHealthBar(); // Piilota health bar alussa
        lootWindow.SetActive(false);
        Button closeButton = lootWindow.transform.Find("CloseButton").GetComponent<Button>();
        targetIndicator.SetActive(false);
        
        if (closeButton != null)
        {
            closeButton.onClick.AddListener(HideLootWindow);
        }
        else
        {
            Debug.LogError("CloseButton ei löytynyt lootWindowin sisältä!");
        }
    }

    public void Update()
    {
    if (playerMovement.showEnemyHealthBar)
    {
        ShowHealthBar();
    }
    else
    {
        HideHealthBar();
    }
    }
    public void createLoot()
    {
        foreach (Item item in lootItems)
        {
            Item tempItem = itemDatabase.GetItemByName(item.itemName);
            item.icon = tempItem.icon;

        }

    }



    private void ShowLootWindow()
    {
      /*  if (lootWindow == null || lootBackground == null || lootButtons == null || lootButtons.Count == 0)
        {
            Debug.LogError("Loot UI -elementit eivät ole asetettu oikein!");
            return;
        }*/
        if (lootWindow != null)
        {
                    lootWindow.SetActive(true);
        }



        for (int i = 0; i < lootButtons.Count; i++)
        {
            if (i < droppedItems.Count) // Tarkistetaan, onko loottiesineitä riittävästi
            {
                lootButtons[i].gameObject.SetActive(true); // Näytä painike, jos lootti on olemassa

                // Jos käytössä on TextMeshPro teksti
                TMP_Text buttonText = lootButtons[i].GetComponentInChildren<TMP_Text>();
                if (buttonText != null)
                {
                    buttonText.text = droppedItems[i].itemName;
                }
                else
                {
                    Debug.LogError("TMP_Text-komponenttia ei löydy painikkeesta!");
                }

                // Asetetaan painikkeen kuva esineen spriten mukaiseksi
                Image buttonImage = lootButtons[i].GetComponent<Image>();
                if (buttonImage != null && droppedItems[i].icon != null)
                {
                    Debug.Log("Dropped item" + droppedItems[i].itemName);
                    Item tempItem = itemDatabase.GetItemByName(droppedItems[i].itemName);
                    buttonImage.sprite = tempItem.icon;
                }
                else
                {
                    Debug.LogError("icon ei löydy tai Image-komponentti puuttuu!");
                }

                int index = i; // Suljetaan indeksi lambdaan
                lootButtons[i].onClick.RemoveAllListeners();
                lootButtons[i].onClick.AddListener(() => CollectItem(index));
            }
            else
            {
                lootButtons[i].gameObject.SetActive(false); // Piilotetaan painike, jos lootti puuttuu
            }
        }
    }

private void CollectItem(int index)
{
    if (index < droppedItems.Count)
    {
        Item item = droppedItems[index];

        // Tarkistetaan, onko esine jo inventaariossa
        Item existingItem = playerInventory.items.Find(i => i.itemName == item.itemName && i.isStackable);

        if (existingItem != null)
        {
            // Jos esine on stackable ja löytyy jo inventaariosta, lisää määrää
            existingItem.quantity += item.quantity;

            // Poistetaan esine loot-windowista, koska se lisätty inventaariossa
            droppedItems.RemoveAt(index);

            // Päivitetään loot window
            ShowLootWindow();

            // Tarkista, onko loot-lista tyhjä
            if (droppedItems.Count == 0)
            {
                DestroyLootWindow(); // Suljetaan loot-ikkuna, kun kaikki loot on kerätty
            }
        }
        else
        {
            // Jos esine ei ole stackable tai sitä ei löydy inventaariosta
            if (playerInventory.items.Count < playerInventory.maxItems)
            {
                playerInventory.AddItem(item);

                // Siirretään OnItemCollected tännekin, jotta quest päivittyy myös silloin

                // Poistetaan esine loot-windowista vain, jos lisäys onnistui
                droppedItems.RemoveAt(index);

                // Päivitetään loot window
                ShowLootWindow();

                // Tarkista, onko loot-lista tyhjä
                if (droppedItems.Count == 0)
                {
                    DestroyLootWindow(); // Suljetaan loot-ikkuna, kun kaikki loot on kerätty
                }
            }
            else
            {
                Debug.LogWarning("Inventaario täynnä! Ei voida lisätä: " + item.itemName);
            }
        }
        if (playerInventory.items.Count < playerInventory.maxItems)
        {

        }
        else
        {
            Debug.Log("invetory full, Cant add quest item");
        }
        QuestManager questManager = FindObjectOfType<QuestManager>();
        if (questManager != null)
        {
            Debug.Log("Quest manager pitäs updatee");
            // Tämän karhun tappaminen lisää progression Quest "DamnBearsQuestID" tavoitteelle
            questManager.UpdateCollectQuestProgress(item);
            
            
        }
    }

}


    public void HideLootWindow()
    {
        lootWindow.SetActive(false);
    }


    public void DestroyLootWindow()
    {
        
        if (trackedBearHealth != null && trackedBearHealth.lootWindow != null)
        {
            
            Destroy(trackedBearHealth.lootWindow);
            trackedBearHealth.lootWindow = null;
        }
        if (droppedItems.Count == 0)
        {
            lootWindow.SetActive(false);
            //Destroy(lootWindow);
        }
    }
    public float CalculateDamageMelee(float skillDamage, bool crit)
    {
        bool isCriticalHit = crit;
        float damage = skillDamage;
 

        if (isCriticalHit)
        {
            damage = (skillDamage * critMultiplier);
            return damage;
        }
        else
        {
             return damage;
        }       
    }
    public float CalculateDamageRanged(float skillDamage, bool crit)
    {
        bool isCriticalHit = crit;
        float damage = skillDamage;

        if (isCriticalHit)
        {
            damage = (skillDamage * critMultiplier);
            return damage;
        }
        else
        {
             return damage;
        }
    }
    public float CalculateDamageMagic(float skillDamage, bool crit)
    {
        bool isCriticalHit = crit;
        float damage = skillDamage;

        if (isCriticalHit)
        {
            damage = (skillDamage * critMultiplier);
            return damage;
        }
        else
        {
             return damage;
        }

    }

    public void TakeDamage(Skill skill, bool isCrit)
    {
        enemyAI.detectionRange = 70;

        float modifier = ElementDamageMatrix.GetDamageModifier(skill.element, enemyElement);
        if (skill.skillType == SkillType.Melee)
        {
            
        finalDamage = CalculateDamageMelee(skill.damage, isCrit) * modifier; 
        Debug.Log("Melee damage " + finalDamage);   
        }
        else if (skill.skillType == SkillType.Ranged)
        {
        finalDamage = CalculateDamageRanged(skill.damage, isCrit) * modifier; 
        Debug.Log("Ranged damage " + finalDamage);  
        }
        else if (skill.skillType == SkillType.Spell)
        {
        finalDamage = CalculateDamageMagic(skill.damage, isCrit) * modifier;
        Debug.Log("Spell damage " + finalDamage);         
        }  
        else if (skill.skillType == SkillType.Passive)
        {
        Debug.Log("passive skill");        
        }
        else
        {
            Debug.Log("skill got no type");
            
        }
        if (finalDamage > 0) // Eli ei ole immune damagelle
        {
            Debug.Log("Interrupt from takedamge");
            isInterrupted = true;
        }
        else
        {
            Debug.Log("Immune, cant interrup");
        }
        animator.SetTrigger("takeDamage");
        currentHealth -= finalDamage;
        EnemyHealthBar enemyHealthBar = GetComponentInChildren<EnemyHealthBar>();
        if (enemyHealthBar != null)
        {
            enemyHealthBar.ShowTextForDuration(enemyHealthBar.enemyTakeDamageText, finalDamage, isCrit, isMiss);
        }
        else
        {
            Debug.LogWarning("EnemyHealthBar not found on enemy: " + gameObject.name);
        }

        
        if (avatarManager != null)
        {
           
            avatarManager.DisplayDamage(finalDamage);
        }
        else
        {
            Debug.Log("Avatarmanageria ei löydy");
        }

        if (currentHealth <= 0)
        {
            isDead = true;
            Debug.Log("Setting enemy isdead true");
            Debug.Log(isDead);

            animator.SetBool("isWalking", false);
            animator.SetBool("isRunning", false);
            
            agent.isStopped = true;
            playerAttack.StopMeleeAttack();
            currentHealth = 0;
            deathPosition = transform.position;
            //enemyHealthBar.ShowTextForDuration(enemyHealthBar.enemyTakeDamageText, finalDamage, isCrit);
            
            PlayerStats playerStats = FindObjectOfType<PlayerStats>();
            if (playerStats != null)
            {
                playerStats.AddExperience(experiencePoints);
            }
            /*Bear bearScript = GetComponent<Bear>();
            if (bearScript != null)
            {
                bearScript.KillBear();
            }
            isDead = true;*/
            
            
            //playerAttack.targetedEnemy.HideHealthBar();
            StartCoroutine(DeathDelay());
            
        }
        //isInterrupted = false;
    }
    public bool CheckIfHits()
    {
        int levelDifference = playerStats.level - monsterLevel;
        float hitChance = playerAttack.hitRating;
        if (levelDifference > 0)
        {
            // Pelaaja korkeampi taso (5% per tasoero)
            hitChance += levelDifference * 5;
        }
        else if (levelDifference < 0)
        {
            // Pelaaja alempi taso (-5% per tasoero)
            hitChance += levelDifference * 5; // Tämä vähentää, koska levelDifference on negatiivinen
        }
        float maxHitChance = levelDifference > 0 ? 99 : 95;

        hitChance = Mathf.Clamp(hitChance, MinHitChance, maxHitChance);


        // Arvotaan osuma
        
        float roll =  Random.Range(0, 100); // Arpoo luvun 0-99
        Debug.Log("Roll " + roll + " vs " + hitChance);

        return roll < hitChance; // Osuu, jos arpa on pienempi kuin osumatarkkuus

    }
    public void TakeDamageMelee(float damage, bool isCrit, Element element)
    {
        float modifier = ElementDamageMatrix.GetDamageModifier(element, enemyElement);
        //animator.SetTrigger("TakeDamage modifier " + modifier);
        float finalDamage = damage * modifier;
        if (!CheckIfHits())
        {
            isMiss = true;
            EnemyHealthBar enemyHealthBar = playerAttack.targetedEnemy.GetComponentInChildren<EnemyHealthBar>();
            enemyHealthBar.ShowTextForDuration(enemyHealthBar.enemyTakeDamageText, finalDamage, isCrit, isMiss);

        }
        else
        {
            isMiss = false;
            animator.SetTrigger("takeDamage");

            currentHealth -= finalDamage;
            EnemyHealthBar enemyHealthBar = playerAttack.targetedEnemy.GetComponentInChildren<EnemyHealthBar>();
            enemyHealthBar.ShowTextForDuration(enemyHealthBar.enemyTakeDamageText, finalDamage, isCrit, isMiss);
        }
        if (avatarManager != null)
        {
           
            avatarManager.DisplayDamage(finalDamage);
        }
        else
        {
           
        }

        if (currentHealth <= 0)
        {
            isDead = true;
            Debug.Log("Setting enemy isdead true");
            Debug.Log(isDead);

            animator.SetBool("isWalking", false);
            animator.SetBool("isRunning", false);
            
            agent.isStopped = true;
            currentHealth = 0;
            deathPosition = transform.position;
            //enemyHealthBar.ShowTextForDuration(enemyHealthBar.enemyTakeDamageText, finalDamage, isCrit);
            
            PlayerStats playerStats = FindObjectOfType<PlayerStats>();
            if (playerStats != null)
            {
                playerStats.AddExperience(experiencePoints);
            }

            
            
            
            //playerAttack.targetedEnemy.HideHealthBar();
            StartCoroutine(DeathDelay());
        
        }
    }    

    private IEnumerator DeathDelay()
    {
        Debug.Log("DIE ANIMATION SET");
        animator.SetTrigger("isDead");
        agent.isStopped = true;
        HideHealthBar();
        yield return new WaitForSeconds(2f);
        DropLoot();
        yield return new WaitForSeconds(25f);
        Die();

        // Pudota loot kuoleman viiveen jälkeen

        // Herätä karhu henkiin 30 sekunnin kuluttua

    }
    public void CalculateDroppedLoot()
    {
        // Karhukohtainen droppi-logiikka
        droppedItems.Clear(); // Tyhjennetään lista varmuuden vuoksi

        foreach (Item item in lootItems)
        {
            int randomValue = Random.Range(0, 1000); // Arvotaan luku 0-999 väliltä

            if (item.dropChance >= randomValue)
            {
                droppedItems.Add(item); // Lisää esine droppedItems-listaan, jos se tippuu
            }
        }

        Debug.Log("BrownBear dropped loot calculated.");
    }


// EnemyHealth-luokassa
    public virtual void Die()
    {
        // Peruslogiikka
        Destroy(gameObject); // Poista vihollinen pelistä
        Revive();
    }

    // Metodi lootin pudottamiseen
    public virtual void DropLoot()
    {
       CalculateDroppedLoot();

    }
    

    // Näytä health bar
    public void ShowHealthBar()
    {
        if (healthBar != null && isDead == false)
        {
            healthBar.SetActive(true); // Näytä health bar
        }
    }

    // Piilota health bar
    public void HideHealthBar()
    {
        if (healthBar != null)
        {
            healthBar.SetActive(false); // Piilota health bar
        }
    }

    // Metodi, joka herättää karhun henkiin ja luo sen alkuperäiseen sijaintiin
public virtual void Revive()
{
    GameObject monsterPrefab = Resources.Load<GameObject>(PrefabPath);
    if (monsterPrefab != null)
    {
        // Create new enemy
        GameObject newMonster = Instantiate(monsterPrefab, spawnPosition, Quaternion.identity);
        EnemyHealth newMonsterHealth = newMonster.GetComponent<EnemyHealth>();
        EnemyBuffManager newMonsterBuffManager = newMonster.GetComponent<EnemyBuffManager>();


        if (newMonsterHealth != null)
        {
            newMonsterHealth.gameObject.SetActive(true);
            ShowHealthBar();
            
            // Close old loot window if exists
            if (trackedBearHealth != null && trackedBearHealth.lootWindow != null)
            {
                trackedBearHealth.lootWindow.SetActive(false); // Hide old loot window
            }

            // Create new LootWindow and assign it to new monster
            GameObject uiParent = GameObject.Find("UI");
            if (uiParent != null)
            {
                GameObject newLootWindow = Instantiate(Resources.Load<GameObject>("LootWindow"), uiParent.transform);
                if (newLootWindow != null)
                {
                    newLootWindow.name = "LootWindow";
                    newLootWindow.SetActive(false); // Hide it initially

                    // Assign the new loot window
                    newMonsterHealth.lootWindow = newLootWindow;

                    // Set up loot window background and buttons
                    Transform lootBackground = newLootWindow.transform.Find("LootBackground");
                    if (lootBackground != null)
                    {
                        newMonsterHealth.lootBackground = lootBackground;
                        newMonsterHealth.lootButtons = new List<Button>();

                        Button[] buttons = lootBackground.GetComponentsInChildren<Button>();
                        foreach (Button button in buttons)
                        {
                            newMonsterHealth.lootButtons.Add(button);
                            button.gameObject.SetActive(false); // Hide buttons initially
                        }
                    }
                    else
                    {
                        Debug.LogError("LootBackground missing in LootWindow!");
                    }

                    // Setup close button
                    Button closeButton = newLootWindow.transform.Find("CloseButton")?.GetComponent<Button>();
                    if (closeButton != null)
                    {
                        closeButton.onClick.RemoveAllListeners();
                        closeButton.onClick.AddListener(newMonsterHealth.HideLootWindow);
                    }
                    else
                    {
                        Debug.LogError("CloseButton missing in LootWindow!");
                    }

                    // Update the tracked health reference to the new monster
                    trackedBearHealth = newMonsterHealth;
                    Camera playerCamera = GameObject.Find("Warrior/PlayerCamera/Camera").GetComponent<Camera>();
                    if (playerCamera != null)
                    {
                        // Oletetaan, että HealthBarissa on playerCamera-viittaus
                        EnemyHealthBar newHealthBar = newMonsterHealth.GetComponentInChildren<EnemyHealthBar>();
                        newHealthBar.gameObject.SetActive(true);
                        newMonsterHealth.ShowHealthBar();
                        if (newHealthBar != null)
                        {
                            newHealthBar.playerCamera = playerCamera;
                        }
                        
                    }
                    else
                    {
                        Debug.LogError("Pelaajan kameraa ei löytynyt!");
                    }
                                        // Assign buffParent programmatically if it's null
                    if (newMonsterBuffManager.buffParent == null)
                    {
                        // Find or create the buffParent (for example in UI)
                        newMonsterBuffManager.buffParent = GameObject.Find("EnemyBuffParent")?.transform;
                        if (newMonsterBuffManager.buffParent == null)
                        {
                            Debug.LogError("BuffParent not found! You should assign it manually in the inspector or make sure it's in the scene.");
                        }
                    }

                    
                }
                else
                {
                    Debug.LogError("LootWindow prefab not found in Resources folder!");
                }
            }
            else
            {
                Debug.LogError("UI object missing in hierarchy!");
            }
        }
        else
        {
            Debug.LogError("EnemyHealth component not found on new monster prefab!");
        }
    }
    else
    {
        Debug.LogError("Monster prefab not found in Resources folder!");
    }
}









// Muuta OnMouseDown-metodia:
private void OnMouseDown()
{
    // Jos vihollinen on kuollut ja sillä on loottia, näytä loot-ikkuna
    if (isDead && droppedItems.Count > 0) 
    {
        ShowLootWindow(); // Näytä loot-ikkuna
    }
    else if (!isDead) // Jos vihollinen ei ole kuollut
    {
        // Piilota edellisen vihollisen health bar, jos se on asetettu
        if (playerAttack.targetedEnemy != null)
        {
            //playerAttack.targetedEnemy.HideHealthBar();
            // Piilota edellisen kohteen merkki
            if (playerAttack.targetedEnemy.targetIndicator != null)
            {
                playerAttack.targetedEnemy.targetIndicator.SetActive(false);
            }
        }

        // Asetetaan tämä vihollinen kohteeksi
        if (playerAttack != null)
        {
            playerAttack.targetedEnemy = this;
        }

        // Päivitetään avatar-paneli
        if (avatarManager != null)
        {
            avatarManager.AssignEnemy(this, enemySprite); // Päivitä avatarin tiedot
            if (avatarManager.avatarPanel != null && !avatarManager.avatarPanel.activeSelf)
            {
                avatarManager.avatarPanel.SetActive(true); // Aktivoi avatar-paneli, jos se ei ole jo näkyvissä
            }
        }

        // Näytetään tämän vihollisen health bar
        //ShowHealthBar();

        // Näytä kohteen merkki (punainen rinkula)
        if (targetIndicator != null)
        {
            targetIndicator.SetActive(true);
        }
    }
}



}
