using System.Collections;
using System.Collections.Generic;
using Minecraft.WorldGeneration;
using UnityEngine;

public class BlockInteraction : MonoBehaviour
{
    public Transform camera = null;

    private bool canHit = true;
    private bool canPlayHitAnim = true;

    private Block currentBlock = null;

    private async void Update() {
        if (Input.GetMouseButton(0)) {
            if (Physics.Raycast(camera.position, camera.forward, out RaycastHit hit, 5)) {
                if (canPlayHitAnim) {
                    canPlayHitAnim = false;
                    Invoke(nameof(CanPlayHitAnim), 0.2f);
                    GetComponentInChildren<Animator>().Play("HitAnimation");
                }

                Vector3 hitBlock = hit.point - hit.normal/2;

                int x = (int)(Mathf.Round(hitBlock.x) - hit.collider.gameObject.transform.position.x);
                int y = (int)(Mathf.Round(hitBlock.y) - hit.collider.gameObject.transform.position.y);
                int z = (int)(Mathf.Round(hitBlock.z) - hit.collider.gameObject.transform.position.z);

                Chunk hitChunk;
                World.chunks.TryGetValue(hit.collider.gameObject.name, out hitChunk);
                if (currentBlock != null && currentBlock != hitChunk.chunkData[x, y, z]) {
                    currentBlock.CancelHit();
                    canHit = true;
                    currentBlock = null;
                    CancelInvoke(nameof(CanHit));
                }

                if (!canHit) return;

                canHit = false;

                currentBlock = hitChunk.chunkData[x, y, z];

                Invoke(nameof(CanHit), (BlockData.timeToBreak[currentBlock.type] - 1) / 10f);

                if (hitChunk != null && currentBlock.HitBlock()) {
                    List<string> updates = new List<string>();
                    float thisChunkx = hitChunk.chunkObject.transform.position.x;
                    float thisChunky = hitChunk.chunkObject.transform.position.y;
                    float thisChunkz = hitChunk.chunkObject.transform.position.z;
                    
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
                            chunk.Redraw();
                        }
                    }
                }
            }
        }

        if (Input.GetMouseButtonUp(0) && currentBlock != null) {
            currentBlock.CancelHit();
            currentBlock = null;
            canHit = true;
            CancelInvoke(nameof(CanHit));
        }
    }

    private void CanHit() {
        canHit = true;
    }

    private void CanPlayHitAnim() {
        canPlayHitAnim = true;
    }
}
