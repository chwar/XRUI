/*
MIT License

Copyright (c) 2016 Matt Favero

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/

using UnityEngine;

namespace com.chwar.xrui
{
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshRenderer))]
    public class CurvedPlane : MonoBehaviour
    {
        private class MeshData
        {
            public Vector3[] Vertices { get; set; }
            public int[] Triangles { get; set; }
            public Vector2[] UVs { get; set; }
        }

        public float height = 1f;
        public float radius = 2f;
        [Range(1, 1024)] public int numSegments = 16;
        [Range(0f, 360f)] public float curvatureDegrees = 60f;
        public bool useArc = true;
        public Mesh mesh;
    
        private MeshData _plane;

        public void Generate(RenderTexture rt)
        {
            GenerateScreen();
            UpdateMeshFilter(rt);
        }

        private void UpdateMeshFilter(RenderTexture rt)
        {
            var filter = GetComponent<MeshFilter>();
            var meshRenderer = GetComponent<MeshRenderer>();
            Material m = new Material(Shader.Find("UI/Default"))
            {
                mainTexture = rt,
                
            };
            meshRenderer.sharedMaterial = m;

            mesh = new Mesh
            {
                vertices = _plane.Vertices,
                triangles = _plane.Triangles,
                uv = _plane.UVs
            };
            mesh.RecalculateBounds();
            mesh.RecalculateNormals();
            mesh.RecalculateTangents();
            filter.mesh = mesh;
        }

        private void GenerateScreen()
        {
            _plane = new MeshData
            {
                Vertices = new Vector3[(numSegments + 2)*2],
                Triangles = new int[numSegments*6],
                UVs = new Vector2[(numSegments + 2)*2]
            };

            int i,j;
            for (i = j = 0; i < numSegments+1; i++)
            {
                GenerateVertexPair(ref i);

                if (i < numSegments)
                {
                    GenerateLeftTriangle(ref i, ref j);
                    GenerateRightTriangle(ref i, ref j);
                }
            }
        }

        private void GenerateVertexPair(ref int i)
        {
            float amt = ((float)i) / numSegments;
            float arcDegrees = curvatureDegrees * Mathf.Deg2Rad;
            float theta = -0.5f + amt;

            var x = useArc ? Mathf.Sin(theta * arcDegrees) * radius : (-0.5f * radius) + (amt * radius);
            var z = Mathf.Cos(theta * arcDegrees) * radius;
            
            _plane.Vertices[i] = new Vector3(x, height / 2f, z);
            _plane.Vertices[i + numSegments + 1] = new Vector3(x, -height / 2f, z);
            _plane.UVs[i] = new Vector2(amt, 1);
            _plane.UVs[i + numSegments + 1] = new Vector2(amt, 0);
        }

        private void GenerateLeftTriangle(ref int i, ref int j)
        {
            _plane.Triangles[j++] = i;
            _plane.Triangles[j++] = i + 1;
            _plane.Triangles[j++] = i + numSegments + 1;
        }

        private void GenerateRightTriangle(ref int i, ref int j)
        {
            _plane.Triangles[j++] = i + 1;
            _plane.Triangles[j++] = i + numSegments + 2;
            _plane.Triangles[j++] = i + numSegments + 1;
        }
    }
}