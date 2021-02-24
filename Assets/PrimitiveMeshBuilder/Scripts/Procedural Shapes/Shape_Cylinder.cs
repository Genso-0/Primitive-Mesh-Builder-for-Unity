#pragma warning disable 0649
using UnityEngine;

namespace Primitive_Mesh_Builder
{
    class Shape_Cylinder : Shape
    {
        public float radius = .25f;
        public float height = 1;
        public NormalDirection normalDirection;
        void Start()
        {
            BuildMesh_Cylinder();
        }
        public override void Generate()
        {
            BuildMesh_Cylinder();
        }
        void BuildMesh_Cylinder()
        {
            var vertCount = (resolution + 2) * 2;
            int indexCount = ((resolution * 3) * 2) + (resolution * 6);
            Vector3 normal = GetNormal(normalDirection);
            SetMeshSize(in vertCount, in indexCount);
            int triIndex = 0;
            BuildCylinderDiscs(normal, ref triIndex);
            ConnectDiscs(triIndex);
            SetMesh(verts, tris, uv);
        }
        void ConnectDiscs(int triIndex)
        {
            for (int i = 0; i < resolution; i++)
            {
                tris[triIndex] = i + 1;
                tris[triIndex + 1] = i + 2 + resolution + 1;
                tris[triIndex + 2] = i + 3 + resolution + 1;

                tris[triIndex + 3] = i + 2;
                tris[triIndex + 4] = i + 1;
                tris[triIndex + 5] = i + 3 + resolution + 1;
                triIndex += 6;
            }
        }
        public void BuildCylinderDiscs(Vector3 normal, ref int triIndex)
        {
            Vector3 axisA = new Vector3(normal.y, normal.z, normal.x);
            Vector3 axisB = Vector3.Cross(normal, axisA);
            float angle = 0.0f;
            float segmentWidth = Mathf.PI * 2f / resolution;
            Vector3 offset = height * normal * .5f * scale;
            BuildDisc(axisA, axisB, offset, angle, segmentWidth, ref triIndex);
            BuildDisc(axisA, axisB, -offset, angle, segmentWidth, ref triIndex, true);
        }
        void BuildDisc(Vector3 axisA, Vector3 axisB, Vector3 offset, float angle, float segmentWidth, ref int triIndex, bool fliped = false)
        {
            var vertCount = verts.Count;
            
            verts.Add(Vector3.zero + offset);
            SetDiscVertex(axisA, axisB, offset, segmentWidth, ref angle);
            int tri1 = fliped ? 2 : 0;
            int tri2 = 1;
            int tri3 = fliped ? 0 : 2;
            int jCount = fliped ? vertCount - 2 : 0;
            for (int i = 2; i < resolution + 2; ++i)
            {
                SetDiscVertex(axisA, axisB, offset, segmentWidth, ref angle);

                var j = ((i - 2) + jCount) * 3;
                tris[j + tri1] = 0 + vertCount;
                tris[j + tri2] = i - 1 + vertCount;
                tris[j + tri3] = i + vertCount;
                triIndex += 3;
            }
        }

        void SetDiscVertex(Vector3 axisA, Vector3 axisB, Vector3 offset, float segmentWidth, ref float angle)
        {
            float x = Mathf.Cos(angle) * radius * scale;
            float y = Mathf.Sin(angle) * radius * scale;

            var point = x * axisA + y * axisB;
            point += offset;
            uv[verts.Count] = new Vector2(x, y); 
            verts.Add(point);
            angle += segmentWidth;
        }
    }
}
