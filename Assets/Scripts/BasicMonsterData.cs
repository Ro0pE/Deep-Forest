using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewMonsterData", menuName = "RPG/Monster Data")]
public class BasicMonsterData : ScriptableObject
{
    [Header("General Settings")]
    public string monsterName;
    public int minLevel;
    public int maxLevel;
    public string prefabPath;
    public Sprite enemySprite;
    public Element enemyElement;

    [Header("Health & Stats")]
    public int baseHealth;
    public int experiencePoints;

    [Header("Damage Modifiers")]
    public List<ElementDamageModifier> damageModifiersList;

    [Header("Loot Settings")]
    public List<LootItemData> lootItems;

    [Header("Quest Settings")]
    public string killQuestName;

}

// Elementtiresistanssit Inspectorissa
[System.Serializable]
public class ElementDamageModifier
{
    public Element element;
    public float modifier;
}

// Loot-itemien drop chance Inspectorissa
[System.Serializable]
public class LootItemData
{
    public string itemName;
    public int dropChance; // Drop chance prosentteina (esim. 200 = 2.00%)
    public string itemType;
}
