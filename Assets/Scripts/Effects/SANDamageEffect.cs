using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SANDamageEffect : Effect
{
    [SerializeField] private int sanDamage;
    
    public override GameAction GetGameAction()
    {
        SANDamageGA sanDamageGA = new(sanDamage);
        return sanDamageGA;
    }
}
