using DG.Tweening;
using UnityEngine;
using System;
using System.Collections.Generic;

public class CardButtonCreator : Singleton<CardButtonCreator>
{
    [SerializeField] private CardButton cardButtonPrefab;

    private readonly Dictionary<Card, CardButton> _map = new();

    /// <summary>
    /// DreamChoicePanel 등에서 호출: CardData(sourceData)를 CardButton.Setup에 반드시 전달!
    /// parent: 붙일 부모(Transform), localPos/Rot: 로컬 배치
    /// mapOverride: 필요 시 버튼의 CardMapType 지정(미지정이면 프리팹 기본값/COMMON)
    /// onClicked: (선택) 외부 클릭 콜백(치료 UI 등에서 사용)
    /// </summary>
    public CardButton CreateCardButton(
        Card runtimeCard,
        CardData sourceData,
        Transform parent,
        Vector3 localPos,
        Quaternion localRot,
        CardMapType? mapOverride = null,
        Action<CardButton> onClicked = null
    )
    {
        var btn = Instantiate(cardButtonPrefab, parent);
        var t   = btn.transform;
        t.localPosition = localPos;
        t.localRotation = localRot;
        t.localScale    = Vector3.zero;

        // ★ 여기서 sourceData를 반드시 전달한다
        btn.Setup(runtimeCard, sourceData);

        if (mapOverride.HasValue)
            btn.SetMapTypeIfExists(mapOverride.Value);

        if (onClicked != null)
            btn.OnClickedExternally = onClicked;

        // 연출
        t.DOScale(Vector3.one, 0.15f);

        _map[runtimeCard] = btn;
        return btn;
    }
    
    public CardButton CreateCardButton(Card card, Transform parent, Vector3 localPos, Quaternion localRot, CardData sourceData)
    {
        CardButton button = Instantiate(cardButtonPrefab, parent);
        button.transform.localPosition = localPos;
        button.transform.localRotation = localRot;

        button.Setup(card, sourceData); // ✅ CardData 함께 전달

        return button;
    }


    public CardButton GetCardButton(Card card) =>
        _map.TryGetValue(card, out var btn) ? btn : null;

    public void RemoveCardButton(Card card)
    {
        if (_map.TryGetValue(card, out var btn))
        {
            if (btn) Destroy(btn.gameObject);
            _map.Remove(card);
        }
    }

    public void RefreshAllCardButtons()
    {
        foreach (var kv in _map)
            kv.Value.Setup(kv.Key, kv.Value.SourceData);
    }
}