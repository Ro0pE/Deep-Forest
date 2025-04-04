using UnityEngine;

public class AOEAttack : MonoBehaviour
{
    public GameObject aoeCirclePrefab;  // Ympyrä prefab
    private GameObject aoeCircle;       // Instansioitu ympyrä
    private bool isAOEActive = false;   // Tarkistaa, onko AOE aktiivinen
    private Camera mainCamera;          // Kamera, jota käytetään hiiren sijainnin saamiseen
    public Transform player;            // Pelaajan sijainti
    public float aoeRadius = 5f;        // AOE-alueen säde (esim. 5 yksikköä)
    public LayerMask enemyLayer;        // Layer, joka tunnistaa viholliset

    void Start()
    {
        mainCamera = Camera.main;  // Haetaan pääkamera
    }

    void Update()
    {
        // Kun pelaaja painaa Q, luodaan AOE ympyrä
        if (Input.GetKeyDown(KeyCode.Q) && !isAOEActive)
        {
            CreateAOECircle();
        }

        // Jos AOE-ympyrä on aktiivinen, seuraa hiiren liikkeitä
        if (isAOEActive)
        {
            FollowMouse();
            
            // Kun hiiren vasen painike painetaan, suoritetaan AOE hyökkäys
            if (Input.GetMouseButtonDown(0))
            {
                PerformAOEAttack();
            }
        }
    }

    // Luo AOE-ympyrän pelaajan alle
    void CreateAOECircle()
    {
        // Asetetaan ympyrä pelaajan sijainnin alle, mutta varmistetaan, että z-koordinaatti on nolla
        Vector3 aoePosition = new Vector3(player.position.x, player.position.y, player.position.z);

        // Luo ympyrä ja aseta se pelaajan lapseksi
        aoeCircle = Instantiate(aoeCirclePrefab, aoePosition, Quaternion.identity);
        aoeCircle.transform.SetParent(player.transform);

        // Varmistetaan, että SphereCollider on päällä
        SphereCollider sphereCollider = aoeCircle.GetComponent<SphereCollider>();
        if (sphereCollider != null)
        {
            sphereCollider.enabled = true;
        }

        isAOEActive = true;
        aoeCircle.SetActive(true); // Varmistetaan, että ympyrä on näkyvissä
    }


    // Seuraa hiiren liikkeitä ja päivittää ympyrän sijainnin
    void FollowMouse()
    {
        // Hakee hiiren maailman koordinaatit
        Vector3 mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;  // Estetään z-akselin liike, koska haluamme litteän ympyrän
        aoeCircle.transform.position = mousePos;
    }

    // Suorittaa AOE hyökkäyksen, kun hiiren vasenta painiketta painetaan
    void PerformAOEAttack()
    {
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(aoeCircle.transform.position, aoeRadius, enemyLayer);

        foreach (Collider2D enemy in hitEnemies)
        {
            // Tässä käsitellään, kuinka vahinko annetaan viholliselle
            Debug.Log("AOE hit: " + enemy.name);
            // Esimerkiksi: enemy.GetComponent<Enemy>().TakeDamage(damageAmount);
        }

        Destroy(aoeCircle);  // Tuhoa AOE-ympyrä hyökkäyksen jälkeen
        isAOEActive = false;  // Poistetaan AOE aktiivinen tila
    }

    // Piirretään AOE-ympyrä näkyviin editorissa, jos tarpeen
    void OnDrawGizmos()
    {
        if (aoeCircle != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(aoeCircle.transform.position, aoeRadius);
        }
    }
}
