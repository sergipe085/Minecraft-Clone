using System;
using System.Collections.Generic;
using UnityEngine;

namespace Minecraft.WorldGeneration 
{
    public static class BlockData
    {

        //All possible vertices
        public static Vector3[] vertices = new Vector3[] {
            new Vector3(-0.5f, -0.5f, 0.5f), //0
            new Vector3( 0.5f, -0.5f, 0.5f), //1
            new Vector3( 0.5f, -0.5f,-0.5f), //2
            new Vector3(-0.5f, -0.5f,-0.5f), //3
            new Vector3(-0.5f,  0.5f, 0.5f), //4
            new Vector3( 0.5f,  0.5f, 0.5f), //5
            new Vector3( 0.5f,  0.5f,-0.5f), //6
            new Vector3(-0.5f,  0.5f,-0.5f), //7
        };

        public static Dictionary<BlockFace, Vector3[]> verticesFace = new Dictionary<BlockFace, Vector3[]>() {
            { BlockFace.TOP, new Vector3[] { vertices[4], vertices[5], vertices[6], vertices[7] } },
            { BlockFace.BOTTOM, new Vector3[] { vertices[0], vertices[1], vertices[2], vertices[3] } },
            { BlockFace.RIGHT, new Vector3[] { vertices[5], vertices[6], vertices[1], vertices[2] } },
            { BlockFace.LEFT, new Vector3[] { vertices[7], vertices[4], vertices[3], vertices[0] } },
            { BlockFace.FRONT, new Vector3[] { vertices[4], vertices[5], vertices[0], vertices[1] } },
            { BlockFace.BACK, new Vector3[] { vertices[6], vertices[7], vertices[2], vertices[3] } },
        };

        public static Vector3[] GetVerticesPositions(BlockFace face, Vector3 position) {
            Vector3[] vertices = new Vector3[verticesFace[face].Length];
            Array.Copy(verticesFace[face], vertices, verticesFace[face].Length);
            for (int i = 0; i < vertices.Length; i++) {
                vertices[i] += position;
            }
            return vertices;
        }

        public static Dictionary<BlockFace, int[]> triangles = new Dictionary<BlockFace, int[]>() {
            { BlockFace.TOP, new int[6] { 3, 1, 2, 3, 0, 1 } },
            { BlockFace.BOTTOM, new int[6] { 1, 0, 3, 2, 1, 3 } },
            { BlockFace.RIGHT, new int[6] { 3, 1, 2, 2, 1, 0 } },
            { BlockFace.LEFT, new int[6] { 3, 1, 2, 2, 1, 0 } },
            { BlockFace.FRONT, new int[6] { 3, 1, 2, 2, 1, 0 } },
            { BlockFace.BACK, new int[6] { 3, 1, 2, 2, 1, 0 } },
        };

        public static int[] GetTrianglesPositions(BlockFace face, int index) {
            int[] _triangles = new int[triangles[face].Length];
            Array.Copy(triangles[face], _triangles, triangles[face].Length);

            for (int i = 0; i < _triangles.Length; i++) {
                _triangles[i] += index * 4;
            }
            return _triangles;
        }

        public static Dictionary<BlockFace, Vector3[]> normals = new Dictionary<BlockFace, Vector3[]>() {
            { BlockFace.TOP, new Vector3[] { Vector3.up, Vector3.up, Vector3.up, Vector3.up } },
            { BlockFace.BOTTOM, new Vector3[] { Vector3.down, Vector3.down, Vector3.down, Vector3.down } },
            { BlockFace.RIGHT, new Vector3[] { Vector3.right, Vector3.right, Vector3.right, Vector3.right } },
            { BlockFace.LEFT, new Vector3[] { Vector3.left, Vector3.left, Vector3.left, Vector3.left } },
            { BlockFace.FRONT, new Vector3[] { Vector3.forward, Vector3.forward, Vector3.forward, Vector3.forward } },
            { BlockFace.BACK, new Vector3[] { Vector3.back, Vector3.back, Vector3.back, Vector3.back } },
        };

