using UnityEngine;
using UnityEngine.UI;

public class ItemEffect : MonoBehaviour
{
    public string effectResourcePath = "UI_Effects/ItemGlow"; // Oletuspolku Resources-kansiossa
    private GameObject effectInstance;

    public void ShowEffect()
    {
        if (effectInstance == null) // Estetään efektin monistuminen
        {
            Debug.Log("Ladataan efekti polusta: " + effectResourcePath);
            GameObject effectPrefab = Resources.Load<GameObject>(effectResourcePath);
            if (effectPrefab != null)
            {
                Debug.Log("Effect löyty");
                Debug.Log("Yritetään instanssioida: " + effectPrefab.name);
                effectInstance = Instantiate(effectPrefab, transform);
                effectInstance.transform.SetAsFirstSibling(); // Asetetaan taustalle
                if (effectInstance == null)
                {
                    Debug.LogError("Instantiate epäonnistui, effectInstance on null!");
                }
                else
                {
                    Debug.Log("Efekti instanssioitu onnistuneesti: " + effectInstance.name);
                    Debug.Log("Efektin sijainti: " + effectInstance.transform.position);
                    Debug.Log("Efektin vanhempi: " + effectInstance.transform.parent);
                }
            }
            else
            {
                Debug.LogError("Efektiä ei löytynyt polusta: " + effectResourcePath);
            }
        }
    }

    public void HideEffect()
    {
        if (effectInstance != null)
        {
            Destroy(effectInstance);
            effectInstance = null;
        }
    }
}
