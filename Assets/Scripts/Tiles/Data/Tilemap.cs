using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Tilemap", menuName = "Data/Tilemap", order = 90)]
public class Tilemap : ScriptableObject {
    [ReadOnly]
    public int[,,] topFaces;
    [ReadOnly]
    public int[,,] frontFaces;
}