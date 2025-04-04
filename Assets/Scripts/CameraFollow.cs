using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player; // Pelaajan transform
    public Vector3 offset; // Kameraetäisyys pelaajasta
    public float smoothSpeed = 0.125f; // Sujuvuuden säätö
    public Transform cameraPivot; // Pivot, jonka ympärillä kamera pyörii
    public float verticalLookRotation; // Kamera pystysuuntainen kulma
    public float rotationSpeed = 2f; // Hiiren herkkyys

    private void Start()
    {
        // Aseta offset, jos sitä ei ole asetettu
        if (offset == Vector3.zero)
        {
            offset = new Vector3(0, 2, -5); // Esimerkki offset
        }

        // Luo pivot, jos sitä ei ole asetettu
        if (cameraPivot == null)
        {
            GameObject pivotObject = new GameObject("CameraPivot");
            cameraPivot = pivotObject.transform;
            cameraPivot.position = player.position;
            cameraPivot.parent = player; // Pivot seuraa pelaajaa
        }

        transform.position = cameraPivot.position + offset;
        transform.LookAt(cameraPivot);
    }

    private void LateUpdate()
    {
        HandleCameraRotation();

        // Laske kameran uusi sijainti pivotin ympärillä
        Vector3 desiredPosition = cameraPivot.position + cameraPivot.TransformDirection(offset);
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

        transform.position = smoothedPosition;
        transform.LookAt(cameraPivot.position + Vector3.up * 1.5f); // Katso pelaajan yläpuolelle
    }

private void HandleCameraRotation()
{
    
 if (Input.GetMouseButton(1))
    {
        // Liike horisontaalisesti (hiiri vasen tai oikea)
        float horizontal = Input.GetAxis("Mouse X") * rotationSpeed;
        // Liike pystysuunnassa (hiiri ylös tai alas)
        float vertical = Input.GetAxis("Mouse Y") * rotationSpeed;

        // Päivitä pystysuuntainen katselukulma
        verticalLookRotation -= vertical;
        verticalLookRotation = Mathf.Clamp(verticalLookRotation, -40f, 60f); // Rajoita pystykulma

        // Päivitä pivotin rotaatio
        cameraPivot.localRotation = Quaternion.Euler(verticalLookRotation, 0f, 0f);

        // Pyöritä pelaajaa vain vaakasuunnassa
        player.Rotate(Vector3.up * horizontal);
    }
}



}
