using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Liikkeen asetukset")]
    public float moveSpeed = 10f; // Pelaajan liikenopeus
    public float jumpForce = 35f; // Hyppyvoima
    public bool isGrounded; // Onko pelaaja maassa?
    public float originalSpeed = 10f;
    public float castingMoveSpeed = 5f;
    private float addedSpeed = 0f;
    private float reducedSpeed = 0f;
    public LayerMask groundLayer; // Maan tarkistamiseen

    private CharacterController controller;
    private Vector3 velocity;
    public float fallMultiplier = 5f; // Nopeampi putoaminen

    [Header("Hiiren ja kameran asetukset")]
    public Transform cameraTransform; // Pelaajan kamera
    public Camera mainCamera; // Pääkamera Field of View -muutoksiin
    public float mouseSensitivity = 2f;
    public float zoomSpeed = 10f; // Zoomauksen nopeus
    public float minFOV = 10f; // Minimi Field of View
    public float maxFOV = 95f; // Maksimi Field of View
    private float verticalLookRotation;
    public bool showEnemyHealthBar = false;
    public PlayerAttack playerAttack;

    [Header("Animaatio")]
    public Animator animator;

    private void Start()
    {
        playerAttack = FindObjectOfType<PlayerAttack>();
        // Alustukset
        controller = GetComponent<CharacterController>();
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }

        if (mainCamera == null)
        {
            mainCamera = Camera.main; // Aseta oletuskamera, jos ei ole määritelty
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
        HandleZoom();

        if (Input.GetKeyDown(KeyCode.V))
        {
            showEnemyHealthBar = !showEnemyHealthBar;
            Debug.Log("Health bar: " + showEnemyHealthBar);
        }
    }
    public bool IsPlayerMoving()
    {
        return Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0;
    }

    private void HandleMovement()
    {
        if (playerAttack.isCasting == true || playerAttack.isChanneling == true || playerAttack.isAttacking)
        {
            return;
        }
        
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
        if (playerAttack.isCasting == true)
        {
            return;
        }
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
            if (playerAttack.attackRange <= 15)
            {
                if (playerAttack.isAttacking)
                {
                    animator.SetTrigger("JumpMeleeAttacking");
                }
                else
                {
                     animator.SetTrigger("JumpMelee");
                }
            }
            else if (playerAttack.attackRange > 30)
            {
                if (playerAttack.isAttacking)
                {
                    animator.SetTrigger("JumpRangedAttack");
                }
                else
                {
                     animator.SetTrigger("JumpRanged");
                }
                
            }
            else 
            {
                animator.SetTrigger("Jump");
            }
            
           
            
            velocity.y = Mathf.Sqrt(jumpForce * -2f * Physics.gravity.y); // Nopeampi hyppy
        }

        // Putoaminen
        if (!isGrounded)
        {
            velocity.y += Physics.gravity.y * fallMultiplier * Time.deltaTime; // Nopeampi putoaminen
        }
        else
        {
            velocity.y = Mathf.Max(velocity.y, -2f); // Varmista, ettei nopeus yläty liian suureksi
        }

        // Liikuta pelaajaa
        controller.Move(velocity * Time.deltaTime);
        //animator.SetBool("isJumping", false);
    }

    private void HandleCamera()
    {
        if (Input.GetMouseButton(1)) // Oikea hiiren nappi
        {
            float horizontal = Input.GetAxis("Mouse X") * mouseSensitivity;
            float vertical = Input.GetAxis("Mouse Y") * mouseSensitivity;

            verticalLookRotation -= vertical;
            verticalLookRotation = Mathf.Clamp(verticalLookRotation, -80f, 80f);

            transform.rotation *= Quaternion.Euler(0f, horizontal, 0f);
            cameraTransform.localRotation = Quaternion.Euler(verticalLookRotation, 0f, 0f);
        }
    }

    private void HandleZoom()
    {
        float scrollInput = Input.GetAxis("Mouse ScrollWheel");
        if (scrollInput != 0)
        {
            mainCamera.fieldOfView -= scrollInput * zoomSpeed;
            mainCamera.fieldOfView = Mathf.Clamp(mainCamera.fieldOfView, minFOV, maxFOV);
        }
    }

    public void SetPlayerSpeed(float speed)
    {
        addedSpeed += speed; // Lisää buffien arvoa
        UpdateMoveSpeed();
    }

    public void ReducePlayerSpeed(float speed)
    {
        reducedSpeed = speed; // Asetetaan suoraan, ei lisätä
        UpdateMoveSpeed();
    }

    public void ReturnPlayerSpeed(float speed) // Poistaa vain tietyn määrän debuffeista
    {
        reducedSpeed = 0; // Vähentää debuffeista annetun määrän
        UpdateMoveSpeed();
    }

    public void ReturnSpeedBuff(float speed) // Poistaa vain tietyn määrän buffeista
    {
        addedSpeed -= speed; // Vähentää buffeista annetun määrän
        UpdateMoveSpeed();
    }

    private void UpdateMoveSpeed()
    {
        moveSpeed = originalSpeed + addedSpeed - reducedSpeed;
    }



    public bool IsPlayerGrounded()
    {
        return isGrounded;
    }
}
