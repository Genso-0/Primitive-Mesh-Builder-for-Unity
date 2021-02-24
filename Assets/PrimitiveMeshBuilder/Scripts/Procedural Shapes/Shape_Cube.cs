
using UnityEngine;
namespace Primitive_Mesh_Builder
{
    public class Shape_Cube : Shape
    {
        public bool quadUV;
        void Start()
        {
            BuildMesh_Cube();
        }
        public override void Generate()
        {
           BuildMesh_Cube();
        }
        private void BuildMesh_Cube()
        {
            var vertCount = resolution * resolution * 6;
            int indexCount = ((resolution - 1) * (resolution - 1) * 6) * 6;
            int triIndex = 0;
            SetMeshSize(in vertCount, in indexCount);

            BuildCubeFace(Vector3.forward, ref triIndex);
            BuildCubeFace(Vector3.up, ref triIndex);
            BuildCubeFace(Vector3.right, ref triIndex);
            BuildCubeFace(-Vector3.forward, ref triIndex);
            BuildCubeFace(-Vector3.up, ref triIndex);
            BuildCubeFace(-Vector3.right, ref triIndex);
            SetMesh(verts, tris, uv);
        }
        void BuildCubeFace(Vector3 normal, ref int triIndex)
        {
            Vector3 axisA = new Vector3(normal.y, normal.z, normal.x);
            Vector3 axisB = Vector3.Cross(normal, axisA);
            int vertCount = verts.Count;
            Vector3 offset = scale * normal * .5f;
            for (int y = 0; y < resolution; y++)
                for (int x = 0; x < resolution; x++)
                {
                    Vector2 percent = new Vector2(x, y) / (resolution - 1);
                    Vector3 point = (percent.x - .5f) * axisA + (percent.y - .5f) * axisB;
                    point = offset + (scale * point);

                    uv[verts.Count] = quadUV ? new Vector2(x, y) : new Vector2(percent.x, percent.y);

                    verts.Add(point);

                    if (x != resolution - 1 && y != resolution - 1)
                    {
                        int i = (x + y * resolution) + vertCount;
                        tris[triIndex] = i;
                        tris[triIndex + 1] = i + resolution + 1;
                        tris[triIndex + 2] = i + resolution;

                        tris[triIndex + 3] = i;
                        tris[triIndex + 4] = i + 1;
                        tris[triIndex + 5] = i + resolution + 1;
                        triIndex += 6;
                    }
                }
        }
    }
}