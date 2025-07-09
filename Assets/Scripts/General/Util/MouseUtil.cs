using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseUtil 
{
    private static Camera camera = Camera.main;

    public static Vector3 GetMousePositionInWorldSpace(float zValue = 0f)
    {
        Plane dragPlane = new Plane(camera.transform.forward, new Vector3(0, 0, zValue));
        Ray ray = camera.ScreenPointToRay(Input.mousePosition);
        if (dragPlane.Raycast(ray, out float dis))
        {
            return ray.GetPoint(dis);
        }
        return Vector3.zero;
    }
}
