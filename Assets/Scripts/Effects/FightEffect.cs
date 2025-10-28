using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FightEffect : Effect
{
    public override GameAction GetGameAction()
    {
        return new FightGA();
    }
}
