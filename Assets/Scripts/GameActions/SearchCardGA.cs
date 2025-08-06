using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SearchCardGA : GameAction
{
    public CardType typeofCard;

    public SearchCardGA(CardType cardType)
    {
        typeofCard = cardType;
    }
}
