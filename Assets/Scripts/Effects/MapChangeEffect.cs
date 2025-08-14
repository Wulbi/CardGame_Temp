using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapChangeEffect : Effect
{
    [SerializeField] private string mapName;
    [SerializeField] private Sprite mapSprite;
    
    public override GameAction GetGameAction()
    {
        return new MapChangeGA(mapName, mapSprite);
    }
}
