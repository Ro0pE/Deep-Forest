using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : EnemyHealth
{
    [Header("Monster Data")]
    public BasicMonsterData monsterData; // Linkitetään ScriptableObject
    protected override string PrefabPath => monsterData.prefabPath; // Käytetään prefab-polku ScriptableObjectista

    // Ylikirjoitetaan EnemyHealthin Start-metodi
    public override void Start()
    {
        
        // Käytetään ScriptableObjectin tietoja
        monsterName = monsterData.monsterName;
        monsterLevel = Random.Range(monsterData.minLevel, monsterData.maxLevel);
        enemySprite = monsterData.enemySprite;
        enemyElement = monsterData.enemyElement;
        enemySprite = monsterData.enemySprite;


        // Damage modifierit: Vahvuudet ja heikkoudet
        foreach (var modifier in monsterData.damageModifiersList)
        {
            damageModifiers[modifier.element] = modifier.modifier;
        }

        maxHealth = monsterLevel * monsterData.baseHealth;

        // Aseta muut logiikat ja sitten kutsu perittyä Start-metodia
      base.Start(); // Kutsutaan EnemyHealthin Start-metodia
        StartCoroutine(WaitForItemDatabaseAndAddLoot());
    }



    public override void UpdateQuestProgress()
    {
        QuestManager questManager = FindObjectOfType<QuestManager>();
        if (questManager != null)
        {
            if (monsterData.killQuestName != null)
            {
            questManager.UpdateKillQuestProgress(monsterData.killQuestName, GoalType.Kill, 1);             
            }

        }
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

            // Haetaan lootit itemDatabase:sta niiden nimien ja drop-chancen perusteella
            foreach (var lootData in monsterData.lootItems)
            {
                var item = itemDatabase.GetItemByName(lootData.itemName);
                if (item != null)
                {
                    // Instantiating a new copy of the item to avoid reference issues
                    var newItem = Instantiate(item);  // Luo uusi instanssi
                    // Asetetaan lootData:sta otettu dropChance
                    newItem.dropChance = lootData.dropChance;
                    lootItems.Add(newItem);  // Lisää uusi instanssi loot-listaan
                    
                }
            }

          
        }


}
