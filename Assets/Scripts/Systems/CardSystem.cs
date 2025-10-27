using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.XR;

public class CardSystem : Singleton<CardSystem>
{
    [SerializeField] private HandView handView;
    [SerializeField] private Transform drawPilePoint;
    [SerializeField] private Transform discardPilePoint;
    private readonly List<Card> drawPile = new();
    private readonly List<Card> discardPile = new();
    private readonly List<Card> hand = new();
    
    public List<Card> GetDrawPile() => drawPile;
    public List<Card> GetDiscardPile() => discardPile;
    public List<Card> GetHand() => hand;
    
    public List<Card> GetAllCards()
    {
        List<Card> allCards = new();
        allCards.AddRange(drawPile);
        allCards.AddRange(discardPile);
        allCards.AddRange(hand);
        return allCards;
    }
    
    void OnEnable()
    {
        ActionSystem.AttachPerformer<DrawCardGA>(DrawCardPerformer);
        ActionSystem.AttachPerformer<DiscardAllCardsGA>(DiscardAllCardsPerformer);
        ActionSystem.AttachPerformer<PlayCardGA>(PlayCardPerformer);
        ActionSystem.AttachPerformer<AddCardsGA>(AddCardsPerformer);
        ActionSystem.AttachPerformer<SearchCardGA>(SearchCardPerformer);
        ActionSystem.AttachPerformer<ClearDeckGA>(ClearDeckPerformer);
        ActionSystem.AttachPerformer<SetDeckGA>(SetDeckPerformer);
        ActionSystem.SubscribeReaction<EnemyTurnGA>(EnemyTurnPreReaction, ReactionTiming.PRE);
        ActionSystem.SubscribeReaction<EnemyTurnGA>(EnemyTurnPostReaction, ReactionTiming.POST);
    }

    void OnDisable()
    {
        ActionSystem.DetachPerformer<DrawCardGA>();
        ActionSystem.DetachPerformer<DiscardAllCardsGA>();
        ActionSystem.DetachPerformer<PlayCardGA>();
        ActionSystem.DetachPerformer<AddCardsGA>();
        ActionSystem.DetachPerformer<SearchCardGA>();
        ActionSystem.DetachPerformer<ClearDeckGA>();
        ActionSystem.DetachPerformer<SetDeckGA>();
        ActionSystem.UnSubscribeReaction<EnemyTurnGA>(EnemyTurnPreReaction, ReactionTiming.PRE);
        ActionSystem.UnSubscribeReaction<EnemyTurnGA>(EnemyTurnPostReaction, ReactionTiming.POST);
    }
    
    
    public void Setup(List<CardData> deckData)
    {
        drawPile.Clear();
        discardPile.Clear();
        handView.ClearAllCards();
        hand.Clear();
        
        foreach (var cardData in deckData)
        {
            Card card = new(cardData);
            drawPile.Add(card);
        }
        
        drawPile.Shuffle();
    }
    
    private IEnumerator DrawCardPerformer(DrawCardGA drawCardGA)
    {
        int actualAmount = Math.Min(drawCardGA.Amount, drawPile.Count);
        int notDrawnAmount = drawCardGA.Amount - actualAmount;
        for (int i = 0; i < actualAmount; i++)
        {
            if (hand.Count >= 10)
                break;
            else yield return DrawCard();
        }

        if (notDrawnAmount > 0)
        {
            RefillDeck();
            for (int i = 0; i < notDrawnAmount; i++)
            {
                if (hand.Count >= 10)
                    break;
                yield return DrawCard();
            }
        }
        LogCardListStates();
    }

    private IEnumerator DiscardAllCardsPerformer(DiscardAllCardsGA discardAllCardsGA)
    {
        foreach (var card in hand)
        {
            discardPile.Add(card);
            CardView cardView = handView.RemoveCard(card);
            if (cardView != null)
            {
                yield return DiscardCard(cardView);
            }
        }
        hand.Clear();
    }

    private IEnumerator PlayCardPerformer(PlayCardGA playCardGA)
    {
        hand.Remove(playCardGA.ThisCard);
        CardView cardView = handView.RemoveCard(playCardGA.ThisCard);
        if (cardView != null)
        {
            discardPile.Add(playCardGA.ThisCard);
            yield return DiscardCard(cardView);
        }

        if (playCardGA.ThisCard.CardType == CardType.ACTION)
        {
            SpendManaGA spendManaGA = new(playCardGA.ThisCard.currentMana);
            ActionSystem.Instance.AddReaction(spendManaGA);
        }
        
        CharmGA charmGA = new(playCardGA.ThisCard.currentCharm);
        ActionSystem.Instance.AddReaction(charmGA);
        
        MoneyGA moneyGA = new(playCardGA.ThisCard.currentMoney);
        ActionSystem.Instance.AddReaction(moneyGA);

        foreach (var effect in playCardGA.ThisCard.Effects)
        {
            PerformEffectGA performEffectGA = new(effect);
            ActionSystem.Instance.AddReaction(performEffectGA);
        }
        
        if (playCardGA.ThisCard.CardType == CardType.ACTION &&
            playCardGA.ThisCard.Mana == 1 &&
            CardCostModifierSystem.Instance.IsLieEffectOn())
        {
            CardCostModifierSystem.Instance.DisableLieEffect();
        }
    }
    
