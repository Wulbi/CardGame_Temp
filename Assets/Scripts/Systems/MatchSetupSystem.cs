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

}
