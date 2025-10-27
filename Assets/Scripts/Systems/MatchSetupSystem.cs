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
    [SerializeField] private List<CardData> deckDataClassroomStreet;
    [SerializeField] private List<CardData> deckDataClassroomRoom;
    [SerializeField] private List<CardData> deckDataStreetRoom;
    
    private List<CardData> mergedDeckData = new();

    private void Start()
    {
        mergedDeckData.AddRange(deckData);
        mergedDeckData.AddRange(deckDataClassroom);
        mergedDeckData.AddRange(deckDataClassroomStreet);
        mergedDeckData.AddRange(deckDataClassroomRoom);
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
        if (BackgroundSystem.Instance.bgName == MapType.CLASSROOM1 ||
            BackgroundSystem.Instance.bgName == MapType.CLASSROOM2)
        {
            mergedDeckData.AddRange(deckDataClassroom);
            mergedDeckData.AddRange(deckDataClassroomStreet);
            mergedDeckData.AddRange(deckDataClassroomRoom);
        }
        else if (BackgroundSystem.Instance.bgName == MapType.STREET)
        {
            mergedDeckData.AddRange(deckDataStreet);
            mergedDeckData.AddRange(deckDataClassroomStreet);
            mergedDeckData.AddRange(deckDataStreetRoom);
        }
        else if (BackgroundSystem.Instance.bgName == MapType.ROOM)
        {
            mergedDeckData.AddRange(deckDataRoom);
            mergedDeckData.AddRange(deckDataClassroomRoom);
            mergedDeckData.AddRange(deckDataStreetRoom);
        }
        return new SetDeckGA(mergedDeckData);
    }
    
     public void RegisterCardForMap(CardData data, CardMapType map)
    {
        if (data == null) return;
        switch (map)
        {
            case CardMapType.COMMON:
                AddCardData(deckData, data); break;
            case CardMapType.CLASSROOM:
                AddCardData(deckDataClassroom, data); break;
            case CardMapType.STREET:
                AddCardData(deckDataStreet, data); break;
            case CardMapType.ROOM:
                AddCardData(deckDataRoom, data); break;
            case CardMapType.CLASSROOMSTREET:
                AddCardData(deckDataClassroomStreet, data); break;
            case CardMapType.CLASSROOMROOM:
                AddCardData(deckDataClassroomRoom, data); break;
            case CardMapType.STREETROOM:
                AddCardData(deckDataStreetRoom, data); break;
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
            case CardMapType.CLASSROOMSTREET:
                removed |= deckDataClassroomStreet.Remove(data); break;
            case CardMapType.CLASSROOMROOM:
                removed |= deckDataClassroomRoom.Remove(data); break;
            case CardMapType.STREETROOM:
                removed |= deckDataStreetRoom.Remove(data); break;
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
        foreach (var cd in deckDataClassroomRoom)      if (cd) yield return (cd, CardMapType.CLASSROOMROOM);
        foreach (var cd in deckDataClassroomStreet)      if (cd) yield return (cd, CardMapType.CLASSROOMSTREET);
        foreach (var cd in deckDataStreetRoom)      if (cd) yield return (cd, CardMapType.STREETROOM);
    }

    private void AddCardData(List<CardData> list, CardData data)
    {
        list.Add(data);
    }

}
