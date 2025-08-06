using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddCardsEffect : Effect
{
    [SerializeField] private CardData targetCardData;

    public override GameAction GetGameAction()
    {
        return new AddCardsGA(targetCardData);
    }
}
