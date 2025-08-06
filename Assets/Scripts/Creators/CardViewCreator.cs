using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class CardViewCreator : Singleton<CardViewCreator>
{
    [SerializeField] private CardView cardViewPrefab;
    
    private Dictionary<Card, CardView> cardViewMap = new();

    // 카드 뷰 생성 및 등록
    public CardView CreateCardView(Card card, Vector3 position, Quaternion rotation)
    {
        CardView view = Instantiate(cardViewPrefab, position, rotation);
        view.transform.localScale = Vector3.zero;
        view.transform.DOScale(Vector3.one, 0.15f); 
        view.Setup(card);
        cardViewMap[card] = view; 
        return view;
    }

    // 카드에 해당하는 뷰 반환
    public CardView GetCardView(Card card)
    {
        return cardViewMap.TryGetValue(card, out var view) ? view : null;
    }

    // 뷰 제거 시 호출
    public void RemoveCardView(Card card)
    {
        cardViewMap.Remove(card);
    }

    // 전체 카드 뷰 갱신
    public void RefreshAllCardViews()
    {
        foreach (var kvp in cardViewMap)
        {
            kvp.Value.Setup(kvp.Key);
        }
    }
    
}
