using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PinkBear : EnemyHealth
{
    protected override string PrefabPath => "PinkBear"; // Vaihtaa prefab-polun

    public PinkBear()
    {
        maxHealth = 25f;
    }
    // Start is called before the first frame update
    public override void Start()
    {
        // Aseta yksilöllinen sprite ennen EnemyHealth-luokan Start-logiikan kutsumista
        enemySprite = Resources.Load<Sprite>("PinkBearAvatar");
        monsterName = "Pink Bear";
        monsterLevel = 2;

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
        Debug.Log("PinkBear revived with special behavior!");
    }

}
