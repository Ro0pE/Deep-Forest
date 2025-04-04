using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ZombieSpikthorn : EnemyHealth
{
    public MonsterSkillManager monsterSkillManager;
    public SpellEffect thornsEffect; // Viittaus SpellEffect-komponenttiin
    private GameObject thornsEffectInstance; // Viittaus aktiiviseen efektiin
    private float thornsDuration;
    private float aoeRange = 9f;
    public float respawnTimer = 0f;
    public List<GameObject> spawnedMinions = new List<GameObject>();
    protected override string PrefabPath => "ZombieSpikthorn";
    protected string MinionPrefabPath => "Spikthorn";
    public Vector3 spawnPoint;
    public float movementRadius = 70f;
    public bool thornsActivated;

    public override void Start()
    {
        
        spawnPoint = transform.position;
        monsterName = "Zombie Spikthorn";
        monsterLevel = Random.Range(11, 13);
        enemySprite = Resources.Load<Sprite>("ZombieSpikthornAvatar");
        enemyElement = Element.Earth;
        damageModifiers[Element.Fire] = 1.5f;
        damageModifiers[Element.Earth] = 0.0f;
        base.maxHealth = monsterLevel * 55;
        base.Start();
        StartCoroutine(WaitForItemDatabaseAndAddLoot());
        StartCoroutine(ActivateThornsRandomly());
    }

    public void Update()
    {
        base.Update();
        if (!enemyAI.isAttacking && !enemyAI.isChasingPlayer && enemyAI.isWandering)
        {
            if (Vector3.Distance(transform.position, spawnPoint) > movementRadius)
            {
                enemyAI.agent.SetDestination(spawnPoint);
                enemyAI.walkSpeed = 55;
            }
            else
            {
                enemyAI.walkSpeed = 5;
            }
        }
    }

    private IEnumerator WaitForItemDatabaseAndAddLoot()
    {
        while (itemDatabase == null || itemDatabase.items.Count == 0)
        {
            yield return null;
        }
        AddLootItemsAtStart();
    }

    public void AddLootItemsAtStart()
    {
        lootItems.Clear();
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

    public override void Die()
    {
        base.Die();
        QuestManager questManager = FindObjectOfType<QuestManager>();
        if (questManager != null)
        {
            questManager.UpdateKillQuestProgress("FungusAmongUs", GoalType.Kill, 1);
        }
    }

    public override void TakeDamage(Skill skill, bool isCrit)
    {
        base.TakeDamage(skill, isCrit);
        if (thornsActivated && finalDamage > 0 && playerHealth != null)
        {
            float damageBackToPlayer = finalDamage / 2;
            playerHealth.TakeDamage(damageBackToPlayer);
        }
    }

    private IEnumerator ActivateThornsRandomly()
    {
        

        while (true)
        {
            
            float randomTime = Random.Range(3f, 8f);
            yield return new WaitForSeconds(randomTime);

            thornsActivated = true;
            thornsDuration = Random.Range(3f, 8f);
            if(isDead)
            {
                thornsDuration = 0;
            }

            if (thornsEffect != null)
            {
          
                thornsEffect.SpawnEffect(transform.position, thornsDuration, this, this);
            }

            yield return new WaitForSeconds(thornsDuration);
            thornsActivated = false;
          
        }
    }
}
