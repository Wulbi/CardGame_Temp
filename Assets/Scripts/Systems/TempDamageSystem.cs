using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TempDamageSystem : Singleton<TempDamageSystem>
{
    public Slider HPSlider;
    public Slider SANSlider;
    
    void OnEnable()
    {
        ActionSystem.AttachPerformer<HealthDamageGA>(HealthDamagePerformer);
        ActionSystem.AttachPerformer<SANDamageGA>(SANDamagePerformer);
    }

    void OnDisable()
    {
        ActionSystem.DetachPerformer<HealthDamageGA>();
        ActionSystem.DetachPerformer<SANDamageGA>();
    }

    private IEnumerator HealthDamagePerformer(HealthDamageGA healthDamageGA)
    {
        int damage = healthDamageGA.Amount;
        HPSlider.value -= damage;
        yield return null;
    }

    private IEnumerator SANDamagePerformer(SANDamageGA sanDamageGA)
    {
        int damage = sanDamageGA.Amount;
        SANSlider.value -= damage;
        yield return null;
    }
}
