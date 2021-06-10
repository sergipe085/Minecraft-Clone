using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace Minecraft.WorldGeneration
{
    public class ChunkGenerator : MonoBehaviour
    {
        public static int chunkSize = 16;

        [SerializeField] private Material material;
        [SerializeField] private GameObject player = null;

        public bool creatingTerrain = true;
        public bool create = false;

        public Vector3Int[] a;
        public Vector3 lastBuildPosition = Vector3Int.zero;

        private Dictionary<Vector3Int, Chunk> chunks = new Dictionary<Vector3Int, Chunk>();

        private async void Start() {
            player.SetActive(false);

            a = GetChunksNearPlayer(new Vector3Int((int)player.transform.position.x, (int)player.transform.position.y, (int)player.transform.position.z)).ToArray();
            await CreateChunks(a).ContinueWith(_ => {
                creatingTerrain = false;
            });
        }

        private async void Update() {
            if (creatingTerrain == false) {
                if (!create) {
                    player.SetActive(true);
                    Vector3 playerPos = player.transform.position;
                    player.transform.position = new Vector3(playerPos.x, Utils.GenerateHeight(playerPos.x, playerPos.z) + 1, playerPos.z);
                    create = true;
                    lastBuildPosition = player.transform.position;
                }

                if (Vector3.Distance(player.transform.position, lastBuildPosition) > chunkSize) {
                    lastBuildPosition = player.transform.position;
                    a = GetChunksNearPlayer(new Vector3Int((int)player.transform.position.x, (int)player.transform.position.y, (int)player.transform.position.z)).ToArray();
                    await CreateChunks(a).ContinueWith(_ =>
                    {
                        creatingTerrain = false;
                    });
                }
            }
        }

        private IEnumerable<Vector3Int> GetChunksNearPlayer(Vector3Int pos) {
            int rad = 2;
            for (int x = -rad; x < rad; x++)
            {
                for (int y = -rad; y < rad; y++)
                {
                    for (int z = -rad; z < rad - y; z++)
                    {
                        yield return pos + new Vector3Int(x, y, z) * chunkSize;
                    }
                }
            }
        }

        private async Task CreateChunks(IEnumerable<Vector3Int> chunkPositions) {
            creatingTerrain = true;

            foreach(Vector3Int chunkPos in chunkPositions) {
                if (!chunks.ContainsKey(chunkPos))
                {
                    Chunk chunk = new Chunk(new Vector3(chunkPos.x, chunkPos.y, chunkPos.z), material);
                    chunks[chunkPos] = chunk;
                    chunk.DrawChunk();
                    await Task.Yield();
                }
            }
        }
    }
}
