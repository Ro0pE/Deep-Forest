using UnityEngine;
using TMPro;

public class NPC : MonoBehaviour
{
    public string name; // NPC:n nimi
    public Transform nameParent; // Nimikyltti
    public TextMeshProUGUI npcNameText; // Nimikyltti
    public Camera playerCamera; // Pelaajan kamera
    public Vector3 nameOffset = new Vector3(-12.5f, 4f, -16f); // Offset NPC:n pään yläpuolelle

    void Start()
    {
        if (npcNameText != null)
        {
            npcNameText.text = name; // Aseta NPC:n nimi tekstiin
        }
    }

    void Update()
    {
        if (npcNameText != null && nameParent != null && playerCamera != null)
        {
            // Aseta nimikyltin sijainti NPC:n yläpuolelle offsetilla
            Vector3 worldPosition = transform.position + nameOffset;
            nameParent.position = worldPosition;

            // Aseta nimikyltti kameraa kohti
            nameParent.rotation = Quaternion.LookRotation(nameParent.position - playerCamera.transform.position);

            // Käännä UI oikeinpäin
            //nameParent.Rotate(0, 180, 0);
        }
    }
}
