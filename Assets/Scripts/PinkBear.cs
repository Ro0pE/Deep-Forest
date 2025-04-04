using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PinkBear : EnemyHealth
{
    protected override string PrefabPath => "PinkBear"; // Vaihtaa prefab-polun
    

    public PinkBear()
    {
        
    }
    public override void Start()
    {
        
        // Aseta yksilöllinen sprite ennen EnemyHealth-luokan Start-logiikan kutsumista
        monsterName = "Pink Bear";
        monsterLevel = Random.Range(1, 3);
        enemySprite = Resources.Load<Sprite>("PinkBearAvatar");
        enemyElement = Element.Earth;
        damageModifiers[Element.Fire] = 1.5f; // Heikko tulta vastaan
        damageModifiers[Element.Earth] = 0.0f; // Vastustaa maata
        maxHealth = monsterLevel * 15;
        StartCoroutine(WaitForItemDatabaseAndAddLoot());
        base.Start();
        
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
        Item strawberry = itemDatabase.GetItemByName("Strawberry"); // questitem
        Equipment bearHelm = itemDatabase.GetItemByName("Helm of the bear") as Equipment;
        Equipment bearAxe = itemDatabase.GetItemByName("Axe of the bear") as Equipment;
        Equipment bearClaw = itemDatabase.GetItemByName("Bear Claw") as Equipment;
        Equipment bearShoulders = itemDatabase.GetItemByName("Shoulders of the bear") as Equipment;
        Equipment bearBoots = itemDatabase.GetItemByName("Boots of the bear") as Equipment;
        Equipment bearGloves = itemDatabase.GetItemByName("Gloves of the bear") as Equipment;
        Equipment bearBelt = itemDatabase.GetItemByName("Belt of the bear") as Equipment;
        Equipment swordBear = itemDatabase.GetItemByName("Sword of the bear") as Equipment;
        Item bearPelt = itemDatabase.GetItemByName("Bear Pelt"); 

        minorHealingPotion.dropChance = 200;
        minorManaPotion.dropChance = 150;
        strawberry.dropChance = 990;
        bearHelm.dropChance = 45;
        bearAxe.dropChance = 55;
        bearClaw.dropChance = 10;
        bearShoulders.dropChance = 70;
        bearBoots.dropChance = 110;
        bearGloves.dropChance = 102;
        bearBelt.dropChance = 120;
        swordBear.dropChance = 75;
        bearPelt.dropChance = 990;

        bearClaw.SetCardSlots(4);
        bearAxe.SetCardSlots(2);

        lootItems.Add(minorHealingPotion);
        lootItems.Add(minorManaPotion);
        lootItems.Add(strawberry);
        lootItems.Add(bearHelm);
        lootItems.Add(bearAxe);
        lootItems.Add(bearClaw);
        lootItems.Add(bearShoulders);
        lootItems.Add(bearBoots);
        lootItems.Add(bearGloves);
        lootItems.Add(bearBelt);
        lootItems.Add(swordBear);
        lootItems.Add(bearPelt);

        Debug.Log("BrownBear loot added.");
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
            questManager.UpdateKillQuestProgress("DamnBears", GoalType.Kill, 1);
            
            //questManager.OnEnemyKill("Bear");
        }

        Debug.Log("Pink Bear died and quest progress updated.");
    }
}
