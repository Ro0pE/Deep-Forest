using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ElementDamageMatrix
{
private static readonly float[,] damageModifiers =
    {
        //   Fire   Water  Earth  Wind   Shadow Holy
        { 0.0f,  1.5f,  1.0f,  1.0f,  1.0f,  1.0f }, // Fire
        { 1.5f,  0.0f,  1.0f,  1.0f,  1.0f,  1.0f }, // Water
        { 1.0f,  1.0f,  0.0f,  1.5f,  1.0f,  1.0f }, // Earth
        { 1.0f,  1.5f,  1.0f,  0.0f,  1.0f,  1.0f }, // Wind
        { 1.0f,  1.0f,  1.0f,  1.0f,  0.0f,  1.5f }, // Shadow
        { 1.0f,  1.0f,  1.0f,  1.0f,  1.5f,  0.0f }  // Holy
    };

    private static readonly float defaultModifier = 1.0f;

    public static float GetDamageModifier(Element attacker, Element defender)
    {
        // Käsittele Combat ja Defense erikseen, koska ne eivät ole vahingonlaskennassa mukana
        if (attacker == Element.Combat || attacker == Element.Defense || defender == Element.Combat || defender == Element.Defense || attacker == Element.Neutral || defender == Element.Neutral)
        {
            return defaultModifier;  // Neutraali vahinko
        }

        // Tarkista, että attacker ja defender ovat oikein määriteltyjä
        if (attacker < Element.Fire || attacker > Element.Holy || defender < Element.Fire || defender > Element.Holy)
        {
            Debug.LogError("Invalid Element provided. Attacker or Defender out of range.");
            return defaultModifier;  // Palautetaan neutraali arvo virhetilanteessa
        }

        return damageModifiers[(int)attacker, (int)defender];
    }
}
