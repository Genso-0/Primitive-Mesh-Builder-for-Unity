
using System.Collections.Generic;
using UnityEngine;
namespace Primitive_Mesh_Builder
{
    public class Shape_Torus : Shape
    {
        public float radius;
        public float thickness;
        public int segments;
        public bool normaliseSegmentFromCenter;
        public NormalDirection torusAxis; 
        void Start()
        {
            BuildMesh_Torus();
        }
        public override void Generate()
        {
            BuildMesh_Torus();
        }
        void BuildMesh_Torus()
        {
            var normal = GetNormal(torusAxis);
            var vertCount = segments * resolution;
            var triCount = (vertCount * 6);
            SetMeshSize(vertCount, triCount);
           var path = BuildTorusPath(normal);
            BuildPipe(path, normal);
            SetMesh(verts, tris, uv);
        }
        List<Vector3> BuildTorusPath(Vector3 localUp)
        {
            var path = new List<Vector3>();
            var segmentWidth = Mathf.PI * 2f / segments;

            Vector3 axisA = new Vector3(localUp.y, localUp.z, localUp.x);
            Vector3 axisB = Vector3.Cross(localUp, axisA);
            float angle = 0.0f;
            for (int i = 0; i < segments; i++)
            {
                path.Add(GetPointOnDisc(axisA, axisB, radius*scale, angle, Vector3.zero) + transform.position);
                angle += segmentWidth;
            }
            return path;
        } 
        void BuildPipe(List<Vector3> path, Vector3 localUp)
        {
            var segmentWidth = Mathf.PI * 2f / resolution;
            var uXMysteryNumber = 0.16f / radius;
            var circumferance = (radius * 2 * scale) * Mathf.PI;
            var triIndex = 0;
            var vertIndex = 0;
            var finalThickness = thickness * scale;
            for (int ringCount = 0; ringCount < path.Count; ringCount++)
            {
                var center = path[ringCount];
                Vector3 localRight = (transform.position - center);//swap these to make outer facing torus into inner facing
                localRight = normaliseSegmentFromCenter ? localRight.normalized : localRight;
                bool notLastRing = ringCount < path.Count - 1;
                float angle = 0.0f;
                //Constructs a ring around each vertex center which we use to then join into a pipe. 
                for (int ringPoint = 0; ringPoint < resolution; ++ringPoint)
                {
                    //Construct verts, uvs and normals for the outwards facing part of the pipe mesh. 
                     verts.Add(GetPointOnDisc(localUp, localRight, finalThickness, angle, center - transform.position)); 
                     uv[vertIndex] = GetUVs(ringPoint, resolution, circumferance, angle, in uXMysteryNumber);
                      normals[vertIndex] = center - verts[vertIndex];
                     
                    bool isLastQuadOfCylinder = ringPoint >= resolution - 1;
                    SetPipeTris(triIndex, notLastRing, ringCount, ringPoint, isLastQuadOfCylinder);
                    vertIndex++;
                    triIndex += 6;
                    angle += segmentWidth;
                }
            }
        }
        internal void SetPipeTris(int triIndex, bool notLastRing, int ringCount, int ringPoint, bool isLastQuadOfCylinder)
        {
            if (notLastRing)  
            {
                int v0 = resolution * ringCount;
                int v1 = ringPoint + v0;
                int v2 = v1 + resolution;
                int v3 = v1 + 1;
                int v4 = v2 + 1;
                if (!isLastQuadOfCylinder)//if we are not on the last quad of the current ring.
                    SetQuad(triIndex, v1, v2, v3, v3, v2, v4);
                else // if we are on the last quad of the current ring we need to treat this special case differently to other quads on the ring.
                    SetQuad(triIndex, v1, v2, v3, v3, v0, v1);
            }
            else //connecting last ring to first ring
            {
                int v0 = resolution * ringCount;
                int v1 = ringPoint + v0;
                int v2 = ringPoint;
                int v3 = v1 + 1;
                int v4 = v2 + 1;
                if (!isLastQuadOfCylinder)
                    SetQuad(triIndex, v1, v2, v3, v4, v3, v2);
                else
                    SetQuad(triIndex, v1, v2, v0, v0, v2, 0);
            }
        }
        internal void SetQuad(int index, int v1, int v2, int v3, int v4, int v5, int v6)
        {
            tris[0 + index] = v1;
            tris[1 + index] = v2;
            tris[2 + index] = v3;

            tris[3 + index] = v4;
            tris[4 + index] = v5;
            tris[5 + index] = v6;
        }
        internal static Vector3 GetPointOnDisc(in Vector3 localUp, in Vector3 localRight, in float radius, in float angle, in Vector3 offset)
        {
            float x = Mathf.Cos(angle) * radius;
            float y = Mathf.Sin(angle) * radius;
            return (x * localUp + y * localRight) + offset;
        }
        internal static Vector2 GetUVs(in int pointOnRing, in int resolution, in float pathLength, in float cumulativeLengthAtVertex, in float uXMysteryNumber)
        {
            //This makes sure that by the time we get back to the start of the mesh the end uv and the start uv have the same value of 0.
            //function used f(x) = 1-|2x-1|
            var u = 1 - Mathf.Abs(2 * ((float)pointOnRing / resolution) - 1) / uXMysteryNumber;
            var v = 1 - Mathf.Abs(2 * (cumulativeLengthAtVertex / pathLength) - 1) * pathLength;
            return new Vector2(u, v);
        }

    }
}