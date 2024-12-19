using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmberWyrm : EnemyHealth
{
    protected override string PrefabPath => "EmberWyrm"; // Vaihtaa prefab-polun
    

    public EmberWyrm()
    {
        
    }
    public override void Start()
    {
        // Aseta yksilöllinen sprite ennen EnemyHealth-luokan Start-logiikan kutsumista
        monsterName = "Ember Wyrm";
        monsterLevel = Random.Range(1, 12);
        enemySprite = Resources.Load<Sprite>("EmberWyrmAvatar");
        enemyElement = Element.Fire;
        damageModifiers[Element.Earth] = 1.5f; 
        damageModifiers[Element.Fire] = 0.0f; 
        maxHealth = monsterLevel * 15f;

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
        Debug.Log("Ember Wyrm revived with special behavior!");
    }

}
