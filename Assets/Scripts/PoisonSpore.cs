using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.AI;
using System.Collections.Generic;
using TMPro;


public class PoisonSpore : EnemyHealth
{
    public MonsterSkillManager monsterSkillManager;
    public SpellEffect spellEffect; // Viittaus SpellEffect-komponenttiin
    private float aoeDamageInterval = 0.5f; // Vahinkoväli sekunteina
    private float aoeDamageAmount = 5f; // Vahingon määrä
    private float aoeDamageTimer = 0f; // Ajastin vahingolle
    private float aoeRange = 9f;
    public float respawnTimer = 0f; // Ajastin spawnauksen seuraamiseen
    public List<GameObject> spawnedMinions = new List<GameObject>(); // Lista spawnatuista minioneista
    protected override string PrefabPath => "PoisonSpore"; // Vaihtaa prefab-polun
    protected string MinionPrefabPath => "Spore"; // Vaihtaa prefab-polun
    public Vector3 spawnPoint;
    public float movementRadius = 70f;

    public PoisonSpore()
    {
        
    }
    public override void Start()
    {
        //spellEffect = GetComponent<SpellEffect>(); // Hae SpellEffect-komponentti
        spawnPoint = transform.position;
        monsterName = "Poison Spore";
        monsterLevel = Random.Range(9, 11);
        enemySprite = Resources.Load<Sprite>("PoisonSporeAvatar");
        enemyElement = Element.Earth;
        damageModifiers[Element.Fire] = 1.5f; // Heikko tulta vastaan
        damageModifiers[Element.Earth] = 0.0f; // Vastustaa maatas
        base.maxHealth = monsterLevel * 45;
        base.Start();
        StartCoroutine(WaitForItemDatabaseAndAddLoot());
            // Hae skill MonsterSkillManagerista
        if (monsterSkillManager != null)
        {
            
            MonsterAOESkill poisonCloud = monsterSkillManager.GetSkill("PoisonCloud") as MonsterAOESkill;
            if (poisonCloud != null)
            {
               
                StartCoroutine(ApplyAOEDamage(poisonCloud));
            if (spellEffect != null)
            {
            
               spellEffect.SpawnEffect(transform.position, poisonCloud.duration, GetComponent<EnemyHealth>(), this);
            }
            else
            {
                Debug.LogWarning("SpellEffect-komponenttia ei löytynyt PoisonSporesta!");
            }
            }
        }


    }

    public void Update()
    {
        base.Update(); // Kutsutaan peritty Update, jos sellainen on.
    if (!enemyAI.isAttacking && !enemyAI.isChasingPlayer && enemyAI.isWandering)
    {
        if (Vector3.Distance(transform.position, spawnPoint) > movementRadius)
        {
            
            enemyAI.agent.SetDestination(spawnPoint); // Palauta vihollinen spawn-pisteeseen
            enemyAI.walkSpeed = 55;
        }
        else
        {
            enemyAI.walkSpeed = 5;
        }
    }



        if (isDead) return;

       
        respawnTimer += Time.deltaTime; // Päivitetään ajastinta

        // Poista kuolleet minionit listalta
        for (int i = spawnedMinions.Count - 1; i >= 0; i--) 
        {
            if (spawnedMinions[i] == null || spawnedMinions[i].GetComponent<EnemyHealth>().isDead) // Minion is dead
            {
                spawnedMinions.RemoveAt(i); // Remove dead minion from list
            }

        // Summon spores every 30 seconds (minimum)

        if (enemyAI.isChasingPlayer && spawnedMinions.Count < 10 && respawnTimer >= respawnTime)
        {
            for (int a = 0; a < 3; a++)
            {
                SummonSpores();
            }
            respawnTimer = 0f;
        }
    }
    }
    private IEnumerator ApplyAOEDamage(MonsterAOESkill aoeSkill)
    {
        while (true) // Tämä looppi pyörii koko ajan
        {
            yield return new WaitForSeconds(aoeSkill.tickInterval); // Odota skillin määrittämä aika

            if (playerHealth != null) 
            {
                float distance = Vector3.Distance(transform.position, playerHealth.transform.position);
                if (distance <= aoeSkill.aoeRadius) // Jos pelaaja on tarpeeksi lähellä, tee vahinkoa
                {
                    if (!isDead)
                    {
                    
                    playerHealth.TakeDamage(aoeSkill.damage);
                                    // Spawnaa myrkkypilven efektin
                    }


                }
            }
        }
    }

    public void OnMinionDeath(GameObject minion)
    {
        spawnedMinions.Remove(minion); // Poista minion listalta, kun se kuolee
    }



    private IEnumerator WaitForItemDatabaseAndAddLoot()
    {
        while (itemDatabase == null || itemDatabase.items.Count == 0)
        {
            yield return null; // Odotetaan seuraavaa framea
        }

        AddLootItemsAtStart();
    }

    public void AddLootItemsAtStart()
    {
        lootItems.Clear(); // Tyhjennetään varmuuden vuoksi

        Item minorHealingPotion = itemDatabase.GetItemByName("Minor Healing Potion");
        Item minorManaPotion = itemDatabase.GetItemByName("Minor Mana Potion");
        Item poisonSporeCard = itemDatabase.GetItemByName("Poison Spore Card");


        minorHealingPotion.dropChance = 200;
        minorManaPotion.dropChance = 150;
        poisonSporeCard.dropChance = 999;


        lootItems.Add(minorHealingPotion);
        lootItems.Add(minorManaPotion);
        lootItems.Add(poisonSporeCard);


        
    }

    // Override to handle death logic
    public override void Die()
    {
        base.Die(); // Kutsu perittyä Die-metodia

        // Päivitä questin progress, kun BrownBear kuolee
        QuestManager questManager = FindObjectOfType<QuestManager>();
        if (questManager != null)
        {
            questManager.UpdateKillQuestProgress("FungusAmongUs", GoalType.Kill, 1);
        }

        
    }

    public virtual void SummonSpores()
    {
        GameObject monsterPrefab = Resources.Load<GameObject>(MinionPrefabPath);
        if (monsterPrefab != null)
        {
            GameObject newMonster = Instantiate(monsterPrefab, enemyAI.transform.position, Quaternion.identity);
            EnemyHealth newMonsterHealth = newMonster.GetComponent<EnemyHealth>();
            newMonsterHealth.noRespawn = true;
            //newMonsterHealth.droppedItems = null;// summonatuil vihuilla ei oo loottia
            EnemyBuffManager newMonsterBuffManager = newMonster.GetComponent<EnemyBuffManager>();


            if (newMonsterHealth != null)
            {
                spawnedMinions.Add(newMonster);  // Lisää minioni listaan

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
    
}
