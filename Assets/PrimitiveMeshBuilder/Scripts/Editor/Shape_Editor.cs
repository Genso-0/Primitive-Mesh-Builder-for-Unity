#if UNITY_EDITOR 
using UnityEditor;
using UnityEngine;

namespace Primitive_Mesh_Builder
{
    [CustomEditor(typeof(Shape), true)]
    public class Shape_Editor : Editor
    {
        [DrawGizmo(GizmoType.Selected | GizmoType.Active)]
        static void DrawGizmos(Shape shape, GizmoType gizmoType)
        {
            var mesh = shape.GetComponent<MeshFilter>().sharedMesh;
            if (mesh == null) return;

            Gizmos.matrix = shape.transform.localToWorldMatrix;

            var verts = mesh.vertices;
            var tris = mesh.triangles;
            var uvs = mesh.uv;
            
            if (shape.showVerts)
                for (int i = 0; i < verts.Length; i++)
                {
                    var v = verts[i];
                    var uv = uvs[i];
                    Gizmos.color = new Color(uv.x, uv.y, 0f);
                    Gizmos.DrawSphere(v, 0.01f);
                }
            Gizmos.color = Color.white;
            if (shape.showTriangles)
                for (int i = 0; i < tris.Length; i += 3)
                {
                    var a = verts[tris[i]];
                    var b = verts[tris[i + 1]];
                    var c = verts[tris[i + 2]];
                    Gizmos.DrawLine(a, b);
                    Gizmos.DrawLine(b, c);
                    Gizmos.DrawLine(c, a);
                }
            Gizmos.matrix = Matrix4x4.identity;
        }
    }
}
#endif