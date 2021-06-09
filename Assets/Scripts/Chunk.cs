using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Minecraft.WorldGeneration 
{
    public class Chunk : MonoBehaviour
    {
        public static int chunckSize = 16;

        public MeshStruct meshStruct = new MeshStruct();
        public Material material;

        public Block[,,] chunckData = new Block[chunckSize, chunckSize, chunckSize];

        public int currentIndex = 0;

        public void GenerateChunk() {
            for (int x = 0; x < chunckSize; x++) {
                for (int y = 0; y < chunckSize; y++) {
                    for (int z = 0; z < chunckSize; z++) {
                        chunckData[x, y, z] = new Block(new Vector3Int(x, y, z), new Vector3(x, y, z), (BlockType)Random.Range(0, 1), this);
                    }
                }
            }

            for (int x = 0; x < chunckSize; x++) {
                for (int y = 0; y < chunckSize; y++) {
                    for (int z = 0; z < chunckSize; z++) {
                        chunckData[x, y, z].GenerateBlock();
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

            MeshFilter filter = gameObject.AddComponent(typeof(MeshFilter)) as MeshFilter;
            filter.mesh = mesh;
            print(filter.mesh.triangles.Length);
            MeshRenderer rend = gameObject.AddComponent(typeof(MeshRenderer)) as MeshRenderer;
            rend.material = material;
        }

        private void Start() {
            GenerateChunk();
            DrawChunk();
        }
    }

    [System.Serializable]
    public struct MeshStruct {
        public List<Vector3> vertices;
        public List<Vector3> normals;
        public List<Vector2> uvs;
        public List<int>     triangles;
    }
}
