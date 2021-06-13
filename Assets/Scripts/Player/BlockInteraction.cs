using System;
using System.Collections;
using System.Collections.Generic;
using Minecraft.WorldGeneration;
using UnityEngine;

public class BlockInteraction : MonoBehaviour
{
    public Transform camera = null;

    private bool canHit = true;
    private bool canDeleteBlock = true;
    private bool canCreateBlock = true;

    private Block currentBlock = null;

    [SerializeField] private LayerMask raycastableLayer;

    private void Update() {
        if (Input.GetMouseButton(0)) {
            LeftMouseAction();
        }

        if (Input.GetMouseButton(1)) {
            BuildBlock();
        }

        if (Input.GetMouseButtonUp(0) && currentBlock != null) {
            currentBlock.CancelHit();
            currentBlock = null;
            canHit = true;
            CancelInvoke(nameof(CanHit));
        }
    }

    private bool DoRaycast(out RaycastHit hit) {
        return Physics.Raycast(camera.position, camera.forward, out hit, 5, raycastableLayer);
    }

    private void LeftMouseAction() {
        Action<RaycastHit> action = null;
        
        if (DoRaycast(out RaycastHit hit)) {
            if (hit.transform.GetComponent<ChunkMB>()) {
                action = DestroyBlock;
            }
        }

        action?.Invoke(hit);
    }


    private void DestroyBlock(RaycastHit hit) {

        Chunk hitChunk;
        if (!World.chunks.TryGetValue(hit.collider.gameObject.name, out hitChunk)) return;

        if (canDeleteBlock) {
            canDeleteBlock = false;
            Invoke(nameof(CanDeleteBlock), 0.2f);
            GetComponentInChildren<Animator>().Play("HitAnimation");
        }

        Vector3 hitBlock = hit.point - hit.normal / 2;

        int x = (int)(Mathf.Round(hitBlock.x) - hit.collider.gameObject.transform.position.x);
        int y = (int)(Mathf.Round(hitBlock.y) - hit.collider.gameObject.transform.position.y);
        int z = (int)(Mathf.Round(hitBlock.z) - hit.collider.gameObject.transform.position.z);

        if (currentBlock != null && currentBlock != hitChunk.chunkData[x, y, z]) {
            currentBlock.CancelHit();
            canHit = true;
            currentBlock = null;
            CancelInvoke(nameof(CanHit));
        }

        if (!canHit) return;

        canHit = false;

        currentBlock = World.GetWorldBlock(hitBlock);

        Invoke(nameof(CanHit), (BlockData.timeToBreak[currentBlock.type] - 1) / 10f);

        if (currentBlock.HitBlock()) {
            hitChunk.changed = true;
            UpdateChunks(x, y, z, hitChunk);
        }
    }

    private void BuildBlock() {
        if (DoRaycast(out RaycastHit hit))
        {
            if (!canCreateBlock) return;
            canCreateBlock = false;
            Invoke(nameof(CanCreateBlock), 0.2f);

            Chunk hitChunk;
            if (!World.chunks.TryGetValue(hit.collider.gameObject.name, out hitChunk)) return;

            Vector3 hitBlock = hit.point + hit.normal / 2;

            currentBlock = World.GetWorldBlock(hitBlock);

            if (currentBlock.localPosition == World.GetWorldBlock(transform.position).localPosition || currentBlock.localPosition == World.GetWorldBlock(transform.position).localPosition + Vector3.up) {
                return;
            }

            GetComponentInChildren<Animator>().Play("HitAnimation");

            int x = (int)(Mathf.Round(hitBlock.x) - hit.collider.gameObject.transform.position.x);
            int y = (int)(Mathf.Round(hitBlock.y) - hit.collider.gameObject.transform.position.y);
            int z = (int)(Mathf.Round(hitBlock.z) - hit.collider.gameObject.transform.position.z);

            if (currentBlock.BuildBlock(BlockType.DIRT))
            {
                hitChunk.changed = true;
                UpdateChunks(x, y, z, hitChunk);
            }
        }
    }

    private void UpdateChunks(int x, int y, int z, Chunk hitChunk) {
        List<string> updates = new List<string>();
        float thisChunkx = hitChunk.chunkObject.transform.position.x;
        float thisChunky = hitChunk.chunkObject.transform.position.y;
        float thisChunkz = hitChunk.chunkObject.transform.position.z;

        if (x == 0)
        {
            updates.Add(World.BuildChunckName(new Vector3(thisChunkx - World.chunkSize, thisChunky, thisChunkz)));
        }
        if (x == World.chunkSize - 1)
        {
            updates.Add(World.BuildChunckName(new Vector3(thisChunkx + World.chunkSize, thisChunky, thisChunkz)));
        }
        if (y == 0)
        {
            updates.Add(World.BuildChunckName(new Vector3(thisChunkx, thisChunky - World.chunkSize, thisChunkz)));
        }
        if (y == World.chunkSize - 1)
        {
            updates.Add(World.BuildChunckName(new Vector3(thisChunkx, thisChunky + World.chunkSize, thisChunkz)));
        }
        if (z == 0)
        {
            updates.Add(World.BuildChunckName(new Vector3(thisChunkx, thisChunky, thisChunkz - World.chunkSize)));
        }
        if (z == World.chunkSize - 1)
        {
            updates.Add(World.BuildChunckName(new Vector3(thisChunkx, thisChunky, thisChunkz + World.chunkSize)));
        }

        foreach (string chunkName in updates)
        {
            Chunk chunk;
            if (World.chunks.TryGetValue(chunkName, out chunk))
            {
                chunk.Redraw();
            }
        }
    }

    private void CanHit() {
        canHit = true;
    }

    private void CanDeleteBlock() {
        canDeleteBlock = true;
    }

    private void CanCreateBlock() {
        canCreateBlock = true;
    }
}
