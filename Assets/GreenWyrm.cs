using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GreenWyrm : EnemyHealth
{
    protected override string PrefabPath => "GreenWyrm"; // Vaihtaa prefab-polun
    

    public GreenWyrm()
    {
        
    }
    public override void Start()
    {
        // Aseta yksilöllinen sprite ennen EnemyHealth-luokan Start-logiikan kutsumista
        monsterName = "Green Wyrm";
        monsterLevel = Random.Range(1, 12);
        enemySprite = Resources.Load<Sprite>("GreenWyrmAvatar");
        enemyElement = Element.Earth;
        damageModifiers[Element.Fire] = 1.5f; 
        damageModifiers[Element.Earth] = 0.0f; 
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
        Debug.Log("Green Wyrm revived with special behavior!");
    }

}
