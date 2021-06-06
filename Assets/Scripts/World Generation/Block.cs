using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block
{
    enum CubeSide  { TOP, BOTTOM, RIGHT, LEFT, FRONT, BACK }
    public enum BlockType { GRASS, DIRT, STONE, COAL, BEDROCK, AIR }

    [SerializeField] private Material   mat        = null;
    [SerializeField] private BlockType  blockType;
    [SerializeField] private Vector3    position   = Vector3.zero;
    [SerializeField] private Chunck     owner      = null;
    [SerializeField] private GameObject parent     = null;
    [SerializeField] private bool       isSolid    = false;

    public Block(Material _mat, BlockType _blockType, Vector3 _position, GameObject _parent, Chunck _owner) {
        mat       = _mat;
        blockType = _blockType;
        position  = _position;
        parent    = _parent;
        owner     = _owner;
        isSolid   = blockType != BlockType.AIR;
    }

    private Vector2[,] blockUVs = new Vector2[,] {
        /*GRASS TOP*/  { new Vector2(0f, 0.9375f),      new Vector2(0.0625f, 0.9375f), new Vector2(0f, 1f),         new Vector2(0.0625f, 1f)     },
        /*GRASS SIDE*/ { new Vector2(0.1875f, 0.9375f), new Vector2(0.25f, 0.9365f),   new Vector2(0.1875f, 1f),    new Vector2(0.25f, 1f)       },
        /*DIRT*/       { new Vector2(0.125f, 0.9375f),  new Vector2(0.1875f, 0.9375f), new Vector2(0.125f, 1f),     new Vector2(0.1875f, 1f)     },
        /*STONE*/      { new Vector2(0.0625f, 0.9375f), new Vector2(0.125f, 0.9375f),  new Vector2(0.0625f, 1f),    new Vector2(0.125f, 1f)      },
        /*COAL*/       { new Vector2(0.125f, 0.8125f),  new Vector2(0.1875f, 0.8125f), new Vector2(0.125f, 0.875f), new Vector2(0.1875f, 0.875f) },
        /*BEDROCK*/    { new Vector2(0.0625f, 0.875f),  new Vector2(0.125f, 0.875f), new Vector2(0.0625f, 0.9375f), new Vector2(0.125f, 0.9375f) },
    };

    private void CreateQuad(CubeSide side) {
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

        if (blockType == BlockType.GRASS && side == CubeSide.TOP) {
            uv00 = blockUVs[0, 0];
            uv10 = blockUVs[0, 1];
            uv01 = blockUVs[0, 2];
            uv11 = blockUVs[0, 3];
        }
        else if (blockType == BlockType.GRASS && side == CubeSide.BOTTOM) {
            uv00 = blockUVs[(int) BlockType.DIRT + 1, 0];
            uv10 = blockUVs[(int) BlockType.DIRT + 1, 1];
            uv01 = blockUVs[(int) BlockType.DIRT + 1, 2];
            uv11 = blockUVs[(int) BlockType.DIRT + 1, 3];
        }
        else {
            uv00 = blockUVs[(int) blockType + 1, 0];
            uv10 = blockUVs[(int) blockType + 1, 1];
            uv01 = blockUVs[(int) blockType + 1, 2];
            uv11 = blockUVs[(int) blockType + 1, 3];
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
                uvs = new Vector2[] { uv11, uv01, uv10, uv00 };
                triangles = new int[] { 3, 1, 2, 2, 1, 0 };
            break;

            case CubeSide.BACK:
                vertices = new Vector3[] { v6, v7, v2, v3 };
                normals = new Vector3[] { Vector3.back, Vector3.back, Vector3.back, Vector3.back };
                uvs = new Vector2[] { uv11, uv01, uv10, uv00 };
                triangles = new int[] { 3, 1, 2, 2, 1, 0 };
            break;

            case CubeSide.RIGHT:
                vertices = new Vector3[] { v5, v6, v1, v2 };
                normals = new Vector3[] { Vector3.right, Vector3.right, Vector3.right, Vector3.right };
                uvs = new Vector2[] { uv11, uv01, uv10, uv00 };
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
        quad.transform.parent = parent.transform;
        quad.transform.position = position;

        MeshFilter meshFilter = quad.AddComponent(typeof(MeshFilter)) as MeshFilter;
        meshFilter.mesh = mesh;

        // MeshRenderer meshRenderer = quad.AddComponent(typeof(MeshRenderer)) as MeshRenderer;
        // meshRenderer.material = mat;
    }

    private int ConvertBlockIndexToLocal(int i) {
        if (i == -1) {
            i = World.chunckSize - 1;
        }
        else if (i == World.chunckSize) {
            i = 0;
        }
        return i;
    }

    private bool HasSolidNeighbour(int x, int y, int z) {
        Block[,,] chunckData;

        if (x < 0 || x >= World.chunckSize ||
            y < 0 || y >= World.chunckSize ||
            z < 0 || z >= World.chunckSize) {
                Vector3 neighbourChunckPos = parent.transform.position + new Vector3((x - (int)position.x) * World.chunckSize, (y - (int)position.y) * World.chunckSize, (z - (int)position.z) * World.chunckSize);
                string neighbourChunckName = World.BuildChunckName(neighbourChunckPos);

                x = ConvertBlockIndexToLocal(x);
                y = ConvertBlockIndexToLocal(y);
                z = ConvertBlockIndexToLocal(z);
                
                Chunck neighbourChunk;
                if (World.chuncks.TryGetValue(neighbourChunckName, out neighbourChunk)) {
                    chunckData = neighbourChunk.chunckData;
                }
                else {
                    return false;
                }
            } else {
                chunckData = owner.chunckData;
            }

        try {
            return chunckData[x, y, z].isSolid;
        } catch (System.IndexOutOfRangeException e) {}

        return false;
    }

    public void Draw() {
        if (blockType == BlockType.AIR) return;

        if (!HasSolidNeighbour((int)position.x, (int)position.y + 1, (int)position.z)) {
            CreateQuad(CubeSide.TOP);
        }
        if (!HasSolidNeighbour((int)position.x, (int)position.y - 1, (int)position.z)) {
            CreateQuad(CubeSide.BOTTOM);
        }
        if (!HasSolidNeighbour((int)position.x, (int)position.y, (int)position.z + 1)) {
            CreateQuad(CubeSide.FRONT);
        }
        if (!HasSolidNeighbour((int)position.x, (int)position.y, (int)position.z - 1)) {
            CreateQuad(CubeSide.BACK);
        }
        if (!HasSolidNeighbour((int)position.x + 1, (int)position.y, (int)position.z)) {
            CreateQuad(CubeSide.RIGHT);
        }
        if (!HasSolidNeighbour((int)position.x - 1, (int)position.y, (int)position.z)) {
            CreateQuad(CubeSide.LEFT);
        }    
    }
}
