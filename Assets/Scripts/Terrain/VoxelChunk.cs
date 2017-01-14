using UnityEngine;
using System.Collections;
using System.Collections.Generic;


namespace Terrain
{
    struct Quad
    {
        public Vector3[] verts;
        public Vector3[] norms;
        public int[] tris;
    }

    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshCollider))]
    [RequireComponent(typeof(MeshRenderer))]
    [RequireComponent(typeof(FastNoiseUnity))]
    public class VoxelChunk : MonoBehaviour
    {

        public Vector3 size;
        public Vector2 startPoint;
        public Vector3 voxelSize;
        public bool showDebug;
        FastNoiseUnity noise;
        
        Mesh chunkMesh;

        float[,,] data;

        void Awake()
        {
            chunkMesh = new Mesh();
            noise = GetComponent<FastNoiseUnity>();
            GenerateChunk();
        }

        void Start()
        {
            GetComponent<MeshFilter>().mesh = chunkMesh;
            GetComponent<MeshCollider>().sharedMesh = chunkMesh;
        }

        void GenerateChunk()
        {
            data = new float[(int)size.x, (int)size.y, (int)size.z];
            for (int i = 0; i < (int)size.x; i++)
            {
                for (int j = 0; j < (int)size.y; j++)
                {
                    for (int k = 0; k < (int)size.z; k++)
                    {
                        data[i, j, k] = 1 - (j / size.y);
                        data[i, j, k] += noise.fastNoise.GetNoise(i + startPoint.x, k + startPoint.y);
                    }
                }
            }

            GenerateMesh();
        }

        void GenerateMesh()
        {
            List<Vector3> verts = new List<Vector3>();
            List<Vector3> norms = new List<Vector3>();
            List<int> tris = new List<int>();

            for (int i = 0; i < (int)size.x; i++)
            {
                for (int j = 0; j < (int)size.y; j++)
                {
                    for (int k = 0; k < (int)size.z; k++)
                    {
                        if (data[i, j, k] >= 0.5f)
                        {
                            Quad q;
                            if (i == (int)size.x - 1 || data[i + 1, j, k] < 0.5)
                            {
                                q = MakeQuad(new Vector3(i, j, k), Vector3.right, verts.Count);
                                verts.AddRange(q.verts);
                                norms.AddRange(q.norms);
                                tris.AddRange(q.tris);
                            }
                            if (i == 0 || data[i - 1, j, k] < 0.5)
                            {
                                q = MakeQuad(new Vector3(i, j, k), Vector3.left, verts.Count);
                                verts.AddRange(q.verts);
                                norms.AddRange(q.norms);
                                tris.AddRange(q.tris);
                            }
                            if (j == (int)size.y - 1 || data[i, j + 1, k] < 0.5)
                            {
                                q = MakeQuad(new Vector3(i, j, k), Vector3.up, verts.Count);
                                verts.AddRange(q.verts);
                                norms.AddRange(q.norms);
                                tris.AddRange(q.tris);
                            }
                            if (j == 0 || data[i, j - 1, k] < 0.5)
                            {
                                q = MakeQuad(new Vector3(i, j, k), Vector3.down, verts.Count);
                                verts.AddRange(q.verts);
                                norms.AddRange(q.norms);
                                tris.AddRange(q.tris);
                            }
                            if (k == (int)size.z - 1 || data[i, j, k + 1] < 0.5)
                            {
                                q = MakeQuad(new Vector3(i, j, k), Vector3.forward, verts.Count);
                                verts.AddRange(q.verts);
                                norms.AddRange(q.norms);
                                tris.AddRange(q.tris);
                            }
                            if (k == 0 || data[i, j, k - 1] < 0.5)
                            {
                                q = MakeQuad(new Vector3(i, j, k), Vector3.back, verts.Count);
                                verts.AddRange(q.verts);
                                norms.AddRange(q.norms);
                                tris.AddRange(q.tris);
                            }
                        }
                    }
                }
            }
            
            chunkMesh.SetVertices(verts);
            chunkMesh.SetNormals(norms);
            chunkMesh.SetTriangles(tris, 0);
        }

        
        Quad MakeQuad(Vector3 index, Vector3 dir, int vCount)
        {


            Quad q = new Quad();
            q.norms = new Vector3[] { dir, dir, dir, dir };
            q.verts = new Vector3[4];

            Vector3 pos = new Vector3(index.x * voxelSize.x + dir.x * voxelSize.x * 0.5f, 
                                      index.y * voxelSize.y + dir.y * voxelSize.y * 0.5f,
                                      index.z * voxelSize.z + dir.z * voxelSize.z * 0.5f);
            Vector3 v1 = new Vector3(dir.z * voxelSize.x, dir.x * voxelSize.y, dir.y * voxelSize.z) * 0.5f;
            Vector3 v2 = new Vector3(dir.y * voxelSize.x, dir.z * voxelSize.y, dir.x * voxelSize.z) * 0.5f;

            q.verts[0] = pos + v1 + v2;
            q.verts[1] = pos + v1 - v2;
            q.verts[2] = pos - v1 + v2;
            q.verts[3] = pos - v1 - v2;

            if (dir.x + dir.y + dir.z > 0)
                q.tris = new int[] { vCount, vCount + 2, vCount + 1, vCount + 1, vCount + 2, vCount + 3 };
            else
                q.tris = new int[] { vCount, vCount + 1, vCount + 2, vCount + 1, vCount + 3, vCount + 2 };

            return q;
        }

        void OnDrawGizmos()
        {
            if (data == null || !showDebug)
                return;
            for (int i = 0; i < (int)size.x; i++)
            {
                for (int j = 0; j < (int)size.y; j++)
                {
                    for (int k = 0; k < (int)size.z; k++)
                    {
                        Gizmos.color = data[i, j, k] > 0.5 ? Color.red : Color.gray;
                        if (data[i, j, k] > 0.5f)
                            Gizmos.DrawWireCube(new Vector3(i * voxelSize.x, j * voxelSize.y, k * voxelSize.z), voxelSize);
                        else
                            Gizmos.DrawWireSphere(new Vector3(i * voxelSize.x, j * voxelSize.y, k * voxelSize.z), 0.05f);

                    }
                }
            }
        }

    }
}