    private IEnumerator AddCardsPerformer(AddCardsGA ga)
    {
        Card newCard = new Card(ga.TargetCardData);
        discardPile.Add(newCard);
        LogCardListStates();
        yield return null;
    }
    
    private IEnumerator SearchCardPerformer(SearchCardGA ga)
    {
        if (drawPile.Count == 0)
        {
            RefillDeck();
            //Debug.Log("[SearchCard] 덱이 비어 셔플을 실행합니다.");
        }
        
        Card foundCard = null;

        // 🔍 덱 앞에서부터 검색
        foreach (var card in drawPile)
        {
            if (card.CardType == ga.typeofCard)
            {
                foundCard = card;
                break;
            }
        }

        if (foundCard != null)
        {
            if (hand.Count >= 10)
            {
                //Debug.Log("[SearchCard] 핸드가 가득 찼습니다. 카드 추가 불가.");
            }
            else
            {
                drawPile.Remove(foundCard);
                hand.Add(foundCard);

                CardView view = CardViewCreator.Instance.CreateCardView(foundCard, drawPilePoint.position, drawPilePoint.rotation);
                yield return handView.AddCard(view);

                LogCardListStates();
            }
        }
        else
        {
            //Debug.Log($"[SearchCard] {ga.typeofCard} 타입 카드를 찾을 수 없음.");
        }


        yield return null;
    }
    
    private IEnumerator ClearDeckPerformer(ClearDeckGA ga)
    {
        drawPile.Clear();
        discardPile.Clear();
        hand.Clear();
        handView.ClearAllCards(); 

        yield return null;
    }
    
    private IEnumerator SetDeckPerformer(SetDeckGA ga)
    {
        drawPile.Clear(); 
        foreach (var cardData in ga.NewDeckData)
        {
            Card card = new Card(cardData);
            drawPile.Add(card);
        }

        drawPile.Shuffle(); 
        LogCardListStates();
        yield return null;
    }

    //Reactions
    private void EnemyTurnPreReaction(EnemyTurnGA enemyTurnGA)
    {
        ResetCards();
        //DiscardAllCardsGA discardAllCardsGA = new();
        //ActionSystem.Instance.AddReaction(discardAllCardsGA);
    }
    
    private void EnemyTurnPostReaction(EnemyTurnGA enemyTurnGA)
    {
        //DrawCardGA drawCardGA = new(5);
        //ActionSystem.Instance.AddReaction(drawCardGA);
        LogCardListStates();
    }
    
    private IEnumerator DrawCard()
    {
        if (drawPile.Count != 0)
        {
            Card card = drawPile.Draw();
            hand.Add(card);
            CardView cardView = CardViewCreator.Instance.CreateCardView(card, drawPilePoint.position, drawPilePoint.rotation);
            yield return handView.AddCard(cardView);
        }
    }

    private void RefillDeck()
    {
        drawPile.AddRange(discardPile);
        discardPile.Clear();
        
        drawPile.Shuffle(); 
        //Debug.Log("[RefillDeck] 덱 셔플 완료.");
    }

    private IEnumerator DiscardCard(CardView cardView)
    {
        cardView.transform.DOScale(Vector3.zero, 0.15f);
        Tween tween = cardView.transform.DOScale(Vector3.one, 0.15f);
        yield return tween.WaitForCompletion();
        Destroy(cardView.gameObject);
        LogCardListStates();
    }
    
    private void LogCardListStates()
    {
        Debug.Log("Draw Pile: " + string.Join(", ", drawPile.ConvertAll(card => card.CardName)));
        Debug.Log("Discard Pile: " + string.Join(", ", discardPile.ConvertAll(card => card.CardName)));
        Debug.Log("Hand: " + string.Join(", ", hand.ConvertAll(card => card.CardName)));
    }

    public void ResetCards()
    {
        foreach (var card in CardSystem.Instance.GetAllCards())
            card.ResetRuntime();

        LogCardListStates();
    }
    
}
