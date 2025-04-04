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
    public Inventory playerInventory;

 

    void Start()
    {
        playerStats = FindObjectOfType<PlayerStats>();
        playerInventory = FindObjectOfType<Inventory>();
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
        
        if (slotIndex >= 0 && slotIndex < actionbarSlots.Count)
        {
            actionbarSlots[slotIndex].SetSkill(skill);
        }
    }

        // Funktio, joka käyttää skillia actionbarin slotista
    public void UseSkill(int skillIndex)
    {
        
        
        ActionbarSlot slot = actionbarSlots[skillIndex];
        if (slot == null)
        {
            Debug.LogError($"No slot found at index {skillIndex}.");
            return;
        }
    
        if (slot.assignedSkill == null || string.IsNullOrEmpty(slot.assignedSkill.skillName))
        {
            
            Debug.LogWarning("No valid skill assigned to this slot.");
            return;
        }
        if (isSkillOnCooldown.ContainsKey(skillIndex) && isSkillOnCooldown[skillIndex])
        {
        Debug.Log("Skill on cd");
            return;
        }
        if (playerAttack.targetedEnemy != null)
        {
            float distanceToEnemy = Vector3.Distance(playerAttack.transform.position, playerAttack.targetedEnemy.transform.position);
            if (distanceToEnemy > playerAttack.castingRange && slot.assignedSkill.noTarget == false)
            {
                

                ShowInfoText("Out of range!");
                return;
            }
        }


        // Tarkista, onko taito käytettävissä
        
        if (playerAttack.isCasting || playerAttack.isAttacking) // Jos taikuus on jo käynnissä, estetään uuden käynnistäminen
        {
            Debug.Log("player is casting");
            
            return;
        }

        int manaCost = slot.assignedSkill.manaCost;
        float speed = slot.assignedSkill.castTime;
        if (playerHealth.currentMana < manaCost)
        {
            Debug.Log("No mana");
            return;
        }

        // Käynnistä Coroutines ja aseta isCasting päälle
        if (slot.assignedSkill.spellType != SpellType.Buff)
        {
            playerAttack.isCasting = true;
        }


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



private IEnumerator CastSkill(int skillIndex, float castTime, int manaCost, ActionbarSlot slot)
{
   

    // Tarkistetaan, onko vihollinen kelvollinen kohde
    if (playerAttack.targetedEnemy == null && slot.assignedSkill.spellType == SpellType.Damage)
    {
       Debug.Log("eenemy null tai spelltype ei damg");
        playerAttack.isCasting = false;
        yield break;
    }
    if (playerAttack.targetedEnemy.isDead)
    {
        Debug.Log("Target enemy dead");
        playerAttack.isCasting = false;
        yield break;
    }

    // Aktivoidaan CastBar
    playerHealth.UseMana(manaCost);
    castBar.fillAmount = 1f;
    castBar.gameObject.SetActive(true);
     Debug.Log("Cast lähttee");
    if (slot.assignedSkill.skillType == SkillType.Ranged)
    {
        playerAttack.animator.SetTrigger("isRangedAttacking");
    }

    // Castausaika
    float elapsed = 0f;
    float newCastTime = castTime - playerAttack.attackSpeedReduction;
    if (newCastTime < 0.2f)
    {
        newCastTime = 0.2f;
    }
    while (elapsed < newCastTime)
    {
        elapsed += Time.deltaTime;

        // **Varmista, että animator.speed päivittyy joka frame**
        playerAttack.animator.speed = 1f / playerAttack.attackSpeed;

        castBar.fillAmount = 1f - (elapsed / newCastTime); // Päivitä CastBar
        if (castTimeText != null)
        {
            castTimeText.text = $"{Mathf.Max(0f, newCastTime - elapsed):0.0} s"; // Näytä jäljellä oleva aika
        }

        // Jos pelaaja keskeyttää castauksen, palauta nopeus
        if (!playerAttack.isCasting)
        {
            Debug.Log("Castaus keskeytetty");
            playerAttack.animator.speed = 1f;
            castBar.fillAmount = 0f;
            castBar.gameObject.SetActive(false);
            yield break;
        }

        yield return null;
    }

    // **Palauta animointinopeus ennen projektiilin laukaisua**
    playerAttack.animator.speed = 1f;
    playerAttack.animator.ResetTrigger("isRangedAttacking");

    // Castaus valmis → ammu projektiili
    SpawnProjectile(slot);

    castBar.fillAmount = 0f;
    castBar.gameObject.SetActive(false);
    playerAttack.isCasting = false;
    Debug.Log("skillin pitäs lähtee");
    slot.UseSkill(); // Käytä skilliä (esim. cooldown tai muu logiikka)
}

        private void ExecuteImmediateSkill(int skillIndex, int manaCost, ActionbarSlot slot)
    {
        // Käynnistä skilli ilman castausaikaa
        playerHealth.UseMana(manaCost);
        SpawnProjectile(slot); // Aseta ja ammu projektiili

        
        //playerMovement.ReturnPlayerSpeed();
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
