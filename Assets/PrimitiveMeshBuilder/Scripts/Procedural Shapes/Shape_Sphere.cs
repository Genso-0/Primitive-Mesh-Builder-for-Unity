
using UnityEngine;
namespace Primitive_Mesh_Builder
{
    public class Shape_Sphere : Shape
    {
        public bool quadUV;
        void Start()
        {
            BuildMesh_Sphere();
        }
        public override void Generate()
        {
            BuildMesh_Sphere();
        }
        void BuildMesh_Sphere()
        {
            var vertCount = resolution * resolution * 6;
            int indexCount = ((resolution - 1) * (resolution - 1) * 6) * 6;
            int triIndex = 0;
            SetMeshSize(in vertCount, in indexCount);

            BuildMesh_SphereFace(Vector3.forward, ref triIndex);
            BuildMesh_SphereFace(Vector3.up, ref triIndex);
            BuildMesh_SphereFace(Vector3.right, ref triIndex);
            BuildMesh_SphereFace(-Vector3.forward, ref triIndex);
            BuildMesh_SphereFace(-Vector3.up, ref triIndex);
            BuildMesh_SphereFace(-Vector3.right, ref triIndex);

            SetMesh(verts, tris, uv);
        }
        void BuildMesh_SphereFace(Vector3 localUp, ref int triIndex)
        {
            var axisA = new Vector3(localUp.y, localUp.z, localUp.x);
            var axisB = Vector3.Cross(localUp, axisA);
            var vertCount = verts.Count;
            var offset = scale * localUp * .5f;
            for (int y = 0; y < resolution; y++)
            {
                for (int x = 0; x < resolution; x++)
                {
                    Vector2 percent = new Vector2(x, y) / (resolution - 1);
                    Vector3 point = scale * ((percent.x - .5f) * axisA + (percent.y - .5f) * axisB);
                    point = offset + point;
                    point = point.normalized * scale * .5f;
                    uv[verts.Count] = quadUV ? new Vector2(x, y) : new Vector2(percent.x, percent.y);
                    verts.Add(point);

                    int i = (x + y * resolution) + vertCount;
                    if (x != resolution - 1 && y != resolution - 1)
                    {
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
}