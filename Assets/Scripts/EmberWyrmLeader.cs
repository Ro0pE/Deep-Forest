using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmberWyrmLeader : EnemyHealth
{
    protected override string PrefabPath => "EmberWyrmLeader"; // Vaihtaa prefab-polun
    

    public EmberWyrmLeader()
    {
        
    }
    
    public override void Start()
    {
        Debug.Log("Leader start");
        // Aseta yksilöllinen sprite ennen EnemyHealth-luokan Start-logiikan kutsumista
        monsterName = "Blazebreaker";
        monsterLevel = 21;
        enemySprite = Resources.Load<Sprite>("EmberWyrmAvatar");
        enemyElement = Element.Water;
        damageModifiers[Element.Wind] = 1.5f; 
        damageModifiers[Element.Earth] = 0.0f; 
        maxHealth = monsterLevel * 30;
        StartCoroutine(WaitForItemDatabaseAndAddLoot());

        base.Start(); // Kutsutaan ylemmän tason logiikkaa
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
        Debug.Log("Blazebreaker revived with special behavior!");
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
        Equipment frostBlade = itemDatabase.GetItemByName("Frost Blade") as Equipment;


        healingPotion.dropChance = 999;
        manaPotion.dropChance = 999;
        frostBlade.dropChance = 990;


        frostBlade.SetCardSlots(4);
  

        lootItems.Add(healingPotion);
        lootItems.Add(manaPotion);
        lootItems.Add(frostBlade);
        
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
            questManager.UpdateKillQuestProgress("TheInferno’sFury", GoalType.Kill, 1);
            
            //questManager.OnEnemyKill("Bear");
        }

        Debug.Log("Blazebreaker died and quest progress updated.");
    }

}
