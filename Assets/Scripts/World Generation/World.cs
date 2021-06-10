using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Minecraft.WorldGeneration
{
    public class World : MonoBehaviour
    {
        [Header("WORLD PROPERTIES")]
        [SerializeField] private Material material;
        public static int chunkSize = 16;
        public static int radius    = 4;

        [Header("CORE")]
        [SerializeField] private GameObject player = null;
        public static ConcurrentDictionary<string, Chunk> chunks = new ConcurrentDictionary<string, Chunk>();
        public Vector3 lastBuildPos = Vector3.zero;
        public bool firstBuild = true;
        public List<string> toRemove = new List<string>();

        private async void Start() {
            Vector3 playerPos = player.transform.position;
            player.transform.position = new Vector3(playerPos.x, Utils.GenerateHeight(playerPos.x, playerPos.z) + 1, playerPos.z);
            lastBuildPos = player.transform.position;

            Vector3Int initialPos = new Vector3Int((int)player.transform.position.x, (int)player.transform.position.y, (int)player.transform.position.z) / chunkSize;
            BuildChunkAt(initialPos);


            RecursiveBuildWorld(initialPos, radius);
            //queue.Run(RecursiveBuildWorldA(initialPos, radius));
            //RecursiveBuildWorld(initialPos, radius);
        }

        private async void Update() {
            if (Vector3.Distance(player.transform.position, lastBuildPos) > chunkSize)
            {
                await BuildNearPlayer();
                lastBuildPos = player.transform.position;
            }

            if (!player.gameObject.activeSelf)
            {
                player.gameObject.SetActive(true);
                firstBuild = false;
            }

            await DrawChunks();
            await RemoveChunks();
        }

        private void BuildChunkAt(Vector3Int pos) {
            Vector3Int chunkPos  = new Vector3Int(pos.x, pos.y, pos.z) * chunkSize;
            string     chunkName = BuildChunckName(chunkPos);

            Chunk c;
            if (!chunks.TryGetValue(chunkName, out c)) {
                c = new Chunk(chunkPos, material);
                c.chunkObject.transform.parent = transform;
                chunks.TryAdd(chunkName, c);
            }
        }

        private async void RecursiveBuildWorld(Vector3Int pos, int rad) {
            rad--;
            
            if (rad <= 0) return;

            Vector3Int newPos;

            newPos = pos + Vector3Int.right;
            BuildChunkAt(newPos);
            RecursiveBuildWorld(newPos, rad);
            await Task.Yield();

            newPos = pos + Vector3Int.left;
            BuildChunkAt(newPos);
            RecursiveBuildWorld(newPos, rad);
            await Task.Yield();

            newPos = pos + Vector3Int.up;
            BuildChunkAt(newPos);
            RecursiveBuildWorld(newPos, rad);
            await Task.Yield();

            newPos = pos + Vector3Int.down;
            BuildChunkAt(newPos);
            RecursiveBuildWorld(newPos, rad);
            await Task.Yield();

            newPos = pos + new Vector3Int(0, 0, 1);
            BuildChunkAt(newPos);
            RecursiveBuildWorld(newPos, rad);
            await Task.Yield();

            newPos = pos + new Vector3Int(0, 0, -1);
            BuildChunkAt(newPos);
            RecursiveBuildWorld(newPos, rad);
            await Task.Yield();
        }
        
        private async Task BuildNearPlayer() {
            RecursiveBuildWorld(new Vector3Int((int)player.transform.position.x, (int)player.transform.position.y, (int)player.transform.position.z) / chunkSize, radius);
            await Task.Yield();
        }

        async Task DrawChunks()
        {
            foreach (KeyValuePair<string, Chunk> c in chunks)
            {
                if (c.Value.status == ChunkStatus.DRAW)
                {
                    await c.Value.DrawChunk();
                }
                else if (c.Value.chunkObject && Vector3.Distance(player.transform.position, c.Value.chunkObject.transform.position) > radius * chunkSize)
                {
                    toRemove.Add(c.Key);
                }
                await Task.Yield();
            }
        }

        private async Task RemoveChunks() {
            for (int i = 0; i < toRemove.Count; i++) {
                Chunk c;
                if (chunks.TryGetValue(toRemove[i], out c))
                {
                    Destroy(c.chunkObject);
                    c.Save();
                    chunks.TryRemove(toRemove[i], out c);
                    toRemove.RemoveAt(i);
                } else {
                    toRemove.RemoveAt(i);
                }
                await Task.Yield();
            }
        }

        public static string BuildChunckName(Vector3 position) {
            return $"{(int)position.x}_{(int)position.y}_{(int)position.z}";
        }
    }
}
