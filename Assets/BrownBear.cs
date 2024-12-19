using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrownBear : EnemyHealth
{
    protected override string PrefabPath => "BrownBear"; // Vaihtaa prefab-polun
    

    public BrownBear()
    {
        
    }
    public override void Start()
    {
        // Aseta yksilöllinen sprite ennen EnemyHealth-luokan Start-logiikan kutsumista
        monsterName = "Brown Bear";
        monsterLevel = Random.Range(1, 12);
        enemySprite = Resources.Load<Sprite>("BrownBearAvatar");
        enemyElement = Element.Earth;
        damageModifiers[Element.Fire] = 1.5f; // Heikko tulta vastaan
        damageModifiers[Element.Water] = 0.5f; // Vastustaa vettä
        damageModifiers[Element.Earth] = 0.0f; // Vastustaa vettä
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
        Debug.Log("BrownBear revived with special behavior!");
    }

}
