using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapChangeEffect : Effect
{
    [SerializeField] private MapType mapName;
    
    public override GameAction GetGameAction()
    {
        return new MapChangeGA(mapName);
    }
}
