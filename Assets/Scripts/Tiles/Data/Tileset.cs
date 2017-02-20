using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Tileset", menuName = "Data/Tileset", order = 90)]
public class Tileset : ScriptableObject {
    public Texture texture;
    public int tileSize;

    public Vector2[] GetUVs(int tile)
    {
        Vector2[] uvs = new Vector2[4];
        tile -= 1;

        if (tile >= (texture.width / tileSize) * (texture.height / tileSize) || tile < 0)
        {
            uvs[0] = Vector2.zero;
            uvs[1] = Vector2.zero;
            uvs[2] = Vector2.zero;
            uvs[3] = Vector2.zero;
            return uvs;
        }
        
        uvs[0].x = (tile % (texture.width / tileSize)) * ((float)tileSize / texture.width);
        uvs[0].y = (tile / (texture.width / tileSize)) * ((float)tileSize / texture.height);

        uvs[1].x = ((tile % (texture.width / tileSize)) + 1) * ((float)tileSize / texture.width);
        uvs[1].y = (tile / (texture.width / tileSize)) * ((float)tileSize / texture.height);

        uvs[2].x = ((tile % (texture.width / tileSize)) + 1) * ((float)tileSize / texture.width);
        uvs[2].y = ((tile / (texture.width / tileSize)) + 1) * ((float)tileSize / texture.height);

        uvs[3].x = (tile % (texture.width / tileSize)) * ((float)tileSize / texture.width);
        uvs[3].y = ((tile / (texture.width / tileSize)) + 1) * ((float)tileSize / texture.height);

        return uvs;
    }
}