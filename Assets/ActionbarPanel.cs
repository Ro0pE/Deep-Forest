using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ActionbarPanel : MonoBehaviour
{
    public GameObject infoPanel;
    public TextMeshProUGUI infoPanelText;
    public int blinkCount = 3; // Kuinka monta kertaa teksti välkkyy
    public float blinkInterval = 0.1f; // Aika yhden välähdyksen välillä
    private Coroutine blinkCoroutine; // Tallennetaan coroutine viite

    public List<ActionbarSlot> actionbarSlots;  // Lista action barin sloteista
    public Image castBar; // Viittaus CastBar-kuvakkeeseen
    public TextMeshProUGUI castTimeText; // Viittaus CastBarin ajan näyttöön
    private EnemyHealth targetedEnemy;
    private Dictionary<int, bool> isSkillOnCooldown = new Dictionary<int, bool>(); // Cooldown-tila
    public PlayerAttack playerAttack;
    private PlayerMovement playerMovement;
    private PlayerStats playerStats;
    public PlayerHealth playerHealth;
    public GameObject projectilePrefab; // Projektiili-prefab
    public GameObject frostSpellPrefab;
    public GameObject shadowBoltPrefab;
    public GameObject healingWavePrefab;
    public GameObject earthSpikesPrefab;
    public Transform castPoint; // Paikka, josta projektiili syntyy

    void Start()
    {
        playerStats = FindObjectOfType<PlayerStats>();
        playerAttack = FindObjectOfType<PlayerAttack>();
        playerMovement = FindObjectOfType<PlayerMovement>();
        playerHealth = FindObjectOfType<PlayerHealth>();
        actionbarSlots = new List<ActionbarSlot>(GetComponentsInChildren<ActionbarSlot>());
        castBar.gameObject.SetActive(false);
        //infoPanel.SetActive(false);
        if (castBar != null)
        {
            castBar.fillAmount = 1f; // Aseta CastBar täydeksi alussa
            castBar.gameObject.SetActive(false); // Piilota CastBar aluksi
        }
    }

    // Funktio, joka liittää skillin slottiin
    public void AssignSkillToSlot(Skill skill, int slotIndex)
    {
        Debug.Log("Assingin skillslots");
        if (slotIndex >= 0 && slotIndex < actionbarSlots.Count)
        {
            actionbarSlots[slotIndex].SetSkill(skill);
        }
    }
        // Funktio, joka käyttää skillia actionbarin slotista
public void UseSkill(int skillIndex)
{
    Debug.Log("use skill");
    
    ActionbarSlot slot = actionbarSlots[skillIndex];
    if (slot == null)
    {
        Debug.LogError($"No slot found at index {skillIndex}.");
        return;
    }
 
    if (slot.assignedSkill == null || string.IsNullOrEmpty(slot.assignedSkill.skillName))
    {
        Debug.Log("Slotti on tyhjä");
        Debug.LogWarning("No valid skill assigned to this slot.");
        return;
    }
    if (isSkillOnCooldown.ContainsKey(skillIndex) && isSkillOnCooldown[skillIndex])
    {
        Debug.Log("Skill is on cooldown, please wait.");
        return;
    }
    if (playerAttack.targetedEnemy != null)
    {
        float distanceToEnemy = Vector3.Distance(playerAttack.transform.position, playerAttack.targetedEnemy.transform.position);
        if (distanceToEnemy > playerAttack.castingRange)
        {
            Debug.Log("Too far!");

            ShowInfoText("Out of range!");
            return;
    }
    }


    // Tarkista, onko taito käytettävissä
    
    if (playerAttack.isCasting) // Jos taikuus on jo käynnissä, estetään uuden käynnistäminen
    {
        Debug.Log("Already casting a skill, please wait.");
        return;
    }

    float manaCost = slot.assignedSkill.manaCost;
    float speed = slot.assignedSkill.castTime;
    if (playerHealth.currentMana < manaCost)
    {
        Debug.Log("Not enough mana.");
        return;
    }

    // Käynnistä Coroutines ja aseta isCasting päälle
    playerAttack.isCasting = true;

    if (speed > 0)
    {
        StartCoroutine(CastSkill(skillIndex, speed - playerStats.castSpeed, manaCost, slot));
    }
    else
    {
        ExecuteImmediateSkill(skillIndex, manaCost, slot);
    }
    StartCoroutine(SkillCooldown(skillIndex, slot.assignedSkill.cooldown));
}
    private IEnumerator SkillCooldown(int skillIndex, float cooldownTime)
    {
        isSkillOnCooldown[skillIndex] = true;

        ActionbarSlot slot = actionbarSlots[skillIndex];
        float elapsed = 0f;

        while (elapsed < cooldownTime)
        {
            elapsed += Time.deltaTime;

            if (slot != null && slot.assignedSkill != null)
            {
                float remaining = Mathf.Clamp(cooldownTime - elapsed, 0, cooldownTime);
                slot.cooldownText.text = $"{remaining:F1}s";
                if (slot.cooldownOverlay != null)
                {
                    slot.cooldownOverlay.fillAmount = remaining / cooldownTime;
                }
            }

            yield return null;
        }

        isSkillOnCooldown[skillIndex] = false;
        if (slot != null && slot.assignedSkill != null)
        {
            slot.cooldownText.text = "Ready!";
            if (slot.cooldownOverlay != null)
            {
                slot.cooldownOverlay.fillAmount = 0f;
            }
        }
    }



    private IEnumerator CastSkill(int skillIndex, float castTime, float manaCost, ActionbarSlot slot)
    {
       Debug.Log("meneekö castiin asti");


        // Varmistetaan, että pelaaja on maassa
       /* if (!playerMovement.isPlayerGrounded())
        {
            Debug.Log("Player is not grounded.");
            playerAttack.isCasting = false;
            yield break;
        } */

        // Tarkistetaan, onko vihollista valittu (vain Damage-taitoja varten)
        if (playerAttack.targetedEnemy == null && slot.assignedSkill.spellType == SpellType.Damage)
        {
            Debug.Log("No target enemy for damage spell.");
            playerAttack.isCasting = false;
            yield break;
        }
        if (playerAttack.targetedEnemy.isDead)
        {
            Debug.Log("Enemy IS DEAD!");
            playerAttack.isCasting = false;
            yield break;
        }

        // Aktivoidaan CastBar
        playerHealth.UseMana(manaCost);
        castBar.fillAmount = 1f;
        castBar.gameObject.SetActive(true);

        // Castausaika
        float elapsed = 0f;
        while (elapsed < castTime)
        {
            playerMovement.SetPlayerSpeed(0.0f);
            elapsed += Time.deltaTime;
            castBar.fillAmount = 1f - (elapsed / castTime); // Päivitä CastBar
            if (castTimeText != null)
            {
                castTimeText.text = $"{Mathf.Max(0f, castTime - elapsed):0.0} s"; // Näytä jäljellä oleva aika
            }
            yield return null;
        }

        // Castaus valmis
        SpawnProjectile(slot);
        castBar.fillAmount = 0f;
        castBar.gameObject.SetActive(false);

        playerAttack.isCasting = false;

        // Valitse ja ammu projektiili


        // Vapauta isCasting ja palauta pelaajan nopeus
   
        playerMovement.ReturnPlayerSpeed();
        slot.UseSkill(); // Käytä skilliä (esim. cooldown tai muu logiikka)
        
    }
        private void ExecuteImmediateSkill(int skillIndex, float manaCost, ActionbarSlot slot)
    {
        // Käynnistä skilli ilman castausaikaa
        playerHealth.UseMana(manaCost);
        SpawnProjectile(slot); // Aseta ja ammu projektiili

        
        playerMovement.ReturnPlayerSpeed();
        slot.UseSkill(); // Käytä skilliä (esim. cooldown tai muu logiikka)
    }

    private void SpawnProjectile(ActionbarSlot slot)
    {
        // Peruslogiikka projektiilin spawnaukseen
        Vector3 spawnPosition = castPoint.position + castPoint.forward * 0.5f + castPoint.up * 3.5f;

        // Erilaisia taikoja
        if (slot.assignedSkill.spellType == SpellType.Heal)
        {
            GameObject projectile = Instantiate(healingWavePrefab, spawnPosition, Quaternion.identity);
            projectile.GetComponent<Projectile>().Initialize(playerHealth.transform);
        }
        else if (slot.assignedSkill.element == Element.Water)
        {
            GameObject projectile = Instantiate(frostSpellPrefab, spawnPosition, Quaternion.identity);
            projectile.GetComponent<Projectile>().Initialize(playerAttack.targetedEnemy.transform);
        }
        else if (slot.assignedSkill.element == Element.Shadow)
        {
            GameObject projectile = Instantiate(shadowBoltPrefab, spawnPosition, Quaternion.identity);
            projectile.GetComponent<Projectile>().Initialize(playerAttack.targetedEnemy.transform);
        }
        else if (slot.assignedSkill.element == Element.Earth)
        {
            GameObject projectile = Instantiate(earthSpikesPrefab, spawnPosition, Quaternion.identity);
            projectile.GetComponent<Projectile>().Initialize(playerAttack.targetedEnemy.transform);
        }
        else
        {
           // GameObject projectile = Instantiate(projectilePrefab, spawnPosition, Quaternion.identity);
           // projectile.GetComponent<Projectile>().Initialize(targetedEnemy.transform);
        }
    }


    public void SetTargetedEnemy(EnemyHealth enemy)
    {
        targetedEnemy = enemy;
        Debug.Log("Target enemy " + targetedEnemy);
    }
    private IEnumerator BlinkText(string info)
    {
        infoPanel.SetActive(true);
        infoPanelText.text = info;

        for (int i = 0; i < blinkCount; i++)
        {
            // Näkyvä
            infoPanel.SetActive(true);
            yield return new WaitForSeconds(blinkInterval);

            // Piilotettu
            infoPanel.SetActive(false);
            yield return new WaitForSeconds(blinkInterval);
        }

        // Piilotetaan teksti viimeisen kerran
        infoPanelText.text = "";
        infoPanel.SetActive(false);
        blinkCoroutine = null; // Nollataan coroutine-viite
    }
    public void ShowInfoText(string info)
    {
        if (blinkCoroutine != null)
        {
            StopCoroutine(blinkCoroutine);  // Käytä oikeaa viitettä
        }
        blinkCoroutine = StartCoroutine(BlinkText(info));
    }

    
}
