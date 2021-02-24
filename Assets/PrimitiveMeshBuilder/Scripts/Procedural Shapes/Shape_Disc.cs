
using UnityEngine;

namespace Primitive_Mesh_Builder
{
    public class Shape_Disc : Shape
    {
        public float radius = .5f;
        public NormalDirection facing;
        public bool doubleSided;
        void Start()
        {
            BuildMesh_Circle();
        }
        public override void Generate()
        {
            BuildMesh_Circle();
        } 
        void BuildMesh_Circle()
        {
            var vertCount = resolution + 2;
            int indexCount = resolution * 3;
            if (doubleSided)
            {
                vertCount *= 2;
                indexCount *= 2;
            }
            Vector3 normal = GetNormal(facing);
            SetMeshSize(in vertCount, in indexCount);
            Vector3 axisA = new Vector3(normal.y, normal.z, normal.x);
            Vector3 axisB = Vector3.Cross(normal, axisA);
            var segmentWidth = Mathf.PI * 2f / resolution;
            BuildDisc(axisA, axisB, segmentWidth);
            if (doubleSided)
                BuildDisc(axisA, axisB, segmentWidth, true);
            SetMesh(verts, tris, uv);
        }
        void BuildDisc(Vector3 axisA, Vector3 axisB, float segmentWidth, bool fliped = false)
        {
            var vertCount = verts.Count;
            float angle = 0.0f;

            verts.Add(Vector3.zero);
            SetDiscVertex(axisA, axisB, segmentWidth, ref angle);
            int tri1 = fliped ? 2 : 0;
            int tri2 = 1;
            int tri3 = fliped ? 0 : 2;
            int jCount = fliped ? vertCount - 2 : 0;
            for (int i = 2; i < resolution + 2; ++i)
            {
                SetDiscVertex(axisA, axisB, segmentWidth, ref angle);

                var j = ((i - 2) + jCount) * 3;
                tris[j + tri1] = 0 + vertCount;
                tris[j + tri2] = i - 1 + vertCount;
                tris[j + tri3] = i + vertCount;
            }
        }
        void SetDiscVertex(Vector3 axisA, Vector3 axisB, float segmentWidth, ref float angle)
        {
            float x = Mathf.Cos(angle) * radius * scale;
            float y = Mathf.Sin(angle) * radius * scale;
            var point = x * axisA + y * axisB;
            uv[verts.Count] = new Vector2(x, y);
            verts.Add(point);
            angle += segmentWidth;
        }
    }
}