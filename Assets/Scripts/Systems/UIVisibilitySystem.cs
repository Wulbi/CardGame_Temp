using System.Collections.Generic;
using UnityEngine;

public class UIVisibilitySystem : MonoBehaviour
{
    public List<GameObject> combatOnlyObjects = new();
    
    private bool awaitingVisibilityByDiscard = false;

    void OnEnable()
    {

        ApplyVisibility(BackgroundSystem.Instance != null ? BackgroundSystem.Instance.bgName : MapType.CLASSROOM1);
        
        ActionSystem.SubscribeReaction<MapChangeGA>(OnMapChangePre, ReactionTiming.PRE);
        
        ActionSystem.SubscribeReaction<DiscardAllCardsGA>(OnDiscardAllCardsPost, ReactionTiming.POST);
        
        ActionSystem.SubscribeReaction<MapChangeGA>(OnMapChangePostFallback, ReactionTiming.POST);
    }

    void OnDisable()
    {
        ActionSystem.UnSubscribeReaction<MapChangeGA>(OnMapChangePre, ReactionTiming.PRE);
        ActionSystem.UnSubscribeReaction<DiscardAllCardsGA>(OnDiscardAllCardsPost, ReactionTiming.POST);
        ActionSystem.UnSubscribeReaction<MapChangeGA>(OnMapChangePostFallback, ReactionTiming.POST);
    }
    
    private void OnMapChangePre(MapChangeGA ga)
    {
        awaitingVisibilityByDiscard = true;
    }
    
    private void OnDiscardAllCardsPost(DiscardAllCardsGA _)
    {
        if (!awaitingVisibilityByDiscard) return; 

        var map = (BackgroundSystem.Instance != null) ? BackgroundSystem.Instance.bgName : MapType.CLASSROOM1;
        ApplyVisibility(map);

        awaitingVisibilityByDiscard = false; 
    }
    
    private void OnMapChangePostFallback(MapChangeGA ga)
    {
        if (!awaitingVisibilityByDiscard) return;
        ApplyVisibility(ga.MapName);
        awaitingVisibilityByDiscard = false;
    }
    
    private void ApplyVisibility(MapType map)
    {
        bool isCombat = BackgroundSystem.IsCombatMap(map);
        foreach (var go in combatOnlyObjects)
        {
            if (go != null) go.SetActive(isCombat);
        }
    }
}
