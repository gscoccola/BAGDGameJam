using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health")]
    [SerializeField, Range(1, 100)] private float initialMaxHealth = 100;
    [SerializeField, ReadOnly, Range(0, 100)] private float currentHealth;
    [SerializeField, ReadOnly, Range(0, 100)] private float currentMaxHealth;
    

    private void Start()
    {
        currentMaxHealth = initialMaxHealth;
        Heal(currentMaxHealth);
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
    }


    #region TakeDamage
    public void TakeDamage(float amount)
    {
        if (amount <= 0) return;
        currentHealth -= amount;
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            if (currentMaxHealth <= 0) Dead();
        }
    }
    
    private void Heal(float amount)
    {
        if (amount <= 0) return;
        currentHealth += amount;
        if (currentHealth > currentMaxHealth) currentHealth = currentMaxHealth;
    }
    
    private void Dead()
    {
        EventManager.Instance.Trigger(Constants.OnPlayerDeath);
        EventManager.Instance.Trigger(Constants.OnDisableControls);
    }
    #endregion
}