using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloodDonationEffect : Effect
{
    public override GameAction GetGameAction()
    {
        return new BloodDonationGA();
    }
}