        #region BLOCK UVS
        //GRASS
        public static Dictionary<TileSide, Vector2[]> grassUVs = new Dictionary<TileSide, Vector2[]>() {
            /*GRASS TOP*/    { TileSide.TOP,    new Vector2[] { new Vector2(0f, 0.9375f), new Vector2(0.0625f, 0.9375f), new Vector2(0.0625f, 1f), new Vector2(0f, 1f) } },
            /*GRASS BOTTOM*/ { TileSide.BOTTOM, new Vector2[] { new Vector2(0.125f, 0.9375f), new Vector2(0.1875f, 0.9375f), new Vector2(0.1875f, 1f), new Vector2(0.125f, 1f) } },
            /*GRASS SIDE*/   { TileSide.SIDE,   new Vector2[]  { new Vector2(0.25f, 1f), new Vector2(0.1875f, 1f), new Vector2(0.25f, 0.9375f), new Vector2(0.1875f, 0.9375f) } },
        };

        //DIRT
        public static Dictionary<TileSide, Vector2[]> dirtUVs = new Dictionary<TileSide, Vector2[]>() {
            /*DIRT TOP*/    { TileSide.TOP,    new Vector2[] { new Vector2(0.125f, 0.9375f), new Vector2(0.1875f, 0.9375f), new Vector2(0.1875f, 1f), new Vector2(0.125f, 1f) } },
            /*DIRT BOTTOM*/ { TileSide.BOTTOM, new Vector2[] { new Vector2(0.125f, 0.9375f), new Vector2(0.1875f, 0.9375f), new Vector2(0.1875f, 1f), new Vector2(0.125f, 1f) } },
            /*DIRT SIDE*/   { TileSide.SIDE,   new Vector2[]  { new Vector2(0.125f, 0.9375f), new Vector2(0.1875f, 0.9375f), new Vector2(0.125f, 1f), new Vector2(0.1875f, 1f) } },
        };

        //STONE
        public static Dictionary<TileSide, Vector2[]> stoneUVs = new Dictionary<TileSide, Vector2[]>() {
            /*STONE TOP*/    { TileSide.TOP,    new Vector2[] { new Vector2(0.0625f, 0.9375f), new Vector2(0.125f, 0.9375f), new Vector2(0.125f, 1f), new Vector2(0.0625f, 1f) } },
            /*STONE BOTTOM*/ { TileSide.BOTTOM, new Vector2[] { new Vector2(0.0625f, 0.9375f), new Vector2(0.125f, 0.9375f), new Vector2(0.125f, 1f), new Vector2(0.0625f, 1f) } },
            /*STONE SIDE*/   { TileSide.SIDE,   new Vector2[]  { new Vector2(0.0625f, 0.9375f), new Vector2(0.125f, 0.9375f), new Vector2(0.0625f, 1f), new Vector2(0.125f, 1f) } },
        };

        //COAL
        public static Dictionary<TileSide, Vector2[]> coalUVs = new Dictionary<TileSide, Vector2[]>() {
            /*COAL TOP*/    { TileSide.TOP,    new Vector2[] { new Vector2(0.125f, 0.8125f),  new Vector2(0.1875f, 0.8125f), new Vector2(0.125f, 0.875f), new Vector2(0.1875f, 0.875f) } },
            /*COAL BOTTOM*/ { TileSide.BOTTOM, new Vector2[] { new Vector2(0.125f, 0.8125f),  new Vector2(0.1875f, 0.8125f), new Vector2(0.125f, 0.875f), new Vector2(0.1875f, 0.875f) } },
            /*COAL SIDE*/   { TileSide.SIDE,   new Vector2[]  { new Vector2(0.125f, 0.8125f), new Vector2(0.1875f, 0.8125f), new Vector2(0.125f, 0.875f), new Vector2(0.1875f, 0.875f) } },
        };

        //BEDROCK
        public static Dictionary<TileSide, Vector2[]> bedrockUVs = new Dictionary<TileSide, Vector2[]>() {
            /*BEDROCK TOP*/    { TileSide.TOP,    new Vector2[] { new Vector2(0.0625f, 0.875f), new Vector2(0.125f, 0.875f), new Vector2(0.0625f, 0.9375f), new Vector2(0.125f, 0.9375f) } },
            /*BEDROCK BOTTOM*/ { TileSide.BOTTOM, new Vector2[] { new Vector2(0.0625f, 0.875f), new Vector2(0.125f, 0.875f), new Vector2(0.0625f, 0.9375f), new Vector2(0.125f, 0.9375f) } },
            /*BEDROCK SIDE*/   { TileSide.SIDE,  new Vector2[]  { new Vector2(0.0625f, 0.875f), new Vector2(0.125f, 0.875f), new Vector2(0.0625f, 0.9375f), new Vector2(0.125f, 0.9375f) } },
        };

