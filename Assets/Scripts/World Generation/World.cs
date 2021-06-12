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

        private void Start() {
            Vector3 playerPos = player.transform.position;
            player.transform.position = new Vector3(playerPos.x, Utils.GenerateHeight(playerPos.x, playerPos.z) + 1, playerPos.z);
            lastBuildPos = player.transform.position;

            Vector3Int initialPos = new Vector3Int((int)player.transform.position.x, (int)player.transform.position.y, (int)player.transform.position.z) / chunkSize;
            BuildChunkAt(initialPos);


            StartCoroutine(RecursiveBuildWorld(initialPos, 4));
        }

        private void Update() {
            if (Vector3.Distance(player.transform.position, lastBuildPos) > chunkSize)
            {
                BuildNearPlayer();
                lastBuildPos = player.transform.position;
            }

            if (!player.gameObject.activeSelf)
            {
                player.gameObject.SetActive(true);
                firstBuild = false;
            }

            DrawChunks();
            RemoveChunks();
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

        private IEnumerator RecursiveBuildWorld(Vector3Int pos, int rad) {
            rad--;
            
            if (rad <= 0) yield break;

            Vector3Int newPos;

            newPos = pos + Vector3Int.right;
            BuildChunkAt(newPos);
            StartCoroutine(RecursiveBuildWorld(newPos, rad));
            yield return null;

            newPos = pos + Vector3Int.left;
            BuildChunkAt(newPos);
            StartCoroutine(RecursiveBuildWorld(newPos, rad));
            yield return null;

            newPos = pos + Vector3Int.up;
            BuildChunkAt(newPos);
            StartCoroutine(RecursiveBuildWorld(newPos, rad));
            yield return null;

            newPos = pos + Vector3Int.down;
            BuildChunkAt(newPos);
            StartCoroutine(RecursiveBuildWorld(newPos, rad));
            yield return null;

            newPos = pos + new Vector3Int(0, 0, 1);
            BuildChunkAt(newPos);
            StartCoroutine(RecursiveBuildWorld(newPos, rad));
            yield return null;

            newPos = pos + new Vector3Int(0, 0, -1);
            BuildChunkAt(newPos);
            StartCoroutine(RecursiveBuildWorld(newPos, rad));
            yield return null;
        }
        
        private void BuildNearPlayer() {
            StartCoroutine(RecursiveBuildWorld(new Vector3Int((int)player.transform.position.x, (int)player.transform.position.y, (int)player.transform.position.z) / chunkSize, radius));
        }

        private void DrawChunks()
        {
            foreach (KeyValuePair<string, Chunk> c in chunks)
            {
                if (c.Value.status == ChunkStatus.DRAW)
                {
                    c.Value.DrawChunk();
                }
                else if (c.Value.chunkObject && Vector3.Distance(player.transform.position, c.Value.chunkObject.transform.position) > radius * chunkSize)
                {
                    toRemove.Add(c.Key);
                }
            }
        }

        private void RemoveChunks() {
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
            }
        }

        public static Block GetWorldBlock(Vector3 pos) {
            int cx, cy, cz;

            if (pos.x < 0) {
                cx = (int) (Mathf.Round(pos.x - chunkSize) / (float)chunkSize) * chunkSize;
            }
            else {
                cx = (int) (Mathf.Round(pos.x) / (float)chunkSize) * chunkSize;
            }
            if (pos.y < 0) {
                cy = (int)(Mathf.Round(pos.y - chunkSize) / (float)chunkSize) * chunkSize;
            }
            else {
                cy = (int)(Mathf.Round(pos.y) / (float)chunkSize) * chunkSize;
            }
            if (pos.z < 0) {
                cz = (int)(Mathf.Round(pos.z - chunkSize) / (float)chunkSize) * chunkSize;
            } 
            else {
                cz = (int)(Mathf.Round(pos.z) / (float)chunkSize) * chunkSize;
            }

            int xBlock, yBlock, zBlock;
            xBlock = (int) Mathf.Abs((float)Mathf.Round(pos.x) - cx);
            yBlock = (int) Mathf.Abs((float)Mathf.Round(pos.y) - cy);
            zBlock = (int) Mathf.Abs((float)Mathf.Round(pos.z) - cz);

            string chunkName = BuildChunckName(new Vector3(cx, cy, cz));
            Chunk chunk;
            if (chunks.TryGetValue(chunkName, out chunk)) {
                return chunk.chunkData[xBlock, yBlock, zBlock];
            }
            else {
                return null;
            }
        }

        public static string BuildChunckName(Vector3 position) {
            return $"{(int)position.x}_{(int)position.y}_{(int)position.z}";
        }
    }
}
