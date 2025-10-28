using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GambleEffect : Effect
{
    public override GameAction GetGameAction()
    {
        return new GambleGA();
    }
}
