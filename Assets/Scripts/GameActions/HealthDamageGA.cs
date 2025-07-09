using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthDamageGA : GameAction
{
    public int Amount;
    
    public HealthDamageGA(int amount)
    {
        Amount = amount;
    }
}
