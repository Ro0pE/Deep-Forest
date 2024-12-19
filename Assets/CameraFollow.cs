using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player; // Pelaajan transform
    public Vector3 offset; // Kamera etäisyys pelaajasta
    public float smoothSpeed = 0.125f; // Sujuvuuden säätö

    void Start()
    {
        // Aseta offset, jos sitä ei ole asetettu
        if (offset == Vector3.zero)
        {
            offset = new Vector3(0, 2, -5); // Esimerkki offset
        }
    }

    void LateUpdate()
    {
        // Laske uusi sijainti pelaajan kanssa
        Vector3 desiredPosition = player.position + player.TransformDirection(offset); // Käytä pelaajan käännöstä
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;

        // Aseta kamera katsomaan pelaajan suuntaan
        transform.LookAt(player.position + Vector3.up * 1.5f); // Suuntaa kameran pelaajan yläpuolelle
    }
}
