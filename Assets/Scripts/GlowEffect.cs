using UnityEngine;
using TMPro;
using UnityEngine.UI;  // Lisätty, jotta pääsemme käsiksi Button-komponenttiin
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class GlowEffect : MonoBehaviour
{
    public Animator glowAnimator; // Lisää tämä muuttuja luokan yläosaan Inspectorille näkyväksi
    public Outline slotOutline; 
    public Image itemFrame;
    public SkillButton skillButton;
    // Start is called before the first frame update
    void Start()
    {
        skillButton  = GetComponent<SkillButton>();
        if (glowAnimator != null)
        {
            glowAnimator.enabled = false;
        }
        if (slotOutline != null)
        {
            Debug.Log("outline okk");
            slotOutline.enabled = true;
            slotOutline.effectColor = Color.red;
            itemFrame.color = Color.red;
        }
        if (glowAnimator != null)
        {
            glowAnimator.enabled = true;
            glowAnimator.speed = 0.4f;  // Hitaampi animaatio; säädä arvoa tarpeen mukaan
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (skillButton.skill.isLearned)
        {
        if (glowAnimator != null)
        {
            glowAnimator.enabled = false;
        }
        if (slotOutline != null)
        {
            slotOutline.enabled = true;
            slotOutline.effectColor = Color.green;
            itemFrame.color = Color.green;
        }
        if (glowAnimator != null)
        {
            glowAnimator.enabled = true;
            glowAnimator.speed = 0.4f;  // Hitaampi animaatio; säädä arvoa tarpeen mukaan
        }
        }
        
    }


}
