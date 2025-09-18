using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// DreamRoot에 붙여 DREAM 시 카드 보상 '1회 선택'을 제어하는 패널.
/// - choiceCardDatas: Dream에서 보여줄 CardData 3개(인스펙터에서 지정)
/// - spawnPoints: 버튼을 배치할 위치 3곳(인스펙터에서 지정) — Transform 기반
/// - DREAM 진입(Post) 시 버튼 생성
/// - 버튼 클릭은 CardButton.OnMouseUp()에서 처리(여기선 TryChoose 사용 안 함)
/// </summary>
public class DreamChoicePanel : MonoBehaviour
{
    [Header("Dream에서 보여줄 카드 데이터 (3장)")]
    [SerializeField] private List<CardData> choiceCardDatas = new();

    [Header("카드 버튼 생성 위치 (3개, Transform)")]
    [SerializeField] private List<Transform> spawnPoints = new();

    [Header("선택 후 DreamRoot 숨김")]
    public bool hideAfterPick = true;

    [Header("한 번의 DREAM 방문에 1회만 선택")]
    public bool onePickPerDream = true;

    private bool _hasPickedInThisDream = false;

    // 시각화를 위한 임시 Card 보관
    private readonly List<Card> _tempCards = new();

    void OnEnable()
    {
        ActionSystem.SubscribeReaction<MapChangeGA>(OnMapChangePost, ReactionTiming.POST);

        if (BackgroundSystem.Instance != null && BackgroundSystem.Instance.bgName == MapType.DREAM)
        {
            _hasPickedInThisDream = false;
            SpawnChoiceButtons();
        }
    }

    void OnDisable()
    {
        ActionSystem.UnSubscribeReaction<MapChangeGA>(OnMapChangePost, ReactionTiming.POST);
        CleanupSpawnedButtons();
        _hasPickedInThisDream = false;
    }

    private void OnMapChangePost(MapChangeGA ga)
    {
        _hasPickedInThisDream = false;
        CleanupSpawnedButtons();

        if (ga.MapName == MapType.DREAM)
            SpawnChoiceButtons();
    }

    private void SpawnChoiceButtons()
    {
        if (choiceCardDatas == null || choiceCardDatas.Count == 0) return;

        int count = Mathf.Min(choiceCardDatas.Count, spawnPoints.Count);
        for (int i = 0; i < count; i++)
        {
            var data = choiceCardDatas[i];
            var parent = spawnPoints[i];
            if (data == null || parent == null) continue;

            // 버튼 시각화를 위해 CardData → Card 임시 변환
            var tempCard = new Card(data);
            _tempCards.Add(tempCard);

            // 🔹 부모 Transform 아래에 로컬(0,0,0)으로 붙임
            CardButtonCreator.Instance.CreateCardButton(
                tempCard,
                data,                    // SourceData 전달
                parent,                  // parent
                Vector3.zero,            // localPos
                Quaternion.identity      // localRot
            );
        }
    }

    private void CleanupSpawnedButtons()
    {
        foreach (var c in _tempCards)
        {
            var btn = CardButtonCreator.Instance.GetCardButton(c);
            if (btn != null) Destroy(btn.gameObject);
            CardButtonCreator.Instance.RemoveCardButton(c);
        }
        _tempCards.Clear();
    }

    // (참고) 필요하면 TryChoose 유지 — 지금은 CardButton에서 직접 처리하므로 사용하지 않음.
    public void TryChoose(CardData card)
    {
        if (card == null) return;
        if (onePickPerDream && _hasPickedInThisDream) return;
        if (BackgroundSystem.Instance == null || BackgroundSystem.Instance.bgName != MapType.DREAM) return;

        _hasPickedInThisDream = true;
        if (hideAfterPick) gameObject.SetActive(false);
    }
}
