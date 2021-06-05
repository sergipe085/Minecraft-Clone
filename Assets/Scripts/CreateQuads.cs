using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateQuads : MonoBehaviour
{
    enum CubeSide  { TOP, BOTTOM, RIGHT, LEFT, FRONT, BACK }
    enum BlockType { GRASS, DIRT, STONE }

    [SerializeField] private Material mat = null;

    private Vector2[,] blockUVs = new Vector2[,] {
        /*GRASS TOP*/  { new Vector2(0f, 0.9375f),      new Vector2(0.0625f, 0.9375f), new Vector2(0f, 1f),      new Vector2(0.0625f, 1f) },
        /*GRASS SIDE*/ { new Vector2(0.1875f, 0.9375f), new Vector2(0.25f, 0.9365f),   new Vector2(0.1875f, 1f), new Vector2(0.25f, 1f)   },
        /*DIRT*/       { new Vector2(0.125f, 0.9375f),  new Vector2(0.1875f, 0.9375f), new Vector2(0.125f, 1f),  new Vector2(0.1875f, 1f) },
        /*STONE*/      { new Vector2(0.0625f, 0.9375f), new Vector2(0.125f, 0.9375f),  new Vector2(0.0625f, 1f), new Vector2(0.125f, 1f)  }
    };

    private void CreateQuad(CubeSide side, BlockType type, Vector3 position) {
        Mesh mesh = new Mesh();
        mesh.name = "ScriptedMesh";

        Vector3[] vertices  = new Vector3[4];
        Vector3[] normals   = new Vector3[4];
        Vector2[] uvs       = new Vector2[4];
        int[]     triangles = new int[6];

        Vector2 uv00;
        Vector2 uv01;
        Vector2 uv11;
        Vector2 uv10;

        if (type == BlockType.GRASS && side == CubeSide.TOP) {
            uv00 = blockUVs[0, 0];
            uv10 = blockUVs[0, 1];
            uv01 = blockUVs[0, 2];
            uv11 = blockUVs[0, 3];
        }
        else if (type == BlockType.GRASS && side == CubeSide.BOTTOM) {
            uv00 = blockUVs[(int) BlockType.DIRT + 1, 0];
            uv10 = blockUVs[(int) BlockType.DIRT + 1, 1];
            uv01 = blockUVs[(int) BlockType.DIRT + 1, 2];
            uv11 = blockUVs[(int) BlockType.DIRT + 1, 3];
        }
        else {
            uv00 = blockUVs[(int) type + 1, 0];
            uv10 = blockUVs[(int) type + 1, 1];
            uv01 = blockUVs[(int) type + 1, 2];
            uv11 = blockUVs[(int) type + 1, 3];
        }

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
                uvs       = new Vector2[] { uv01, uv11, uv10, uv00 };
                triangles = new int[]     { 3, 0, 2, 0, 1, 2 };
            break;

            case CubeSide.BOTTOM:
                vertices = new Vector3[] { v3, v2, v1, v0 };
                normals = new Vector3[] { Vector3.down, Vector3.down, Vector3.down, Vector3.down };
                uvs = new Vector2[] { uv01, uv11, uv10, uv00 };
                triangles = new int[] { 3, 0, 2, 0, 1, 2 };
            break;

            case CubeSide.FRONT:
                vertices = new Vector3[] { v4, v5, v0, v1 };
                normals = new Vector3[] { Vector3.forward, Vector3.forward, Vector3.forward, Vector3.forward };
                uvs = new Vector2[] { uv01, uv11, uv00, uv10 };
                triangles = new int[] { 3, 1, 2, 2, 1, 0 };
            break;

            case CubeSide.BACK:
                vertices = new Vector3[] { v6, v7, v2, v3 };
                normals = new Vector3[] { Vector3.back, Vector3.back, Vector3.back, Vector3.back };
                uvs = new Vector2[] { uv01, uv11, uv00, uv10 };
                triangles = new int[] { 3, 1, 2, 2, 1, 0 };
            break;

            case CubeSide.RIGHT:
                vertices = new Vector3[] { v5, v6, v1, v2 };
                normals = new Vector3[] { Vector3.right, Vector3.right, Vector3.right, Vector3.right };
                uvs = new Vector2[] { uv01, uv11, uv00, uv10 };
                triangles = new int[] { 3, 1, 2, 2, 1, 0 };
            break;

            case CubeSide.LEFT:
                vertices = new Vector3[] { v0, v3, v4, v7 };
                normals = new Vector3[] { Vector3.left, Vector3.left, Vector3.left, Vector3.left };
                uvs = new Vector2[] { uv00, uv10, uv01, uv11 };
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
        quad.transform.position = position;
        MeshFilter meshFilter = quad.AddComponent(typeof(MeshFilter)) as MeshFilter;
        meshFilter.mesh = mesh;
        MeshRenderer meshRenderer = quad.AddComponent(typeof(MeshRenderer)) as MeshRenderer;
        meshRenderer.material = mat;
    }

    private void CombineMeshes() {
        MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();
        CombineInstance[] combineInstances = new CombineInstance[meshFilters.Length];
        int i = 0;
        while (i < meshFilters.Length) {
            combineInstances[i].mesh = meshFilters[i].mesh;
            combineInstances[i].transform = meshFilters[i].transform.localToWorldMatrix;
            i++;
        }

        MeshFilter meshFilter = gameObject.AddComponent(typeof(MeshFilter)) as MeshFilter;
        meshFilter.mesh = new Mesh();
        meshFilter.mesh.name = "CombinedMesh";
        meshFilter.mesh.CombineMeshes(combineInstances);

        MeshRenderer meshRenderer = gameObject.AddComponent(typeof(MeshRenderer)) as MeshRenderer;
        meshRenderer.material = mat;

        foreach (Transform quad in transform) {
            Destroy(quad.gameObject);
        }
    }

    private void CreateCube(BlockType type, Vector3 position) {
        CreateQuad(CubeSide.TOP, type, position);
        CreateQuad(CubeSide.BOTTOM, type, position);
        CreateQuad(CubeSide.FRONT, type, position);
        CreateQuad(CubeSide.BACK, type, position);
        CreateQuad(CubeSide.RIGHT, type, position);
        CreateQuad(CubeSide.LEFT, type, position);
    }

    private IEnumerator Start() {
        for(int z = 0; z < 4; z++) {
            for (int y = 0; y < 16; y++) {
                for (int x = 0; x < 4; x++) {
                    BlockType type = BlockType.GRASS;
                    if (y < 12) type = BlockType.STONE;
                    else if (y < 15) type = BlockType.DIRT;
                    CreateCube(type, new Vector3(x, y, z));
                    yield return null;
                }
            }
        }
        CombineMeshes();
    }
}
