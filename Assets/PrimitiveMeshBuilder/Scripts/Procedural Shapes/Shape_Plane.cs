using UnityEngine;
namespace Primitive_Mesh_Builder
{
    public class Shape_Plane : Shape
    {
        public NormalDirection normalDirection;
        public bool doubleSided;
        public bool quadUV;
        void Start()
        {
            BuildMesh_Plane();
        }
        public override void Generate()
        {
            BuildMesh_Plane();
        }
        void BuildMesh_Plane()
        {
            var vertCount = resolution * resolution;
            int indexCount = (resolution - 1) * (resolution - 1) * 6;
            if (doubleSided)
            {
                vertCount *= 2;
                indexCount *= 2;
            }
            SetMeshSize(in vertCount, in indexCount);
            Vector3 normal = GetNormal(normalDirection);
            int triIndex = 0;
            Vector3 axisA = new Vector3(normal.y, normal.z, normal.x);
            Vector3 axisB = Vector3.Cross(normal, axisA);
            BuildPlane(axisA, axisB, ref triIndex);
            if (doubleSided)
                BuildPlane(axisA, axisB, ref triIndex, true);
            SetMesh(verts, tris, uv);
        }
        void BuildPlane(Vector3 axisA, Vector3 axisB, ref int triIndex, bool fliped = false)
        {
            int vertCount = verts.Count;
            int tri1 = fliped ? 2 : 0;
            int tri2 = 1;
            int tri3 = fliped ? 0 : 2;
            int tri4 = fliped ? 5 : 3;
            int tri5 = 4;
            int tri6 = fliped ? 3 : 5;
            for (int y = 0; y < resolution; y++)
            {
                for (int x = 0; x < resolution; x++)
                {
                    Vector2 percent = new Vector2(x, y) / (resolution - 1);
                    Vector3 point = (percent.x - .5f) * axisA + (percent.y - .5f) * axisB;
                    point = scale * point;
                    uv[verts.Count] = quadUV ? new Vector2(x, y) : new Vector2(percent.x, percent.y);
                    verts.Add(point);

                    if (x != resolution - 1 && y != resolution - 1)
                    {
                        int i = (x + y * resolution) + vertCount;
                        tris[triIndex + tri1] = i;
                        tris[triIndex + tri2] = i + resolution + 1;
                        tris[triIndex + tri3] = i + resolution;

                        tris[triIndex + tri4] = i;
                        tris[triIndex + tri5] = i + 1;
                        tris[triIndex + tri6] = i + resolution + 1;
                        triIndex += 6;
                    }
                }
            }
        }
    }
}
