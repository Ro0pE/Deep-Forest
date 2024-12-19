using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Liikkeen asetukset")]
    public float moveSpeed = 10f; // Pelaajan liikenopeus
    public float jumpForce = 35f; // Hyppyvoima
    private bool isGrounded; // Onko pelaaja maassa?
    public float originalSpeed = 10f;
    public float castingMoveSpeed = 5f;
    public LayerMask groundLayer; // Maan tarkistamiseen

    private CharacterController controller;
    private Vector3 velocity;
    public float fallMultiplier = 5f; // Nopeampi putoaminen

    [Header("Hiiren ja kameran asetukset")]
    public Transform cameraTransform; // Pelaajan kamera
    public float mouseSensitivity = 2f;
    private float verticalLookRotation;

    [Header("Animaatio")]
    public Animator animator;

    private void Start()
    {

        // Alustukset
        controller = GetComponent<CharacterController>();
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }

        // Lukitsee hiiren
        //Cursor.lockState = CursorLockMode.Locked;
        //Cursor.visible = false;
    }

    private void Update()
    {
   
        HandleMovement();
        HandleJump();
        HandleCamera();
    }

    private void HandleMovement()
    {
        // Liikkumistiedot
        float moveHorizontal = Input.GetAxisRaw("Horizontal"); // Vasemmalle/oikealle liike
        float moveVertical = Input.GetAxisRaw("Vertical"); // Eteenpäin/taaksepäin liike

        // Laske liikesuunta (eteenpäin/taaksepäin ja vasemmalle/oikealle)
        Vector3 moveDirection = transform.forward * moveVertical + transform.right * moveHorizontal;

        // Liikuta pelaajaa CharacterControllerin avulla
        controller.Move(moveDirection * moveSpeed * Time.deltaTime);

        // Lasketaan animaatioparametrit MoveSpeed
        float currentSpeed = moveDirection.magnitude; // MoveDirectionin pituus
        if (moveVertical != 0 && moveHorizontal != 0)
        {
            currentSpeed *= 0.7071f; // Kerroin, joka tasoittaa nopeuden, kun liikkuu molempiin suuntiin (koska sinin ja kosinin yhdistelmä on noin 0.7071)
        }
        currentSpeed *= moveSpeed; // Kerro liikesuunnan nopeudella
        animator.SetFloat("MoveSpeed", currentSpeed); // Päivitetään MoveSpeed (käytetään Blend Treessä)

        // Päivitetään MoveVertical (eteenpäin/taaksepäin liikkuminen)
        if (moveVertical > 0) // Eteenpäin liikkuminen
        {
            animator.SetFloat("MoveVertical", 1); // MoveVertical = 1
        }
        else if (moveVertical < 0) // Taaksepäin liikkuminen
        {
            animator.SetFloat("MoveVertical", -1); // MoveVertical = -1
        }
        else // Ei liikettä pystysuunnassa
        {
            animator.SetFloat("MoveVertical", 0); // MoveVertical = 0
        }

        // Päivitetään MoveHorizontal (vasemmalle/oikealle liikkuminen)
        if (moveHorizontal > 0) // Oikealle liikkuminen
        {
            animator.SetFloat("MoveHorizontal", 1); // MoveHorizontal = 1 (Oikea)
        }
        else if (moveHorizontal < 0) // Vasemmalle liikkuminen
        {
            animator.SetFloat("MoveHorizontal", -1); // MoveHorizontal = -1 (Vasen)
        }
        else // Ei liikettä vaakasuunnassa
        {
            animator.SetFloat("MoveHorizontal", 0); // MoveHorizontal = 0 (Ei strafea)
        }
    }



    private void HandleJump()
    {
        // Tarkistetaan, onko pelaaja maassa
        if (Physics.Raycast(transform.position, Vector3.down, 1f, groundLayer))
        {
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }

        // Jos pelaaja on maassa ja liikkuu alaspäin, nollaa pystysuora nopeus
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // Varmistetaan, ettei pelaaja jää leijumaan
        }

        // Hyppy
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
         
            // Lisää hyppyvoimaa ja tee siitä nopeampi
            velocity.y = Mathf.Sqrt(jumpForce * -2f * Physics.gravity.y); // Nopeampi hyppy
        }

        // Putoaminen
        if (!isGrounded) 
        {
            // Nopeampi putoaminen
            velocity.y += Physics.gravity.y * fallMultiplier * Time.deltaTime; 
        }
        else
        {
            // Jos pelaaja on maassa, pysyy nopeus vähintään -2 (ei leiju)
            velocity.y = Mathf.Max(velocity.y, -2f);
        }

        // Liikuta pelaajaa
        controller.Move(velocity * Time.deltaTime);
    }


    private void HandleCamera()
    {
        // Tarkista, onko oikea hiiren näppäin painettuna
        if (Input.GetMouseButton(1))  // 1 on oikea hiiren näppäin
        {
            // Hiiren liike kameran pyörittämiseksi (vain jos oikea hiiren näppäin on painettu)
            float horizontal = Input.GetAxis("Mouse X") * mouseSensitivity;
            float vertical = Input.GetAxis("Mouse Y") * mouseSensitivity;

            // Rajoita pystysuuntaista kameran kääntymistä
            verticalLookRotation -= vertical;
            verticalLookRotation = Mathf.Clamp(verticalLookRotation, -100f, 100f);

            // Aseta kameran paikallinen rotaatio (pystysuunta)
            cameraTransform.localRotation = Quaternion.Euler(verticalLookRotation, 0f, 0f);

            // Käännä pelaajan suuntaa vaakasuunnassa
            transform.rotation *= Quaternion.Euler(0f, horizontal, 0f);
        }
    }

    private void ResetAnimation()
    {
        // Resetoi animaatiot, jos pelaaja ei ole maassa

    }



    public void SetPlayerSpeed(float speed)
    {
        moveSpeed = speed;
    }

    public void ReturnPlayerSpeed()
    {
        moveSpeed = originalSpeed;
    }

    public bool IsPlayerGrounded()
    {
        return isGrounded;
    }
}
