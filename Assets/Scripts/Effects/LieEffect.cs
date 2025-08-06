using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine;

public class LieEffect : Effect
{
    public override GameAction GetGameAction()
    {
        return new EnableLieEffectGA();
    }
}
