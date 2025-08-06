using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardCostModifierSystem : Singleton<CardCostModifierSystem>
{
    private bool lieEffectOn = false;
    
    public bool IsLieEffectOn()
    {
        return lieEffectOn;
    }

    void OnEnable()
    {
        ActionSystem.AttachPerformer<EnableLieEffectGA>(EnableLieEffectPerformer);
    }

    void OnDisable()
    {
        ActionSystem.DetachPerformer<EnableLieEffectGA>();
    }

    private IEnumerator EnableLieEffectPerformer(EnableLieEffectGA ga)
    {
        lieEffectOn = true;
        ApplyLieEffectToCards();
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
}
