using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class multiMentalEffect : Effect
{
    [SerializeField] private int multiplier;

    public override GameAction GetGameAction()
    {
        multiMentalGA mmGA = new(multiplier);
        return mmGA;
    }
}
