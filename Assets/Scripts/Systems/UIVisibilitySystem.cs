using System.Collections.Generic;
using UnityEngine;

public class UIVisibilitySystem : MonoBehaviour
{
    public List<GameObject> combatOnlyObjects = new();
    
    public List<GameObject> dreamOnlyObjects = new();
    
    public List<GameObject> therapyOnlyObjects = new();

    public GameObject cardViewHover = null;
    
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
        
        awaitingVisibilityByDiscard = false;
    }
    
    private void OnMapChangePre(MapChangeGA ga)
    {
        awaitingVisibilityByDiscard = true;
        
    }
    
    private void OnDiscardAllCardsPost(DiscardAllCardsGA _)
    {
        if (cardViewHover) cardViewHover.SetActive(false);
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
        bool isDream  = (map == MapType.DREAM);
        bool isTherapy = (map == MapType.THERAPY);
        
        SetActiveList(combatOnlyObjects,  isCombat);
        SetActiveList(dreamOnlyObjects,   isDream);
        SetActiveList(therapyOnlyObjects, isTherapy);
    }

    private static void SetActiveList(List<GameObject> list, bool active)
    {
        if (list == null) return;
        for (int i = 0; i < list.Count; i++)
        {
            var go = list[i];
            if (go) go.SetActive(active);
        }
    }
}
