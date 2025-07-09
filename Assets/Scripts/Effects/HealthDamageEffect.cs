using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthDamageEffect : Effect
{
    [SerializeField] private int healthDamage;
    
    public override GameAction GetGameAction()
    {
        HealthDamageGA healthDamageGA = new(healthDamage);
        return healthDamageGA;
    }
    
}
