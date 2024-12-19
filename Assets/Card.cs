using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]

    public class Card
    {
        public string cardName; // Kortin nimi
        public string description; // Kortin kuvaus
        public Sprite cardIcon; // Kortin kuva

        public int statBonus; // Kortin antamat bonusarvot (esimerkiksi vahvistus tietylle statille)
        
    }