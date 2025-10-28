using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CardTriggerSystem : Singleton<CardTriggerSystem>
{
    [SerializeField] private int gambleStacks = 0;
    [SerializeField] private int fightStacks = 0;
    [SerializeField] public bool bloodDonationOn = false;
    public int bd_decreasedMoney;
    
    
    private void OnEnable()
    {
        ActionSystem.AttachPerformer<GambleGA>(GamblePerformer);
        ActionSystem.AttachPerformer<FightGA>(FightPerformer);
        ActionSystem.AttachPerformer<BloodDonationGA>(BloodDonationPerformer);
        ActionSystem.SubscribeReaction<DrawCardGA>(DrawCardPostReaction, ReactionTiming.POST);
        ActionSystem.SubscribeReaction<EnemyTurnGA>(EnemyTurnPreReaction, ReactionTiming.PRE);
    }

    private void OnDisable()
    {
        ActionSystem.DetachPerformer<GambleGA>();
        ActionSystem.DetachPerformer<FightGA>();
        ActionSystem.DetachPerformer<BloodDonationGA>();
        ActionSystem.UnSubscribeReaction<DrawCardGA>(DrawCardPostReaction, ReactionTiming.POST);
        ActionSystem.UnSubscribeReaction<EnemyTurnGA>(EnemyTurnPreReaction, ReactionTiming.PRE);
    }
    
    private IEnumerator GamblePerformer(GambleGA ga)
    {
        CheckGambleStacks();
        yield return null;
    }
    
    private IEnumerator FightPerformer(FightGA ga)
    {
        CheckFightStacks();
        yield return null;
    }

    private IEnumerator BloodDonationPerformer(BloodDonationGA ga)
    {
        if (!bloodDonationOn)
        {
            foreach (var card in CardSystem.Instance.GetAllCards())
            {
                if (card.CardName == "군것질")
                {
                    bd_decreasedMoney = card.currentMoney;
                    card.addMoney -= bd_decreasedMoney;
                    card.RecomputeCurrent();
                }
            }

            bloodDonationOn = true;
        }
        yield return null;
    }
    private void DrawCardPostReaction(DrawCardGA ga)
    {
       CheckGambleStacks();
       CheckFightStacks();
    }

    private void EnemyTurnPreReaction(EnemyTurnGA enemyTurnGA)
    {
        gambleStacks = 0;
        fightStacks = 0;
        CheckGambleStacks();
        CheckFightStacks();
    }
    private void CheckGambleStacks()
    {
        foreach (var card in CardSystem.Instance.GetHand())
        {
            if (card.CardType == CardType.ACTION)
            {
                card.addMoney += gambleStacks;
                card.addCharm += gambleStacks;
            }
        }
        gambleStacks = 0;
        foreach (var card in CardSystem.Instance.GetHand())
        {
            if (card.CardName == "도박")
            {
                gambleStacks++;
            }
        }
        foreach (var card in CardSystem.Instance.GetHand())
        {
            if (card.CardType == CardType.ACTION)
            {
                if (card.CardName != "도박" && card.CardName != "PC방" && card.CardName != "노래방")
                {
                    card.addMoney -= gambleStacks;
                    card.addCharm -= gambleStacks;
                    card.RecomputeCurrent();
                }
            }
        }

    }

    private void CheckFightStacks()
    {
        foreach (var card in CardSystem.Instance.GetHand())
        {
            card.addMoney += fightStacks;
            card.addCharm += fightStacks;
            
        }
        fightStacks = 0;
        foreach (var card in CardSystem.Instance.GetHand())
        {
            if (card.CardName == "싸움")
            {
                fightStacks++;
            }
        }
        foreach (var card in CardSystem.Instance.GetHand())
        {
            if (card.CardName != "싸움" && card.CardName != "PC방" && card.CardName != "노래방")
            {
                card.addMoney -= fightStacks;
                card.addCharm -= fightStacks;
                card.RecomputeCurrent();
            }
        }
    }

    
}
