using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ElementDamageMatrix
{
private static readonly float[,] damageModifiers =
{
    //   Fire   Water  Earth  Wind   Shadow Holy
    { 0.0f,  0.5f,  1.5f,  1.0f,  1.0f,  1.0f }, // Fire -> vahva Earth, heikko Water
    { 1.5f,  0.0f,  1.0f,  0.5f,  1.0f,  1.0f }, // Water -> vahva Fire, heikko Wind
    { 0.5f,  1.0f,  0.0f,  1.5f,  1.0f,  1.0f }, // Earth -> vahva Wind, heikko Fire
    { 1.0f,  1.5f,  0.5f,  0.0f,  1.0f,  1.0f }, // Wind -> vahva Water, heikko Earth
    { 1.0f,  1.0f,  1.0f,  1.0f,  0.0f,  1.5f }, // Shadow -> vahva Holy
    { 1.0f,  1.0f,  1.0f,  1.0f,  1.5f,  0.0f }  // Holy -> vahva Shadow
};



    private static readonly float defaultModifier = 1.0f;

    public static float GetDamageModifier(Element attacker, Element defender)
    {
        // Käsittele Combat ja Defense erikseen, koska ne eivät ole vahingonlaskennassa mukana
        if (attacker == Element.Melee || attacker == Element.Defense || defender == Element.Melee || defender == Element.Defense || attacker == Element.Neutral || defender == Element.Neutral || attacker == Element.Ranged || defender == Element.Ranged)
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
