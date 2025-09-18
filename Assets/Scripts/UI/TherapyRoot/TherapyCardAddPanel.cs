using System;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TherapyCardAddPanel : MonoBehaviour
{
    [Header("Therapy에서 보여줄 카드 데이터 (3장)")]
    [SerializeField] private List<CardData> fallbackCandidates = new();

    [Header("카드 버튼 생성 위치 (3개, Transform)")]
    [SerializeField] private List<Transform> spawnPoints = new();

    [Header("동작 옵션")]
    [SerializeField] private bool onePickPerOpen = true;   // 한 번 열릴 때 1장만 허용
    [SerializeField] private bool closeAfterPick = true;   // 선택 후 자동 닫기
    [SerializeField] private CardMapType defaultMapType = CardMapType.COMMON;

    private bool _picked;
    private readonly List<Card> _runtimeCards = new();

    // ❸ 패널이 닫힐 때 호출될 콜백 (TherapyPanel에서 넘겨줌)
    private Action _onClosed;

    /// <summary>TherapyPanel에서 호출. 패널 표시 + 후보/닫힘콜백 설정.</summary>
    public void Open(CardData[] candidates = null, Action onClosed = null)
    {
        gameObject.SetActive(true);
        _picked = false;
        _onClosed = onClosed;

        Cleanup();

        var list = new List<CardData>();
        if (candidates != null && candidates.Length > 0) list.AddRange(candidates);
        else list.AddRange(fallbackCandidates);

        int count = Mathf.Min(3, Mathf.Min(list.Count, spawnPoints.Count));
        for (int i = 0; i < count; i++)
        {
            var cd = list[i];
            var anchor = spawnPoints[i];
            if (cd == null || anchor == null) continue;

            var card = new Card(cd);
            _runtimeCards.Add(card);

            var btn = CardButtonCreator.Instance.CreateCardButton(
                card, cd, anchor, Vector3.zero, Quaternion.identity
            );

            // Therapy 전용: 외부 콜백으로 처리
            btn.OnClickedExternally = OnPickedTherapy;

            // 트윈(선택)
            btn.transform.localScale = Vector3.zero;
            btn.transform.DOScale(Vector3.one, 0.15f);
        }
    }

    public void Close()
    {
        Cleanup();
        gameObject.SetActive(false);

        // ❹ 닫힘 콜백 호출 → TherapyPanel에서 버튼 복구
        _onClosed?.Invoke();
        _onClosed = null;
    }

    private void Cleanup()
    {
        foreach (var c in _runtimeCards)
        {
            var ui = CardButtonCreator.Instance.GetCardButton(c);
            if (ui != null)
            {
                DOTween.Kill(ui.transform, false);
                UnityEngine.Object.Destroy(ui.gameObject);
            }
            CardButtonCreator.Instance.RemoveCardButton(c);
        }
        _runtimeCards.Clear();
    }

    private void OnPickedTherapy(CardButton btn)
    {
        if (onePickPerOpen && _picked) return;
        if (btn == null || btn.SourceData == null) return;

        // 덱 등록 (버튼에 지정된 맵 타입이 있으면 사용, 없으면 default)
        var mapType = btn.GetMapTypeOrDefault(defaultMapType);
        MatchSetupSystem.Instance.RegisterCardForMap(btn.SourceData, mapType);

        _picked = true;

        // ❺ Therapy에서도 임시로 EnemyTurnGA 실행
        ActionSystem.Instance.Perform(new EnemyTurnGA());

        if (closeAfterPick) Close();
    }
}
