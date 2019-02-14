using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour {

    public Vector2 WorldPosXY;



    private float WorldPosx {
        get { return this.transform.position.x; }
    }

    private float WorldPosy
    {
        get { return this.transform.position.y; }
    }

    public void Update()
    {
        WorldPosXY = new Vector2(WorldPosx, WorldPosy);
    }
}
