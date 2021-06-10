using System.Collections;
using System.Collections.Generic;
using Minecraft.WorldGeneration;
using UnityEngine;

public class BlockInteraction : MonoBehaviour
{
    public Transform camera = null;

    private async void Update() {
        if (Input.GetMouseButtonDown(0)) {
            if (Physics.Raycast(camera.position, camera.forward, out RaycastHit hit, 5)) {
                Vector3 hitBlock = hit.point - hit.normal/2;

                int x = (int)(Mathf.Round(hitBlock.x) - hit.collider.gameObject.transform.position.x);
                int y = (int)(Mathf.Round(hitBlock.y) - hit.collider.gameObject.transform.position.y);
                int z = (int)(Mathf.Round(hitBlock.z) - hit.collider.gameObject.transform.position.z);

                List<string> updates = new List<string>();
                float thisChunkx = hit.collider.gameObject.transform.position.x;
                float thisChunky = hit.collider.gameObject.transform.position.y;
                float thisChunkz = hit.collider.gameObject.transform.position.z;
                
                updates.Add(hit.collider.gameObject.name);

                if (x == 0) {
                    updates.Add(World.BuildChunckName(new Vector3(thisChunkx - World.chunkSize, thisChunky, thisChunkz)));
                }
                if (x == World.chunkSize - 1) {
                    updates.Add(World.BuildChunckName(new Vector3(thisChunkx + World.chunkSize, thisChunky, thisChunkz)));
                }
                if (y == 0) {
                    updates.Add(World.BuildChunckName(new Vector3(thisChunkx, thisChunky - World.chunkSize, thisChunkz)));
                }
                if (y == World.chunkSize - 1)
                {
                    updates.Add(World.BuildChunckName(new Vector3(thisChunkx, thisChunky + World.chunkSize, thisChunkz)));
                }
                if (z == 0) {
                    updates.Add(World.BuildChunckName(new Vector3(thisChunkx, thisChunky, thisChunkz - World.chunkSize)));
                }
                if (z == World.chunkSize - 1)
                {
                    updates.Add(World.BuildChunckName(new Vector3(thisChunkx, thisChunky, thisChunkz + World.chunkSize)));
                }

                foreach(string chunkName in updates) {
                    Chunk chunk;
                    if (World.chunks.TryGetValue(chunkName, out chunk))
                    {
                        DestroyImmediate(chunk.chunkObject.GetComponent<MeshFilter>());
                        DestroyImmediate(chunk.chunkObject.GetComponent<MeshRenderer>());
                        DestroyImmediate(chunk.chunkObject.GetComponent<MeshCollider>());

                        chunk.meshStruct.vertices.Clear();
                        chunk.meshStruct.triangles.Clear();
                        chunk.meshStruct.uvs.Clear();
                        chunk.meshStruct.normals.Clear();
                        chunk.currentIndex = 0;

                        chunk.chunkData[x, y, z].SetType(BlockType.AIR);
                        chunk.GenerateBlock();
                        chunk.DrawChunk();
                    }
                }
            }
        }
    }
}
