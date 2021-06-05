using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateQuads : MonoBehaviour
{
    [SerializeField] private Material mat = null;

    private void CreateQuad() {
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
        Vector3 v7 = new Vector3(-0.5f, 0.5f, -0.5f);

        vertices  = new Vector3[] { v4, v5, v0, v1 };
        normals   = new Vector3[] { Vector3.forward, Vector3.forward, Vector3.forward, Vector3.forward };
        uvs       = new Vector2[] { uv00, uv01, uv11, uv10 };
        triangles = new int[]     { 3, 1, 2, 2, 1, 0 };

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

    private void Start() {
        CreateQuad();
    }
}
