 
using System.Collections.Generic;
using UnityEngine;

namespace Primitive_Mesh_Builder
{
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshRenderer))]
    [ExecuteAlways]
    public class Shape : MonoBehaviour
    {
        [HideInInspector] public MeshFilter meshFilter;
        [HideInInspector] public MeshCollider mesh_colider;
        [Header("Debug")]
        public bool showVerts;
        public bool showTriangles;

        [Header("Properties")]
        [Range(2, 100)] public int resolution = 20;
        public float scale = 1; 
        //Mesh Components
        internal List<Vector3> verts;
        internal int[] tris;
        internal Vector2[] uv;
        internal Vector3[] normals;
        public virtual void Generate()
        {
            Debug.Log("Generate was called from shape base");
        }
        void OnValidate()
        {
            Generate();
        }
        void Awake()
        {
            meshFilter = GetComponent<MeshFilter>();
            mesh_colider = GetComponent<MeshCollider>();
            meshFilter.sharedMesh = new Mesh(); 
        }
        internal void SetMeshSize(in int vertCount, in int triCount)
        {
            verts = new List<Vector3>(vertCount);
            tris = new int[triCount];
            uv = new Vector2[vertCount];
            normals = new Vector3[vertCount];
        }
        internal void SetMesh(List<Vector3> vertices, int[] tris, Vector2[] uv)
        {
            meshFilter.sharedMesh.Clear();
            meshFilter.sharedMesh.SetVertices(vertices);
            meshFilter.sharedMesh.SetIndices(tris, MeshTopology.Triangles, 0);
            meshFilter.sharedMesh.uv = uv;

            if (mesh_colider != null)
                mesh_colider.sharedMesh = meshFilter.sharedMesh;
            meshFilter.sharedMesh.RecalculateBounds(); 
            meshFilter.sharedMesh.RecalculateNormals();
            meshFilter.sharedMesh.RecalculateTangents();
            meshFilter.sharedMesh.Optimize();
        }
        internal Vector3 GetNormal(NormalDirection normal)
        {
            switch (normal)
            {
                case NormalDirection.Forward:
                    return Vector3.forward;
                case NormalDirection.Up:
                    return Vector3.up;
                case NormalDirection.Right:
                    return Vector3.right;
                case NormalDirection.Back:
                    return -Vector3.forward;
                case NormalDirection.Down:
                    return -Vector3.up;
                case NormalDirection.Left:
                    return -Vector3.right;
                default: return Vector3.forward;
            }
        }
        public enum NormalDirection
        {
            Forward,
            Up,
            Right,
            Back,
            Down,
            Left,
        }
        public void Log(string text)
        {
            Debug.Log(text);
        }
        public void DebugTris(int t1, int t2, int t3)
        {
            Debug.Log($"{t1}:{tris[t1]}," +
                $" {t2}:{tris[t2]}," +
                $" {t3}:{tris[t3]}");
        }
    }
}