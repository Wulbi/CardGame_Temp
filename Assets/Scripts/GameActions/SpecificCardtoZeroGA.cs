using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecificCardtoZeroGA : GameAction
{
    public CardData TargetCardData;

    public SpecificCardtoZeroGA(CardData cardData)
    {
        TargetCardData = cardData;
    }
}
