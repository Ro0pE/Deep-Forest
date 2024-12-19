using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class BuffManager : MonoBehaviour
{
    public Transform buffParent; 
    public GameObject buffPrefab;



    public List<Buff> activeBuffs = new List<Buff>();
    public PlayerHealth playerHealth;
    public PlayerMovement playerMovement;
    public PlayerAttack playerAttack;

    private void Start()
    {
        playerAttack = GetComponent<PlayerAttack>();
        playerHealth = GetComponent<PlayerHealth>();
        playerMovement = GetComponent<PlayerMovement>();
    }

public void AddBuff(Buff buff)
{
    Buff existingBuff = activeBuffs.Find(b => b.name == buff.name);
    
    if (existingBuff != null)
    {
        if (buff.isStackable)
        {
            existingBuff.stacks++;
            existingBuff.duration = buff.duration;
            existingBuff.applyEffect();
            UpdateEffectText(existingBuff);
        }
        else
        {
            existingBuff.duration = buff.duration;
            existingBuff.applyEffect();
        }
    }
    else
    {
        activeBuffs.Add(buff);
        buff.applyEffect();
        UpdateEffectText(buff);

        if (buff.isStackable)
        {
            buff.stacks = 1; // Aloita yhdellä stackilla
        }

        // Luo ja tallenna UI-elementti
        GameObject newBuffUI = Instantiate(buffPrefab, buffParent);
        buffUI buffUIComponent = newBuffUI.GetComponent<buffUI>();

        // Aseta ikonit ja kesto
        buffUIComponent.buffIcon.sprite = buff.buffIcon; // Vaihda oikeaksi
        buffUIComponent.UpdateDuration(buff.duration, buff.stacks);

        buffUIComponent.Initialize(buff);
        buff.uiComponent = buffUIComponent;

    }
}
    private void UpdateEffectText(Buff buff)
    {

        // Päivitetään effectText, kun stackeja lisätään
        if (buff.effectText.Contains("damage per second"))
        {
            // Jos kyseessä on vahinko, laske se stackien mukaan
            float totalDamage = buff.damage * buff.stacks;
            buff.effectText = $"Inflicts {totalDamage} damage per second";
        }
        else if (buff.effectText.Contains("Slow"))
        {
            // Jos kyseessä on hidastus
            buff.effectText = $"Slows target by {buff.effectValue * 100}%";
   

        }
        else if (buff.effectText.Contains("ATK"))
        {
            // Jos kyseessä on hyökkäyksen alennus
            buff.effectText = $"Reduces ATK by {buff.effectValue * 100}%";
        }
        if (buff.uiComponent != null && buff.uiComponent.tooltipPanel != null)
        {
            var tooltipText = buff.uiComponent.tooltipPanel.GetComponentInChildren<TextMeshProUGUI>();
            if (tooltipText != null)
            {
                tooltipText.text = buff.effectText; // Päivitetään tooltipin teksti
            }
        }

        // Päivitä tooltip-teksti UI:ssa
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

            // Poista UI
            if (buff.uiComponent != null)
            {
                Destroy(buff.uiComponent.gameObject);
            }
        }
    }


    private void Update()
    {
        for (int i = activeBuffs.Count - 1; i >= 0; i--)
        {
            Buff buff = activeBuffs[i];
            buff.duration -= Time.deltaTime;

            // Päivitä UI:n kesto
            if (buff.uiComponent != null)
            {
                buff.uiComponent.UpdateDuration(buff.duration, buff.stacks);
            }

            // Poista buffi, kun kesto loppuu
            if (buff.duration <= 0)
            {
                RemoveBuff(buff);
            }
        }
    }

}
