using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TempDamageSystem : Singleton<TempDamageSystem>
{
    public TMP_Text HP;
    public int currentHP;
    public TMP_Text SAN;
    public int currentSAN;
    
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

    private void Start()
    {
        HP.text = currentHP.ToString();
        SAN.text = currentSAN.ToString();
    }

    private IEnumerator HealthDamagePerformer(HealthDamageGA healthDamageGA)
    {
        int damage = healthDamageGA.Amount;
        currentHP -= damage;
        if(currentHP <= 0)
            currentHP = 0;
        HP.text = currentHP.ToString();
        yield return null;
    }

    private IEnumerator SANDamagePerformer(SANDamageGA sanDamageGA)
    {
        int damage = sanDamageGA.Amount;
        currentSAN -= damage;
        if(currentSAN <= 0)
            currentSAN = 0;
        SAN.text = currentSAN.ToString();
        yield return null;
    }
}
