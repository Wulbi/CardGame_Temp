using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayCardGA : GameAction
{
    public Card ThisCard { get; set; }

    public PlayCardGA(Card card)
    {
        ThisCard = card;
    }
    
}
