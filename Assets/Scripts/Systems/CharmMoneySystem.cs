using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CharmMoneySystem : Singleton<CharmMoneySystem>
{
    public TMP_Text Money;
    public int currentMoney;
    public TMP_Text Charm;
    public int currentCharm;
    
    void OnEnable()
    {
        ActionSystem.AttachPerformer<CharmGA>(CharmPerformer);
        ActionSystem.AttachPerformer<MoneyGA>(MoneyPerformer);
    }

    void OnDisable()
    {
        ActionSystem.DetachPerformer<CharmGA>();
        ActionSystem.DetachPerformer<MoneyGA>();
    }
    
    private void Start()
    {
        Money.text = currentMoney.ToString();
        Charm.text = currentCharm.ToString();
    }

    private IEnumerator CharmPerformer(CharmGA charmGA)
    {
        int gain = charmGA.Amount;
        currentMoney += gain;
        if(currentMoney <= 0)
            currentMoney = 0;
        Money.text = currentMoney.ToString();
        yield return null;
    }

    private IEnumerator MoneyPerformer(MoneyGA moneyGA)
    {
        int gain = moneyGA.Amount;
        currentCharm += gain;
        if(currentCharm <= 0)
            currentCharm = 0;
        Charm.text = currentCharm.ToString();
        yield return null;
    }
}
