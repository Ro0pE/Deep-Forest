using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SpellEffect : MonoBehaviour
{
    public GameObject effectPrefab; // Efektin prefab (esim. myrkkypilvi, tulipallo jne.)

    // Luo efektin ja aseta se vihollisen lapseksi
    public void SpawnEffect(Vector3 position, float duration, EnemyHealth target, MonoBehaviour caller)
    {
        if (effectPrefab != null && target != null)
        {
            GameObject effect = Instantiate(effectPrefab, position, Quaternion.identity);
            effect.SetActive(true);
            effect.transform.SetParent(target.transform); // Nyt efekti seuraa vihollista
            if (duration > 0)
            {
                Destroy(effect, duration); // Poista efekti ajan kuluttua
            }
            else
            {
                 caller.StartCoroutine(CheckForDeath(effect, target)); // Käynnistä korutiini kutsujasta
            }
        }
        else
        {
            Debug.LogWarning("Effect prefab ei ole määritelty tai target puuttuu!");
        }
    }

    private IEnumerator CheckForDeath(GameObject effect, EnemyHealth target)
    {
        while (effect != null && target != null)
        {
            if (target.isDead) // Jos vihollinen kuolee, tuhoa efekti
            {
                
                Destroy(effect);
                yield break;
            }
            yield return null; // Odota seuraavaan frameen
        }
    }



}
