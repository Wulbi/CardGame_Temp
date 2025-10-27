using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class TherapyCardDeletePanel : MonoBehaviour
{
    [Header("Layout Parent (UI)")]
    [SerializeField] private RectTransform content;              // 버튼들이 붙을 부모 (Grid/Vertical Layout)

    [Header("Delete Button Prefab (UI)")]
    [SerializeField] private DeleteCardButtonUI buttonPrefab;    // 아래 3) 스크립트

    [Header("정렬 옵션")]
    [SerializeField] private SortMode defaultSort = SortMode.NameAsc;
    public enum SortMode { NameAsc, ManaAsc, MoneyAsc, CharmAsc }

    [Header("동작 옵션")]
    [SerializeField] private bool closeAfterDelete = true;
    [SerializeField] private bool performEnemyTurnAfterDelete = true; // 원하면 true로

    private readonly List<GameObject> _spawned = new();
    private Action _onClosed;

    // 내부용: 덱에 있는 카드와 소속 맵을 함께 기억
    private struct CardRow
    {
        public CardData data;
        public Card     runtime;   // 표시용
        public CardMapType map;
    }

    void Awake() {
        Debug.Log($"[TherapyDeletePanel] Awake: {name}({GetInstanceID()}) performEnemyTurnAfterDelete={performEnemyTurnAfterDelete}");
    }
    void OnEnable() {
        Debug.Log($"[TherapyDeletePanel] OnEnable: {name}({GetInstanceID()}) performEnemyTurnAfterDelete={performEnemyTurnAfterDelete}");
    }
    
    public void Open(Action onClosed = null)
    {
        gameObject.SetActive(true);
        _onClosed = onClosed;

        ClearButtons();

        // 1) 덱 수집
        var rows = CollectAllDecks();

        // 2) 정렬
        ApplySort(rows, defaultSort);

        // 3) 버튼 생성
        SpawnButtons(rows);
    }

    public void Close()
    {
        ClearButtons();
        gameObject.SetActive(false);
        _onClosed?.Invoke();
        _onClosed = null;
    }

    private void ClearButtons()
    {
        foreach (var go in _spawned)
        {
            if (go)
            {
                DOTween.Kill(go.transform, false);
                Destroy(go);
            }
        }
        _spawned.Clear();
    }

    // 수집부
    private List<CardRow> CollectAllDecks()
    {
        var rows = new List<CardRow>();
        var ms = MatchSetupSystem.Instance;
        if (ms == null)
        {
            Debug.LogWarning("[TherapyCardDeletePanel] MatchSetupSystem.Instance is null.");
            return rows;
        }

        Debug.Log("[TherapyCardDeletePanel] Collecting decks...");

        foreach (var (data, map) in ms.EnumerateAll())
        {
            rows.Add(new CardRow {
                data = data,
                map  = map,
                runtime = new Card(data) // 표시용
            });
        }

        Debug.Log($"[TherapyCardDeletePanel] Collected rows: {rows.Count}");
        return rows;
    }

    // 삭제부
    private void OnClickDelete(CardData data, CardMapType map)
    {
        if (data == null) return;

        var ok = MatchSetupSystem.Instance.UnregisterCardForMap(data, map);
        if (!ok) Debug.LogWarning("[TherapyCardDeletePanel] Remove failed (not found).");

        if (performEnemyTurnAfterDelete)
            ActionSystem.Instance.Perform(new EnemyTurnGA());

        if (closeAfterDelete) Close();
        else Open(_onClosed); // 남은 목록 갱신
    }



    // 리플렉션 없이도 동작하도록, 존재하면 캐스팅/리턴, 없으면 null
    private List<CardData> GetListSafe(object obj, string fieldName)
    {
        var f = obj.GetType().GetField(fieldName);
        if (f != null)
        {
            var val = f.GetValue(obj) as List<CardData>;
            return val;
        }
        return null;
    }

    private void AddDeck(MatchSetupSystem ms, List<CardData> deck, CardMapType map, List<CardRow> result)
    {
        if (deck == null) return;
        foreach (var cd in deck)
        {
            if (cd == null) continue;
            result.Add(new CardRow
            {
                data    = cd,
                runtime = new Card(cd),
                map     = map
            });
        }
    }

    private void ApplySort(List<CardRow> rows, SortMode mode)
    {
        switch (mode)
        {
            case SortMode.ManaAsc:
                rows.Sort((a,b) => GetMana(a.runtime).CompareTo(GetMana(b.runtime)));
                break;
            case SortMode.MoneyAsc:
                rows.Sort((a,b) => a.runtime.currentMoney.CompareTo(b.runtime.currentMoney));
                break;
            case SortMode.CharmAsc:
                rows.Sort((a,b) => a.runtime.currentCharm.CompareTo(b.runtime.currentCharm));
                break;
            default: // NameAsc
                rows.Sort((a,b) => string.Compare(a.runtime.CardName, b.runtime.CardName, StringComparison.Ordinal));
                break;
        }

        int GetMana(Card c) => (c.CardType == CardType.ACTION) ? c.currentMana : int.MaxValue; // 멘탈카드 등은 뒤로
    }

    private void SpawnButtons(List<CardRow> rows)
    {
        if (content == null || buttonPrefab == null) return;

        foreach (var row in rows)
        {
            var btn = Instantiate(buttonPrefab, content);
            btn.Setup(row.runtime, row.data, row.map, OnClickDelete);

            var rt = (RectTransform)btn.transform;
            rt.localScale = Vector3.zero;
            rt.DOScale(Vector3.one, 0.15f);

            _spawned.Add(btn.gameObject);
        }
    }

    private bool TryUnregisterByAPI(CardData data, CardMapType map)
    {
        var ms = MatchSetupSystem.Instance;
        var m  = ms.GetType().GetMethod("UnregisterCardForMap");
        if (m != null)
        {
            // 시그니처: bool UnregisterCardForMap(CardData, CardMapType)
            var ok = (bool)m.Invoke(ms, new object[] { data, map });
            return ok;
        }
        return false;
    }

    private void TryRemoveFromKnownLists(CardData data, CardMapType map)
    {
        var ms = MatchSetupSystem.Instance;
        bool removed = false;

        void RemoveFrom(List<CardData> list)
        {
            if (list == null) return;
            int idx = list.IndexOf(data);
            if (idx >= 0)
            {
                list.RemoveAt(idx);
                removed = true;
            }
        }

        switch (map)
        {
            case CardMapType.COMMON:
                RemoveFrom(GetListSafe(ms, "deckData"));
                break;
            case CardMapType.CLASSROOM:
                RemoveFrom(GetListSafe(ms, "deckDataClassroom"));
                break;
            case CardMapType.STREET:
                RemoveFrom(GetListSafe(ms, "deckDataStreet"));
                break;
            case CardMapType.ROOM:
                RemoveFrom(GetListSafe(ms, "deckDataRoom"));
                break;
            case CardMapType.CLASSROOMROOM:
                RemoveFrom(GetListSafe(ms, "deckDataClassroomRoom"));
                break;
            case CardMapType.CLASSROOMSTREET:
                RemoveFrom(GetListSafe(ms, "deckDataClassroomStreet"));
                break;
            case CardMapType.STREETROOM:
                RemoveFrom(GetListSafe(ms, "deckDataStreetRoom"));
                break;
            default:
                break;
        }

        if (!removed)
            Debug.LogWarning("[TherapyCardDeletePanel] 카드 제거 실패: 맵 라우팅/리스트 구성이 다를 수 있습니다.");
    }
    
}
