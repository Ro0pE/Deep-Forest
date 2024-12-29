using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.AI;
using System.Collections.Generic;
using TMPro;

public class BrownBear : EnemyHealth
{
    protected override string PrefabPath => "BrownBear"; // Vaihtaa prefab-polun

    public BrownBear() { }

    public override void Start()
    {
        // Aseta yksilöllinen sprite ennen EnemyHealth-luokan Start-logiikan kutsumista
        monsterName = "Brown Bear";
        monsterLevel = Random.Range(1, 5);
        enemySprite = Resources.Load<Sprite>("BrownBearAvatar");
        enemyElement = Element.Earth;
        damageModifiers[Element.Fire] = 1.5f; // Heikko tulta vastaan
        damageModifiers[Element.Earth] = 0.0f; // Vastustaa maata
        maxHealth = monsterLevel * 15f;
        experiencePoints = monsterLevel * 12;

        StartCoroutine(WaitForItemDatabaseAndAddLoot());
        base.Start();
    }
    private IEnumerator WaitForItemDatabaseAndAddLoot()
    {
        // Odotetaan, että ItemDatabase on ladannut esineet
        while (itemDatabase == null || itemDatabase.items.Count == 0)
        {
            yield return null; // Odotetaan seuraavaa framea
        }

        // Kun ItemDatabase on valmis, lisätään lootit viholliselle
        AddLootItemsAtStart();
    }

    public void AddLootItemsAtStart()
    {
        // Karhukohtainen loot-logiikka
        lootItems.Clear(); // Tyhjennetään varmuuden vuoksi

        Item healingPotion = itemDatabase.GetItemByName("Minor Healing Potion");
        Item manaPotion = itemDatabase.GetItemByName("Minor Mana Potion");
        Equipment bearHelm = itemDatabase.GetItemByName("Helm of the bear") as Equipment;
        Equipment bearAxe = itemDatabase.GetItemByName("Axe of the bear") as Equipment;
        Equipment bearClaw = itemDatabase.GetItemByName("Bear Claw") as Equipment;
        Equipment bearShoulders = itemDatabase.GetItemByName("Shoulders of the bear") as Equipment;
        Equipment bearBoots = itemDatabase.GetItemByName("Boots of the bear") as Equipment;
        Equipment bearGloves = itemDatabase.GetItemByName("Gloves of the bear") as Equipment;
        Equipment bearBelt = itemDatabase.GetItemByName("Belt of the bear") as Equipment;
        Equipment swordBear = itemDatabase.GetItemByName("Sword of the bear") as Equipment;

        healingPotion.dropChance = 200;
        manaPotion.dropChance = 150;
        bearHelm.dropChance = 45;
        bearAxe.dropChance = 55;
        bearClaw.dropChance = 10;
        bearShoulders.dropChance = 70;
        bearBoots.dropChance = 110;
        bearGloves.dropChance = 102;
        bearBelt.dropChance = 120;
        swordBear.dropChance = 75;

        bearClaw.SetCardSlots(4);
        bearAxe.SetCardSlots(2);

        lootItems.Add(healingPotion);
        lootItems.Add(manaPotion);
        lootItems.Add(bearHelm);
        lootItems.Add(bearAxe);
        lootItems.Add(bearClaw);
        lootItems.Add(bearShoulders);
        lootItems.Add(bearBoots);
        lootItems.Add(bearGloves);
        lootItems.Add(bearBelt);
        lootItems.Add(swordBear);
        

        Debug.Log("BrownBear loot added.");
    }


}
