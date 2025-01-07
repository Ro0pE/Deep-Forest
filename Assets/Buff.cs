using System;
using UnityEngine;

public enum BuffType
{
    Buff,
    Debuff
}

[System.Serializable]
public class Buff
{
    public string name;
    public float duration;
    public bool isStackable;
    public int stacks;
    public Sprite buffIcon;
    public BuffType buffType;
    public float damage; // Vahinkoa tekevien buffien arvo
    public string effectText; // Tekstin generointi tässä
    public float effectValue; // Buffin teho (esim. nopeuden muutos, ATK vähennys)

    public System.Action applyEffect;
    public System.Action removeEffect;

    [System.NonSerialized]
    public buffUI uiComponent;


    
    public Buff(string name, float duration, bool isStackable, int stacks, Sprite icon, BuffType buffType,
                float damage, string effect, float effectValue, System.Action applyEffect, System.Action removeEffect)
    {
        this.name = name;
        this.duration = duration;
        this.isStackable = isStackable;
        this.stacks = stacks;
        this.buffIcon = icon;
        this.buffType = buffType;
        this.damage = damage;
        this.effectText = GenerateEffectText(effect);
        this.effectValue = effectValue;
        this.applyEffect = applyEffect;
        this.removeEffect = removeEffect;

        
    }

    private string GenerateEffectText(String effect)
    {
        if (damage > 0)
        {
            return $"Inflicts {damage * stacks} damage per second.";
        }

        if (buffType == BuffType.Debuff)
        {
            if (name.ToLower().Contains("stun"))
            {
                return $"Stunned for {duration} second{(duration > 1 ? "s" : "")}.";
            }
            if (effectValue < 0)
            {
                return $"Reduces {name} by {Mathf.Abs(effectValue) * 100}%";
            }
            else
            {
                return $"Slows target by {effectValue * 100}%";
            }
        }

        if (buffType == BuffType.Buff)
        {
            return effect;
        }

        return "No effect.";
    }

}
