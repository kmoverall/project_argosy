using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
public class Tile : MonoBehaviour {

    public enum Facing { Top, Front }

    [System.NonSerialized]
    public TilemapRenderer tilemap;
    Mesh tileMesh;

    public Facing facing = Facing.Top;
    public int tileIndex = -1;

    Vector2[] uvs;

    void Awake()
    {
        OnEnable();
    }

	void OnEnable () 
    {
        tilemap = GetComponentInParent<TilemapRenderer>();
        CreateMesh();
	}
	
	void Update () {
        GetComponent<MeshRenderer>().sharedMaterial.mainTexture = tilemap.tileset.texture;

        Vector3 newPos = transform.localPosition;
        newPos.x = Mathf.Round(newPos.x);
        newPos.y = Mathf.Round(newPos.y);
        newPos.z = Mathf.Round(newPos.z);
        transform.localPosition = newPos;

        transform.localScale = Vector3.one;

        uvs = tilemap.tileset.GetUVs(tileIndex);
        tileMesh.uv = uvs;
    }

    void OnValidate()
    {
        CreateMesh();
    }

    void CreateMesh()
    {
        if (tilemap == null || tilemap.tileset == null)
            return;

        Mesh oldMesh = tileMesh;
        tileMesh = new Mesh();
        DestroyImmediate(oldMesh);

        tileMesh = new Mesh();
        List<Vector3> verts = new List<Vector3>();
        List<int> tris = new List<int>();
        List<Vector3> norms = new List<Vector3>();
        uvs = new Vector2[4];
        if (facing == Facing.Top)
        {
            verts.Add(new Vector3(0, 0, 0));
            verts.Add(new Vector3(1, 0, 0));
            verts.Add(new Vector3(1, 0, 1));
            verts.Add(new Vector3(0, 0, 1));

            norms.Add(Vector3.up);
            norms.Add(Vector3.up);
            norms.Add(Vector3.up);
            norms.Add(Vector3.up);
        }
        else
        {
            verts.Add(new Vector3(0, 0, 0));
            verts.Add(new Vector3(1, 0, 0));
            verts.Add(new Vector3(1, 1, 0));
            verts.Add(new Vector3(0, 1, 0));

            norms.Add(Vector3.back);
            norms.Add(Vector3.back);
            norms.Add(Vector3.back);
            norms.Add(Vector3.back);
        }

        tris.Add(0);
        tris.Add(3);
        tris.Add(2);
        tris.Add(0);
        tris.Add(2);
        tris.Add(1);

        uvs[0] = Vector2.zero;
        uvs[1] = Vector2.zero;
        uvs[2] = Vector2.zero;
        uvs[3] = Vector2.zero;

        uvs = tilemap.tileset.GetUVs(tileIndex);

        tileMesh.SetVertices(verts);
        tileMesh.SetTriangles(tris, 0);
        tileMesh.SetNormals(norms);
        tileMesh.uv = uvs;

        GetComponent<MeshFilter>().mesh = tileMesh;

    }
}
