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
    
    private int pcMulti = 4;
    private int kaMulti = 3;
    
    private const int MAX_HAND = 10;
    
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
        ActionSystem.AttachPerformer<MeditationGA>(MeditationPerformer);
        ActionSystem.AttachPerformer<RebelGA>(RebelPerformer);
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
        ActionSystem.DetachPerformer<MeditationGA>();
        ActionSystem.DetachPerformer<RebelGA>();
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
        CheckPCKaraoke();
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

        if (playCardGA.ThisCard.CardName == "군것질")
        {
            if (CardTriggerSystem.Instance.bloodDonationOn)
            {
                CardTriggerSystem.Instance.bloodDonationOn = false;
                playCardGA.ThisCard.addMoney += CardTriggerSystem.Instance.bd_decreasedMoney;
            }
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

    private IEnumerator MeditationPerformer(MeditationGA ga)
    {
        // 1) 현재 손패에서 ACTION 카드 수집 (안전한 별도 리스트)
        var actionInHand = new List<Card>();
        foreach (var c in hand)
            if (c.CardType == CardType.ACTION)
                actionInHand.Add(c);

        int targetCount = actionInHand.Count;

        // 2) 손패에서 제거 + 버림
        foreach (var card in actionInHand)
        {
            hand.Remove(card);                 // 실제 손패에서 제거
            discardPile.Add(card);             // 버림 더미로 이동
            var cardView = handView.RemoveCard(card);
            if (cardView != null)
                yield return DiscardCard(cardView);
        }

        // 3) 동일 수만큼 ACTION 카드를 덱/버림에서 찾아 드로우 (핸드 최대 10 준수)
        int drawn = 0;
        while (drawn < targetCount && hand.Count < 10)
        {
            // 덱이 비었으면 필요 시 리필
            if (drawPile.Count == 0)
            {
                if (discardPile.Count == 0) break; // 더 이상 가져올 카드가 없음
                RefillDeck();
            }

            // 덱에서 ACTION 카드 한 장 탐색 (foreach 제거, 인덱스로 안전 처리)
            int idx = drawPile.FindIndex(c => c.CardType == CardType.ACTION);
            if (idx == -1)
            {
                // 현재 덱에 ACTION이 없다면, 버림 더미를 리필하여 한 번 더 시도
                if (discardPile.Count > 0)
                {
                    RefillDeck();
                    idx = drawPile.FindIndex(c => c.CardType == CardType.ACTION);
                    if (idx == -1) break; // 정말로 더 없음 → 종료
                }
                else break;
            }

            // 4) 선택한 ACTION 카드 드로우
            var chosen = drawPile[idx];
            drawPile.RemoveAt(idx);
            hand.Add(chosen);

            var view = CardViewCreator.Instance.CreateCardView(chosen, drawPilePoint.position, drawPilePoint.rotation);
            yield return handView.AddCard(view);

            drawn++;
        }

        LogCardListStates();
        yield return null;
    }
    
    private IEnumerator RebelPerformer(RebelGA ga)
    {
        // 1) 현재 손패에서 Action / Mental 수 카운트
        int actionCountInHand = 0;
        int mentalCountInHand = 0;
        foreach (var c in hand)
        {
            if (c.CardType == CardType.ACTION) actionCountInHand++;
            else if (c.CardType == CardType.MENTAL) mentalCountInHand++;
        }

        // 2) 손패 전부 버리기 (시각 제거 포함)
        var toDiscard = new List<Card>(hand);
        foreach (var card in toDiscard)
        {
            hand.Remove(card);
            discardPile.Add(card);

            var removedView = handView.RemoveCard(card);
            if (removedView != null)
                yield return DiscardCard(removedView);
        }

        // 3) 덱 셔플 (방금 버린 카드까지 포함되도록 discard → draw + shuffle)
        RefillDeck();

        // 4) Action개수만큼 Mental 드로우
        if (actionCountInHand > 0 && hand.Count < MAX_HAND)
            yield return DrawCardsOfType(actionCountInHand, CardType.MENTAL);

        // 5) Mental개수만큼 Action 드로우
        if (mentalCountInHand > 0 && hand.Count < MAX_HAND)
            yield return DrawCardsOfType(mentalCountInHand, CardType.ACTION);

        LogCardListStates();
        yield return null;
    }

    // ADD: 지정 타입 카드를 count만큼 드로우(핸드 최대 10장 준수)
    private IEnumerator DrawCardsOfType(int count, CardType type)
    {
        int drawn = 0;
        while (drawn < count && hand.Count < MAX_HAND)
        {
            // 덱 비었으면 한 번 리필
            if (drawPile.Count == 0 && discardPile.Count > 0)
                RefillDeck();

            // 현재 덱에서 해당 타입 탐색
            int idx = drawPile.FindIndex(c => c.CardType == type);
            if (idx == -1)
            {
                // 한 번 더 시도: 버림이 남아 있다면 리필 후 재검색
                if (discardPile.Count > 0)
                {
                    RefillDeck();
                    idx = drawPile.FindIndex(c => c.CardType == type);
                }
                // 그래도 못 찾으면 종료
                if (idx == -1) break;
            }

            // 드로우 적용
            var chosen = drawPile[idx];
            drawPile.RemoveAt(idx);
            hand.Add(chosen);

            var view = CardViewCreator.Instance.CreateCardView(chosen, drawPilePoint.position, drawPilePoint.rotation);
            yield return handView.AddCard(view);

            drawn++;
        }
    }
    
    public void CheckPCKaraoke()
    {
        foreach (var card in GetAllCards())
        {
            if (card.CardName == "PC방")
            {
                card.currentMana = ManaSystem.Instance.GetCurrentMana();
                card.currentMoney = - pcMulti * card.currentMana;
                card.SetDesc("남은 시간을 전부 소모합니다. 남은 시간에 비례하여 돈 "+(-card.currentMoney)+"을/를 소모하고 멘탈 " +(pcMulti * card.currentMana)+ "을/를 회복합니다");
            }

            if (card.CardName == "노래방")
            {
                card.currentMana = ManaSystem.Instance.GetCurrentMana();
                card.currentMoney = - kaMulti * card.currentMana;
                card.SetDesc("남은 시간을 전부 소모합니다. 남은 시간에 비례하여 돈 "+ (-card.currentMoney)+"을/를 소모하고 멘탈 " +(kaMulti * card.currentMana)+ "을/를 회복합니다");
            }
        }
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
            card.ResetAll();

        LogCardListStates();
    }
    
}
