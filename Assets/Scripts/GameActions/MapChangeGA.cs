using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapChangeGA : GameAction
{
    public string MapName;
    public Sprite MapSprite;

    public MapChangeGA(string name, Sprite sprite)
    {
        MapName = name;
        MapSprite = sprite;
    }

}
