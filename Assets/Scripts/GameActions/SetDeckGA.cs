using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetDeckGA : GameAction
{
    public List<CardData> NewDeckData;

    public SetDeckGA(List<CardData> deckData)
    {
        NewDeckData = deckData;
    }
}
