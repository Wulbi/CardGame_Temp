using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BackgroundSystem : Singleton<BackgroundSystem>
{
    public Sprite bgSprite;
    public MapType bgName;
    public SpriteRenderer bgImage;

    public int weekCounter;
    void OnEnable()
    {
        bgName = MapType.CLASSROOM1;
        ActionSystem.AttachPerformer<MapChangeGA>(MapChangePerformer);
        ActionSystem.SubscribeReaction<EnemyTurnGA>(EnemyTurnPreReaction, ReactionTiming.PRE);
        ActionSystem.SubscribeReaction<EnemyTurnGA>(EnemyTurnPostReaction, ReactionTiming.POST);
        
        weekCounter = 0;
        Setup(); 
    }

    void OnDisable()
    {
        ActionSystem.DetachPerformer<MapChangeGA>();
        ActionSystem.UnSubscribeReaction<EnemyTurnGA>(EnemyTurnPreReaction, ReactionTiming.PRE);
        ActionSystem.UnSubscribeReaction<EnemyTurnGA>(EnemyTurnPostReaction, ReactionTiming.POST);
    }
    void Setup()
    {
        if (bgImage == null) { Debug.LogWarning("bgImage not set"); return; }
        
        if (bgName == MapType.CLASSROOM1 || bgName == MapType.CLASSROOM2)
        {
            bgSprite = Resources.Load<Sprite>("Background/Classroom");
        }
        else if (bgName == MapType.STREET)
        {
            bgSprite = Resources.Load<Sprite>("Background/Street");
        }
        else if (bgName == MapType.ROOM)
        {
            bgSprite = Resources.Load<Sprite>("Background/Room");
        }
        else if (bgName == MapType.DREAM)
        {
            bgSprite = Resources.Load<Sprite>("Background/Dream");
        }
        else if (bgName == MapType.THERAPY)
        {
            bgSprite = Resources.Load<Sprite>("Background/Therapy");
        }
        bgImage.sprite = bgSprite;
    }

    private IEnumerator MapChangePerformer(MapChangeGA ga)
    {
        bgName = ga.MapName;
        Setup();

        ActionSystem.Instance.AddReaction(new DiscardAllCardsGA());
        ActionSystem.Instance.AddReaction(new RefillManaGA());
        ActionSystem.Instance.AddReaction(new ClearDeckGA());
        ActionSystem.Instance.AddReaction(MatchSetupSystem.Instance.GetDeckSetupGA());
        if (IsCombatMap(bgName))
        {
            ActionSystem.Instance.AddReaction(new DrawCardGA(5));
        }
        yield return null;
    }
    
    private void EnemyTurnPreReaction(EnemyTurnGA enemyTurnGA)
    {
        var next = GetNextMap(bgName, weekCounter);
        if (bgName == MapType.ROOM)
        {
            if (weekCounter == 0) weekCounter = 1;     
            else if (weekCounter == 1) weekCounter = 0;    
        }
        ActionSystem.Instance.AddReaction(new MapChangeGA(next));
    }
    
    private void EnemyTurnPostReaction(EnemyTurnGA enemyTurnGA)
    {
        
    }
    
    private MapType GetNextMap(MapType current, int week)
    {
        if (current == MapType.CLASSROOM1) return MapType.CLASSROOM2;
        if (current == MapType.CLASSROOM2) return MapType.STREET;
        if (current == MapType.STREET)     return MapType.ROOM;
        if (current == MapType.ROOM)
        {
            return week == 0 ? MapType.DREAM : MapType.THERAPY;
        }
        if (current == MapType.THERAPY || current == MapType.DREAM)
        {
            return MapType.CLASSROOM1;
        }
        
        return MapType.CLASSROOM1;
    }
    
    public static bool IsCombatMap(MapType map)
    {
        return map == MapType.CLASSROOM1
               || map == MapType.CLASSROOM2
               || map == MapType.STREET
               || map == MapType.ROOM;
    }
}


    

