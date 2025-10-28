using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeditationEffect : Effect
{
    public override GameAction GetGameAction()
    {
        return new MeditationGA();
    }
}
