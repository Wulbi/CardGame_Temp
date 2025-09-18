using DG.Tweening;
using UnityEngine;
using System.Collections.Generic;

public class CardButtonCreator : Singleton<CardButtonCreator>
{
    [SerializeField] private CardButton cardButtonPrefab;

    private Dictionary<Card, CardButton> cardButtonMap = new();

    // 🔹 Transform parent + local pos/rot + SourceData까지 세팅
    public CardButton CreateCardButton(Card card, CardData sourceData, Transform parent, Vector3 localPos, Quaternion localRot)
    {
        if (cardButtonPrefab == null)
        {
            Debug.LogError("[CardButtonCreator] cardButtonPrefab is not assigned.");
            return null;
        }
        if (parent == null)
        {
            Debug.LogError("[CardButtonCreator] parent is null.");
            return null;
        }

        CardButton button = Instantiate(cardButtonPrefab, parent);
        button.transform.localPosition = localPos;
        button.transform.localRotation = localRot;

        button.transform.localScale = Vector3.zero;
        button.transform.DOScale(Vector3.one, 0.15f);

        // SourceData도 같이 주입
        button.Setup(card, sourceData);

        cardButtonMap[card] = button;
        return button;
    }

    // (필요 시) 월드 좌표 생성 버전도 제공
    public CardButton CreateCardButtonWorld(Card card, CardData sourceData, Vector3 worldPos, Quaternion worldRot)
    {
        if (cardButtonPrefab == null)
        {
            Debug.LogError("[CardButtonCreator] cardButtonPrefab is not assigned.");
            return null;
        }

        CardButton button = Instantiate(cardButtonPrefab, worldPos, worldRot);
        button.transform.localScale = Vector3.zero;
        button.transform.DOScale(Vector3.one, 0.15f);

        button.Setup(card, sourceData);

        cardButtonMap[card] = button;
        return button;
    }

    public CardButton GetCardButton(Card card)
    {
        return cardButtonMap.TryGetValue(card, out var button) ? button : null;
    }

    public void RemoveCardButton(Card card)
    {
        if (cardButtonMap.TryGetValue(card, out var button))
        {
            if (button != null)
                Destroy(button.gameObject);
            cardButtonMap.Remove(card);
        }
    }

    public void RefreshAllCardButtons()
    {
        foreach (var kvp in cardButtonMap)
        {
            // SourceData를 잃지 않도록 재세팅
            kvp.Value.Setup(kvp.Key, kvp.Value.SourceData);
        }
    }
}
