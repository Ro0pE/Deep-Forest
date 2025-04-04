using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrostWyrmScript : EnemyHealth
{
    protected override string PrefabPath => "FrostWyrm"; // Vaihtaa prefab-polun
    

    public FrostWyrmScript()
    {
        
    }
    public override void Start()
    {
        // Aseta yksilöllinen sprite ennen EnemyHealth-luokan Start-logiikan kutsumista
        monsterName = "Frost Wyrm";
        monsterLevel = Random.Range(1, 12);
        enemySprite = Resources.Load<Sprite>("FrostWyrmAvatar");
        enemyElement = Element.Water;
        damageModifiers[Element.Wind] = 1.5f; 
        damageModifiers[Element.Earth] = 0.0f; 
        maxHealth = monsterLevel * 15;

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
        Debug.Log("Frost Wyrm revived with special behavior!");
    }

}
