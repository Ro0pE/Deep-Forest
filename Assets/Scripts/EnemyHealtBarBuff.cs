using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.UI;

public class EnemyHealthBarBuff : MonoBehaviour
{

    public Image buffIcon; // Buffin kuvake
    public string buffName;
    

    void Start()
    {
        
    }


    public void Initialize(Buff buff)
    {
        buffIcon.sprite = buff.buffIcon;
       
    }
}
