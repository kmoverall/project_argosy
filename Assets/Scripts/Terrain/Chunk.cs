using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Terrain 
{
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshCollider))]
    [RequireComponent(typeof(MeshRenderer))]
    public class Chunk : MonoBehaviour 
    {
        FastNoiseUnity noise;
        Mesh chunkMesh;
        public Vector2 startIndex;
        public int size;
        public float maxHeight;

        void Awake() 
        {
            chunkMesh = new Mesh();
            noise = GetComponent<FastNoiseUnity>();

            GenerateMesh();
        }

        void Start()
        {

            GetComponent<MeshFilter>().mesh = chunkMesh;
            GetComponent<MeshCollider>().sharedMesh = chunkMesh;
        }

        void Update()
        {
            //GenerateMesh();
        }

        void GenerateMesh()
        {
            List<Vector3> verts = new List<Vector3>();
            List<Vector3> norms = new List<Vector3>();
            List<int> tris = new List<int>();

            //Generate Surface
            for (int i = 0; i <= size; i++) 
            {
                for (int j = 0; j <= size; j++)
                {
                    Vector3 v = new Vector3();
                    v.x = i;
                    v.y = noise.fastNoise.GetNoise(i, j) * maxHeight;
                    v.z = j;
                    verts.Add(v);

                    Vector3 n = new Vector3();
                    n.x = noise.fastNoise.GetNoise(i - 1, j) * maxHeight;
                    n.x -= noise.fastNoise.GetNoise(i + 1, j) * maxHeight;
                    n.y = 2;
                    n.z = noise.fastNoise.GetNoise(i, j - 1) * maxHeight;
                    n.z -= noise.fastNoise.GetNoise(i, j + 1) * maxHeight;
                    n = Vector3.Normalize(n);
                    norms.Add(n);
                }
            }

            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    Vector3 v = new Vector3();
                    v = verts[i * (size+1) + j] + verts[(i + 1) * (size + 1) + j] + verts[(i + 1) * (size + 1) + j + 1] + verts[i * (size + 1) + j + 1];
                    v /= 4.0f;
                    verts.Add(v);

                    Vector3 n = new Vector3();
                    n = norms[i * (size + 1) + j] + norms[(i + 1) * (size + 1) + j] + norms[(i + 1) * (size + 1) + j + 1] + norms[i * (size + 1) + j + 1];
                    n = Vector3.Normalize(n);
                    norms.Add(n);

                    int vIndex = i * (size + 1) + j;
                    int center = (size + 1) * (size + 1) + i * size + j;

                    tris.Add(vIndex);
                    tris.Add(vIndex + 1);
                    tris.Add(center);

                    tris.Add(vIndex + 1);
                    tris.Add(vIndex + (size + 1) + 1);
                    tris.Add(center);

                    tris.Add(vIndex + (size + 1) + 1);
                    tris.Add(vIndex + (size + 1));
                    tris.Add(center);

                    tris.Add(vIndex + (size + 1));
                    tris.Add(vIndex);
                    tris.Add(center);
                }
            }

            //Generate Sides

            int vertCount = verts.Count;

            for (int i = 0; i <= size; i++)
            {
                Vector3 v = new Vector3();
                v = verts[i];
                verts.Add(v);
                v.y = -maxHeight * 0.5f - 1;
                verts.Add(v);

                v = verts[i * (size + 1)];
                verts.Add(v);
                v.y = -maxHeight * 0.5f - 1;
                verts.Add(v);

                v = verts[size*(size + 1) + i];
                verts.Add(v);
                v.y = -maxHeight * 0.5f - 1;
                verts.Add(v);

                v = verts[i * (size + 1) + size];
                verts.Add(v);
                v.y = -maxHeight * 0.5f - 1;
                verts.Add(v);

                norms.Add(Vector3.left);
                norms.Add(Vector3.left);
                norms.Add(Vector3.back);
                norms.Add(Vector3.back);
                norms.Add(Vector3.right);
                norms.Add(Vector3.right);
                norms.Add(Vector3.forward);
                norms.Add(Vector3.forward);

                if (i < size)
                {
                    tris.Add(vertCount + i * 8);
                    tris.Add(vertCount + i * 8 + 1);
                    tris.Add(vertCount + (i + 1) * 8);
                    tris.Add(vertCount + (i + 1) * 8);
                    tris.Add(vertCount + i * 8 + 1);
                    tris.Add(vertCount + (i + 1) * 8 + 1);

                    tris.Add(vertCount + i * 8 + 2);
                    tris.Add(vertCount + (i + 1) * 8 + 2);
                    tris.Add(vertCount + i * 8 + 3);
                    tris.Add(vertCount + (i + 1) * 8 + 2);
                    tris.Add(vertCount + (i + 1) * 8 + 3);
                    tris.Add(vertCount + i * 8 + 3);
                    
                    tris.Add(vertCount + i * 8 + 4);
                    tris.Add(vertCount + (i + 1) * 8 + 4);
                    tris.Add(vertCount + i * 8 + 5);
                    tris.Add(vertCount + (i + 1) * 8 + 4);
                    tris.Add(vertCount + (i + 1) * 8 + 5);
                    tris.Add(vertCount + i * 8 + 5);

                    tris.Add(vertCount + i * 8 + 6);
                    tris.Add(vertCount + i * 8 + 7);
                    tris.Add(vertCount + (i + 1) * 8 + 6);
                    tris.Add(vertCount + (i + 1) * 8 + 6);
                    tris.Add(vertCount + i * 8 + 7);
                    tris.Add(vertCount + (i + 1) * 8 + 7);
                }
            }

            vertCount = verts.Count;

            //Generate Bottom
            verts.Add(new Vector3(0, -maxHeight * 0.5f - 1, 0));
            verts.Add(new Vector3(size, -maxHeight * 0.5f - 1, 0));
            verts.Add(new Vector3(size, -maxHeight * 0.5f - 1, size));
            verts.Add(new Vector3(0, -maxHeight * 0.5f - 1, size));

            norms.Add(Vector3.down);
            norms.Add(Vector3.down);
            norms.Add(Vector3.down);
            norms.Add(Vector3.down);

            tris.Add(vertCount);
            tris.Add(vertCount + 1);
            tris.Add(vertCount + 2);
            tris.Add(vertCount);
            tris.Add(vertCount + 2);
            tris.Add(vertCount + 3);

            chunkMesh.SetVertices(verts);
            chunkMesh.SetNormals(norms);
            chunkMesh.SetTriangles(tris, 0);
            
        }
    
    }
}
