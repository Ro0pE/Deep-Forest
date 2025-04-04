using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WhiteBear : EnemyHealth
{
    protected override string PrefabPath => "WhiteBear";

    public override void Start()
    {
        base.Start();

        // Hae EnemyAI-komponentti
        enemyAI = GetComponent<EnemyAI>();

        // Aseta WhiteBearille yksilölliset nopeudet ja arvot
        if (enemyAI != null)
        {
            enemyAI.walkSpeed = 9; // Muutettu nopeus vaeltamiseen
            enemyAI.runSpeed = 15; // Muutettu nopeus pelaajaa jahdatessa
            enemyAI.detectionRange = 70; // Muutettu havaitsemisetäisyys
        }

        monsterName = "The Great White";
        monsterLevel = 12;
        enemySprite = Resources.Load<Sprite>("FrostWyrmAvatar");
        enemyElement = Element.Earth;
        damageModifiers[Element.Fire] = 1.5f; // Heikko tulta vastaan
        damageModifiers[Element.Earth] = 0.0f; // Vastustaa maata
        maxHealth = monsterLevel * 25;
        currentHealth = maxHealth;
        experiencePoints = monsterLevel * 30;

        StartCoroutine(WaitForItemDatabaseAndAddLoot());
    }



    // Update is called once per frame
    void Update()
    {
        base.Update();
    }
    public override void Revive()
    {
        base.Revive(); // Kutsutaan EnemyHealthin toteutusta, jos se on tarpeen

        // Tässä voit lisätä PinkBearin erityisiä ominaisuuksia tai toimintalogiikkaa
        Debug.Log("white bear revived with special behavior!");
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

        Item healingPotion = itemDatabase.GetItemByName("Healing Potion");
        Item manaPotion = itemDatabase.GetItemByName("Mana Potion");
        Equipment emeraldEye = itemDatabase.GetItemByName("Emerald Eye") as Equipment;
        Equipment bearHelm = itemDatabase.GetItemByName("Helm of the bear") as Equipment;
        Equipment bearShoulders = itemDatabase.GetItemByName("Shoulders of the bear") as Equipment;
        Equipment bearBoots = itemDatabase.GetItemByName("Boots of The bear") as Equipment;
        Equipment bearBelt = itemDatabase.GetItemByName("Belt of the bear") as Equipment;
        Equipment bearGloves = itemDatabase.GetItemByName("Gloves of the bear") as Equipment;


        healingPotion.dropChance = 999;
        manaPotion.dropChance = 999;
        emeraldEye.dropChance = 999;
        bearHelm.dropChance = 150;
        bearShoulders.dropChance = 125;
        bearBoots.dropChance = 250;
        bearBelt.dropChance = 250;
        bearGloves.dropChance = 150;


        emeraldEye.SetCardSlots(4);
        bearHelm.SetCardSlots(4);
        bearShoulders.SetCardSlots(4);
        bearBoots.SetCardSlots(4);
        bearBelt.SetCardSlots(4);
        bearGloves.SetCardSlots(4);

        lootItems.Add(healingPotion);
        lootItems.Add(manaPotion);
        lootItems.Add(emeraldEye);
        lootItems.Add(bearHelm);
        lootItems.Add(bearShoulders);
        lootItems.Add(bearBoots);
        lootItems.Add(bearBelt);
        lootItems.Add(bearGloves);
        
    }

    // Override to handle death logic
    public override void Die()
    {
        base.Die(); // Kutsu perittyä Die-metodia

        // Päivitä questin progress, kun BrownBear kuolee
        QuestManager questManager = FindObjectOfType<QuestManager>();
        if (questManager != null)
        {
            // Tämän karhun tappaminen lisää progression Quest "DamnBearsQuestID" tavoitteelle
            questManager.UpdateKillQuestProgress("TheGreatWhite", GoalType.Kill, 1);
            
            //questManager.OnEnemyKill("Bear");
        }

        Debug.Log("Frostbane died and quest progress updated.");
    }

}
