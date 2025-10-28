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
    private bool lieEffectOn = false;
    //[SerializeField] private bool cleaningEffectOn = false;
    
    void OnEnable()
    {
        ActionSystem.AttachPerformer<CharmGA>(CharmPerformer);
        ActionSystem.AttachPerformer<MoneyGA>(MoneyPerformer);
        ActionSystem.AttachPerformer<TheftGA>(TheftEffectPerformer);
        ActionSystem.SubscribeReaction<EnableLieEffectGA>(LieEffectPostReaction, ReactionTiming.POST);
        ActionSystem.SubscribeReaction<CleaningGA>(CleaningPostReaction, ReactionTiming.POST);
        ActionSystem.SubscribeReaction<SetDeckGA>(SetDeckPreReaction, ReactionTiming.PRE);
        targetMoney = 10;
    }

    void OnDisable()
    {
        ActionSystem.DetachPerformer<CharmGA>();
        ActionSystem.DetachPerformer<MoneyGA>();
        ActionSystem.DetachPerformer<TheftGA>();
        ActionSystem.UnSubscribeReaction<EnableLieEffectGA>(LieEffectPostReaction, ReactionTiming.POST);
        ActionSystem.UnSubscribeReaction<CleaningGA>(CleaningPostReaction, ReactionTiming.POST);
        ActionSystem.UnSubscribeReaction<SetDeckGA>(SetDeckPreReaction, ReactionTiming.PRE);
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
        Charm.text = currentCharm.ToString();
        yield return null;
    }

    private IEnumerator MoneyPerformer(MoneyGA moneyGA)
    {
        int gain = moneyGA.Amount;
        currentMoney += gain;
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

    private void LieEffectPostReaction (EnableLieEffectGA ga)
    {
        if (!lieEffectOn)
        {
            lieEffectOn = true;
            foreach (var card in CardSystem.Instance.GetAllCards())
            {
                if (card.CardName == "용돈 조르기")
                {
                    card.CharmMultiplier *= 2;
                    card.MoneyMultiplier *= 2;
                    card.RecomputeCurrent(); 
                
                    CardView view = CardViewCreator.Instance.GetCardView(card);
                    if (view != null)
                        view.Setup(card);
                }
            
            }
        }
    }

    private void CleaningPostReaction(CleaningGA ga)
    {
        /*
        if (!cleaningEffectOn)
        {
            cleaningEffectOn = true;
            foreach (var card in CardSystem.Instance.GetAllCards())
            {
                if (card.CardName == "방청소")
                {
                    card.CharmMultiplier /= 2;
                    card.MoneyMultiplier /= 2;
                    card.RecomputeCurrent(); 
                
                    CardView view = CardViewCreator.Instance.GetCardView(card);
                    if (view != null)
                        view.Setup(card);
                    Debug.Log("방청소 효과 적용 완료");
                }
            
            }
        }
        */
    }
    
    public void SetDeckPreReaction(SetDeckGA ga)
    {
        lieEffectOn = false;
        //cleaningEffectOn = false;
    }
    
}
