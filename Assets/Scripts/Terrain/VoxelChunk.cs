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

        public Vector3 chunkSize = new Vector3(16, 16, 16);
        public Vector2 startPoint = Vector3.zero;
        public Vector3 voxelSize = Vector3.one;
        public Vector2 sampleRate = Vector2.one;
        public float heightVariation = 1;
        public float baseSurfaceHeight = 0.5f;
        public bool showDebug = false;
        FastNoiseUnity noise;

        Mesh chunkMesh;

        float[,,] data;
        Vector3[,,] positions;

        #region Marching Cubes Lookup Tables
        int[] edgeTable ={
        0x0  , 0x109, 0x203, 0x30a, 0x406, 0x50f, 0x605, 0x70c,
        0x80c, 0x905, 0xa0f, 0xb06, 0xc0a, 0xd03, 0xe09, 0xf00,
        0x190, 0x99 , 0x393, 0x29a, 0x596, 0x49f, 0x795, 0x69c,
        0x99c, 0x895, 0xb9f, 0xa96, 0xd9a, 0xc93, 0xf99, 0xe90,
        0x230, 0x339, 0x33 , 0x13a, 0x636, 0x73f, 0x435, 0x53c,
        0xa3c, 0xb35, 0x83f, 0x936, 0xe3a, 0xf33, 0xc39, 0xd30,
        0x3a0, 0x2a9, 0x1a3, 0xaa , 0x7a6, 0x6af, 0x5a5, 0x4ac,
        0xbac, 0xaa5, 0x9af, 0x8a6, 0xfaa, 0xea3, 0xda9, 0xca0,
        0x460, 0x569, 0x663, 0x76a, 0x66 , 0x16f, 0x265, 0x36c,
        0xc6c, 0xd65, 0xe6f, 0xf66, 0x86a, 0x963, 0xa69, 0xb60,
        0x5f0, 0x4f9, 0x7f3, 0x6fa, 0x1f6, 0xff , 0x3f5, 0x2fc,
        0xdfc, 0xcf5, 0xfff, 0xef6, 0x9fa, 0x8f3, 0xbf9, 0xaf0,
        0x650, 0x759, 0x453, 0x55a, 0x256, 0x35f, 0x55 , 0x15c,
        0xe5c, 0xf55, 0xc5f, 0xd56, 0xa5a, 0xb53, 0x859, 0x950,
        0x7c0, 0x6c9, 0x5c3, 0x4ca, 0x3c6, 0x2cf, 0x1c5, 0xcc ,
        0xfcc, 0xec5, 0xdcf, 0xcc6, 0xbca, 0xac3, 0x9c9, 0x8c0,
        0x8c0, 0x9c9, 0xac3, 0xbca, 0xcc6, 0xdcf, 0xec5, 0xfcc,
        0xcc , 0x1c5, 0x2cf, 0x3c6, 0x4ca, 0x5c3, 0x6c9, 0x7c0,
        0x950, 0x859, 0xb53, 0xa5a, 0xd56, 0xc5f, 0xf55, 0xe5c,
        0x15c, 0x55 , 0x35f, 0x256, 0x55a, 0x453, 0x759, 0x650,
        0xaf0, 0xbf9, 0x8f3, 0x9fa, 0xef6, 0xfff, 0xcf5, 0xdfc,
        0x2fc, 0x3f5, 0xff , 0x1f6, 0x6fa, 0x7f3, 0x4f9, 0x5f0,
        0xb60, 0xa69, 0x963, 0x86a, 0xf66, 0xe6f, 0xd65, 0xc6c,
        0x36c, 0x265, 0x16f, 0x66 , 0x76a, 0x663, 0x569, 0x460,
        0xca0, 0xda9, 0xea3, 0xfaa, 0x8a6, 0x9af, 0xaa5, 0xbac,
        0x4ac, 0x5a5, 0x6af, 0x7a6, 0xaa , 0x1a3, 0x2a9, 0x3a0,
        0xd30, 0xc39, 0xf33, 0xe3a, 0x936, 0x83f, 0xb35, 0xa3c,
        0x53c, 0x435, 0x73f, 0x636, 0x13a, 0x33 , 0x339, 0x230,
        0xe90, 0xf99, 0xc93, 0xd9a, 0xa96, 0xb9f, 0x895, 0x99c,
        0x69c, 0x795, 0x49f, 0x596, 0x29a, 0x393, 0x99 , 0x190,
        0xf00, 0xe09, 0xd03, 0xc0a, 0xb06, 0xa0f, 0x905, 0x80c,
        0x70c, 0x605, 0x50f, 0x406, 0x30a, 0x203, 0x109, 0x0   };
        
        int[][] triTable = new int[][] {
            new int[] {  },
            new int[] { 0, 2, 1 },
            new int[] { 0, 1, 2 },
            new int[] { 0, 2, 1, 3, 2, 0 },
            new int[] { 0, 1, 2 },
            new int[] { 0, 4, 3, 1, 2, 5 },
            new int[] { 2, 1, 3, 0, 1, 2 },
            new int[] { 0, 2, 1, 0, 4, 2, 4, 3, 2 },
            new int[] { 1, 2, 0 },
            new int[] { 0, 3, 1, 2, 3, 0 },
            new int[] { 1, 4, 0, 2, 3, 5 },
            new int[] { 0, 4, 1, 0, 3, 4, 3, 2, 4 },
            new int[] { 1, 2, 0, 3, 2, 1 },
            new int[] { 0, 3, 1, 0, 2, 3, 2, 4, 3 },
            new int[] { 1, 2, 0, 1, 4, 2, 4, 3, 2 },
            new int[] { 1, 0, 2, 2, 0, 3 },
            new int[] { 0, 1, 2 },
            new int[] { 2, 1, 0, 3, 1, 2 },
            new int[] { 0, 1, 5, 4, 2, 3 },
            new int[] { 2, 0, 4, 2, 3, 0, 3, 1, 0 },
            new int[] { 0, 1, 5, 4, 2, 3 },
            new int[] { 3, 4, 5, 3, 0, 4, 1, 2, 6 },
            new int[] { 5, 1, 6, 5, 0, 1, 4, 2, 3 },
            new int[] { 0, 5, 4, 0, 4, 3, 0, 3, 1, 3, 4, 2 },
            new int[] { 4, 2, 3, 1, 5, 0 },
            new int[] { 4, 2, 3, 4, 1, 2, 1, 0, 2 },
            new int[] { 7, 0, 1, 6, 4, 5, 2, 3, 8 },
            new int[] { 2, 3, 5, 4, 2, 5, 4, 5, 1, 4, 1, 0 },
            new int[] { 1, 5, 0, 1, 6, 5, 3, 4, 2 },
            new int[] { 1, 5, 4, 1, 2, 5, 1, 0, 2, 3, 5, 2 },
            new int[] { 2, 3, 4, 5, 0, 7, 5, 7, 6, 7, 0, 1 },
            new int[] { 0, 1, 4, 0, 4, 2, 2, 4, 3 },
            new int[] { 2, 1, 0 },
            new int[] { 5, 3, 2, 0, 4, 1 },
            new int[] { 0, 3, 2, 1, 3, 0 },
            new int[] { 4, 3, 2, 4, 1, 3, 1, 0, 3 },
            new int[] { 0, 1, 5, 4, 3, 2 },
            new int[] { 3, 0, 6, 1, 2, 8, 4, 7, 5 },
            new int[] { 3, 1, 4, 3, 2, 1, 2, 0, 1 },
            new int[] { 0, 5, 3, 1, 0, 3, 1, 3, 2, 1, 2, 4 },
            new int[] { 4, 3, 2, 0, 1, 5 },
            new int[] { 0, 6, 1, 0, 4, 6, 2, 5, 3 },
            new int[] { 0, 5, 4, 0, 1, 5, 2, 3, 6 },
            new int[] { 1, 0, 3, 1, 3, 4, 1, 4, 5, 2, 4, 3 },
            new int[] { 5, 1, 6, 5, 0, 1, 4, 3, 2 },
            new int[] { 2, 5, 3, 0, 4, 1, 4, 6, 1, 4, 7, 6 },
            new int[] { 3, 2, 0, 3, 0, 5, 3, 5, 4, 5, 0, 1 },
            new int[] { 1, 0, 2, 1, 2, 3, 3, 2, 4 },
            new int[] { 3, 1, 2, 0, 1, 3 },
            new int[] { 4, 1, 0, 4, 2, 1, 2, 3, 1 },
            new int[] { 0, 3, 4, 0, 1, 3, 1, 2, 3 },
            new int[] { 0, 2, 1, 1, 2, 3 },
            new int[] { 5, 3, 4, 5, 2, 3, 6, 0, 1 },
            new int[] { 7, 1, 2, 6, 4, 0, 4, 3, 0, 4, 5, 3 },
            new int[] { 4, 0, 1, 4, 1, 2, 4, 2, 3, 5, 2, 1 },
            new int[] { 0, 4, 2, 0, 2, 1, 1, 2, 3 },
            new int[] { 3, 5, 2, 3, 4, 5, 1, 6, 0 },
            new int[] { 4, 2, 3, 4, 3, 1, 4, 1, 0, 1, 3, 5 },
            new int[] { 2, 3, 7, 0, 1, 6, 1, 5, 6, 1, 4, 5 },
            new int[] { 4, 1, 0, 4, 0, 3, 3, 0, 2 },
            new int[] { 5, 2, 4, 4, 2, 3, 6, 0, 1, 6, 1, 7 },
            new int[] { 2, 3, 0, 2, 0, 4, 3, 6, 0, 1, 0, 5, 6, 5, 0 },
            new int[] { 6, 5, 0, 6, 0, 1, 5, 2, 0, 4, 0, 3, 2, 3, 0 },
            new int[] { 3, 2, 0, 1, 3, 0 },
            new int[] { 2, 1, 0 },
            new int[] { 0, 4, 1, 2, 5, 3 },
            new int[] { 4, 0, 1, 2, 5, 3 },
            new int[] { 0, 4, 1, 0, 5, 4, 2, 6, 3 },
            new int[] { 0, 3, 2, 1, 3, 0 },
            new int[] { 1, 5, 4, 1, 2, 5, 3, 0, 6 },
            new int[] { 4, 3, 2, 4, 0, 3, 0, 1, 3 },
            new int[] { 2, 5, 4, 2, 4, 0, 2, 0, 3, 1, 0, 4 },
            new int[] { 0, 1, 5, 4, 3, 2 },
            new int[] { 6, 0, 4, 6, 1, 0, 5, 3, 2 },
            new int[] { 0, 1, 6, 2, 3, 8, 4, 7, 5 },
            new int[] { 2, 6, 3, 0, 5, 1, 5, 7, 1, 5, 4, 7 },
            new int[] { 3, 1, 4, 3, 2, 1, 2, 0, 1 },
            new int[] { 0, 4, 5, 0, 5, 2, 0, 2, 1, 2, 5, 3 },
            new int[] { 1, 5, 3, 0, 1, 3, 0, 3, 2, 0, 2, 4 },
            new int[] { 1, 0, 3, 1, 3, 4, 4, 3, 2 },
            new int[] { 1, 5, 2, 0, 3, 4 },
            new int[] { 2, 1, 0, 2, 5, 1, 4, 3, 6 },
            new int[] { 1, 7, 0, 3, 8, 4, 6, 2, 5 },
            new int[] { 7, 4, 3, 0, 6, 5, 0, 5, 1, 5, 6, 2 },
            new int[] { 4, 0, 1, 4, 3, 0, 2, 5, 6 },
            new int[] { 1, 2, 5, 5, 2, 6, 3, 0, 4, 3, 4, 7 },
            new int[] { 6, 2, 5, 7, 0, 3, 0, 4, 3, 0, 1, 4 },
            new int[] { 5, 1, 6, 5, 6, 2, 1, 0, 6, 3, 6, 4, 0, 4, 6 },
            new int[] { 1, 8, 0, 5, 6, 2, 7, 4, 3 },
            new int[] { 3, 6, 4, 2, 5, 1, 2, 1, 0, 1, 5, 7 },
            new int[] { 0, 1, 9, 4, 7, 8, 2, 3, 11, 5, 10, 6 },
            new int[] { 6, 1, 0, 6, 8, 1, 6, 2, 8, 5, 8, 2, 3, 7, 4 },
            new int[] { 6, 2, 5, 1, 7, 3, 1, 3, 0, 3, 7, 4 },
            new int[] { 3, 1, 6, 3, 6, 4, 1, 0, 6, 5, 6, 2, 0, 2, 6 },
            new int[] { 0, 3, 7, 0, 4, 3, 0, 1, 4, 8, 4, 1, 6, 2, 5 },
            new int[] { 2, 1, 4, 2, 4, 5, 0, 3, 4, 3, 5, 4 },
            new int[] { 3, 0, 2, 1, 0, 3 },
            new int[] { 2, 6, 3, 2, 5, 6, 0, 4, 1 },
            new int[] { 4, 0, 1, 4, 3, 0, 3, 2, 0 },
            new int[] { 4, 1, 0, 4, 0, 3, 4, 3, 2, 3, 0, 5 },
            new int[] { 0, 2, 4, 0, 1, 2, 1, 3, 2 },
            new int[] { 3, 0, 6, 1, 2, 7, 2, 4, 7, 2, 5, 4 },
            new int[] { 0, 1, 2, 2, 1, 3 },
            new int[] { 4, 1, 0, 4, 0, 2, 2, 0, 3 },
            new int[] { 5, 2, 4, 5, 3, 2, 6, 0, 1 },
            new int[] { 0, 4, 1, 1, 4, 7, 2, 5, 6, 2, 6, 3 },
            new int[] { 3, 7, 2, 0, 1, 5, 0, 5, 4, 5, 1, 6 },
            new int[] { 3, 2, 0, 3, 0, 5, 2, 4, 0, 1, 0, 6, 4, 6, 0 },
            new int[] { 4, 3, 2, 4, 1, 3, 4, 0, 1, 5, 3, 1 },
            new int[] { 4, 6, 1, 4, 1, 0, 6, 3, 1, 5, 1, 2, 3, 2, 1 },
            new int[] { 1, 4, 3, 1, 3, 0, 0, 3, 2 },
            new int[] { 1, 0, 2, 3, 1, 2 },
            new int[] { 1, 4, 0, 1, 2, 4, 2, 3, 4 },
            new int[] { 0, 3, 1, 0, 5, 3, 0, 4, 5, 2, 3, 5 },
            new int[] { 5, 2, 3, 1, 5, 3, 1, 3, 4, 1, 4, 0 },
            new int[] { 4, 2, 3, 4, 3, 0, 0, 3, 1 },
            new int[] { 0, 1, 2, 0, 2, 4, 0, 4, 5, 4, 2, 3 },
            new int[] { 2, 4, 6, 2, 6, 1, 4, 5, 6, 0, 6, 3, 5, 3, 6 },
            new int[] { 3, 4, 0, 3, 0, 2, 2, 0, 1 },
            new int[] { 3, 1, 0, 2, 3, 0 },
            new int[] { 0, 1, 7, 6, 2, 4, 6, 4, 5, 4, 2, 3 },
            new int[] { 1, 0, 3, 1, 3, 6, 0, 4, 3, 2, 3, 5, 4, 5, 3 },
            new int[] { 1, 6, 0, 1, 5, 6, 1, 7, 5, 4, 5, 7, 2, 3, 8 },
            new int[] { 5, 1, 0, 5, 0, 3, 4, 2, 0, 2, 3, 0 },
            new int[] { 4, 5, 2, 4, 2, 3, 5, 0, 2, 6, 2, 1, 0, 1, 2 },
            new int[] { 0, 4, 1, 5, 2, 3 },
            new int[] { 3, 4, 0, 3, 0, 2, 1, 5, 0, 5, 2, 0 },
            new int[] { 1, 2, 0 },
            new int[] { 1, 0, 2 },
            new int[] { 1, 0, 4, 5, 3, 2 },
            new int[] { 0, 1, 4, 5, 3, 2 },
            new int[] { 4, 0, 5, 4, 1, 0, 6, 3, 2 },
            new int[] { 4, 0, 1, 2, 5, 3 },
            new int[] { 1, 2, 7, 3, 0, 6, 4, 8, 5 },
            new int[] { 1, 4, 0, 1, 5, 4, 2, 6, 3 },
            new int[] { 2, 7, 3, 0, 6, 1, 6, 4, 1, 6, 5, 4 },
            new int[] { 3, 0, 1, 2, 0, 3 },
            new int[] { 3, 0, 4, 3, 2, 0, 2, 1, 0 },
            new int[] { 2, 5, 4, 2, 3, 5, 0, 1, 6 },
            new int[] { 0, 2, 1, 0, 4, 2, 0, 5, 4, 4, 3, 2 },
            new int[] { 4, 3, 2, 4, 0, 3, 0, 1, 3 },
            new int[] { 5, 3, 2, 1, 3, 5, 1, 4, 3, 1, 0, 4 },
            new int[] { 0, 1, 3, 0, 3, 5, 0, 5, 4, 2, 5, 3 },
            new int[] { 1, 0, 4, 1, 4, 2, 2, 4, 3 },
            new int[] { 1, 2, 0, 3, 2, 1 },
            new int[] { 1, 3, 4, 1, 0, 3, 0, 2, 3 },
            new int[] { 4, 3, 6, 4, 2, 3, 5, 0, 1 },
            new int[] { 4, 2, 3, 4, 3, 1, 4, 1, 0, 5, 1, 3 },
            new int[] { 3, 4, 2, 3, 6, 4, 1, 5, 0 },
            new int[] { 1, 2, 6, 3, 0, 7, 0, 5, 7, 0, 4, 5 },
            new int[] { 2, 7, 4, 2, 3, 7, 0, 1, 5, 1, 6, 5 },
            new int[] { 5, 4, 1, 5, 1, 0, 4, 2, 1, 6, 1, 3, 2, 3, 1 },
            new int[] { 4, 0, 1, 4, 2, 0, 2, 3, 0 },
            new int[] { 0, 2, 1, 2, 3, 1 },
            new int[] { 1, 7, 0, 2, 3, 4, 2, 4, 5, 4, 3, 6 },
            new int[] { 0, 4, 2, 0, 2, 1, 1, 2, 3 },
            new int[] { 4, 0, 1, 4, 3, 0, 4, 2, 3, 3, 5, 0 },
            new int[] { 4, 1, 0, 4, 0, 3, 3, 0, 2 },
            new int[] { 2, 3, 1, 2, 1, 4, 3, 6, 1, 0, 1, 5, 6, 5, 1 },
            new int[] { 3, 2, 0, 1, 3, 0 },
            new int[] { 0, 4, 1, 3, 2, 5 },
            new int[] { 0, 6, 1, 2, 7, 3, 8, 5, 4 },
            new int[] { 3, 0, 1, 3, 2, 0, 5, 4, 6 },
            new int[] { 7, 5, 4, 6, 1, 2, 1, 3, 2, 1, 0, 3 },
            new int[] { 6, 3, 2, 7, 0, 1, 5, 4, 8 },
            new int[] { 6, 11, 7, 1, 2, 10, 0, 8, 3, 4, 9, 5 },
            new int[] { 5, 4, 7, 3, 2, 6, 2, 1, 6, 2, 0, 1 },
            new int[] { 1, 2, 6, 1, 3, 2, 1, 0, 3, 7, 3, 0, 8, 5, 4 },
            new int[] { 5, 0, 1, 5, 4, 0, 3, 2, 6 },
            new int[] { 7, 3, 2, 0, 6, 4, 0, 4, 1, 4, 6, 5 },
            new int[] { 3, 6, 2, 3, 7, 6, 1, 5, 0, 5, 4, 0 },
            new int[] { 4, 1, 6, 4, 6, 5, 1, 0, 6, 2, 6, 3, 0, 3, 6 },
            new int[] { 6, 3, 2, 7, 0, 4, 0, 5, 4, 0, 1, 5 },
            new int[] { 1, 4, 8, 1, 5, 4, 1, 0, 5, 6, 5, 0, 7, 3, 2 },
            new int[] { 2, 0, 6, 2, 6, 3, 0, 1, 6, 4, 6, 5, 1, 5, 6 },
            new int[] { 3, 2, 5, 3, 5, 4, 1, 0, 5, 0, 4, 5 },
            new int[] { 1, 3, 0, 1, 4, 3, 4, 2, 3 },
            new int[] { 1, 3, 5, 0, 3, 1, 0, 2, 3, 0, 4, 2 },
            new int[] { 0, 5, 4, 0, 2, 5, 0, 1, 2, 2, 3, 5 },
            new int[] { 3, 4, 1, 3, 1, 2, 2, 1, 0 },
            new int[] { 0, 1, 6, 5, 2, 7, 5, 7, 4, 7, 2, 3 },
            new int[] { 0, 8, 3, 0, 5, 8, 0, 6, 5, 4, 5, 6, 1, 2, 7 },
            new int[] { 6, 4, 2, 6, 2, 3, 4, 0, 2, 5, 2, 1, 0, 1, 2 },
            new int[] { 3, 5, 1, 3, 1, 2, 0, 4, 1, 4, 2, 1 },
            new int[] { 2, 4, 5, 2, 0, 4, 2, 3, 0, 1, 4, 0 },
            new int[] { 4, 2, 3, 4, 3, 0, 0, 3, 1 },
            new int[] { 1, 4, 6, 1, 6, 0, 4, 5, 6, 3, 6, 2, 5, 2, 6 },
            new int[] { 0, 2, 3, 1, 0, 3 },
            new int[] { 0, 1, 3, 0, 3, 6, 1, 4, 3, 2, 3, 5, 4, 5, 3 },
            new int[] { 5, 1, 0, 5, 0, 3, 4, 2, 0, 2, 3, 0 },
            new int[] { 0, 1, 4, 2, 3, 5 },
            new int[] { 2, 0, 1 },
            new int[] { 3, 0, 2, 1, 0, 3 },
            new int[] { 6, 2, 5, 6, 3, 2, 4, 1, 0 },
            new int[] { 2, 6, 3, 2, 5, 6, 1, 4, 0 },
            new int[] { 6, 3, 2, 6, 7, 3, 5, 4, 0, 4, 1, 0 },
            new int[] { 4, 0, 1, 4, 3, 0, 3, 2, 0 },
            new int[] { 0, 6, 3, 1, 2, 5, 1, 5, 4, 5, 2, 7 },
            new int[] { 4, 3, 2, 4, 1, 3, 4, 0, 1, 1, 5, 3 },
            new int[] { 3, 2, 0, 3, 0, 6, 2, 5, 0, 1, 0, 4, 5, 4, 0 },
            new int[] { 0, 2, 4, 0, 1, 2, 1, 3, 2 },
            new int[] { 4, 1, 0, 4, 2, 1, 4, 3, 2, 5, 1, 2 },
            new int[] { 6, 0, 1, 4, 7, 3, 4, 3, 5, 3, 7, 2 },
            new int[] { 5, 4, 1, 5, 1, 0, 4, 3, 1, 6, 1, 2, 3, 2, 1 },
            new int[] { 0, 1, 2, 1, 3, 2 },
            new int[] { 0, 4, 3, 0, 3, 1, 1, 3, 2 },
            new int[] { 4, 0, 1, 4, 1, 2, 2, 1, 3 },
            new int[] { 3, 2, 1, 0, 3, 1 },
            new int[] { 1, 2, 0, 1, 3, 2, 3, 4, 2 },
            new int[] { 3, 0, 2, 3, 5, 0, 3, 4, 5, 5, 1, 0 },
            new int[] { 0, 1, 5, 4, 2, 6, 4, 6, 7, 6, 2, 3 },
            new int[] { 5, 6, 2, 5, 2, 3, 6, 1, 2, 4, 2, 0, 1, 0, 2 },
            new int[] { 1, 3, 0, 1, 4, 3, 1, 5, 4, 2, 3, 4 },
            new int[] { 0, 4, 6, 0, 6, 3, 4, 5, 6, 2, 6, 1, 5, 1, 6 },
            new int[] { 0, 1, 3, 0, 3, 5, 1, 6, 3, 2, 3, 4, 6, 4, 3 },
            new int[] { 4, 2, 3, 0, 5, 1 },
            new int[] { 0, 3, 5, 1, 3, 0, 1, 2, 3, 1, 4, 2 },
            new int[] { 3, 4, 1, 3, 1, 2, 2, 1, 0 },
            new int[] { 3, 8, 2, 3, 5, 8, 3, 6, 5, 4, 5, 6, 0, 1, 7 },
            new int[] { 3, 5, 1, 3, 1, 2, 0, 4, 1, 4, 2, 1 },
            new int[] { 4, 2, 3, 4, 3, 1, 1, 3, 0 },
            new int[] { 0, 2, 3, 1, 0, 3 },
            new int[] { 4, 2, 3, 4, 3, 1, 5, 0, 3, 0, 1, 3 },
            new int[] { 2, 0, 1 },
            new int[] { 0, 4, 1, 0, 2, 4, 2, 3, 4 },
            new int[] { 0, 4, 1, 2, 5, 3, 5, 7, 3, 5, 6, 7 },
            new int[] { 1, 4, 5, 1, 5, 2, 1, 2, 0, 3, 2, 5 },
            new int[] { 1, 0, 2, 1, 2, 4, 0, 5, 2, 3, 2, 6, 5, 6, 2 },
            new int[] { 2, 5, 3, 4, 5, 2, 4, 1, 5, 4, 0, 1 },
            new int[] { 7, 5, 4, 7, 8, 5, 7, 1, 8, 2, 8, 1, 0, 6, 3 },
            new int[] { 4, 3, 2, 4, 2, 1, 1, 2, 0 },
            new int[] { 5, 3, 2, 5, 2, 0, 4, 1, 2, 1, 0, 2 },
            new int[] { 0, 4, 5, 0, 3, 4, 0, 1, 3, 3, 2, 4 },
            new int[] { 5, 6, 3, 5, 3, 2, 6, 1, 3, 4, 3, 0, 1, 0, 3 },
            new int[] { 3, 5, 6, 3, 6, 2, 5, 4, 6, 1, 6, 0, 4, 0, 6 },
            new int[] { 0, 5, 1, 4, 3, 2 },
            new int[] { 2, 4, 0, 2, 0, 3, 3, 0, 1 },
            new int[] { 2, 5, 1, 2, 1, 3, 0, 4, 1, 4, 3, 1 },
            new int[] { 2, 0, 1, 3, 2, 1 },
            new int[] { 0, 2, 1 },
            new int[] { 1, 2, 0, 2, 3, 0 },
            new int[] { 1, 0, 2, 1, 2, 4, 4, 2, 3 },
            new int[] { 0, 1, 3, 0, 3, 2, 2, 3, 4 },
            new int[] { 1, 0, 2, 3, 1, 2 },
            new int[] { 0, 1, 4, 0, 4, 3, 3, 4, 2 },
            new int[] { 3, 0, 4, 3, 4, 5, 1, 2, 4, 2, 5, 4 },
            new int[] { 0, 1, 3, 2, 0, 3 },
            new int[] { 1, 0, 2 },
            new int[] { 0, 1, 2, 0, 2, 4, 4, 2, 3 },
            new int[] { 2, 3, 1, 0, 2, 1 },
            new int[] { 2, 3, 4, 2, 4, 5, 0, 1, 4, 1, 5, 4 },
            new int[] { 0, 2, 1 },
            new int[] { 0, 1, 2, 3, 0, 2 },
            new int[] { 0, 2, 1 },
            new int[] { 0, 1, 2 },
            new int[] {  }
        };
        #endregion

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
            data = new float[(int)chunkSize.x, (int)chunkSize.y, (int)chunkSize.z];
            positions = new Vector3[(int)chunkSize.x, (int)chunkSize.y, (int)chunkSize.z];
            for (int i = 0; i < (int)chunkSize.x; i++)
            {
                for (int j = 0; j < (int)chunkSize.y; j++)
                {
                    for (int k = 0; k < (int)chunkSize.z; k++)
                    {
                        data[i, j, k] = 1 - (j / chunkSize.y);
                        data[i, j, k] += noise.fastNoise.GetNoise((i + startPoint.x) * sampleRate.x, (k + startPoint.y) * sampleRate.y) * heightVariation;
                        positions[i, j, k] = new Vector3(i * voxelSize.x, j * voxelSize.y, k * voxelSize.z);
                    }
                }
            }

            GenerateSmoothMesh();
        }

        void GenerateMesh()
        {
            List<Vector3> verts = new List<Vector3>();
            List<Vector3> norms = new List<Vector3>();
            List<int> tris = new List<int>();

            for (int i = 0; i < (int)chunkSize.x; i++)
            {
                for (int j = 0; j < (int)chunkSize.y; j++)
                {
                    for (int k = 0; k < (int)chunkSize.z; k++)
                    {
                        if (data[i, j, k] >= baseSurfaceHeight)
                        {
                            Quad q;
                            if (i == (int)chunkSize.x - 1 || data[i + 1, j, k] < baseSurfaceHeight)
                            {
                                q = MakeQuad(positions[i, j, k], Vector3.right, verts.Count);
                                verts.AddRange(q.verts);
                                norms.AddRange(q.norms);
                                tris.AddRange(q.tris);
                            }
                            if (i == 0 || data[i - 1, j, k] < baseSurfaceHeight)
                            {
                                q = MakeQuad(positions[i, j, k], Vector3.left, verts.Count);
                                verts.AddRange(q.verts);
                                norms.AddRange(q.norms);
                                tris.AddRange(q.tris);
                            }
                            if (j == (int)chunkSize.y - 1 || data[i, j + 1, k] < baseSurfaceHeight)
                            {
                                q = MakeQuad(positions[i, j, k], Vector3.up, verts.Count);
                                verts.AddRange(q.verts);
                                norms.AddRange(q.norms);
                                tris.AddRange(q.tris);
                            }
                            if (j == 0 || data[i, j - 1, k] < baseSurfaceHeight)
                            {
                                q = MakeQuad(positions[i, j, k], Vector3.down, verts.Count);
                                verts.AddRange(q.verts);
                                norms.AddRange(q.norms);
                                tris.AddRange(q.tris);
                            }
                            if (k == (int)chunkSize.z - 1 || data[i, j, k + 1] < baseSurfaceHeight)
                            {
                                q = MakeQuad(positions[i, j, k], Vector3.forward, verts.Count);
                                verts.AddRange(q.verts);
                                norms.AddRange(q.norms);
                                tris.AddRange(q.tris);
                            }
                            if (k == 0 || data[i, j, k - 1] < baseSurfaceHeight)
                            {
                                q = MakeQuad(positions[i, j, k], Vector3.back, verts.Count);
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


        Quad MakeQuad(Vector3 pos, Vector3 dir, int vCount)
        {


            Quad q = new Quad();
            q.norms = new Vector3[] { dir, dir, dir, dir };
            q.verts = new Vector3[4];

            pos = new Vector3(pos.x + dir.x * voxelSize.x * 0.5f,
                              pos.y + dir.y * voxelSize.y * 0.5f,
                              pos.z + dir.z * voxelSize.z * 0.5f);
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
            for (int i = 0; i < (int)chunkSize.x; i++)
            {
                for (int j = 0; j < (int)chunkSize.y; j++)
                {
                    for (int k = 0; k < (int)chunkSize.z; k++)
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

        //http://paulbourke.net/geometry/polygonise/
        void GenerateSmoothMesh()
        {
            List<Vector3> verts = new List<Vector3>();
            List<Vector3> norms = new List<Vector3>();
            List<int> tris = new List<int>();

            for (int i = 0; i < (int)chunkSize.x - 1; i++)
            {
                for (int j = 0; j < (int)chunkSize.y - 1; j++)
                {
                    for (int k = 0; k < (int)chunkSize.z - 1; k++)
                    {
                        int vertCount = verts.Count;

                        /*
                          Determine the index into the edge table which
                          tells us which vertices are inside of the surface
                        */
                        int cubeindex = 0;
                        if (data[i, j, k] < baseSurfaceHeight) cubeindex |= 1;
                        if (data[i + 1, j, k] < baseSurfaceHeight) cubeindex |= 2;
                        if (data[i + 1, j, k + 1] < baseSurfaceHeight) cubeindex |= 4;
                        if (data[i, j, k + 1] < baseSurfaceHeight) cubeindex |= 8;
                        if (data[i, j + 1, k] < baseSurfaceHeight) cubeindex |= 16;
                        if (data[i + 1, j + 1, k] < baseSurfaceHeight) cubeindex |= 32;
                        if (data[i + 1, j + 1, k + 1] < baseSurfaceHeight) cubeindex |= 64;
                        if (data[i, j + 1, k + 1] < baseSurfaceHeight) cubeindex |= 128;

                        /* Cube is entirely in/out of the surface */
                        if (edgeTable[cubeindex] == 0)
                            continue;

                        /* Find the vertices where the surface intersects the cube */
                        if ((edgeTable[cubeindex] & 1) == 1)
                            verts.Add(Vector3.Lerp(positions[i, j, k], positions[i + 1, j, k], 0.5f));
                        if ((edgeTable[cubeindex] & 2) == 2)
                            verts.Add(Vector3.Lerp(positions[i + 1, j, k], positions[i + 1, j, k + 1], 0.5f));
                        if ((edgeTable[cubeindex] & 4) == 4)
                            verts.Add(Vector3.Lerp(positions[i + 1, j, k + 1], positions[i, j, k + 1], 0.5f));
                        if ((edgeTable[cubeindex] & 8) == 8)
                            verts.Add(Vector3.Lerp(positions[i, j, k + 1], positions[i, j, k], 0.5f));
                        if ((edgeTable[cubeindex] & 16) == 16)
                            verts.Add(Vector3.Lerp(positions[i, j + 1, k], positions[i + 1, j + 1, k], 0.5f));
                        if ((edgeTable[cubeindex] & 32) == 32)
                            verts.Add(Vector3.Lerp(positions[i + 1, j + 1, k], positions[i + 1, j + 1, k + 1], 0.5f));
                        if ((edgeTable[cubeindex] & 64) == 64)
                            verts.Add(Vector3.Lerp(positions[i + 1, j + 1, k + 1], positions[i, j + 1, k + 1], 0.5f));
                        if ((edgeTable[cubeindex] & 128) == 128)
                            verts.Add(Vector3.Lerp(positions[i, j + 1, k + 1], positions[i, j + 1, k], 0.5f));
                        if ((edgeTable[cubeindex] & 256) == 256)
                            verts.Add(Vector3.Lerp(positions[i, j, k], positions[i, j + 1, k], 0.5f));
                        if ((edgeTable[cubeindex] & 512) == 512)
                            verts.Add(Vector3.Lerp(positions[i + 1, j, k], positions[i + 1, j + 1, k], 0.5f));
                        if ((edgeTable[cubeindex] & 1024) == 1024)
                            verts.Add(Vector3.Lerp(positions[i + 1, j, k + 1], positions[i + 1, j + 1, k + 1], 0.5f));
                        if ((edgeTable[cubeindex] & 2048) == 2048)
                            verts.Add(Vector3.Lerp(positions[i, j, k + 1], positions[i, j + 1, k + 1], 0.5f));
                            
                        for (int n = triTable[cubeindex].Length - 1; n >= 0 ; n--)
                        {
                            tris.Add(triTable[cubeindex][n] + vertCount);
                        }
                    }
                }
            }
            
            chunkMesh.SetVertices(verts);
            chunkMesh.SetTriangles(tris, 0);
            chunkMesh.RecalculateNormals();
        }

    }
}