        public static Dictionary<TileSide, Vector2[]> noCrackUVs = new Dictionary<TileSide, Vector2[]>() {
            /*BEDROCK TOP*/    { TileSide.TOP,    new Vector2[] { new Vector2(0.625f, 0f), new Vector2(0.6875f, 0f), new Vector2(0.625f, 0.0625f), new Vector2(0.6875f, 0.0625f) } },
            /*BEDROCK BOTTOM*/ { TileSide.BOTTOM, new Vector2[] { new Vector2(0.625f, 0f), new Vector2(0.6875f, 0f), new Vector2(0.625f, 0.0625f), new Vector2(0.6875f, 0.0625f) } },
            /*BEDROCK SIDE*/   { TileSide.SIDE,   new Vector2[] { new Vector2(0.625f, 0f), new Vector2(0.6875f, 0f), new Vector2(0.625f, 0.0625f), new Vector2(0.6875f, 0.0625f) } },
        };

        public static Dictionary<TileSide, Vector2[]> crack1UVs = new Dictionary<TileSide, Vector2[]>() {
            /*BEDROCK TOP*/    { TileSide.TOP,    new Vector2[] { new Vector2(0.0f, 0.0f), new Vector2(0.0625f, 0.0f), new Vector2(0.0625f, 0.0625f), new Vector2(0.0f, 0.0625f) } },
            /*BEDROCK BOTTOM*/ { TileSide.BOTTOM, new Vector2[] { new Vector2(0.0f, 0.0f), new Vector2(0.0625f, 0.0f), new Vector2(0.0625f, 0.0625f), new Vector2(0.0f, 0.0625f) } },
            /*BEDROCK SIDE*/   { TileSide.SIDE,   new Vector2[] { new Vector2(0.0625f, 0.0625f), new Vector2(0.0f, 0.0625f), new Vector2(0.0625f, 0.0f), new Vector2(0.0f, 0.0f) } },
        };

        public static Dictionary<TileSide, Vector2[]> crack2UVs = new Dictionary<TileSide, Vector2[]>() {
            /*BEDROCK TOP*/    { TileSide.TOP,    new Vector2[] { new Vector2(0.0625f, 0.0f), new Vector2(0.125f, 0f), new Vector2(0.125f, 0.0625f), new Vector2(0.0625f, 0.0625f) } },
            /*BEDROCK BOTTOM*/ { TileSide.BOTTOM, new Vector2[] { new Vector2(0.0625f, 0.0f), new Vector2(0.125f, 0f), new Vector2(0.125f, 0.0625f), new Vector2(0.0625f, 0.0625f) } },
            /*BEDROCK SIDE*/   { TileSide.SIDE,  new Vector2[]  { new Vector2(0.125f, 0.0625f), new Vector2(0.0625f, 0.0625f), new Vector2(0.125f, 0f), new Vector2(0.0625f, 0.0f) } },
        };

        public static Dictionary<TileSide, Vector2[]> crack3UVs = new Dictionary<TileSide, Vector2[]>() {
            /*BEDROCK TOP*/    { TileSide.TOP,    new Vector2[] { new Vector2(0.125f, 0.0f), new Vector2(0.1875f, 0f), new Vector2(0.1875f, 0.0625f), new Vector2(0.125f, 0.0625f) } },
            /*BEDROCK BOTTOM*/ { TileSide.BOTTOM, new Vector2[] { new Vector2(0.125f, 0.0f), new Vector2(0.1875f, 0f), new Vector2(0.1875f, 0.0625f), new Vector2(0.125f, 0.0625f) } },
            /*BEDROCK SIDE*/   { TileSide.SIDE,  new Vector2[]  { new Vector2(0.1875f, 0.0625f), new Vector2(0.125f, 0.0625f), new Vector2(0.1875f, 0f), new Vector2(0.125f, 0.0f) } },
        };

