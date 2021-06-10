using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Minecraft.WorldGeneration 
{
    public class Chunk
    {
        public MeshStruct meshStruct;
        public Material material;

        public Block[,,] chunkData   = new Block[World.chunkSize, World.chunkSize, World.chunkSize];
        public GameObject chunkObject = null;
        public ChunkStatus status;

        public int currentIndex = 0;

        public Chunk(Vector3 _chunkPosition, Material _material) {
            chunkObject = new GameObject(World.BuildChunckName(_chunkPosition));
            chunkObject.transform.position = _chunkPosition;
            material = _material;

            meshStruct = new MeshStruct();
            meshStruct.vertices  = new List<Vector3>();
            meshStruct.normals   = new List<Vector3>();
            meshStruct.uvs       = new List<Vector2>();
            meshStruct.triangles = new List<int>();

            GenerateChunk();
        }

        public void GenerateChunk() {
            for (int x = 0; x < World.chunkSize; x++) {
                for (int y = 0; y < World.chunkSize; y++) {
                    for (int z = 0; z < World.chunkSize; z++) {
                        Vector3Int worldPos = new Vector3Int((int)chunkObject.transform.position.x + x, (int)chunkObject.transform.position.y + y, (int)chunkObject.transform.position.z + z);
                        BlockType  type     = GetBlockType(worldPos.x, worldPos.y, worldPos.z);
                        chunkData[x, y, z] = new Block(new Vector3Int(x, y, z), worldPos, type, this);
                        status = ChunkStatus.DRAW;
                    }
                }
            }

            for (int x = 0; x < World.chunkSize; x++) {
                for (int y = 0; y < World.chunkSize; y++) {
                    for (int z = 0; z < World.chunkSize; z++) {
                        chunkData[x, y, z].GenerateBlock();
                    }
                }
            }
        }

        public void DrawChunk() {

            Mesh mesh      = new Mesh();
            mesh.vertices  = meshStruct.vertices.ToArray();
            mesh.triangles = meshStruct.triangles.ToArray();
            mesh.uv        = meshStruct.uvs.ToArray();
            mesh.normals   = meshStruct.normals.ToArray();
            mesh.RecalculateBounds();

            MeshFilter filter = chunkObject.AddComponent(typeof(MeshFilter)) as MeshFilter;
            filter.mesh = mesh;
            MeshRenderer rend = chunkObject.AddComponent(typeof(MeshRenderer)) as MeshRenderer;
            rend.material = material;
            MeshCollider collider = chunkObject.AddComponent(typeof(MeshCollider)) as MeshCollider;
            collider.sharedMesh = filter.mesh;
            chunkObject.layer = LayerMask.NameToLayer("Ground");
            status = ChunkStatus.DONE;
        }

        private BlockType GetBlockType(int xWorld, int yWorld, int zWorld)
        {
            BlockType type;

            if (yWorld < Utils.GenerateBedrockHeight(xWorld, zWorld)) {
                type = BlockType.BEDROCK;
            }
            else {
                if (Utils.FBM3D(xWorld, yWorld, zWorld, 0.03f, 3, 2) < 0.43f) {
                    type = BlockType.AIR;
                }
                else if (yWorld <= Utils.GenerateStoneHeight(xWorld, zWorld)) {

                    if (Utils.FBM3D(xWorld, yWorld, zWorld, 0.25f, 1, 1f) < 0.39f) {
                        type = BlockType.COAL;
                    }
                    else {
                        type = BlockType.STONE;
                    }
                }
                else if (yWorld == Utils.GenerateHeight(xWorld, zWorld)) {
                    type = BlockType.GRASS;
                }
                else if (yWorld < Utils.GenerateHeight(xWorld, zWorld)) {
                    type = BlockType.DIRT;
                }
                else {
                    type = BlockType.AIR;
                }
            }

            return type;
        }
    }

    [System.Serializable]
    public struct MeshStruct {
        public List<Vector3> vertices;
        public List<Vector3> normals;
        public List<Vector2> uvs;
        public List<int>     triangles;
    }

    public enum ChunkStatus { KEEP, DRAW, DONE }
}
