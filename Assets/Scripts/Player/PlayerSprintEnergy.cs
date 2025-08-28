using System.Collections;
using UnityEngine;

public class PlayerSprintEnergy : MonoBehaviour
{
    [Header("Energy")]
    [SerializeField, ReadOnly, Range(0f, 100f)] public float currentEnergy;
    [SerializeField, ReadOnly, Range(0f, 100f)] private float maxEnergy = 100;
    [SerializeField, Range(0f, 5f)] private float energyRechargeDelayTimeInSeconds = 1;
    public bool CanActiveBurst => minPercentForBurst < currentEnergy / maxEnergy * 100;
    [SerializeField, Range(0f, 100f)] public float minPercentForBurst = 70;
    private Coroutine _rechargeEnergyCooldownCoroutine;
    private bool _canRechargeEnergy;
    [Space]
    [SerializeField, Range(0f, 100f)] private float energyUsedPerSecond = 10;
    [SerializeField, Range(0f, 100f)] private float energyRechargePerSecond = 10;
    [Space]
    [SerializeField, Range(0f, 100f)] private float burstEnergyUsedPerSecond = 20;
    [SerializeField, Range(0f, 100f)] private float burstEnergyRechargePerSecond = 5;
    private float EnergyBarValue => currentEnergy / maxEnergy;

    
    private void Awake()
    {
        SetMaxEnergy();
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
    }

    private void Update()
    {
        RechargeEnergy();
    }


    #region Energy
    private void SetMaxEnergy()
    {
        currentEnergy = maxEnergy;
    }

    private void RechargeEnergy()
    {
        if (!_canRechargeEnergy || currentEnergy >= maxEnergy) return;
        currentEnergy += (CanActiveBurst ? burstEnergyRechargePerSecond : energyRechargePerSecond) * Time.deltaTime;
        if (currentEnergy > maxEnergy) currentEnergy = maxEnergy;
        EventManager.Instance.Trigger(Constants.OnModifyEnergy, EnergyBarValue);
    }
    
    public void ConsumeEnergy()
    {
        currentEnergy -= (CanActiveBurst ? burstEnergyUsedPerSecond : energyUsedPerSecond) * Time.deltaTime;
        if (currentEnergy < 0) currentEnergy = 0;
        if (_rechargeEnergyCooldownCoroutine != null) StopCoroutine(_rechargeEnergyCooldownCoroutine);
        _rechargeEnergyCooldownCoroutine = StartCoroutine(RechargeEnergyCooldown());
        EventManager.Instance.Trigger(Constants.OnModifyEnergy, EnergyBarValue);
    }

    private IEnumerator RechargeEnergyCooldown()
    {
        _canRechargeEnergy = false;
        yield return new WaitForSeconds(energyRechargeDelayTimeInSeconds);
        _canRechargeEnergy = true;
    }
    #endregion
}