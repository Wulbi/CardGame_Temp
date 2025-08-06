using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class SearchCardEffect : Effect
{
    [SerializeField] private CardType cardType;

    public override GameAction GetGameAction()
    {
        SearchCardGA searchCardGA = new SearchCardGA(cardType);
        return searchCardGA;
    }
    
    private CardType GetRandomCardType()
    {
        Array values = Enum.GetValues(typeof(CardType));
        return (CardType)values.GetValue(Random.Range(0, values.Length));
    }
    
}
