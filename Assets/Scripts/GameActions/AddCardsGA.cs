using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddCardsGA : GameAction
{
    public CardData TargetCardData;

    public AddCardsGA(CardData cardData)
    {
        TargetCardData = cardData;
    }
}
