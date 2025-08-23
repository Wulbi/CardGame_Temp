using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapChangeGA : GameAction
{
    public MapType MapName;

    public MapChangeGA(MapType mapType)
    {
        MapName = mapType;
    }

}
