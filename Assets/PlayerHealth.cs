using UnityEngine;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    public float maxHealth = 200f;
    public float baseHealth = 200f;
    public float currentHealth;
    public float maxMana = 200f;
    public float baseMana = 200f;
    public float currentMana;
    public float manaRegen = 0f;
    public float healthRegen = 0f;
    public float defence = 0;
    public float dodgeChance = 0;
    private float regenTimer = 0f;
    public float takeDamageAmount = 0f;
    private float regenInterval = 3f; // Aseta viiden sekunnin välein kutsuttava aika
    private Animator animator;
    public PlayerHealthBar playerHealthBar; // Viittaus PlayerHealthBariin
    public Transform spawnPoint;
    public AudioSource audioSource;
    public AudioClip dodgeHitSound;
    public AudioClip getHitSound;

    void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        currentHealth = maxHealth;
        currentMana = maxMana;
        animator = GetComponent<Animator>();
        playerHealthBar = FindObjectOfType<PlayerHealthBar>(); // Etsii pelaajan terveyspalkin
        if (audioSource == null)
        {
            Debug.LogError("AudioSource komponentti puuttuu pelaajalta!");
        }

    }

    void Update()
    {
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
        if (currentMana > maxMana)
        {
            currentMana = maxMana;
        }
        regenTimer += Time.deltaTime;

        if (regenTimer >= regenInterval)
        {
            if(currentHealth < maxHealth)
            {
                HealthRegen();
            }
            if (currentMana < maxMana)
            {
                ManaRegen();
            }
            
            regenTimer = 0f; // Nollaa ajastin seuraavaa regen-sykliä varten
        }
    }

    public void Heal(float amount)
    {
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth); // Parantaa ja rajoittaa terveyden max-arvoon
        playerHealthBar.ShowTextForDuration(playerHealthBar.hpRegenText, amount);

    }
    public void RestoreMana(float manaAmount)
    {
        currentMana = Mathf.Min(currentMana + manaAmount, maxMana); // Parantaa ja rajoittaa terveyden max-arvoon
        playerHealthBar.ShowTextForDuration(playerHealthBar.manaRegenText, manaAmount);
   

    }
    public bool checkDodge()
    {
        float randomValue = Random.Range(0f, 100f); // Satunnaisluku väliltä 0-100
        return randomValue < dodgeChance; // Kriittinen osuma, jos satunnaisluku on alle critChance:n
    }

    public void setMaxHealthandMaxMana() // uusi terveys tasonnousun jälkeen
    {
        maxHealth = maxHealth * 1.1f; // Lisää terveyttä
        maxMana = maxMana * 1.05f; // Lisää manaa
    }

    public void ManaRegen()
    {

        
        currentMana = currentMana + manaRegen;
        if (currentMana > maxMana)
        {
            currentMana = maxMana;
        }

        // Näytetään mana regen teksti
        playerHealthBar.ShowTextForDuration(playerHealthBar.manaRegenText, manaRegen);
        
    }

    public void HealthRegen()
    {
        currentHealth = currentHealth + healthRegen;
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }

        // Näytetään hp regen teksti
        playerHealthBar.ShowTextForDuration(playerHealthBar.hpRegenText, healthRegen);
    }

    public void UseMana(float mana)
    {
        if (currentMana < mana)
        {
           
        }
        else
        {
            currentMana -= mana;
        }
    }
    private void PlayDodgeSound()
    {
        if (audioSource != null && dodgeHitSound != null)
        {
            Debug.Log("Playing dodge sound");
            audioSource.PlayOneShot(dodgeHitSound);// Soittaa AudioSourceen asetetun klipin
        }
        else
        {
            Debug.LogWarning("Ääniklipin toisto epäonnistui. Varmista että AudioSource ja ääni ovat asetettu.");
        }
    }
    private void PlayGetHitSound()
    {
        if (audioSource != null && dodgeHitSound != null)
        {
            Debug.Log("Get hit sound");
            audioSource.PlayOneShot(getHitSound);// Soittaa AudioSourceen asetetun klipin
        }
        else
        {
            Debug.LogWarning("Ääniklipin toisto epäonnistui. Varmista että AudioSource ja ääni ovat asetettu.");
        }
    }

    public void TakeDamage(float damage)
    {
        if (checkDodge())
        {
           PlayDodgeSound();
        }
        else
        {

        float calculateDef = (defence/100) - 1; 
        takeDamageAmount = damage * Mathf.Abs(calculateDef);
        animator.SetTrigger("isHit");
        PlayGetHitSound();
        if (takeDamageAmount < 0)
        {
            takeDamageAmount = 0;
        }
        currentHealth -= takeDamageAmount;
        float correctDamageText = (takeDamageAmount);
    
        
        // Näytetään damage teksti
        playerHealthBar.ShowTextForDuration(playerHealthBar.takeDamageText, correctDamageText);

        if (currentHealth <= 0)
        {
            
            animator.SetTrigger("Death");
            RespawnPlayer();
            //Die();
        }
        }
    }

    void Die()
    {
        // Kuoleman käsittely, esim. animoitu kuolema tai esineen tiputtaminen
        Destroy(gameObject);
    }
    private void RespawnPlayer()
{

    if (spawnPoint != null)
    {
        transform.position = spawnPoint.position;
        transform.rotation = spawnPoint.rotation;
    }

    // Palautetaan täysi HP ja SP
    currentHealth = maxHealth;
    currentMana = maxMana;

    Debug.Log("Player has respawned with full health and SP.");

    // Annetaan pelaajalle pieni "invincibility frame" syntymän jälkeen
    //StartCoroutine(GrantTemporaryInvincibility());
}

    private IEnumerator ShakeEnemy()
    {
        Vector3 originalPosition = transform.position;
        float shakeDuration = 0.2f; // Tärinän kesto
        float shakeMagnitude = 0.1f; // Tärinän voimakkuus
        float elapsed = 0.0f;

        while (elapsed < shakeDuration)
        {
            float x = Random.Range(-2f, 2f) * shakeMagnitude;
            float y = Random.Range(-2f, 12f) * shakeMagnitude;
            transform.position = new Vector3(originalPosition.x + x, originalPosition.y + y, originalPosition.z);

            elapsed += Time.deltaTime;
            yield return null;
        }

        // Palauta alkuperäiseen sijaintiin
        transform.position = originalPosition;
    }
}