        public static Dictionary<TileSide, Vector2[]> crack4UVs = new Dictionary<TileSide, Vector2[]>() {
            /*BEDROCK TOP*/    { TileSide.TOP,    new Vector2[] { new Vector2(0.1875f, 0.0f), new Vector2(0.25f, 0f), new Vector2(0.25f, 0.0625f), new Vector2(0.1875f, 0.0625f) } },
            /*BEDROCK BOTTOM*/ { TileSide.BOTTOM, new Vector2[] { new Vector2(0.1875f, 0.0f), new Vector2(0.25f, 0f), new Vector2(0.25f, 0.0625f), new Vector2(0.1875f, 0.0625f) } },
            /*BEDROCK SIDE*/   { TileSide.SIDE,  new Vector2[]  { new Vector2(0.25f, 0.0625f), new Vector2(0.1875f, 0.0625f), new Vector2(0.25f, 0f), new Vector2(0.1875f, 0.0f) } },
        };

        public static Dictionary<TileSide, Vector2[]> crack5UVs = new Dictionary<TileSide, Vector2[]>() {
            /*BEDROCK TOP*/    { TileSide.TOP,    new Vector2[] { new Vector2(0.25f, 0.0f), new Vector2(0.3125f, 0f), new Vector2(0.3125f, 0.0625f), new Vector2(0.25f, 0.0625f) } },
            /*BEDROCK BOTTOM*/ { TileSide.BOTTOM, new Vector2[] { new Vector2(0.25f, 0.0f), new Vector2(0.3125f, 0f), new Vector2(0.3125f, 0.0625f), new Vector2(0.25f, 0.0625f) } },
            /*BEDROCK SIDE*/   { TileSide.SIDE,  new Vector2[]  { new Vector2(0.3125f, 0.0625f), new Vector2(0.25f, 0.0625f), new Vector2(0.3125f, 0f), new Vector2(0.25f, 0.0f) } },
        };

        public static Dictionary<TileSide, Vector2[]> crack6UVs = new Dictionary<TileSide, Vector2[]>() {
            /*BEDROCK TOP*/    { TileSide.TOP,    new Vector2[] { new Vector2(0.3125f, 0.0f), new Vector2(0.375f, 0f), new Vector2(0.375f, 0.0625f), new Vector2(0.3125f, 0.0625f) } },
            /*BEDROCK BOTTOM*/ { TileSide.BOTTOM, new Vector2[] { new Vector2(0.3125f, 0.0f), new Vector2(0.375f, 0f), new Vector2(0.375f, 0.0625f), new Vector2(0.3125f, 0.0625f) } },
            /*BEDROCK SIDE*/   { TileSide.SIDE,  new Vector2[]  { new Vector2(0.375f, 0.0625f), new Vector2(0.3125f, 0.0625f), new Vector2(0.375f, 0f), new Vector2(0.3125f, 0.0f) } },
        };

        public static Dictionary<TileSide, Vector2[]> crack7UVs = new Dictionary<TileSide, Vector2[]>() {
            /*BEDROCK TOP*/    { TileSide.TOP,    new Vector2[] { new Vector2(0.375f, 0.0f), new Vector2(0.4375f, 0f), new Vector2(0.4375f, 0.0625f), new Vector2(0.375f, 0.0625f) } },
            /*BEDROCK BOTTOM*/ { TileSide.BOTTOM, new Vector2[] { new Vector2(0.375f, 0.0f), new Vector2(0.4375f, 0f), new Vector2(0.4375f, 0.0625f), new Vector2(0.375f, 0.0625f) } },
            /*BEDROCK SIDE*/   { TileSide.SIDE,  new Vector2[]  { new Vector2(0.4375f, 0.0625f), new Vector2(0.375f, 0.0625f), new Vector2(0.4375f, 0f), new Vector2(0.375f, 0.0f) } },
        };

        public static Dictionary<TileSide, Vector2[]> crack8UVs = new Dictionary<TileSide, Vector2[]>() {
            /*BEDROCK TOP*/    { TileSide.TOP,    new Vector2[] { new Vector2(0.4375f, 0.0f), new Vector2(0.5f, 0f), new Vector2(0.5f, 0.0625f), new Vector2(0.4375f, 0.0625f) } },
            /*BEDROCK BOTTOM*/ { TileSide.BOTTOM, new Vector2[] { new Vector2(0.4375f, 0.0f), new Vector2(0.5f, 0f), new Vector2(0.5f, 0.0625f), new Vector2(0.4375f, 0.0625f) } },
            /*BEDROCK SIDE*/   { TileSide.SIDE,  new Vector2[]  { new Vector2(0.5f, 0.0625f), new Vector2(0.4375f, 0.0625f), new Vector2(0.5f, 0f), new Vector2(0.4375f, 0.0f) } },
        };

