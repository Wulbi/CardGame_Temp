using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TheftEffect : Effect
{
    public override GameAction GetGameAction()
    {
        return new TheftGA();
    }
}
