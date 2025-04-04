using UnityEngine;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 200;
    public int baseHealth = 200;
    public int currentHealth;
    public int maxMana = 200;
    public int baseMana = 200;
    public int currentMana;
    public float manaRegen = 0f;
    public float healthRegen = 0f;
    public float defence = 0;
    public float dodgeChance = 0;
    private float regenTimer = 0f;
    public int takeDamageAmount = 0;
    private float regenInterval = 10f; // Aseta 10 sekunnin välein kutsuttava aika
    public BuffManager buffManager;
    private Animator animator;
    public PlayerHealthBar playerHealthBar; // Viittaus PlayerHealthBariin
    public Transform spawnPoint;
    public AudioSource audioSource;
    public AudioClip dodgeHitSound;
    public AudioClip getHitSound;

    void Start()
    {
        buffManager = FindObjectOfType<BuffManager>();
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

    public void Heal(int amount)
    {
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth); // Parantaa ja rajoittaa terveyden max-arvoon
        playerHealthBar.ShowTextForDuration(playerHealthBar.hpRegenText, amount);

    }
    public void RestoreMana(int manaAmount)
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
        maxHealth = Mathf.RoundToInt(maxHealth * 1.1f); // Lisää terveyttä
        maxMana = Mathf.RoundToInt(maxMana * 1.05f); // Lisää manaa
    }

    public void ManaRegen()
    {

        
        currentMana = Mathf.RoundToInt(currentMana + manaRegen);
        if (currentMana > maxMana)
        {
            currentMana = maxMana;
        }

        // Näytetään mana regen teksti
        playerHealthBar.ShowTextForDuration(playerHealthBar.manaRegenText, manaRegen);
        
    }

    public void HealthRegen()
    {
        currentHealth = Mathf.RoundToInt(currentHealth + healthRegen);
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }

        // Näytetään hp regen teksti
        playerHealthBar.ShowTextForDuration(playerHealthBar.hpRegenText, healthRegen);
    }

    public void UseMana(int mana)
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
           
            audioSource.PlayOneShot(getHitSound);// Soittaa AudioSourceen asetetun klipin
        }
        else
        {
            Debug.LogWarning("Ääniklipin toisto epäonnistui. Varmista että AudioSource ja ääni ovat asetettu.");
        }
    }
    private IEnumerator ResetHitTrigger()
    {
        yield return new WaitForSeconds(0.1f); // Odota vähän ennen kuin resetoi
        animator.ResetTrigger("isHit");
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
        takeDamageAmount = Mathf.RoundToInt(damage * Mathf.Abs(calculateDef));
        animator.SetTrigger("isHit");
        PlayGetHitSound();
        Buff huntersResilienceBuff = buffManager.activeBuffs.Find(b => b.name == "HuntersResilience");
        Debug.Log("Damagea tulee " + takeDamageAmount);
        if (huntersResilienceBuff != null)
        {
            // Jos HuntersResilience buffi on aktiivinen, tee tietty toiminto (esimerkiksi puolita vahinko)
            takeDamageAmount = Mathf.RoundToInt(takeDamageAmount * 0.2f); // Esimerkki: Vahinko puolittuu
            Debug.Log("HuntersResilience buffi on aktiivinen, vahinkoa vaan 20%!");
            Debug.Log("Vähennettyä damagea tulee " + takeDamageAmount);
        }
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
        StartCoroutine(ResetHitTrigger());
        }
    }
    public void TakeSpellDamage(int damage)
    {
        float calculateDef = (defence/100) - 1; 
        takeDamageAmount = Mathf.RoundToInt(damage * Mathf.Abs(calculateDef));
        //animator.SetTrigger("isHit");
        //PlayGetHitSound(); // VAIHDA ÄÄNI
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
