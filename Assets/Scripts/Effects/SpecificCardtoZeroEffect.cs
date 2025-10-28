using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecificCardtoZeroEffect : Effect
{
    [SerializeField] private CardData targetCardData;

    public override GameAction GetGameAction()
    {
        return new SpecificCardtoZeroGA(targetCardData);
    }
}
