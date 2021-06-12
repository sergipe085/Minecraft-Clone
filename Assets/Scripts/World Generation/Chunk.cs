using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using UnityEngine;

namespace Minecraft.WorldGeneration 
{
    [System.Serializable]
    class ChunkBlockData 
    {
        public BlockType[,,] matrix;

        public ChunkBlockData() {}

        public ChunkBlockData(Block[,,] b) {
            matrix = new BlockType[World.chunkSize, World.chunkSize, World.chunkSize];
            for (int x = 0; x < World.chunkSize; x++)
            {
                for (int y = 0; y < World.chunkSize; y++)
                {
                    for (int z = 0; z < World.chunkSize; z++)
                    {
                        matrix[x, y, z] = b[x, y, z].type;
                    }
                }
            }
        }
    }

    public class Chunk
    {
        public MeshStruct meshStruct;
        public Material material;

        public Block[,,] chunkData   = new Block[World.chunkSize, World.chunkSize, World.chunkSize];
        public GameObject chunkObject = null;
        public ChunkStatus status;

        public int currentIndex = 0;
        public bool changed = false;

        private ChunkBlockData chunkBlockData;

        string BuildChunkFileName(Vector3 v) {
            return Application.persistentDataPath + $"/savedata/Chunk_{(int)v.x}_{(int)v.y}_{(int)v.z}_{World.chunkSize}_{World.radius}.dat";
        }

        bool Load() {
            string chunkFile = BuildChunkFileName(chunkObject.transform.position);
            if (File.Exists(chunkFile)) {
                BinaryFormatter bf = new BinaryFormatter();
                FileStream file = File.Open(chunkFile, FileMode.Open);
                chunkBlockData = new ChunkBlockData();
                chunkBlockData = (ChunkBlockData) bf.Deserialize(file);
                file.Close();
                return true;
            }
            return false;
        }

        public void Save() {
            string chunkFile = BuildChunkFileName(chunkObject.transform.position);

            if (!File.Exists(chunkFile)) {
                Directory.CreateDirectory(Path.GetDirectoryName(chunkFile));
            }

            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(chunkFile, FileMode.OpenOrCreate);
            chunkBlockData = new ChunkBlockData(chunkData);
            bf.Serialize(file, chunkBlockData);
            file.Close();
        }

        public Chunk(Vector3 _chunkPosition, Material _material) {
            chunkObject = new GameObject(World.BuildChunckName(_chunkPosition));
            chunkObject.transform.position = _chunkPosition;
            ChunkMB chunkMB = chunkObject.AddComponent(typeof(ChunkMB)) as ChunkMB;
            chunkMB.SetOwner(this);

            material = _material;

            meshStruct = new MeshStruct();
            meshStruct.vertices  = new List<Vector3>();
            meshStruct.normals   = new List<Vector3>();
            meshStruct.uvs       = new List<Vector2>();
            meshStruct.suvs      = new List<Vector2>();
            meshStruct.triangles = new List<int>();

            GenerateChunk();
        }

        public void GenerateChunk() {
            bool dataFromFile = false;
            dataFromFile = Load();

            for (int x = 0; x < World.chunkSize; x++) {
                for (int y = 0; y < World.chunkSize; y++) {
                    for (int z = 0; z < World.chunkSize; z++) {
                        Vector3Int worldPos = new Vector3Int((int)chunkObject.transform.position.x + x, (int)chunkObject.transform.position.y + y, (int)chunkObject.transform.position.z + z);
                        status = ChunkStatus.DRAW;
                        
                        if (dataFromFile) {
                            chunkData[x, y, z] = new Block(new Vector3Int(x, y, z), worldPos, chunkBlockData.matrix[x, y, z], this);
                            continue;
                        }

                        BlockType  type     = GetBlockType(worldPos.x, worldPos.y, worldPos.z);
                        chunkData[x, y, z] = new Block(new Vector3Int(x, y, z), worldPos, type, this);
                    }
                }
            }

            GenerateBlock();
        }

        public void GenerateBlock() {
            for (int x = 0; x < World.chunkSize; x++)
            {
                for (int y = 0; y < World.chunkSize; y++)
                {
                    for (int z = 0; z < World.chunkSize; z++)
                    {
                        chunkData[x, y, z].GenerateBlock();
                    }
                }
            }
        }

        public void Redraw() {
            GameObject.DestroyImmediate(chunkObject.GetComponent<MeshFilter>());
            GameObject.DestroyImmediate(chunkObject.GetComponent<MeshRenderer>());
            GameObject.DestroyImmediate(chunkObject.GetComponent<MeshCollider>());

            meshStruct.vertices.Clear();
            meshStruct.triangles.Clear();
            meshStruct.uvs.Clear();
            meshStruct.suvs.Clear();
            meshStruct.normals.Clear();
            currentIndex = 0;

            GenerateBlock();
            DrawChunk();
        }

        public void DrawChunk() {

            Mesh mesh      = new Mesh();
            mesh.vertices  = meshStruct.vertices.ToArray();
            mesh.triangles = meshStruct.triangles.ToArray();
            mesh.uv        = meshStruct.uvs.ToArray();
            mesh.SetUVs(1, meshStruct.suvs);
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
        public List<Vector2> suvs;
        public List<int>     triangles;
    }

    public enum ChunkStatus { KEEP, DRAW, DONE }
}
