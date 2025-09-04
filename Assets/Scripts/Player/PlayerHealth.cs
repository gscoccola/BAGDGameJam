using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health")]
    [SerializeField, Range(1, 100)] private int initialMaxHealth = 5;
    [SerializeField, ReadOnly, Range(0, 100)] private int currentHealth;
    [SerializeField, ReadOnly, Range(0, 100)] private int currentMaxHealth;
    [SerializeField] private float timerWithNoDamageToHeal;
    private float _timer;
    
    [Header("Post-Processing")]
    [SerializeField] private Volume takeDamageVolume;
    
    
    [Header("Sounds")]
    [SerializeField] private CustomAudioClip deadSound;
    [SerializeField] private CustomAudioClip[] takeDamageSounds;
    
    [SerializeField] private AudioClip lossSound;
    
    

    private void Start()
    {
        takeDamageVolume.weight = 0;
        currentMaxHealth = initialMaxHealth;
        Heal(currentMaxHealth);
    }

    private void OnDestroy()
    {
        //StopAllCoroutines();
    }

    private void Update()
    {
        if (currentHealth >= initialMaxHealth) return;
        
        _timer += Time.deltaTime;
        if (_timer >= timerWithNoDamageToHeal)
        {
            _timer = 0;
            currentHealth++;
            if (currentHealth > currentMaxHealth) currentHealth = currentMaxHealth;
            
            if (currentHealth == 4) takeDamageVolume.weight = 0.25f;
            else if (currentHealth == 3) takeDamageVolume.weight = 0.5f;
            else if (currentHealth == 2) takeDamageVolume.weight = 0.75f;
            else takeDamageVolume.weight = 0;
        }
    }


    #region TakeDamage
    public void TakeDamage(int amount)
    {
        if (amount <= 0) return;
        currentHealth -= amount;
        
        
        _timer = 0;
        if (currentHealth < initialMaxHealth)
        {
            if (currentHealth == 4) takeDamageVolume.weight = 0.25f;
            else if (currentHealth == 3) takeDamageVolume.weight = 0.5f;
            else if (currentHealth == 2) takeDamageVolume.weight = 0.75f;
            else takeDamageVolume.weight = 1;
        }
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Dead();
            AudioSystem.Instance.PlaySound(deadSound);
        }
        else
        {
            int randomSound = Random.Range(0, takeDamageSounds.Length);
            AudioSystem.Instance.PlaySound(takeDamageSounds[randomSound]);
        }
    }
    
    private void Heal(int amount)
    {
        if (amount <= 0) return;
        currentHealth += amount;
        if (currentHealth > currentMaxHealth) currentHealth = currentMaxHealth;
    }
    
    private void Dead()
    {
        StartCoroutine(CooldownCoroutine());
        EventManager.Instance.Trigger(Constants.OnPlayerDeath);
        EventManager.Instance.Trigger(Constants.OnDisableControls);
    }
    #endregion


    private IEnumerator CooldownCoroutine()
    {
        yield return new WaitForSeconds(1f);
        AudioSystem.Instance.PlaySound(lossSound);
    }
}