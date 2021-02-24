 
using UnityEngine;
namespace Primitive_Mesh_Builder
{
    public class Shape_Cone : Shape
    {
        public float radius = .5f;
        public float height = .5f;
        public NormalDirection facing;
        void Start()
        {
            BuildMesh_Cone();
        }
        public override void Generate()
        {
            BuildMesh_Cone();
        }
        void BuildMesh_Cone()
        {
            var vertCount = ((resolution + 2)*2);
            int indexCount = (resolution * 3)*2;
         

            Vector3 normal = GetNormal(facing);
            SetMeshSize(vertCount, indexCount);
            Vector3 axisA = new Vector3(normal.y, normal.z, normal.x);
            Vector3 axisB = Vector3.Cross(normal, axisA);
            var segmentWidth = Mathf.PI * 2f / resolution;
            BuildDisc(axisA, axisB, Vector3.zero, segmentWidth);
            BuildDisc(axisA, axisB, -Vector3.Cross(axisA,axisB).normalized*height*scale, segmentWidth, true); 
            SetMesh(verts, tris, uv);
        }
        void BuildDisc(Vector3 axisA, Vector3 axisB, Vector3 center, float segmentWidth, bool fliped = false)
        {
            var vertCount = verts.Count;
            float angle = 0.0f;

            verts.Add(center);
            uv[verts.Count] = new Vector2(center.x, center.y);
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