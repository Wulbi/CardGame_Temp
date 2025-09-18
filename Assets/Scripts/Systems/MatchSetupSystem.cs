using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MatchSetupSystem : Singleton<MatchSetupSystem>
{
    [SerializeField] private List<CardData> deckData;
    [SerializeField] private List<CardData> deckDataClassroom;
    [SerializeField] private List<CardData> deckDataStreet;
    [SerializeField] private List<CardData> deckDataRoom;
    
    private List<CardData> mergedDeckData = new();

    private void Start()
    {
        mergedDeckData.AddRange(deckData);
        mergedDeckData.AddRange(deckDataClassroom);
        CardSystem.Instance.Setup(mergedDeckData);
        RefillManaGA refillManaGA = new();
        ActionSystem.Instance.Perform(refillManaGA, () =>
        {
            DrawCardGA drawCardGA = new(5);
            ActionSystem.Instance.Perform(drawCardGA);
        });
    }

    public SetDeckGA GetDeckSetupGA()
    {
        mergedDeckData.Clear();
    
        mergedDeckData.AddRange(deckData);
        if (BackgroundSystem.Instance.bgName == MapType.CLASSROOM1 || BackgroundSystem.Instance.bgName == MapType.CLASSROOM2)
            mergedDeckData.AddRange(deckDataClassroom);
        else if (BackgroundSystem.Instance.bgName == MapType.STREET)
            mergedDeckData.AddRange(deckDataStreet);
        else if (BackgroundSystem.Instance.bgName == MapType.ROOM)
            mergedDeckData.AddRange(deckDataRoom);

        return new SetDeckGA(mergedDeckData);
    }
    
     public void RegisterCardForMap(CardData data, CardMapType map)
    {
        if (data == null) return;
        switch (map)
        {
            case CardMapType.COMMON:
                AddUnique(deckData, data); break;
            case CardMapType.CLASSROOM:
                AddUnique(deckDataClassroom, data); break;
            case CardMapType.STREET:
                AddUnique(deckDataStreet, data); break;
            case CardMapType.ROOM:
                AddUnique(deckDataRoom, data); break;

            case CardMapType.CLASSROOMSTREET:
                AddUnique(deckDataClassroom, data);
                AddUnique(deckDataStreet, data);
                break;
            case CardMapType.CLASSROOMROOM:
                AddUnique(deckDataClassroom, data);
                AddUnique(deckDataRoom, data);
                break;
            case CardMapType.STREETROOM:
                AddUnique(deckDataStreet, data);
                AddUnique(deckDataRoom, data);
                break;
        }
        Debug.Log($"[MatchSetupSystem] Registered {data.name} -> {map}");
    }

    // 카드 제거 (반환값으로 성공 여부)
    public bool UnregisterCardForMap(CardData data, CardMapType map)
    {
        if (data == null) return false;
        bool removed = false;
        switch (map)
        {
            case CardMapType.COMMON:
                removed |= deckData.Remove(data); break;
            case CardMapType.CLASSROOM:
                removed |= deckDataClassroom.Remove(data); break;
            case CardMapType.STREET:
                removed |= deckDataStreet.Remove(data); break;
            case CardMapType.ROOM:
                removed |= deckDataRoom.Remove(data); break;

            // 복합 타입은 양쪽에서 제거
            case CardMapType.CLASSROOMSTREET:
                removed |= deckDataClassroom.Remove(data);
                removed |= deckDataStreet.Remove(data);
                break;
            case CardMapType.CLASSROOMROOM:
                removed |= deckDataClassroom.Remove(data);
                removed |= deckDataRoom.Remove(data);
                break;
            case CardMapType.STREETROOM:
                removed |= deckDataStreet.Remove(data);
                removed |= deckDataRoom.Remove(data);
                break;
        }
        if (removed) Debug.Log($"[MatchSetupSystem] Unregistered {data.name} <- {map}");
        return removed;
    }

    // DeletePanel이 덱 전부를 쉽게 읽도록 열거자 제공
    public IEnumerable<(CardData data, CardMapType map)> EnumerateAll()
    {
        foreach (var cd in deckData)          if (cd) yield return (cd, CardMapType.COMMON);
        foreach (var cd in deckDataClassroom) if (cd) yield return (cd, CardMapType.CLASSROOM);
        foreach (var cd in deckDataStreet)    if (cd) yield return (cd, CardMapType.STREET);
        foreach (var cd in deckDataRoom)      if (cd) yield return (cd, CardMapType.ROOM);
    }

    private void AddUnique(List<CardData> list, CardData data)
    {
        if (!list.Contains(data)) list.Add(data);
    }

}
