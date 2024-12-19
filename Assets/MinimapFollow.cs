using UnityEngine;

public class MinimapFollow : MonoBehaviour
{
    public Transform player; // Viittaus pelaajan objektiin

    void LateUpdate()
    {
        Vector3 newPosition = player.position;
        newPosition.y = transform.position.y; // Pidä korkeus samana
        transform.position = newPosition;
    }
}
