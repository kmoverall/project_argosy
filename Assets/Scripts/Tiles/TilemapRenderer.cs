using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TilemapRenderer : MonoBehaviour 
{
    public Tileset tileset;
    [SerializeField]
    Tilemap tilemapData;

    [SerializeField]
    bool isBaked = false;

    Mesh tilemapMesh;
    Material mat;


	void Start () {
	    
	}
}
