using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawCardEffect : Effect
{
    [SerializeField] private int drawAmount;

    public override GameAction GetGameAction()
    {
        DrawCardGA drawCardGA = new(drawAmount);
        return drawCardGA;
    }
}
