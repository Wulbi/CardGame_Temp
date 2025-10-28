using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RebelEffect : Effect
{
    public override GameAction GetGameAction()
    {
        return new RebelGA();
    }
}
