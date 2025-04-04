using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlueDragonNightmare : EnemyHealth
{
    protected override string PrefabPath => "BlueDragonNightmare"; // Vaihtaa prefab-polun

    public BlueDragonNightmare()
    {
     
    }
    public override void Start()
    {
        monsterName = "Ice Dragon";
        monsterLevel = Random.Range(20, 30);
        maxHealth = monsterLevel * 50;
        // Aseta yksilöllinen sprite ennen EnemyHealth-luokan Start-logiikan kutsumista
        enemySprite = Resources.Load<Sprite>("BlueDragonAvatar");

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
   
    }

}
