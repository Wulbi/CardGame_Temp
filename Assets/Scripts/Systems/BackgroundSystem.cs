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

    void OnEnable()
    {
        bgName = MapType.CLASSROOM;
        ActionSystem.AttachPerformer<MapChangeGA>(MapChangePerformer);
        Setup(); 
    }

    void OnDisable()
    {
        ActionSystem.DetachPerformer<MapChangeGA>();
    }
    void Setup()
    {
        if (bgName == MapType.CLASSROOM)
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
        ActionSystem.Instance.AddReaction(new ClearDeckGA());
        ActionSystem.Instance.AddReaction(MatchSetupSystem.Instance.GetDeckSetupGA());
        ActionSystem.Instance.AddReaction(new DrawCardGA(5));


        yield return null;
    }
    
}
