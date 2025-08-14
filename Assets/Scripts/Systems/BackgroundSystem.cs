using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BackgroundSystem : MonoBehaviour
{
    public Sprite bgSprite;
    public string bgName;
    
    public SpriteRenderer bgImage;

    void OnEnable()
    {
        bgName = "교실";
        bgSprite = Resources.Load<Sprite>("Background/Classroom");
        ActionSystem.AttachPerformer<MapChangeGA>(MapChangePerformer);
        Setup(); 
    }

    void OnDisable()
    {
        ActionSystem.DetachPerformer<MapChangeGA>();
    }
    void Setup()
    {
        bgImage.sprite = bgSprite;
    }

    private IEnumerator MapChangePerformer(MapChangeGA ga)
    {
        bgName = ga.MapName;
        bgSprite = ga.MapSprite;
        Setup();

        yield return null;
    }
    
}
