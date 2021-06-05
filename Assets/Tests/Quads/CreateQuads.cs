using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateQuads : MonoBehaviour
{
    [SerializeField] private Material mat = null;

    enum CubeSide { TOP, BOTTOM, RIGHT, LEFT, FRONT, BACK }

    private void CreateQuad(CubeSide side) {
        Mesh mesh = new Mesh();
        mesh.name = "ScriptedMesh";

        Vector3[] vertices  = new Vector3[4];
        Vector3[] normals   = new Vector3[4];
        Vector2[] uvs       = new Vector2[4];
        int[]     triangles = new int[6];

        //All possible uvs
        Vector2 uv00 = new Vector2(0f, 0f);
        Vector2 uv01 = new Vector2(0f, 1f);
        Vector2 uv11 = new Vector2(1f, 1f);
        Vector2 uv10 = new Vector2(1f, 0f);

        //All possible vertices
        Vector3 v0 = new Vector3(-0.5f, -0.5f, 0.5f);
        Vector3 v1 = new Vector3( 0.5f, -0.5f, 0.5f);
        Vector3 v2 = new Vector3( 0.5f, -0.5f,-0.5f);
        Vector3 v3 = new Vector3(-0.5f, -0.5f,-0.5f);
        Vector3 v4 = new Vector3(-0.5f,  0.5f, 0.5f);
        Vector3 v5 = new Vector3( 0.5f,  0.5f, 0.5f);
        Vector3 v6 = new Vector3( 0.5f,  0.5f,-0.5f);
        Vector3 v7 = new Vector3(-0.5f,  0.5f,-0.5f);

        switch(side) {
            case CubeSide.TOP:
                vertices  = new Vector3[] { v4, v5, v6, v7 };
                normals   = new Vector3[] { Vector3.up, Vector3.up, Vector3.up, Vector3.up };
                uvs       = new Vector2[] { uv01, uv11, uv00, uv10 };
                triangles = new int[]     { 3, 1, 2, 2, 1, 0 };
            break;

            case CubeSide.BOTTOM:
                vertices = new Vector3[] { v0, v1, v2, v3 };
                normals = new Vector3[] { Vector3.down, Vector3.down, Vector3.down, Vector3.down };
                uvs = new Vector2[] { uv01, uv11, uv00, uv10 };
                triangles = new int[] { 3, 1, 2, 2, 1, 0 };
            break;

            case CubeSide.FRONT:
                vertices = new Vector3[] { v4, v5, v0, v1 };
                normals = new Vector3[] { Vector3.forward, Vector3.forward, Vector3.forward, Vector3.forward };
                uvs = new Vector2[] { uv01, uv11, uv00, uv10 };
                triangles = new int[] { 3, 1, 2, 2, 1, 0 };
            break;
        }

        mesh.vertices  = vertices;
        mesh.normals   = normals;
        mesh.uv        = uvs;
        mesh.triangles = triangles;

        mesh.RecalculateBounds();
        mesh.RecalculateNormals();

        GameObject quad = new GameObject("quad");
        quad.transform.parent = this.transform;
        MeshFilter meshFilter = quad.AddComponent(typeof(MeshFilter)) as MeshFilter;
        meshFilter.mesh = mesh;
        MeshRenderer meshRenderer = quad.AddComponent(typeof(MeshRenderer)) as MeshRenderer;
        meshRenderer.material = mat;
    }

    private void CreateCube() {
        CreateQuad(CubeSide.TOP);
        CreateQuad(CubeSide.BOTTOM);
        CreateQuad(CubeSide.FRONT);
    }

    private void Start() {
        CreateCube();
    }
}
