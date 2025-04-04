using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBuffManager : MonoBehaviour
{
    public Transform buffParent; // UI Parent for buffs (optional)
    public GameObject buffPrefab; // UI Prefab for buffs (optional)

    public List<Buff> activeBuffs = new List<Buff>();
    public EnemyHealth enemyHealth;
    public AvatarManager avatarManager;


    private void Start()
    {
        enemyHealth = GetComponent<EnemyHealth>();
        avatarManager = FindObjectOfType<AvatarManager>();

    }

    public void AddBuff(Buff buff)
    {
         Debug.Log("Add buff to enemy: " + enemyHealth);

        Buff existingBuff = activeBuffs.Find(b => b.name == buff.name);
        EnemyHealthBar enemyHealthBar = enemyHealth.GetComponent<EnemyHealthBar>();

        if (existingBuff != null)
        {
            if (buff.isStackable)
            {
                existingBuff.stacks++;
                existingBuff.duration = buff.duration;
                existingBuff.applyEffect();
                UpdateEffectText(existingBuff);
                enemyHealthBar.AddBuffIcon(buff);
            }
            else
            {
                existingBuff.duration = buff.duration;
                existingBuff.applyEffect();
                enemyHealthBar.AddBuffIcon(buff);
            }
        }
        else
        {
            activeBuffs.Add(buff);
            buff.applyEffect();
            UpdateEffectText(buff);

            if (buff.isStackable)
            {
                buff.stacks = 1; // Start with one stack
            }

            // Päivitetään UI
            if (buffParent != null && buffPrefab != null)
            {
            if (avatarManager != null && avatarManager.enemyHealth == this.enemyHealth)
            {
                // Tarkistetaan, onko buffilla jo UI-komponentti
                if (buff.uiComponent == null)
                {
                    avatarManager.CreateBuffUI(buff);
                }
            }

            }
        }
    }


    private void UpdateEffectText(Buff buff)
    {
       
        if (buff.effectText.Contains("damage per second"))
        {
            float totalDamage = buff.damage * buff.stacks;
            buff.effectText = $"Inflicts {totalDamage} damage per second";
        }
        else if (buff.effectText.Contains("Slow"))
        {
            Debug.Log("TEST  " +buff.effectText);
            buff.effectText = $"Slows target by {buff.effectValue * 100}%";
        }
        else if (buff.effectText.Contains("ATK"))
        {
            buff.effectText = $"Reduces ATK by {buff.effectValue * 100}%";
        }
        else if (buff.effectText.Contains("Stun"))
        {
            Debug.Log("TEST 2 " +buff.effectText);
            buff.effectText = $"Stunned for {buff.duration } second";
        }


        if (buff.uiComponent != null && buff.uiComponent.tooltipPanel != null)
        {
            var tooltipText = buff.uiComponent.tooltipPanel.GetComponentInChildren<TMPro.TextMeshProUGUI>();
            if (tooltipText != null)
            {
                tooltipText.text = buff.effectText;
            }
        }
    }

    public void RemoveBuff(Buff buff)
    {
        
        if (buff.isStackable && buff.stacks > 1)
        {
            buff.stacks--;
        }
        else
        {
            activeBuffs.Remove(buff);
            buff.removeEffect();

            if (buff.uiComponent != null)
            {
                Destroy(buff.uiComponent.gameObject);
            }
        }
    }

    public void Update()
    {
        for (int i = activeBuffs.Count - 1; i >= 0; i--)
        {
            Buff buff = activeBuffs[i];
            buff.duration -= Time.deltaTime;

            if (buff.uiComponent != null)
            {
                buff.uiComponent.UpdateDuration(buff.duration, buff.stacks);
            }

            if (buff.duration <= 0)
            {
                RemoveBuff(buff);
            }
            if (enemyHealth.isDead)
            {
                RemoveBuff(buff);
            }
        }
    }
}