        public static Dictionary<TileSide, Vector2[]> crack9UVs = new Dictionary<TileSide, Vector2[]>() {
            /*BEDROCK TOP*/    { TileSide.TOP,    new Vector2[] { new Vector2(0.5f, 0.0f), new Vector2(0.5625f, 0f), new Vector2(0.5625f, 0.0625f), new Vector2(0.5f, 0.0625f) } },
            /*BEDROCK BOTTOM*/ { TileSide.BOTTOM, new Vector2[] { new Vector2(0.5f, 0.0f), new Vector2(0.5625f, 0f), new Vector2(0.5625f, 0.0625f), new Vector2(0.5f, 0.0625f) } },
            /*BEDROCK SIDE*/   { TileSide.SIDE,  new Vector2[]  { new Vector2(0.5625f, 0.0625f), new Vector2(0.5f, 0.0625f), new Vector2(0.5625f, 0f), new Vector2(0.5f, 0.0f) } },
        };

        public static Dictionary<TileSide, Vector2[]> crack10UVs = new Dictionary<TileSide, Vector2[]>() {
            /*BEDROCK TOP*/    { TileSide.TOP,    new Vector2[] { new Vector2(0.5625f, 0.0f), new Vector2(0.625f, 0f), new Vector2(0.625f, 0.0625f), new Vector2(0.5625f, 0.0625f) } },
            /*BEDROCK BOTTOM*/ { TileSide.BOTTOM, new Vector2[] { new Vector2(0.5625f, 0.0f), new Vector2(0.625f, 0f), new Vector2(0.625f, 0.0625f), new Vector2(0.5625f, 0.0625f) } },
            /*BEDROCK SIDE*/   { TileSide.SIDE,  new Vector2[]  { new Vector2(0.625f, 0.0625f), new Vector2(0.5625f, 0.0625f), new Vector2(0.625f, 0f), new Vector2(0.5625f, 0.0f) } },
        };
        #endregion

        public static Dictionary<BlockType, Dictionary<TileSide, Vector2[]>> uvs = new Dictionary<BlockType, Dictionary<TileSide, Vector2[]>>() {
            { BlockType.GRASS,   grassUVs   },
            { BlockType.DIRT,    dirtUVs    },
            { BlockType.STONE,   stoneUVs   },
            { BlockType.COAL,    coalUVs    },
            { BlockType.BEDROCK, bedrockUVs },
            { BlockType.NOCRACK, noCrackUVs },
            { BlockType.CRACK1,  crack1UVs  },
            { BlockType.CRACK2,  crack2UVs  },
            { BlockType.CRACK3,  crack3UVs  },
            { BlockType.CRACK4,  crack4UVs  },
            { BlockType.CRACK5,  crack5UVs  },
            { BlockType.CRACK6,  crack6UVs  },
            { BlockType.CRACK7,  crack7UVs  },
            { BlockType.CRACK8,  crack8UVs  },
            { BlockType.CRACK9,  crack9UVs  },
            { BlockType.CRACK10, crack10UVs  },
        };

        public static Dictionary<BlockType, float> timeToBreak = new Dictionary<BlockType, float>() {
            { BlockType.GRASS,   3f },
            { BlockType.DIRT,    2f },
            { BlockType.STONE,   1f },
            { BlockType.COAL,    30f },
            { BlockType.BEDROCK,-1f },
            { BlockType.AIR,    -1f },
        };
    }

    public enum BlockFace { TOP, BOTTOM, RIGHT, LEFT, FRONT, BACK }
    public enum BlockType { GRASS, DIRT, STONE, COAL, BEDROCK, AIR, NOCRACK, CRACK1, CRACK2, CRACK3, CRACK4, CRACK5, CRACK6, CRACK7, CRACK8, CRACK9, CRACK10 }
    public enum TileSide  { TOP, BOTTOM, SIDE }
}