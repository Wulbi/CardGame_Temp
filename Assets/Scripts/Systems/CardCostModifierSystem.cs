using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class CardCostModifierSystem : Singleton<CardCostModifierSystem>
{
    private bool lieEffectOn = false;
    [SerializeField] private bool cleaningEffectOn = false;
    
    public bool IsLieEffectOn()
    {
        return lieEffectOn;
    }

    void OnEnable()
    {
        ActionSystem.AttachPerformer<EnableLieEffectGA>(EnableLieEffectPerformer);
        ActionSystem.AttachPerformer<CleaningGA>(CleaningPerformer);
        ActionSystem.AttachPerformer<SpecificCardtoZeroGA>(SCZeroPerformer);
        ActionSystem.SubscribeReaction<SetDeckGA>(SetDeckPreReaction, ReactionTiming.PRE);
    }

    void OnDisable()
    {
        ActionSystem.DetachPerformer<EnableLieEffectGA>();
        ActionSystem.DetachPerformer<CleaningGA>();
        ActionSystem.DetachPerformer<SpecificCardtoZeroGA>();
        ActionSystem.UnSubscribeReaction<SetDeckGA>(SetDeckPreReaction, ReactionTiming.PRE);
    }

    private IEnumerator EnableLieEffectPerformer(EnableLieEffectGA ga)
    {
        lieEffectOn = true;
        ApplyLieEffectToCards();
        yield return null;
    }

    private IEnumerator CleaningPerformer(CleaningGA ga)
    {
        if (!cleaningEffectOn)
        {
            cleaningEffectOn = true;
            ApplyCleaningEffect();
        }
        yield return null;
    }

    private IEnumerator SCZeroPerformer(SpecificCardtoZeroGA ga)
    {
        foreach (var card in CardSystem.Instance.GetAllCards())
        {
           if(card.CardName == ga.TargetCardData.CardName)
           {
               card.addMana -= card.currentMana;
           }
        }

        yield return null;
    }
    public void DisableLieEffect()
    {
        lieEffectOn = false;
        ApplyLieEffectToCards(); 
        CardViewCreator.Instance.RefreshAllCardViews(); 
    }
    
    public void ModifyCost(Card card, int modifiedCost)
    {
        card.currentMana = modifiedCost;
        CardView view = CardViewCreator.Instance.GetCardView(card);
        if (view != null)
        {
            view.Setup(card); 
        }
    }
    
    public void ApplyLieEffectToCards()
    {
        foreach (var card in CardSystem.Instance.GetAllCards())
        {
            if (lieEffectOn && card.CardType == CardType.ACTION && card.Mana == 1)
            {
                ModifyCost(card, 0);
            }
            else
            {
                ModifyCost(card, card.Mana);
            }
        }
    }

    public void ApplyCleaningEffect()
    {
        foreach (var card in CardSystem.Instance.GetAllCards())
        {
            if (card.CardName == "방청소")
            {
                card.ManaMultiplier /= 2;
                card.MoneyMultiplier /= 2;
                card.CharmMultiplier /= 2;
                
                card.RecomputeCurrent(); 
                
                CardView view = CardViewCreator.Instance.GetCardView(card);
                if (view != null)
                    view.Setup(card);
            }
        }
    }

    public void SetDeckPreReaction(SetDeckGA ga)
    {
        lieEffectOn = false;
        cleaningEffectOn = false;
    }
}
