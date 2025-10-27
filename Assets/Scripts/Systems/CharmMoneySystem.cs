using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CharmMoneySystem : Singleton<CharmMoneySystem>
{
    public TMP_Text Money;
    public int currentMoney;
    public int targetMoney;
    public TMP_Text Charm;
    public int currentCharm;
    
    void OnEnable()
    {
        ActionSystem.AttachPerformer<CharmGA>(CharmPerformer);
        ActionSystem.AttachPerformer<MoneyGA>(MoneyPerformer);
        ActionSystem.AttachPerformer<TheftGA>(TheftEffectPerformer);
        targetMoney = 10;
    }

    void OnDisable()
    {
        ActionSystem.DetachPerformer<CharmGA>();
        ActionSystem.DetachPerformer<MoneyGA>();
        ActionSystem.DetachPerformer<TheftGA>();
    }
    
    private void Start()
    {
        Money.text = currentMoney.ToString();
        Charm.text = currentCharm.ToString();
    }

    private IEnumerator CharmPerformer(CharmGA charmGA)
    {
        int gain = charmGA.Amount;
        currentCharm += gain; 
        if(currentCharm <= 0)
            currentCharm = 0;
        Charm.text = currentCharm.ToString();
        yield return null;
    }

    private IEnumerator MoneyPerformer(MoneyGA moneyGA)
    {
        int gain = moneyGA.Amount;
        currentMoney += gain;
        if(currentMoney <= 0)
            currentMoney = 0;
        Money.text = currentMoney.ToString();
        yield return null;
    }
    
    private IEnumerator TheftEffectPerformer(TheftGA ga)
    {
        foreach (var card in CardSystem.Instance.GetAllCards())
        {
            if (card.CardName == "절도")
            {
                card.CharmMultiplier *= 2;
                card.MoneyMultiplier *= 2;
                card.RecomputeCurrent(); 
                
                CardView view = CardViewCreator.Instance.GetCardView(card);
                if (view != null)
                    view.Setup(card);

                //Debug.Log($"[TheftBoost] 절도 카드 강화: Charm={card.currentCharm}, Money={card.currentMoney}");
            }
        }

        yield return null;
    }
    
}
