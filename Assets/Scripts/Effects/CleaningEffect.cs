using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CleaningEffect : Effect
{
    public override GameAction GetGameAction()
    {
        return new CleaningGA();
    }
}
